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
    markers: {},
    circles: {},
    initZoom: 10,
    maxRadius: 20000,
    minRadius: 1000,
    colors: ["#c0392b", "#f6b53f", "#6faab0", "#229954", "#223199", "#3EC6B6", "#e96188"],
    addCircle: function (dotNetHelper, point)
    {
        const pointId = point.id.toString()

        // * remove the previous circle if it exists
        if(MAPCONTEXT.circles[pointId])
        {
            MAPCONTEXT.map.removeLayer(MAPCONTEXT.circles[pointId]);
        }

        // * create the circle polygon with the colors
        var centerCircle = [point.lat, point.lon];
        var circleOptions = {
            color: MAPCONTEXT.colors[point.id - 1],
            fillColor: MAPCONTEXT.colors[point.id - 1],
            fillopacity: 0.75,
        };

        // * calculate the radius based on the income
        var circleRadius = 0.04 * point.income;
        circleRadius = Math.max(MAPCONTEXT.minRadius, Math.min(MAPCONTEXT.maxRadius, circleRadius));

        var circle = L.circle(centerCircle, circleRadius, circleOptions).addTo(MAPCONTEXT.map);

        // * save the circle reference
        MAPCONTEXT.circles[pointId] = circle;


        // * remove the previous textLabel if it exists
        if(MAPCONTEXT.markers[pointId])
        {
            MAPCONTEXT.map.removeLayer(MAPCONTEXT.markers[pointId]);
        }

        // * create the text label for the circle
        var textLabel = L.marker(centerCircle, {
            icon: L.divIcon({
                className: 'circle-text-label',
                html: `${point.title}`,
            }),
            zIndexOffset: 1000
        }).addTo(MAPCONTEXT.map);

        // * save the text label reference
        MAPCONTEXT.markers[pointId] = textLabel;
    },
    clearMarkers: ()=>
    {
        Object.values(MAPCONTEXT.markers).forEach(m => {
            MAPCONTEXT.map.removeLayer(m);
        });
        MAPCONTEXT.markers = {};
    },
    clearCircles: ()=>
    {
        Object.values(MAPCONTEXT.circles).forEach(m => {
            MAPCONTEXT.map.removeLayer(m);
        });
        MAPCONTEXT.circles = {};
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
export function moveMap(dotNetHelper, point, zoom)
{
    MAPCONTEXT.map.setView([point.latitude, point.longitude], zoom !== null && zoom !== void 0 ? zoom : 17);
    //var marker = MAPCONTEXT.markers[point.idCuenta.toString()];
    var marker = MAPCONTEXT.markers.find( item => item.descripcion == point.descripcion);
    if (marker != null)
        {
        marker.openPopup();
    }
}

/**
 * 
 * @param {any} dotNetHelper 
 * @param {Array<OfficeMapMark>} marks
 */
export function updateMark(dotNetHelper, point)
{
    const pointId = point.id.toString();
    MAPCONTEXT.addCircle(dotNetHelper, point);
}

export function updateMarks(dotNetHelper, marks)
{
    // clear previous data
    MAPCONTEXT.clearMarkers();
    MAPCONTEXT.clearCircles();

    // Add pushpins
    marks.forEach(point =>
    {
        MAPCONTEXT.addCircle(dotNetHelper, point);
    });

}
