$(document)
	.on('click', '.modal__closeBtn', closeModal)
	.on('modal:show', showModal)
	.on('modal:show:login', showLogin)


function closeModal(){
  $(this).closest('.modal').addClass('hide')
  $(document.body).removeClass('no-overflow')
}

function showModal(e, {$modal, cleanup}) {
  $(document.body).addClass('no-overflow')

  if (cleanup) {
	$modal.find('.modal__closeBtn').nextAll('div').remove()
  }

  $modal.removeClass('hide')
}

export function createModal( content, {addToBody, show, onClose, modalClass = '', modalContentClass = ''}) {
	const $modal = $(`
		<div class='modal p-xl ${modalClass} hide'>
			<div class='modal__content ${modalContentClass}'>
				<button class='modal__closeBtn btn-unset'>&times;</button>
			</div>
		</div>`
	);

	$modal.find('.modal__closeBtn').on('click', onClose)

	if (content) {
		$modal.find('.modal__content').append( content )
	}

	if (addToBody) {
		$(document.body).append( $modal )
	}

	if (show) {
		showModal(null, {$modal})
	}

	return $modal
}




function showLogin(){
	if( $('.modal--login-register').length ){
		$(document).trigger('modal:show', { $modal: $('.modal--login-register') })
	}
}