import './general/index.js'
import loader from './general/loader.js'
import { filterForm } from './general/forms.js';
import { getTemplate } from './general/getTemplate.js';

const $usersFilterForm = $('.form.users-filter')
const $users = $('.users') // users list
let usersData = [];

const API = {
	// data is set to this defaults until API is fixed to work when no reuqest has no data
	getUsers( data = {Name:'', Lastname:'', filterBy:'', Role:''}) {
		return $.ajax({
			url: "api/Admin/GetUsers",
			type: "GET",
			dataType: "json",
			async : false,
			data
		})
	},

	deleteUser(data) {
		return $.ajax({
			url: "api/Admin/DeleteUser?username=" + data,
			type: "DELETE"
		})
	},

	updateUser(data) {
		return $.ajax({
			url: "api/Admin/UpdateRole",
			type: "PUT",
			data,
		})
	},

	lockUser(data) {
		return $.ajax({
			url: "api/Admin/LockUser",
			type: "PUT",
			data,
		})
	}
}


// bind events
filterForm($usersFilterForm, {
	onSubmit: loadUsers, // call this function when the filter (search) form submited 
	onReset: loadUsers,  // call this function when the search form was reset
	$sorters: $('.sortBy') // use these sorters
})

$users
  .on('click', '.btn-delete', deleteUser)
  .on('change', '.user-rule', updateUserRole)
  .on('click', '.btn-lock', lockUser)

// when the page first loads, get all users and render them
loadUsers()

// get users
function loadUsers( searchData  ) {
	message( loader ); // cleaup any previous error message

	return API.getUsers( searchData )
		.done(function (data) {
			usersData = data;
			renderUsers(data)
		})

		.fail(function (err) {
			message("Something went wrong");

			if (err.status == 302) {
				window.location = err.getResponseHeader('FORCE_REDIRECT');
				return;
			};
		})
}

function getMockUserImageUrl(gender, idx) {
  return `https://randomuser.me/api/portraits/${gender == 'male' ? 'men' : 'women'}/${idx}.jpg`
}

// print error message
function message( msg = '' ) {
	$users.find("tfoot td").html(msg)
}
// render users from template
// https://developer.mozilla.org/en-US/docs/Web/HTML/Element/template
function renderUsers( usersData ) {
	// cleaup previous users list
	$users.find("tbody").empty()
	
	// if no search results found
	if (usersData.length == 0) {
		message("No results")
		return
	}

	message()

	usersData.forEach((userData, idx) => {
		// Instantiate the table with the existing HTML tbody
		// and the row with the template
		const template = getTemplate('#user_row_template');
		const td = $(template).find("td");


		template.firstElementChild.setAttribute('data-user', userData.Username)
		td.eq(1).html($('<img>').attr('src', getMockUserImageUrl(userData.UserGender, idx)))
		td.eq(2).text(userData.Username)
		td.eq(3).text(userData.Name)
		td.eq(4).text(userData.Lastname)
		td.eq(5).text(userData.Email)
		td.eq(6).find('select').val(userData.UserRole)
		td.eq(7).text(userData.Status)

		if( userData.Status == 'DENIED' ){
			td.eq(7).addClass('text-red-3')
		}

		$users.find("tbody").append(template)  
	})
}



// needs to terminate the user's cookie also
function deleteUser() {
	const $row = $(this).closest('tr').addClass('loading')
	const username = $row.data('user')

	if( !username ) return false;

	API.deleteUser(username)

	.done(function() {
		// refresh page
		location.reload();
	})

	.fail(function(err) {
		$row.removeClass('loading')
		alert("error deleting user")
	})
}



function updateUserRole() {
	const $row = $(this).closest('tr').addClass('loading')
	const username = $row.data('user')
	const value = $(this).val()

	if( !username || !value ) {
		return false;
	}

	API.updateUser({ role: value, username })
  
	.done(function() {
		// refresh pageto 
		location.reload();
	})

	.fail(function(err) {
		alert("error updating user " + username)
	})

	// once the request finishes, no matter if failed or succeeded, remove the "loading" class
	.always(function () {
		$row.removeClass('loading')
	})
}

function getUserData(username) {
	return usersData.find(user => user.Username == username)
}

function lockUser() {
	const $row = $(this).closest('tr').addClass('loading')
	const username = $row.data('user')
	const userData = getUserData(username);

	if (!userData) {
		return false;
	}

	const isLocked = userData.Locked

	if (!username) {
		return false;
	}

	API.lockUser({username})

	.done(function () {
		// refresh page
		location.reload();
	})

	.fail(function (err) {
		$row.removeClass('loading')
		alert("error locking user")
	})

	.always(function () {
	})
}

