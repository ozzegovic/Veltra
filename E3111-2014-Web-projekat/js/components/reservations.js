import '../general/index.js'
import loader from '../general/loader.js'
import { getCookieDetails } from '../general/parseQueryParams.js';
import { filterForm } from '../general/forms.js';
import { getTemplate } from '../general/getTemplate.js';
import { createModal } from '../general/modals.js';
import * as reservation from './reservation.js';
import { formatDate } from '../general/formatDate.js'

const currentUser = getCookieDetails()
const $reservationsFilterForm = $('.form.reservations-filter')
const $reservations = $('.reservations') // reservations list
let reservationsData = [];

const API = {
	getReservations( data ) {
		return $.ajax({
			url: "api/Reservation",
			type: "GET",
			dataType: "json",
			async: false,
			data
		})
	},
	cancelReservation(data) {
		return $.ajax({
			url: "api/Reservation/Cancel",
			type: "PUT",
			data
		})
	}
}

// bind events
filterForm( $reservationsFilterForm, {
	onSubmit: loadReservations, // call this function when the filter (search) form submited 
	onReset: loadReservations,  // call this function when the search form was reset
	$sorters: $('.sortBy') // use these sorters
})

$reservations
	.on('click', '.btn-cancel', function(){
		if (confirm("Are you sure?")) {
			cancelReservation( $(this).closest('tr') )
		}
	})
	.on('click', '.btn-details', function () {
		const reservationId = $(this).closest('tr').data('id')
		showReservationModal(reservationId)
	})

// when the page first loads, get all reservations and render them
loadReservations()

// get reservations
function loadReservations( searchData ) {  // "extraData" can be what to sort by, coming from "filterForm" function
	message(loader); // cleaup any previous error message

	return API.getReservations( searchData )
		.done(function (data) {
			reservationsData = data;
			renderReservations(data)
		})

		.fail(function (err) {
			message("Something went wrong");

			if (err.status == 302) {
				window.location = err.getResponseHeader('FORCE_REDIRECT');
				return;
			};
		})
}
// Find a value in an array of objects 
// https://stackoverflow.com/a/12462414/104380
function getReservationDataById(id) {
	return reservationsData.find(item => item.Id == id)
}

// print error message
function message(msg = '') {
	$reservations.find("tfoot td").html(msg)
}
// render reservations from template
// https://developer.mozilla.org/en-US/docs/Web/HTML/Element/template
function renderReservations(reservationsData) {
	// cleaup previous users list
	$reservations.find("tbody").empty()

	// if no search results found
	if (reservationsData.length == 0) {
		message("No results")
		return
	}

	message()

	reservationsData.forEach((reservationsData, idx) => {
		// Instantiate the table with the existing HTML tbody
		// and the row with the template
		const template = getTemplate('#reservations_row_template');
		const td = $(template).find("td");
		
		template.firstElementChild.setAttribute('data-id', reservationsData.Id)
		td.eq(1).find('img').attr('src', reservationsData.Photo)
		td.eq(2).text(reservationsData.TravelPackageId)
		td.eq(3).find('a').text(reservationsData.TravelPackageName)
		td.eq(4).text( formatDate(reservationsData.StartDate) + " - " + formatDate(reservationsData.EndDate) )
		td.eq(5).text(reservationsData.TouristUsername)
		td.eq(6).text('$' + reservationsData.TotalPrice.toLocaleString())
		td.eq(7).text(reservationsData.Status)

		// set links to the package page
		$(template).find('a').attr('href', '/package.html?id=' + reservationsData.TravelPackageId)

		if( reservationsData.Status == 'CANCELED' ){
			td.eq(7).addClass('text-red-3')
		}
		else if ( currentUser.role == 'TOURIST' ){
			$(template).find('.btn-cancel').removeClass('hide')
		}

		$reservations.find("tbody").append(template)
	})
}
function cancelReservation( $row ) {
	$row.addClass('loading')
	const reservation = $row.data('id')
	const reservationData = getReservationDataById(reservation);

	if (!reservationData) return

	const isLocked = reservationData.Locked

	if (!reservation) return false;

	console.log(reservationData);

	API.cancelReservation(reservationData)

		.done(function () {
			$row.remove();
			loadReservations();
		})

		.fail(function (err) {
			$row.removeClass('loading')
			alert("error canceling reservation")
		})

		.always(function () {
		})
}
function showReservationModal( reservationId ) { 
	// else, create new modal and load the HTML template file into it
	$.get("views/reservation.html", function (HTML) {
		// show loader inside modal
		const $modal = createModal( HTML,
			{
				addToBody: true,
				show: true,
			    onClose: function(){ $modal.remove() },
				modalContentClass: 'pt-xl' // padding top extra large
			}
		)

		reservation.init(reservationId, $modal)
	});
}



