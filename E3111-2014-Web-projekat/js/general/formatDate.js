// Exmaple: from any date format to "June 25, 2021"
// https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Date/toLocaleDateString
export function formatDate(date) {
  return new Date(date).toLocaleDateString(undefined, {year: 'numeric', month: 'long', day: 'numeric' })
}

// https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Intl/DateTimeFormat/DateTimeFormat#examples
export function formatTime(date) {
  return new Intl.DateTimeFormat("en", {timeStyle: "short"}).format( new Date(date) )
}

// checks if first date is before second. second defaults to "now" timestamp
export function isDateBefore( date1, date2 = Date.now() ){
	return  new Date(date1).getTime() < new Date(date2).getTime()
}