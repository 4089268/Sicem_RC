/// <reference path="../ts/types/MicrosoftMaps/Microsoft.Maps.d.ts" />
function loadMap(apikey, callbackName){
    // Load script
    const script = document.createElement("script");
    script.src = `https://www.bing.com/api/maps/mapcontrol?callback=${callbackName}&key=${apikey}`;
    script.async = true;
    script.defer = true;
    document.body.appendChild(script);
}

function initIncomeMap(){
    SICEM_INCOME_MAP.init();
}
function actualizarItems(points) {
    SICEM_INCOME_MAP.cargarItems(points);
}
function actualizarItem(point) {
    SICEM_INCOME_MAP.cargarItem(point);
}

function moveMap(config){
    SICEM_INCOME_MAP.moverMapa(config.latitude, config.longitude, config.zoom);
}

const SICEM_INCOME_MAP = {
    map: null,
    points:[],
    initZoom: 7,
    media: 10000,
    center: {
        "latitude": 18.518430776297095,
        "longitude": -88.27933085742902
    },
    colors: [ "#c0392b55", "#F6B53F55", "#6FAAB055", "#22995455", "#22319955", "#3EC6B655", "#E9618855"],
    init: function(){
        SICEM_INCOME_MAP.cargarBingMaps();
    },
    cargarBingMaps: function(){
        
        var _center = new Microsoft.Maps.Location(this.center.latitude, this.center.longitude);
        // Inicializar mapa
        SICEM_INCOME_MAP.map = new Microsoft.Maps.Map(document.getElementById('map'), {
            center: _center,
            mapTypeId: Microsoft.Maps.MapTypeId.canvasLight,
            zoom: this.initZoom,
            showDashboard: false, 
            showMapTypeSelector: false, 
            showScalebar: true,
            disableScrollWheelZoom: false,
            disablePanning: false,
            disableZooming: false,
            maxBounds: new Microsoft.Maps.LocationRect(_center, 20, 15)
        });

    },
    cargarItems: function(points){
        SICEM_INCOME_MAP.points = points;
        Microsoft.Maps.loadModule(['Microsoft.Maps.SpatialMath', 'Microsoft.Maps.Contour'], function () {
            var circles = [];
            let i = 0;
            SICEM_INCOME_MAP.points.forEach(point => {
                if(point.lat == 0 ){
                    return;
                }
                var _location = new Microsoft.Maps.Location(point.lat, point.lon);
                circles.push(
                    SICEM_INCOME_MAP.createCircle(_location, SICEM_INCOME_MAP.calculate(point.income), SICEM_INCOME_MAP.colors[point.id-1])
                );
                i++;
            });

            SICEM_INCOME_MAP.map.layers.clear();

            circles.forEach(circle => {
                var customLayer = new Microsoft.Maps.ContourLayer( [circle], {
                    colorCallback: function (val) {
                        return val;
                    },
                    polygonOptions: {
                        strokeThickness: 1
                    }
                });
                SICEM_INCOME_MAP.map.layers.insert(customLayer);
            });
            
        });

        // Agregar los pushpin
        SICEM_INCOME_MAP.map.entities.clear();
        SICEM_INCOME_MAP.points.forEach(point => {
            if(point.lat == 0 ){
                return;
            }
            var _location = new Microsoft.Maps.Location(point.lat, point.lon);
            var pushpin = new Microsoft.Maps.Pushpin( _location , {
                anchor: new Microsoft.Maps.Point(15, 15), // Set the anchor point to the center of the SVG
                text: point.title,
                icon: `
                <svg width="200" height="80" xmlns="http://www.w3.org/2000/svg">
                    <circle cx="25" cy="25" r="20" fill="#0000" />
                    <text x="50%" y="35%" dominant-baseline="middle" text-anchor="middle" font-weight="bold" font-family="sans-serif" fill="#322312" font-size="14">{text}</text>
                    <text x="50%" y="10%" dominant-baseline="middle" text-anchor="middle" font-weight="bold" font-family="sans-serif" fill="#322312" font-size="14">${point.office}</text>
                </svg>`
            });
            var pushpin2 = new Microsoft.Maps.Pushpin( _location , {
                color: "#322312"
            });

            try {
                SICEM_INCOME_MAP.map.entities.push(pushpin);
            } catch (error) { }
            try {
                SICEM_INCOME_MAP.map.entities.push(pushpin2);
            } catch (error) { }
            
            Microsoft.Maps.Events.addHandler(pushpin, 'click', function () { SICEM_INCOME_MAP.pushpinClick(point); });
        });

    },
    cargarItem: function(point){

        if( SICEM_INCOME_MAP.points.map( p => p.id ).includes( point.id ) ){
            // refresh point
            SICEM_INCOME_MAP.points.find( item => item.id == point.id).title = point.title;
            SICEM_INCOME_MAP.updatePoint(point);
            
        }else{
            // add point
            SICEM_INCOME_MAP.points.push( point );
            SICEM_INCOME_MAP.addPoint( point );
        }

    },
    addPoint: function(point){
        if(point.lat == 0 ){
            return;
        }

        let pointLocation = new Microsoft.Maps.Location(point.lat, point.lon);
        
        // insert colour circle
        Microsoft.Maps.loadModule(['Microsoft.Maps.SpatialMath', 'Microsoft.Maps.Contour'], function () {
            
            let circle = SICEM_INCOME_MAP.createCircle(
                pointLocation,
                SICEM_INCOME_MAP.calculate(point.income),
                SICEM_INCOME_MAP.colors[point.id-1]
            );
                

            // SICEM_INCOME_MAP.map.layers.clear();
            var customLayer = new Microsoft.Maps.ContourLayer( [circle], {
                colorCallback: function (val) {
                    return val;
                },
                polygonOptions: {
                    strokeThickness: 1
                }
            });
            customLayer.metadata = { id: point.id};
            SICEM_INCOME_MAP.map.layers.insert(customLayer);
        });

        // Agregar los pushpin
        // SICEM_INCOME_MAP.map.entities.clear();
        
        var pushpin = new Microsoft.Maps.Pushpin( pointLocation , {
            anchor: new Microsoft.Maps.Point(15, 15), // Set the anchor point to the center of the SVG
            text: point.title,
            icon: `
            <svg width="200" height="80" xmlns="http://www.w3.org/2000/svg">
                <circle cx="25" cy="25" r="20" fill="#0000" />
                <text x="50%" y="35%" dominant-baseline="middle" text-anchor="middle" font-weight="bold" font-family="sans-serif" fill="#322312" font-size="14">{text}</text>
                <text x="50%" y="10%" dominant-baseline="middle" text-anchor="middle" font-weight="bold" font-family="sans-serif" fill="#322312" font-size="14">${point.office}</text>
            </svg>`
        });
        var pushpin2 = new Microsoft.Maps.Pushpin( pointLocation , {
            color: "#322312"
        });

        try {
            SICEM_INCOME_MAP.map.entities.push(pushpin);
        } catch (error) { }
        try {
            SICEM_INCOME_MAP.map.entities.push(pushpin2);
        } catch (error) { }
        
        Microsoft.Maps.Events.addHandler(pushpin, 'click', function () { SICEM_INCOME_MAP.pushpinClick(point); });

    },
    updatePoint: function(point){
        if(point.lat == 0 ){
            return;
        }
        
        let pointLocation = new Microsoft.Maps.Location(point.lat, point.lon);


        // remove old layer 
        var layer = null;
        var _layers = SICEM_INCOME_MAP.map.layers;
        for (var i = 0; i < _layers.length; i++) {
            if (_layers[i].metadata && _layers[i].metadata.id === point.id) {
                layer = _layers[i];
            }
        }
        if(layer){
            SICEM_INCOME_MAP.map.layers.remove(layer);
        }

        // insert colour circle layer
        Microsoft.Maps.loadModule(['Microsoft.Maps.SpatialMath', 'Microsoft.Maps.Contour'], function () {
            
            let circle = SICEM_INCOME_MAP.createCircle(
                pointLocation,
                SICEM_INCOME_MAP.calculate(point.income),
                SICEM_INCOME_MAP.colors[point.id-1]
            );
                

            // SICEM_INCOME_MAP.map.layers.clear();
            var customLayer = new Microsoft.Maps.ContourLayer( [circle], {
                colorCallback: function (val) {
                    return val;
                },
                polygonOptions: {
                    strokeThickness: 1
                }
            });
            customLayer.metadata = { id: point.id};
            SICEM_INCOME_MAP.map.layers.insert(customLayer);
        });


        // Agregar los pushpin
        SICEM_INCOME_MAP.map.entities.clear();
        SICEM_INCOME_MAP.points.forEach(point => {
            var pointLocation = new Microsoft.Maps.Location(point.lat, point.lon);
            var pushpin = new Microsoft.Maps.Pushpin( pointLocation , {
                anchor: new Microsoft.Maps.Point(15, 15), // Set the anchor point to the center of the SVG
                text: point.title,
                icon: `
                <svg width="200" height="80" xmlns="http://www.w3.org/2000/svg">
                    <circle cx="25" cy="25" r="20" fill="#0000" />
                    <text x="50%" y="35%" dominant-baseline="middle" text-anchor="middle" font-weight="bold" font-family="sans-serif" fill="#322312" font-size="14">{text}</text>
                    <text x="50%" y="10%" dominant-baseline="middle" text-anchor="middle" font-weight="bold" font-family="sans-serif" fill="#322312" font-size="14">${point.office}</text>
                </svg>`
            });
            var pushpin2 = new Microsoft.Maps.Pushpin( pointLocation , {
                color: "#322312"
            });
    
            try {
                SICEM_INCOME_MAP.map.entities.push(pushpin);
            } catch (error) { }
            try {
                SICEM_INCOME_MAP.map.entities.push(pushpin2);
            } catch (error) { }
            
            Microsoft.Maps.Events.addHandler(pushpin, 'click', function () { SICEM_INCOME_MAP.pushpinClick(point); });
            
        });
        

    },
    moverMapa: function(lat, lon, zoom){
        SICEM_INCOME_MAP.map.setView({
            center: new Microsoft.Maps.Location(lat, lon),
            zoom: zoom
        });
    },
    pushpinClick: function(pointInfo){
        console.log('Pushpin clicked!!');
    },
    createCircle: function (center, radius, color) {
        var locs = Microsoft.Maps.SpatialMath.getRegularPolygon(center, radius, 36, Microsoft.Maps.SpatialMath.DistanceUnits.Kilometers);
        return new Microsoft.Maps.ContourLine(locs, color);
    },
    calculate: function(value){
        return 10;
        var radius = ( value / SICEM_INCOME_MAP.media );
        if( radius > 70 ){
            return 70.0;
        }else{
            return radius;
        }
    }
};