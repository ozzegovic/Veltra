import { parseQueryParams, getCookieDetails } from '../general/parseQueryParams.js'
import loader from '../general/loader.js'
import { API as unitsAPI } from './units.js'
import { filterForm } from '../general/forms.js'
import { getTemplate } from '../general/getTemplate.js';
import { formatDate, formatTime, isDateBefore } from '../general/formatDate.js'
import { loadMap } from '../general/map.js'
import { API as commentsAPI, getCommentTemplate } from './comments.js'

const API = {
	getPackage( id ) {
		return $.ajax({
			url: "api/TravelPackage/" + id,
			type: "GET",
			dataType: "json",
			async : false,
		})
	},

	book( data ) {
		return $.ajax({
			url: "api/Reservation/Book",
			type: "POST",
			dataType: "json",
			data
		})
	}
}



// read browser URL to know which package to get from server
const currentUser = getCookieDetails()
const queryParams = parseQueryParams(document.location.search)
const $package = $('.travel-package-single')
const $pageStatus = $('.page-status')
const $comments = $('.comments-package-section')
const $commentsList = $('.comments-list')
const $bookPackageSection = $('.book-package-section')

let TravelPackageData; // will be populated from AJAX

if ( queryParams.id ){
	getPackage( queryParams.id )
}

// if for some reason there is no "id" query params for the page, redirect to packages page
else {
	window.location = '/'
}

$(document).on('click', '.book-unit-btn', function(){
	const unitId = $(this).closest('tr').data('id')
	const username = currentUser.loggedIn

	// if user not logged in, show login modal
	if (!currentUser.loggedIn) {
		// a listener for global event "modal:show:login" is in modals.js
		$(document).trigger('modal:show:login')
		return false
	}

	const $bookBtn = $(this).attr('loading', true);

	// returns the AJAX object
	book({
		TouristUsername: username,
		TravelPackage: TravelPackageData.Id,
		AccommodationUnit: unitId
	})
	.always(() => $bookBtn.removeAttr('loading') )
})

// print error message or loading status
function pageStatus(msg = '') {
	$pageStatus.html(msg)
}


function book( data ) {
	return API.book( data )
		.done(function () {
			// go to reservations page
			window.location = 'reservations.html'
		})

		.fail(function (err) {
			alert("Booking failed 😭");
			$('.book-unit-btn').removeAttr('loading')
			console.warn(err)
		})
}

// get accommodations
function getPackage( id ) {
	pageStatus( loader ); // cleaup any previous error message

	API.getPackage( id )
		.done(function (data) {
			$package.removeClass('hide')
			TravelPackageData = data
			renderPackage(data)
		})

		.fail(function (err) {
			pageStatus("Could not get package 😭");
		})

		.always(function () {
			pageStatus()
		})

  return false
}

function loadComments( packageId ) {
	// show loader while loading comments from server
	$commentsList.html( loader )

	commentsAPI.getAll( packageId )
		.done(function (commentsData) {
			$commentsList.empty()

			if ( !commentsData || commentsData.length == 0 ) {
				$commentsList.text("No comments yet")
				return false;
			}

			// create HTML templates for each comment
			commentsData.forEach(function( commentData ){
				const $template = getCommentTemplate(commentData)

				// add the template to the page once it's filled with data
				$commentsList.append($template)
			})
		})

		.fail(function (err, textStatus, xhr) {
			$commentsList.text("Could not load comments")
		})
}

// bind comments events:
function bindCommentsEvents( packageData ) {
	// note - only uses who participated can write a comment
	$comments.on('submit', '.comments-write-comment', function(){
		const inputValue = $comments.find('.comment-input').val()
		const ratingValue = $comments.find('[name="Rating"]').val()

		commentsAPI.create({
			TouristUsername: currentUser.loggedIn,
			TravelPackageId: packageData.Id,
			Content: inputValue,
			Rating: ratingValue,
		})

		return false // prevent page refresh
	})

	$comments.on('change', '.approve-comment', function(){
		const commentId = $(this).closest('.comment').attr('data-id')
		const isApproved = $(this).prop('checked')  // get checkout "checked" value: https://stackoverflow.com/a/19932511/104380

		commentsAPI.approve({
			Id: commentId,
			Status: isApproved ? "APPROVED" : "DENIED"
		})
	})
}

function renderPackage( data ) {
	const isPastPackage = isDateBefore(data.StartDate)

	// set browser tab title
	document.title += " - " + data.Destination

	// if this package already happned, make some chanegs to the page
	if ( isPastPackage ) {
		$('.LowestPrice').parent().prev().remove() // hide the line seperator
		$('.LowestPrice').parent().remove() // hide price

        $bookPackageSection.remove()

		$comments.removeClass('hide')
		loadComments( data.Id )
		bindCommentsEvents( data )
	}
	else {
		$bookPackageSection.removeClass('hide')
	}

	if ( data.Participated ) {
		// only tourists who participated may write a comment
		$comments.find('.comments-write-comment').removeClass('hide')
	}

	// for the manager which created this package
	if ( data.CreatedByMe ) {
		// show the "remove comment" button, for each comment
		$comments.find('.remove-comment-btn').removeClass('hide')
		$comments.find('.approve-comment').removeClass('hide')
		$comments.find('.comment-actions').removeClass('hide')
	}
	
	$('.package-cover-img').attr('src', data.Photos)
	$('.package-destination').text(data.Destination)
	$('.package-name').text(data.Name)
	$('.package-start-date').text( formatDate(data.StartDate) )
	$('.package-end-date').text( formatDate(data.EndDate) )

	$('.MaxCapacity').text(data.MaxCapacity)
	$('.PackageType').text(data.PackageType)
	$('.TransportationType').text(data.TransportationType)
	$('.LowestPrice').text(data.LowestPrice.toLocaleString()) // format price with commas in number: 1,234

	$('.package-description').text(data.Description)
	$('.Itinerary').text(data.Itinerary)

	$('.meeting-time').text( formatTime(data.Time) )
	$('.meeting-city').text(data.Location.Address.City)
	$('.meeting-street').text(data.Location.Address.Street)
	$('.meeting-number').text(data.Location.Address.Number)
	$('.meeting-zip').text(data.Location.Address.PostalCode)


	// load map 
	// (pass the HTML element id to render it in, and the coorditades)
	loadMap('meeting-map', {lonLat:[data.Location.Longitude, data.Location.Latitude], pinLocation:true})

	// render accommodations
	data.Accommodations.forEach((accData) => {
		// Instantiate the table with the existing HTML tbody
		// and the row with the template
		const $template = getAccommodationTemplate(accData)
	
		// add template to the list
		$('.accommodations-tbody').append( $template )  
	})
}

// get tmeplate, fill the template with data and return the template filled
function getAccommodationTemplate( accData ) {
	// Instantiate the table with the existing HTML tbody
	// and the row with the template
	const $accTtemplate = $( getTemplate('#accommodation-row-template') )
	const td = $accTtemplate.find("tr.accommodation td")
	const $unitsTableRow = td.parent().next('tr')

	// set the accommodation id on the <tr> element
	td.parent().attr('data-id', accData.Id)

	td.eq(0).text(accData.AccommodationType)
	td.eq(1).text(accData.Name)
	td.eq(2).text( "★★★★★".slice(0, accData.Stars) ) 
	td.eq(3).text( accData.Ammenities.join(', ') )
	td.eq(4).text(accData.AvailableUnitsCount + ' Rooms left')

	// when open a row, need to fetch units for this accommodation id
	// only if there are available rooms for this accommodation
	if( accData.AvailableUnitsCount ){
		bindUnitsFilterForm( $unitsTableRow.find('.units-filter'), accData.Id )

		// get the units (by accommodation id) and pass where to add them to in the HTML
		getUnitsForAccommodation({ id: accData.Id }, $accTtemplate.find('.units-tbody'))
	}
	else {
		// removes the units section completly
		$unitsTableRow.remove()
	}

	return $accTtemplate
}

function getUnitsForAccommodation( data, $renderTarget ){
	return unitsAPI.getAll(data)
		.done(function ( unitsData ) {
			// itterate results
			unitsData.forEach((unitData) => {
				// Instantiate the table with the existing HTML tbody
				// and the row with the template
				
				const $unitTemplate = getUnitTemplate(unitData)

				// add units template to row of the specific accommodation it belongs to
				$renderTarget.append($unitTemplate)  			
			})
		})
}

function getUnitTemplate( unitData ) {
	const $template = $( getTemplate('#accommodation-unit-row-template') );
	const td = $template.find("tr.unit td");

	// set the accommodation id on the <tr> element
	td.parent().attr('data-id', unitData.Id)

	td.eq(0).text(unitData.Capacity)
	td.eq(1).text(unitData.Ammenities.join(', '))
	td.eq(2).text('$' + unitData.Price.toLocaleString()) 

	// if unit is unavailable
	if( !unitData.IsAvailable ){
		$template.find('.book-unit-btn').hide().parent().text("Not Available")
	}
	

	return $template
}

function bindUnitsFilterForm( $form, accId ) {
	const $unitsArea = $form.parent().find('.units-tbody')
	const $unitsMessage = $form.parent().find('.units-message')  // if no results will be found, fill this with sad text

	// bind events for filtering / sorting and other stuff
	filterForm( $form, {
		// call this function when the filter (search) form submited 
		onSubmit: function( data ){
			$unitsMessage.empty()
			$unitsArea.empty() // empty previous units

			return getUnitsForAccommodation({ id:accId, ...data }, $unitsArea) // load new filtered units
				.done(function ( unitsData ) {
					if( unitsData.length == 0 ){
						$unitsMessage.html('Nothing Found')
					}
				})
		}, 

		// call this function when the search form was reset
		onReset: function(){
			$unitsMessage.empty()
			$unitsArea.empty() // empty previous units
			getUnitsForAccommodation({ id:accId }, $unitsArea)
		},

       // $sorters: $form.parent().find('.sortBy--unit-price') // use these sorters
        $sorters: $('.sortBy--unit-price')
	})
}