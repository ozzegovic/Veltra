export function getFormData($form) {
	var keyValues = {};

	$form.serializeArray().forEach(entry => {
		keyValues[entry.name] = entry.value
	})

	return keyValues;
}

// first argument is the form element (jquery selector) 
// second argument is an Object with two keys: onFormSubmit, onFormReset
export function filterForm( $form, {onSubmit, onReset, $sorters}) {
	// bind events
	$form
		.on('submit', function(e){
			e.preventDefault(); // do not refresh the page when form is submited
			onFormSubmit()
		})
		.on('reset', onFormReset)

	// if there are also sodting buttons, check when they are clicked, and then run the search again
	if ($sorters) {
		$sorters.on('click', onSortClick)
	}

	function onSortClick() {
		$sorters.removeClass('active')  // remove any previous "active" class from other siblings sort buttons 
	
		$(this) // the sort element which was clicked
			.addClass('active') // add it a class of "active"  
			.attr('data-order', this.dataset.order == 'asc' ? 'desc' : 'asc') // flip the order
			
		onFormSubmit({
			sortBy: this.dataset.by,
			sortOrder: this.dataset.order
		})
	}

	function onFormReset(e) {
	  // wait for the form to reset and then call the "packagesSearch" 
	  setTimeout(() => {
		onReset();
	  })
	}

	function onFormSubmit( extraData = {} ) {
		const formData = getFormData($form);
		const $submitBtn = $form.find('button[type="submit"]').attr('loading', true);

		
		// creates an object all formData and all extraData together
		// https://stackoverflow.com/a/171256/104380
		const submitResult = onSubmit({ ...formData, ...extraData})

		
		// if this is an AJAX async promise
		if (submitResult && submitResult.then) {
			submitResult.always(function () {
				$submitBtn.removeAttr('loading')
			})
		}
		else {
			$submitBtn.removeAttr('loading')
		}

		return false; // prevert page refresh when submitting the form
	}
}