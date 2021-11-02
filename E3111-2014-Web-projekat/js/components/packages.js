import { filterForm } from '../general/forms.js'
import { getTemplate } from '../general/getTemplate.js'
import loader from '../general/loader.js'
import { createModal } from '../general/modals.js'
import { formatDate, isDateBefore } from '../general/formatDate.js'

const API = {
	search( data ) {
		return $.ajax({
			url: "api/TravelPackage/Search",
			type: "POST",
			async: false,
			data: JSON.stringify(data),
			contentType: "application/json",
			dataType: "json",
		})
	},

	// only those not finished (in past)
	currentPackages(){ 
		return $.ajax({
			url: 'api/TravelPackage/Current',
			type: "GET",
			dataType: "json",
		})
		.fail(function (err, textStatus, xhr) {
			message("Something went wrong");
		})
	},

	editPackage( data ) {
		return $.ajax({
			url: 'api/TravelPackage/Create',
			type: "PUT",
			data,
		})
		.fail(function (err, textStatus, xhr) {
			alert("Something went wrong");
		})
	},

	deletePackage( id ) {	
		return $.ajax({
			url: 'api/TravelPackage/DeletePackage?id=' + id,
			type: "DELETE"
			
		})
		.fail(function (err, textStatus, xhr) {
			alert("Something went wrong");
		})
	}
}


const $packages = $('.travel-packages')
const $filterForm = $('.packages-search')

// reset the packages search form when page loads.
// needs a timeout because it doesn't workout without for some reason...
setTimeout(function(){
	$filterForm.trigger('reset')
}, 0)

$packages
	.on('click', '.edit-package-btn', editPackage)
	.on('click', '.delete-package-btn', deletePackage)


// open edit modal
function editPackage(){
	const $package = $(this).closest('.package')
	const data = $package.data('package')

	let $modal = $('.modal--edit-package');

	// if modal was already loaded before (jquery selector found it), show same one again
	if ( $modal.length ) {
		$(document).trigger('modal:show', { $modal })
		$(document).trigger('modal:manage-packages', {$modal, data}) 
		return;
	}

	// else, create new modal and load the HTML template file into it
	$.get( "views/manage-packages.html", function( editPackageHTML ) {
		$modal = createModal(editPackageHTML, {addToBody:true, show:true, cleanup:true, modalClass:'modal--edit-package'})
		$(document).trigger('modal:manage-packages', {$modal, data}) 
	});
}

function deletePackage(){
	const $package = $(this).closest('.package').addClass('opacity-05 lock')
	const id = $package.attr('data-id')

	if( confirm("Are you sure?") ){
		API.deletePackage( id )
			.done(function () {
				$package.hide(600, function () {
					$package.remove() // remove thr HTML of this package after hiding is finihsed
				})
			})

			// if not "done" for some reason
			.always(function () {
				$package.removeClass('opacity-05 lock')
			})
	}
}


// calls a global "filterForm" function which is for all forms that do search for another component
filterForm( $filterForm, { 
	onSubmit: searchPackages, // call this function when the filter (search) form submited 
	onReset: searchReset,  // call this function when the search form was reset
	$sorters: $('.packages-sort .sortBy') // use these sorters
}) 

$('.packages-filter input').on('change', function(){
	// when changing the inputs of the packages filter, by clicking one (current/paste/byUser)
	// submit the search form automatically - and new data will come
	$filterForm.submit()
})



// get packages on page load
loadPackages()


// print message like "loading" or "error" 
function message( msg ) {
	$packages.html("<p class='text-center text-2xl'>" + msg + "</p>")
}

export function loadPackages() {
	message( loader ); 

	return API.search().done(function (data) {
		renderPackages(data)
	})
}


function getPackageTemplate( data ) {
	// fill package template HTML with data
	const $template = $( getTemplate('#travel-package-template') )
	const $actionsTemplate = $( getTemplate('#travel-package-actions-template') )

	// put comma in thousands for price
	// https://stackoverflow.com/a/32154217/104380
	const price = data.LowestPrice.toLocaleString()

	const isPastPackage = isDateBefore(data.StartDate)

	$template.find('> .package')
		.attr('data-id', data.Id) // set attribute of the id
		.data('package', data) // set the whole package data on the jQuery element (to be able to edit later)

	$template.find('.package-cover-img').attr('src', data.Photos).parent('a').attr('href', '/package.html?id=' + data.Id)
	$template.find('.package-title').text(data.Name)
	$template.find('.package-description').text(data.Description)
	$template.find('.package-price').text("From $" + price).attr('href', '/package.html?id=' + data.Id)
	$template.find('.package-dates').find('span')
		.eq(0).text( formatDate(data.StartDate) )
		.next('span').text( formatDate(data.EndDate) )

	if( isPastPackage ){
		$template.find('.package-price').text('View')
	}

	if (data.CreatedByMe) {
		$template.find('.content').append( $actionsTemplate )
	}

	return $template
}

// https://developer.mozilla.org/en-US/docs/Web/HTML/Element/template
// render packages data
export function renderPackages( packagesData ) {
	$packages.empty() // cleaup before everything

	// if no search results found
	if (packagesData.length == 0) {
	  message("No Packages Found")
	  return
	}

	packagesData.forEach(addPackageToPage)
}

function addPackageToPage( data ) {
	const $template = getPackageTemplate(data);
	$packages.append($template)
}


// filtering & sorting of packages
function searchPackages( searchData ) {
	return API.search( searchData )
		.done(function( packagesData ) {
			renderPackages( packagesData );
		})
}

function searchReset() {
	loadPackages()
}