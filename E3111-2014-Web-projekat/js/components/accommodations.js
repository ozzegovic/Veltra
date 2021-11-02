import '../general/index.js';
import loader from '../general/loader.js'
import { createModal } from '../general/modals.js';
import { filterForm } from '../general/forms.js';
import { getTemplate } from '../general/getTemplate.js';
import * as units from './units.js';

const API = {
  getAll( data ) {
	return $.ajax({
		url: "api/Accommodation/All",
		type: "GET",
		dataType: "json",
		async : false,
		data: data 
	})
  },

  delete(data) {
	return $.ajax({
		url: "api/Accommodation/DeleteAccommodation?id=" + data,
		type: "DELETE"
	})
  },

  update(data) {
	return $.ajax({
		url: "api/Accommodation/Create",
		async: false,
		type: "PUT",
		data,
	})
  },

  create(data) {
	return $.ajax({
		url: "api/Accommodation/Create",
		async: false,
		type: "POST",
		data,
	})
  },
}

const $FilterForm = $('.accommodations-filter') // form - filters for the list of accommodations
const $accommodations = $('.accommodations') // table - list of accommodations 
let accommodationsData = []; // array

// bind events
filterForm( $FilterForm, {
	onSubmit: getAccommodations, // call this function when the filter (search) form submited 
	onReset: getAccommodations,  // call this function when the search form was reset
	$sorters: $('.sortBy') // use these sorters
})

$accommodations
	.on('click', '.delete-btn', function(e){
		if (confirm("Are you sure?")) {
			const $row = $(this).closest('tr')
			deleteAccommodation( $row  )
		}
	})
	.on('change', 'input, select', updateAccommodation)
	.on('click', '.create-btn', createAccommodation)
	.on('click', '.units-btn', function(){
		const accommodationId = $(this).closest('tr').data('id') 
		showUnitsModal( accommodationId )
	})
	// when updating units, update the number in the table row for the accommodation
	.on('units:change', onUnitsChange)

// when the page first loads, get all accommodations and render them
getAccommodations()


// get accommodations
function getAccommodations( searchData ) {
	message( loader ); // cleaup any previous error message

	API.getAll( searchData )
		.done(function (data) {
			accommodationsData = data;
			renderAccommodations(data || [])
		})

		.fail(function (err, textStatus, xhr) {
			debugger
			message("Something went wrong");
			if (err.status == 302) {
				window.location = err.getResponseHeader('FORCE_REDIRECT');
			};
		})

		.always(function () {
	  
		})

  return false
}

// print error message
function message(msg = '') {
	$accommodations.find("tfoot td").html(msg)
}

function getAccommodationTemplate( data ) {
	// Instantiate the table with the existing HTML tbody
	// and the row with the template
	const $template = $( getTemplate('#accommodations_template') );
	const td = $template.find("td");

	// fill the template with data
	if( data.Id ){
		// set the accommodation id on the <tr> element
		$template.find('tr').attr('data-id', data.Id)

		$template.find('input[name="Name"]').val(data.Name)
		$template.find('select[name="AccommodationType"]').val(data.AccommodationType)
		$template.find('.units-btn').prepend(data.AccommodationUnitIds.length + ' ')
		td.eq(4).text(data.AvailableUnitsCount)
		$template.find('select[name="Stars"]').val(data.Stars)
		data.Ammenities.forEach(function(ammenity){
			$template.find('input[name="' + ammenity + '"]').prop('checked', true)
		})
	}
	else {
		$template.find('button').addClass('hide') // hide all buttons
		$template.find('.create-btn').removeClass('hide') // show only the "add" button
	}

	return $template
}

// render accommodations
// https://developer.mozilla.org/en-US/docs/Web/HTML/Element/template
function renderAccommodations(accommodationsData = []) {
	$accommodations.find("tbody").empty()

	// if no search results found
	if (accommodationsData.length == 0) {
		message("No results")
	}

	message()

	// push an empty template to the begning of the array:
	// https://stackoverflow.com/a/8159547/104380
	accommodationsData.unshift({})

	// itterate all accommodations and create table rows
	accommodationsData.forEach((data) => {
		// Instantiate the table with the existing HTML tbody
		// and the row with the template
		const $template = getAccommodationTemplate(data)
	
		// add template to the list
		$accommodations.find("tbody").append($template)  
	})
}



function deleteAccommodation( $row ) {
  $row.addClass('loading')
  const id = $row.data('id') // get "id" of accommodation from row element

  if( !id ) return false;

  API.delete(id)
	.done(function() {
		$row.remove(); // removes the row from HTML
	})

	.fail(function(err) {
	  $row.removeClass('loading')
	  alert("error deleting accommodation")
	})

	.always(function () {
	})
}



// Find a value in an array of objects 
// https://stackoverflow.com/a/12462414/104380
function getAccommodationDataById( id ){
  return accommodationsData.find(item => item.Id == id)
}



function createAccommodation() {
	const $row = $(this).closest('tr') // get the row element closest in the HTML to the clicked button 
  
	let isValid = true;
	let accommodationData = {
		Ammenities: []
	}
  
	// go over all the inputs and fill the data and then send to the server
	$row.find('input, select').each(function(index, input){
		// if this is a checkbox, get the "for" name for which this belongs (probably "Ammenities") and then push the name of that input to the array
		if (input.type == 'checkbox' && input.checked) {
			accommodationData[input.dataset.for].push( input.name )
		}
		else{
			accommodationData[input.name] = input.value

			// if value not defined, do not continue
			if( !input.value ){
				isValid = false
			}
		}
	})

	// only create if all data is valid
	if (!isValid) {
		return; 
	}

	$row.addClass('loading')

	API.create(accommodationData)
		.done(function( data ){ // "data" should be returned with Id
			const $template = getAccommodationTemplate(data)

			// add template to the top of the list, after the first "tr" which is the template for adding new ones
			$accommodations.find("> tbody > tr:first").after($template)  

			// add it to the array of existing accommodations
			accommodationsData.push(data) 
		})

		.fail(function(err) {
			alert("error adding accommodation")
		})

		.always(function () {
			$row.removeClass('loading')
		})

	return false // prevents the form to refresh the page 
}



function updateAccommodation() {
  const $row = $(this).closest('tr') // get the row element closest in the HTML to the clicked button 
  const id = $row.data('id') // get "id" of accommodation from row element
  const accommodationData = getAccommodationDataById(id)

  // no point in continuing if these are empty for some reason
  if( !id || !accommodationData ){ 
	return false; 
	console.warn("no id or accommodationData", id, accommodationData)
  }

  $row.addClass('loading') // set row "loading" state until the ajax finish

  const fieldName = this.name;
  const fieldValue = this.type == 'checkbox' ? this.checked : $(this).val()
  const fieldFor = this.dataset.for;

  // add/remove item from "Ammenities" array
  if (fieldFor == 'Ammenities') {
	// add ammenity to the array 
	if (fieldValue) {
		accommodationData.Ammenities.push(fieldName)
	}
	else {
	  // return a new array which filters only the items that are not the one that needs to be removed (fieldName)
	  accommodationData.Ammenities = accommodationData.Ammenities.filter(function (item) {
		return item != fieldName
	  })
	}
  }
  
  // if the field name exists already in accommodationData, change it
  else if( fieldName in accommodationData ){
	accommodationData[fieldName] = fieldValue
  }

  API.update(accommodationData)
	.fail(function(err) {
		alert("error updating accommodation")
	})

	.always(function () {
	  $row.removeClass('loading')
	})
}


function showUnitsModal( accommodationId ) {
	let $modal = $('.modal--units');
	const accommodationData = getAccommodationDataById(accommodationId)

	// if the "units" modal already exists in the HTML (loaded from before), use it and load new content into it
	if ($modal.length) {
	$(document).trigger('modal:show', {$modal, cleanup:true})
	loadUnitsIntoModal(accommodationId)
	return;
	}
  
	// else, create new modal and load the HTML template file into it
	$.get( "views/units.html", function( HTML ) {
		// show loader inside modal
		createModal(
			HTML.replace("#ACCOMMODATION#", "<span class='font-normal mr-l'>"+accommodationData.Name+"</span>"), 
			{
				addToBody: true, 
				show: true, 
				modalContentClass: 'p-xl'
			})


		units.init( accommodationId )
	});
}

function onUnitsChange(e, {unitsData, AccommodationId}) {
	// find the row with that ID and update the number of units
	$accommodations.find(`tr[data-id=${AccommodationId}]`).find('.units-btn').text( unitsData.length )
}