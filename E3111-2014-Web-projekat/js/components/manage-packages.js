import { getFormData } from '../general/forms.js'
import { loadMap } from '../general/map.js'

$(document).on('modal:manage-packages', onAddEditPackageModalLoad) // listen to global event which fired from "header.js"

let packageData;
let accommodationsData;

function onAddEditPackageModalLoad(e, { $modal, data }) {
	// if in edit mode, "data" will be available from "packages.js" -> "editPackage()"
	// else if was opened from the "header.js" by clicking "add package" button, then "data" is undefined
	packageData = data; 

	const $AccommodationIds = $modal.find("select[name='AccommodationIds']");

	// remove any previous message
	$modal.find('.message').empty();
	
	// get all possible accommodations 
	$.ajax({
		url: "api/Accommodation/All",
		type: "GET",
		dataType: "json",
		async : false,
	})
		.done(function( accommodationsData ){
			accommodationsData.forEach(function(accData){
				$AccommodationIds.append( $(`<option value="${accData.Id}">${accData.Name}</option>`) )
			})

			// if modal is loaded with "data" (when editing a package), set the field value only after retrieved all the accommodations data
			if (data && data.AccommodationIds) {
				$modal.find('[name="AccommodationIds"]').val( data.AccommodationIds )
			}
		})

	$modal
		.off() // remove all previous events and bind new ones each time the modal is opened
		.on('submit', '.package-details', onPackageUpdateCreate)
		.on('change', 'input[type="file"]', previewUpload)
		.on('click', '.delete-photo-btn', deletePhoto)

	let lonLat;

	// cleaup any previous values if modal was already opened
	$modal.find('form').trigger('reset') // jquery form reset: https://stackoverflow.com/a/24505433/104380
	$modal.find('img').removeAttr('src')
	$modal.find('#package-meeting-map').empty() 
		
	if( data ){
		if( data.Location ){
			lonLat = [data.Location.Longitude, data.Location.Latitude]
		}

		$modal.find('[name="Name"]').val( data.Name )
		$modal.find('[name="Destination"]').val( data.Destination )
		$modal.find('[name="StartDate"]').val( data.StartDate.split('T')[0] )
		$modal.find('[name="EndDate"]').val( data.EndDate.split('T')[0] )
		$modal.find('[name="Description"]').val( data.Description )  
		$modal.find('[name="PackageType"]').val( data.PackageType )
		$modal.find('[name="TransportationType"]').val( data.TransportationType )
		$modal.find('[name="MaxCapacity"]').val( data.MaxCapacity )
		$modal.find('[name="City"]').val( data.Location.Address.City )
		$modal.find('[name="Street"]').val( data.Location.Address.Street  )
		$modal.find('[name="Number"]').val( data.Location.Address.Number  )
		$modal.find('[name="PostalCode"]').val( data.Location.Address.PostalCode  )
		$modal.find('[name="Time"]').val( data.Time.split('T')[1] )
		$modal.find('[name="Itinerary"]').val( data.Itinerary )
		$modal.find('img').attr('src', data.Photos )
	}

	// load map 
	// (pass the HTML element id to render it in, and the coorditades)
	const map = loadMap('package-meeting-map', {
		lonLat: lonLat || [20, 45.33], // fallback to center on Serbia
		zoom: 7
	})


	function onMapClick(e) {
		// convert coordinates:
		// https://stackoverflow.com/a/52070033/104380
		const lonLat = ol.proj.transform(e.coordinate, 'EPSG:3857', 'EPSG:4326')
		setPosition(lonLat)
	}

	function setPosition( lonLat ){
		$modal.find('.package-details input[name="Longitude"]').val( lonLat[0] )
		$modal.find('.package-details input[name="Latitude"]').val( lonLat[1] )
		$modal.find('.lonLat').text( lonLat.join(', ') )
	}

	if( lonLat ){
		setPosition(lonLat)
	}

	map.on('click', onMapClick);
}


function previewUpload() {
	const file = this.files[0]

	if (file) {
		$(this).parent().find('img').attr('src', URL.createObjectURL(file))
	}
}

function deletePhoto(e) {
	e.preventDefault() // do not open the image uploader

	// "this" is the delete button which originated this callback
	$(this).parent().find('img').removeAttr('src')
	$(this).parent().find('input').val('')
}


function validate( $form ) {
	// if in "edit" mode
	if( packageData ){
		// if the image was deleted just now by the user
		if( !packageData.Photos ){
			return { text: "Missing photo",	color: "red" }
		}
	}
	// if in "add package" for creating new one:
	else if( $form.find('[name="image1"]').val() == "" ) {
		return { text: "Missing photo",	color: "red" }
	}

	const $lon = $form.find('input[name="Longitude"]')
	const $lat = $form.find('input[name="Latitude"]')
	// lon/lat validation
	if ($lon.val() == '' || $lat.val() == '') {
		return { text: "Missing Lon/Lat: Click the map to select location",	color: "red" }
	}

	return true // is valid, all good
}

window.xxx = onPackageUpdateCreate

function onPackageUpdateCreate() {
	const $form = $(this)
	const $message = $form.find('.message')
	const isValid = validate( $form )

	// validate image field & lon/lat manually
	if( isValid != true ){
		$message.html(`<span class='text-${isValid.color}-3'>${isValid.text}</span>`)
		return false; // do not continue
	}

	const $submitBtn = $form.find('button[type="submit"]').attr('loading', true)
	const formData = new FormData(this);
	const inputs = $form.find(":input:not([type=file]):not([type=submit]):not([type=reset])");
	const accIds = $form.find('#AccommodationIds').val(); 
	const data = getFormData(inputs);

	data["AccommodationIds"] = accIds;

	// if modal was opened in "edit" mode then "packageData" will be available
	if ( packageData && packageData.Id ) {
		data["Id"] = packageData.Id;
	}

	formData.append('data', JSON.stringify(data));

	$.ajax({
		url: "api/TravelPackage/Create",
		type: packageData ? "PUT" : "POST",
		processData: false,
		contentType: false,
		async: false,
		data: formData
	})
		.done(function( data ) {
			window.location = '/' // reload the page
		})

		.fail(function(err) {
			alert("something went wrong")
		})

		.always(function () {
			$submitBtn.removeAttr('loading')
		})
}

