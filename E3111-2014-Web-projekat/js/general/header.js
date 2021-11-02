import { getCookieDetails } from './parseQueryParams.js';
import { createModal } from './modals.js';

const currentUser = getCookieDetails()




function fakeUserPhotoById() {
	const gender = currentUser.User.split(',')[0]
	const userIdNumber = currentUser.User.split(',')[1].replace( /\D+/g, '').slice(0,2)

	if( userIdNumber && gender ){
		$('.profile-image').attr('src', `https://randomuser.me/api/portraits/${gender == 'MALE' ? 'men' : 'women'}/${userIdNumber}.jpg`)
	}
}



// show these elements only for users with spepcifc roles
$('[data-show-for]').each(function (idx, element) {
	if (element.dataset.showFor.indexOf(currentUser.role) != -1) {
		element.classList.remove('hide')
	}
})

// bind events
$(document)
  .on('click', '.logout-btn', logout)
  .on('click', '.login-btn', showLogin)
  .on('click', '.add-package-button', showAddPackage)

let $loginRegisterModal;

// load the login-register modal on page load
$.get( "views/login-register.html", function( html ) {
	$loginRegisterModal = createModal(html, {addToBody:true, modalClass:'modal--login-register'})
});


function showLogin(){
	// a listener for global event "modal:show:login" is in modals.js
	$(document).trigger('modal:show:login')
}

if (currentUser && currentUser.loggedIn) {
	$('.main-header__user-name').text(currentUser.loggedIn)
	$('.main-header__user').removeClass('hide')
	fakeUserPhotoById()
}

function logout(e) {
	$.get("api/Account/logout")
		.done(function () {
			$('.main-header__user-name').empty()
			$('.main-header__user').addClass('hide')
			window.location = '/'
		})
}

function showAddPackage() {
	let $modal = $('.modal--edit-package');

	// if modal was already loaded before, show same one again
	if ( $modal.length ) {
		$(document).trigger('modal:show', { $modal })
		$(document).trigger('modal:manage-packages', {$modal}) 
		return;
	}

	// else, create new modal and load the HTML template file into it
	$.get( "views/manage-packages.html", function( createPackageHTML ) {
		const $modal = createModal(createPackageHTML, {addToBody:true, show:true, modalClass:'modal--edit-package'})
		$(document).trigger('modal:manage-packages', {$modal})
	});
}