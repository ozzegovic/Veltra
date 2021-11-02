import '../general/index.js';
import loader from '../general/loader.js'
import { getTemplate } from '../general/getTemplate.js';
import { formatDate, formatTime } from '../general/formatDate.js'

let reservationData;

const API = {
    getAll(ReservationId) {
        return $.ajax({
            url: "api/Reservation/Details?id=" + ReservationId,
            type: "GET",
            dataType: "json",
            async: false
        })
    }
}

// defined empty variables which will be filled after units.html file will be loaded:
// 1. from the "reservations" page 
let $table;
let ReservationId;


// print messages like errors or loading
function message(msg = "") {
    $table.find("th:first").html(msg)
}

// this is called from the reservations page when it loads the modal for the reservation details
export function init( resId, $modal ) {
    $table = $modal.find('table')
    ReservationId = resId  // remeber this id to use later when searching/editing/deleting

    // finally load the data
    getReservation( resId )
}

// get units
export function getReservation( resId) {
    message(loader); // cleaup any previous error message

    return API.getAll( resId )
        .done(function (data) {
            reservationData = data;
            renderReservation(data)
        })

        .fail(function (err, textStatus, xhr) {
            message("Something went wrong");
            if (xhr.status == 302) {
                window.location = err.getResponseHeader('FORCE_REDIRECT');
                return;
            };
        })

        .always(function () {

        })
}

function splitCapitalLetters( str ) {
	return str.split(/(?=[A-Z])/).join(' ')
}

// render reservation
// https://developer.mozilla.org/en-US/docs/Web/HTML/Element/template
export function renderReservation(resData = [], ReservationId) {
	const allowedKeys = [
		"Name",
		"Lastname",
		"Email",
		"Name",
		"PackageType",
		"TransportationType",
		"Destination",
		"StartDate",
		"EndDate",
		"Time",
		"MaxCapacity",
		"Capacity",
		"Price",
		"Name",
		"Stars",
		"TotalPrice"
	]

    // if no search results found
    if (reservationData.length == 0) {
        message("No results")
    }

	const $tbody = $("<tbody></tbody>")
	$table.html($tbody)

    // iterate and create table rows
	for( let keyA in resData ){
		const $titleTemplate = $(getTemplate('#reservation-row-title-template'));

		if( typeof resData[keyA] == 'object' ){
			// split by capital letters
			// https://stackoverflow.com/a/7888303/104380
			$titleTemplate.find('.row-title').text( splitCapitalLetters(keyA) )
			$tbody.append( $titleTemplate )

			for( let keyB in resData[keyA] ){
				// only allow certain data to be printed
				if( allowedKeys.indexOf(keyB) != -1 ){
					const $dataTemplate = $(getTemplate('#reservation-row-template'));
					$dataTemplate.find('td').eq(0).text( splitCapitalLetters(keyB) ) // key

					let value = resData[keyA][keyB]

					// format time
					if( keyB == 'Time')
						value = formatTime(value)

					// format date
					if( keyB == 'StartDate' || keyB == 'EndDate')
						value = formatDate(value)

					// format price
					if ( keyB.indexOf('Price') != -1 ) {
						value = "$" + value.toLocaleString()
					}

					if( keyB == 'Stars')
						value = "★★★★★".slice(0,value)
					

					$dataTemplate.find('td').eq(1).text( value ) // value

					$tbody.append( $dataTemplate )
				}
			}
		}	
		else if( allowedKeys.indexOf(keyA) != -1 ){
			const $dataTemplate = $(getTemplate('#reservation-row-template'));

			if ( keyA == 'TotalPrice' ) {
				resData[keyA] = "$" + resData[keyA].toLocaleString()
			}

			$dataTemplate.find('td').eq(0).text( splitCapitalLetters(keyA) ) // key
			$dataTemplate.find('td').eq(1).text( resData[keyA] ) // value
			$tbody.append( $dataTemplate )
		}
    }
}


