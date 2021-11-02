// https://developer.mozilla.org/en-US/docs/Web/HTML/Element/template
// https://stackoverflow.com/a/46699845/104380
export function getTemplate( selector ){
  return document.querySelector(selector).content.cloneNode(true);
}