const markerUrl = 'https://7icons.files.wordpress.com/2010/08/pin48.png?w=48&h=48'


// openlayers maps, loaded in the HTML file of a page
// https://stackoverflow.com/q/59720237/104380
export function loadMap( mapElement, {lonLat, zoom, pinLocation}) {
	const iconFeature = new ol.Feature({
		geometry: new ol.geom.Point(ol.proj.fromLonLat(lonLat)),
		name: 'Somewsngham',
	});

	const layers = [
		new ol.layer.Tile({
			source: new ol.source.OSM()
		})
	]

	if ( pinLocation ) {
		layers.push(
			new ol.layer.Vector({
				source: new ol.source.Vector({
					features: [iconFeature]
				}),
				style: new ol.style.Style({
					image: new ol.style.Icon({
						anchor: [0.5, 46],
						anchorXUnits: 'fraction',
						anchorYUnits: 'pixels',
						src: markerUrl
					})
				})
			})
		)
	}

	const map = new ol.Map({
		target: mapElement,
		layers: layers,
		view: new ol.View({
			center: ol.proj.fromLonLat(lonLat),
			zoom: zoom || 10,
			minZoom: 2,
			maxZoom: 20,
		}),
    })

	return map
}