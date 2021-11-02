import { getTemplate } from '../general/getTemplate.js';

export const API = {
	getAll( packageId ) {
		return $.ajax({
			url: "api/Comment?id=" + packageId,
			type: "GET",
			dataType: "json",
			async : false,
		})
	},

	/*
		TouristUsername
		TravelPackageId
		Content
		Rating (1-5)
	 */
	create( data ) {
		return $.ajax({
			url: "api/Comment/Create",
			type: "POST",
			dataType: "json",
			data
		})
	},

	/*
		Id (comment Id)
		Status (APPROVED / DENIED)
	 */
	approve( data ) {
		return $.ajax({
			url: "api/Comment/Create",
			type: "PUT",
			dataType: "json",
			data
		})
	}
}

/* single comment */
export function getCommentTemplate( commentData ) {
	const $template = $( getTemplate('#comment-template') )
	const $comment = $template.find("> .comment")

	$comment.attr('data-id', commentData.Id).attr('data-package-id', commentData.TravelPackageId)


	$comment.find('.approve-label').attr('for', commentData.Id) // should be same as below

	$comment.find('.approve-comment')
		.attr('id', commentData.Id)  // set the id for this input field
		.prop('checked', commentData.Status == 'APPROVED')

	$comment.find('.comment__by').text(commentData.TouristUsername + ":")
	$comment.find('.comment__text').text(commentData.Content)
	$comment.find('.comment__rating').text('★★★★★'.slice(0, commentData.Rating))
	
	// commentData.Rating
	// $comment.find('')
	
	return $template
}