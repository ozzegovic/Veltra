import {getFormData} from '../general/forms.js';

// bind events
$(document)
  .on('submit', '.login-form', login)
  .on('submit', '.register-form', register)


/**
 * LOGIN FLOW
 */

function login(e) {
  e.preventDefault();
  const $form = $(".login-form");
  const data = getFormData($form);

  const $loginBtn = $('.login-register__loginBtn').attr('loading', true)
  const $message = $('.login-form__message').empty()

  $.ajax({
      url: "api/Account/login",
      type: "POST",
      async: false,
      data: JSON.stringify(data),
      contentType: "application/json",
      dataType: "text",
  })
  .done(function( data ) {
      window.location.replace("/"); // go back to home page
  })
  .fail(function(err) {
      $message.text('sign-in unsuccessful')
  })
  .always(function () {
      $loginBtn.removeAttr('loading')
  })

  return false; // prevert page refresh when submitting the form
}



/**
 * REGISTER FLOW
 */

function register(e) {
  e.preventDefault();

  const $form = $(".register-form");

  validateForm( $form[0] )

  const data = getFormData($form);
  const $registerBtn = $('.login-register__loginBtn').attr('loading', true)
  const $message = $('.register-form__message').empty()

  $.ajax({
      url: "api/Account/Register",
      type: "POST",
      async: false,
      data: JSON.stringify(data),
      contentType: "application/json",
      dataType: "text",
  })
  .done(function (data) {
      window.location.replace("/"); // go back to home page
  })
  .fail(function (err) {
      $message.text('Sign-up unsuccessful')
  })
  .always(function () {
      $registerBtn.removeAttr('loading')
  })

  return false; // prevert page refresh when submitting the form
}


function validateForm( form ) {
  // get all form fields: https://stackoverflow.com/a/4431223/104380
  const fields = form.elements
  
  // if passswords don't match
  if (fields.password.value != fields.confirmpassword.value) {
	// setCustomValidity: 
	// https://stackoverflow.com/a/61873952/104380
	// https://developer.mozilla.org/en-US/docs/Web/API/HTMLObjectElement/setCustomValidity
	fields.confirmpassword.setCustomValidity('Passwords do not match')
	fields.confirmpassword.reportValidity()
	return false
  }

  return true
}
