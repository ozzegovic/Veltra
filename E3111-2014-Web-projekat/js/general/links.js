$(document).on('click', 'a[data-no-refresh]', onLinkClickNoRefresh)
$(document).on('click', '.login-btn', showLogin)


function onLinkClickNoRefresh(){
  const url = this.href
  window.history.replaceState({}, '', url) // change the url of the browser (params, title, url)
  return false // do no actually go to the url
}

function showLogin(){
  $('.modal--login-register').removeClass('hide');
}