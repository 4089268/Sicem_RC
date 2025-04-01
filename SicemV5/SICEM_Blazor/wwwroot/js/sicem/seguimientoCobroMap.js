/**
 * @typedef {Object} OfficeMapMark
 * @property {number} id
 * @property {string} office
 * @property {string} title
 * @property {number} lat
 * @property {number} lon
 * @property {number} bills
 * @property {number} income
 * @property {number} radius
 */

/**
 * @typedef {Object} MapPoint
 * @property {number} latitude
 * @property {number} longitude
 */

const MAPCONTEXT = {
    pushpinColors: {
        "color1": "#8BA6E9",
        "color2": "#7E96C4",
        "color3": "#BED0F4",
    },
    map: null,
    markers: [],
    circles: [],
    initZoom: 11.5,
    clearMarkers: ()=>{
        MAPCONTEXT.markers.forEach(m => {
            MAPCONTEXT.map.removeLayer(m);
        });
        MAPCONTEXT.markers = [];
    },
    clearCircles: ()=>{
        MAPCONTEXT.circles.forEach(m => {
            MAPCONTEXT.map.removeLayer(m);
        });
        MAPCONTEXT.circles = [];
    },
};

/**
 * 
 * @param {any} dotNetHelper 
 * @param {String} elementId 
 * @param {OfficeMapMark} mapPoint 
 */
export function initialize(dotNetHelper, elementId, mapPoint) {
    
    const initZoom = MAPCONTEXT.initZoom;
    
    if( MAPCONTEXT.map != null ) {
        MAPCONTEXT.map.remove();
    }

    // initialize map
    MAPCONTEXT.map = L.map(
        elementId,
        {
            zoomDelta: 0.5,
            minZoom: 9,
            maxBounds: [
                [28.708136, -102.396155],
                [27.362075, -100.263216]
            ]
        }
    ).setView([mapPoint.latitude, mapPoint.longitude], mapPoint.zoom ?? initZoom);
    
    
    // set the layer of streetmaps
    L.tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', {
        maxZoom: 19,
        attribution: '&copy; <a href="http://www.openstreetmap.org/copyright">OpenStreetMap</a>'
    }).addTo(MAPCONTEXT.map);


    // load data panel
    dotNetHelper.invokeMethodAsync('MapLoaded');
}

/**
 * 
 * @param {any} dotNetHelper 
 * @param {MapPoint} point 
 * @param {Number} zoom 
 */
export function moveMap(dotNetHelper, point, zoom) {
    
    MAPCONTEXT.map.setView([point.latitude, point.longitude], zoom !== null && zoom !== void 0 ? zoom : 17);
    //var marker = MAPCONTEXT.markers[point.idCuenta.toString()];
    var marker = MAPCONTEXT.markers.find( item => item.descripcion == point.descripcion);
    if (marker != null) {
        marker.openPopup();
    }
}

/**
 * 
 * @param {any} dotNetHelper 
 * @param {Array<OfficeMapMark>} marks
 */
export function updateMarks(dotNetHelper, marks) {
    // var customIcon = L.icon({
    //     iconUrl: '/img/caev-map-icon.png',
    //     iconSize:     [30, 35], // size of the icon
    //     iconAnchor:   [15, 17.5], // point of the icon which will correspond to marker's location
    //     popupAnchor:  [0, -10] // point from which the popup should open relative to the iconAnchor
    // });

    // clear previous data
    MAPCONTEXT.clearMarkers();
    MAPCONTEXT.clearCircles();

    // Add pushpins
    MAPCONTEXT.points = marks;
    marks.forEach(point => {

        var marker = L.marker([point.lat, point.lon],
        {
            opacity: 0.75,
            riseOnHover: true,
            // icon: customIcon
            icon: L.divIcon({
                className: 'custom-map-label', // Set class for CSS styling
                html: `<div class='container'><div class='blur-bg'></div> <div class='text'>${point.office}<br/>${point.title}</div> </div>`,
                iconSize: [140, 55],
                iconAnchor: [70, 20]
            }),
        });
        marker.bindPopup(`<b>${point.office}</b> <br/> ${point.title}`).openPopup();
        
        // added the event
        marker.on('click', function (e) {
            dotNetHelper.invokeMethodAsync('PushpinClick', point.id);
        });
        
        // save the reference of the marker
        MAPCONTEXT.markers.push(marker);

        // create the circle
        var circle = L.circle([point.lat, point.lon], point.radius, {
            opacity:0.9,
            fillColor: MAPCONTEXT.pushpinColors["color1"],
            color: MAPCONTEXT.pushpinColors["color2"]
        });

        MAPCONTEXT.circles.push(circle);


        // add the marker to the map
        MAPCONTEXT.map.addLayer(marker);
        MAPCONTEXT.map.addLayer(circle);
    });

}
