// https://stackoverflow.com/questions/901115/how-can-i-get-query-string-values-in-javascript
// parse cookies in the browser from string to object with keys and values
export function parseQueryParams(str) {
	var urlSearchParams = new URLSearchParams(str);

	for(var value of urlSearchParams.values()) {
		console.count("x")
		console.log(value);
	}
	return Object.fromEntries(urlSearchParams.entries());
}

// https://stackoverflow.com/a/37856924/104380
export function getCookieDetails() {
	return document.cookie.split(";")
		.map(function(cookieString) {
			return cookieString.trim().split("=");
		})
		.reduce(function(acc, curr) {
			acc[curr[0]] = decodeURIComponent(curr[1])
			return acc;
		}, {});
}