import '../general/index.js';
import loader from '../general/loader.js'
import { filterForm } from '../general/forms.js';
import { getTemplate } from '../general/getTemplate.js';

let unitsData;

export const API = {
	getAll( data ) {
		return $.ajax({
			url: "api/AccommodationUnit/Added",
			type: "GET",
			dataType: "json",
			async: false,
			data
		})
	},

	delete( id ) {
		return $.ajax({   
			url: "api/AccommodationUnit/DeleteUnit",
			data: {
				id,
				AccommodationId
			},
			async:false,
			type: "GET"
		})
	},

	update( data ) {
		return $.ajax({
			url: "api/AccommodationUnit/Create",
			type: "PUT",
			async: false,
			data: {...data, AccommodationId},
		})
	},

	create( data ) {
		return $.ajax({
			url: "api/AccommodationUnit/Create",
			type: "POST",
			async: false,
			data: {...data, AccommodationId},
		})
	},
}

// defined empty variables which will be filled after units.html file will be loaded:
// 1. from the "accommodations" page (by managers)
// 2. from a packahe page (by anyone)
let $units;
let $FilterForm;
let AccommodationId;

// this is called from the accommodations page when it loads the modal for the units
export function init( accId ) {
	$units = $('.units')
	AccommodationId = accId  // remeber this id to use later when searching/editing/deleting

	// bind events for filtering / sorting and other stuff
	filterForm($('.units-filter'), {
		// call this function when the filter (search) form submited 
		onSubmit: function(data){
			return getUnits(data)  // must return something so the page won't refresh when submiting a form
		}, 

		// call this function when the search form was reset
		onReset: function(){
			return getUnits()
		}, 

		$sorters: $('.sortBy') // use these sorters
	})
	
	$('.units')
		.on('submit', $FilterForm, () => getUnits())  
		.on('click', '.delete-btn', function(){
			if( confirm("Are you sure?") ){
				const $row = $(this).closest('tr')
				deleteUnit( $row  )
			}
		})
		.on('click', '.create-btn', createUnit)
		.on('change', 'input, select', updateUnit)
		
	// finally load the data
	getUnits()
}


// print messages like errors or loading
function message(msg = "") {
	$('.units').find("tfoot td").html(msg)
}

// get units
export function getUnits( searchData ) {
	message( loader ); // cleaup any previous error message

	return API.getAll({ id: AccommodationId, ...searchData }) 
		.done(function (data) {
			unitsData = data;
			renderUnitsEdit(data)
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


function getUnitTemplate( data ) {
	// Instantiate the table with the existing HTML tbody
	// and the row with the template
	const $template = $( getTemplate('#editable_unit_template') );
	const td = $template.find("td");

	// fill the template with data (first row has no Id, so skip it)
	if( data.Id ){
		// set the room unit ID and the accommodation id on the <tr> element
		$template.find('tr').attr('data-id', data.Id)

		td.eq(1).text(data.Capacity)

		if (data.IsAvailable) {
			td.eq(2).text('✔️')
		}

		td.eq(3).text( data.Ammenities.join(', ') )
		td.eq(4).text(data.Price)
	}
	else {
		  $template.find('button').addClass('hide') // hide all buttons
		  $template.find('.create-btn').removeClass('hide') // show only the "add" button
	}

	return $template
}


function getEditableUnitTemplate( data ) {
	// Instantiate the table with the existing HTML tbody
	// and the row with the template
	const $template = $( getTemplate('#editable_unit_template') );
	const td = $template.find("td");

	// fill the template with data (first row has no Id, so skip it)
	if( data.Id ){
		// set the room unit ID and the accommodation id on the <tr> element
		$template.find('tr').attr('data-id', data.Id)

		$template.find('input[name="Capacity"]').val(data.Capacity)
		$template.find('input[name="Price"]').val(data.Price)

		if (data.IsAvailable) {
			td.eq(2).text('✔️')
		}

		if( data.Ammenities ){
			data.Ammenities.forEach(function(ammenity){
				$template.find('input[name="' + ammenity + '"]').prop('checked', true)
			})
		}
	}
	else {
		  $template.find('button').addClass('hide') // hide all buttons
		  $template.find('.create-btn').removeClass('hide') // show only the "add" button
	}

	return $template
}

// render units
// https://developer.mozilla.org/en-US/docs/Web/HTML/Element/template
export function renderUnitsEdit( unitsData = [], AccommodationId ) {
	const $units = $('.units');

	$units.find("tbody").empty()

	// if no search results found
	if (unitsData.length == 0) {
		message("No results")
	}

	message() // empty the message to be safe

	// push an empty template to the begining of the array that will be the row which adds new unit:
	// https://stackoverflow.com/a/8159547/104380
	unitsData.unshift({})

	// itterate all accommodations and create table rows
	unitsData.forEach((data) => {
		// Instantiate the table with the existing HTML tbody
		// and the row with the template
		const $template = getEditableUnitTemplate(data)
	
		// add template to the list
		$units.find("tbody").append($template)  
	})
}



function deleteUnit( $row ) {
	$row.addClass('loading')

	const id = $row.data('id') // get "id" of accommodation from row element

	if( !id ) return false;

	API.delete(id)
		.done(function() {
			$row.remove(); // removes the row from HTML

			// remove that unit from the unitsData array
			unitsData = unitsData.filter(function (data) {
				return data.Id != id
			})

			$('.accommodations').trigger('units:change', {unitsData, AccommodationId})
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
function getUnitDataById( id ){
  return unitsData.find(item => item.Id == id)
}


function updateUnit() {
	const $row = $(this).closest('tr')
	//.addClass('loading') // get the row element and set it as "loading" until the ajax finish

	const id = $row.data('id') // get "id" of accommodation from row element

	const unitData = getUnitDataById(id)
	let updatedUnitData = {}


	// no point in continuing if these are empty for some reason
	if( !id || !unitData ){ 
		return false; 
		console.warn("no id or accommodationData", id, unitData)
	}

	// clone this data, and after the ajax is done, replace the unitsData array item with the updated one
	for (let key in unitData) {
		updatedUnitData[key] = unitData[key]
	}

	const fieldName = this.name;
	const fieldValue = this.type == 'checkbox' ? this.checked : $(this).val()
	const fieldFor = this.dataset.for;

	// add/remove item from "Ammenities" array
	if (fieldFor == 'Ammenities') {
		// add ammenity to the array 
		if (fieldValue) {
			updatedUnitData.Ammenities.push(fieldName)
		}

		else {
			// return a new array  which filters only the items that are not the one that needs to be removed (fieldName)
			updatedUnitData.Ammenities = updatedUnitData.Ammenities.filter(function (item) {
				return item != fieldName
			})
		}
	}
  
	// if the field name exists already in accommodationData, change it
	else if( fieldName in updatedUnitData ){
		updatedUnitData[fieldName] = fieldValue
	}

	API.update(updatedUnitData)
		.done(function () {
			// copy the update to the state array after successful server update
			for (let key in updatedUnitData) {
				unitData[key] = updatedUnitData[key]
			}
		})

		.fail(function() {
			alert("Error updating accommodation")
		})

		.always(function () {
			 $row.removeClass('loading')
		})
}


function createUnit() {
	const $row = $(this).closest('tr') // get the row element closest in the HTML to the clicked button 
  
	let isValid = true;
	let unitData = {
		Ammenities: []
	}
  
	// go over all the inputs and fill the data and then send to the server
	$row.find('input, select').each(function(index, input){
		// if this is a checkbox, get the "for" name for which this belongs (probably "Ammenities") and then push the name of that input to the array
		if (input.type == 'checkbox' && input.checked) {
			unitData[input.dataset.for].push( input.name )
		}
		else{
			unitData[input.name] = input.value

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

	API.create(unitData)
		.done(function( data ){ // "data" should be returned with Id
			const $template = getEditableUnitTemplate(data)
	
			// add template to the top of the list, after the first "tr" which is the template for adding new ones
			$units.find("> tbody > tr:first").after($template)  
	
			unitsData.push(data) // update the array of units with the new unit

			// clear the form
			createPackageForm.reset()

			$('.accommodations').trigger('units:change', {unitsData, AccommodationId})
		})

		.fail(function(err) {
			alert("error adding unit")
		})

		.always(function () {
			$row.removeClass('loading')
		})
}
