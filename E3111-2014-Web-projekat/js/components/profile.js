import { getFormData } from '../general/forms.js';

const $form = $('.profile-form') // users list
const $message = $('.profile-form__message')

let userData;

const API = {
	getUserDetails() {
		return $.ajax({
			url: "api/Account/Profile",
			type: "GET",
			dataType: "json",
			async : false,
		})
	},

	updateProfile() {
		return $.ajax({
			url: "api/Account/UpdateProfile",
			type: "POST",
			dataType: "json",
			async: false,
			data: getFormData($form)
		})
	}
}
// bind events
$('.profile-form').on('submit', updateProfile)


loadDetails(); // on load it will try to get details, if there is a loggedin user

function loadDetails() {
	return API.getUserDetails()
		.done(function (data) {
			userData = data;
			renderUser(data)
		})

		.fail(function (err) {
		   /// message("Something went wrong");

			if (err.status == 302) {
				window.location = err.getResponseHeader('FORCE_REDIRECT');
				return;
			};
		})

}

function renderUser( userData ) {
	// cleaup previous users list
	const form = $('.profile-form')
	

	// fill data from server in relevant fields
	if (userData) {
		form.find('[name="username"]').val(userData.Username)
		form.find('[name="name"]').val(userData.Name)
		form.find('[name="lastname"]').val(userData.Lastname)
		form.find('[name="email"]').val(userData.Email)
		form.find('[name="usergender"]').val(userData.UserGender)
		form.find('[name="role"]').val(userData.UserRole)
		form.find('[name="status"]').val(userData.Status)
		form.find('[name="birthday"]').val(userData.Birthday.split('T')[0])
	}
	
}

function validateForm( form ) {
  // get all form fields: https://stackoverflow.com/a/4431223/104380
  const fields = form.elements
  
  // if passswords don't match
  if (fields.newpassword.value != fields.confirmpassword.value) {
	// setCustomValidity: 
	// https://stackoverflow.com/a/61873952/104380
	// https://developer.mozilla.org/en-US/docs/Web/API/HTMLObjectElement/setCustomValidity
	fields.confirmpassword.setCustomValidity('Passwords do not match')
	fields.confirmpassword.reportValidity()
	return false
  }

  return true
}

function updateProfile() {
	const isValid = validateForm(this)

	// if NOT valid
	if ( !isValid ) {
		return false; // do not continue
	}

	return API.updateProfile()
		.done(function (data) {
			userData = data;
			renderUser(data)
		})

		.fail(function (err) {
		   // message("Something went wrong");

			if (err.status == 302) {
				window.location = err.getResponseHeader('FORCE_REDIRECT');
				return;
			};
		})
}