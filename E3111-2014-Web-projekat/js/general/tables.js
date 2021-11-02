$(document).on('click', '.toggle-table-row', toggleTableRow)

function toggleTableRow() {
	const $row = $(this).closest('tr')
	const $rowToToggle = $row.next('tr.table-toggle-target')
	
	$rowToToggle.toggleClass('hide')
}