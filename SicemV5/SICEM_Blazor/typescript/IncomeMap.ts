/// <reference path="../wwwroot/ts/types/MicrosoftMaps/Microsoft.Maps.d.ts" />
/// <reference path="../wwwroot/ts/types/MicrosoftMaps/Modules/SpatialMath.d.ts" />
/// <reference path="../wwwroot/ts/types/MicrosoftMaps/Modules/Contour.d.ts" />

interface OfficePushpinMap {
    id: number;
    title: string;
    office: string;
    lat: number;
    lon: number;
    bills: number;
    income: number;
}

interface DotNetHelper {
    invokeMethodAsync(methodName: string, ...args: any[]): void;
}
interface PushPinMetadata {
    idOficina: number,
    title: string|null
}

const SICEM_INCOME_MAP = {
    map: null as Microsoft.Maps.Map | null,
    points: [] as Array<OfficePushpinMap>,
    createCircle: function( center: Microsoft.Maps.Location, radius:number, color:string): Microsoft.Maps.ContourLayer {
       
        var location = Microsoft.Maps.SpatialMath.getRegularPolygon(center, radius, 36, Microsoft.Maps.SpatialMath.DistanceUnits.Kilometers);

        var _polygon =  new Microsoft.Maps.ContourLine(location, color);

        var layer = new Microsoft.Maps.ContourLayer( [_polygon], {
            colorCallback: function (val: number | string) {
                return val as string;
            },
            polygonOptions: {
                strokeThickness: 1
            }
        })

        return layer;
    },
    createPushpin: function( officePushpinMap: OfficePushpinMap ) : Microsoft.Maps.Pushpin {
        
        // create pushpin for the text
        var pushPin = new Microsoft.Maps.Pushpin( new Microsoft.Maps.Location( officePushpinMap.lat, officePushpinMap.lon), {
            anchor: new Microsoft.Maps.Point(15, 15), // Set the anchor point to the center of the SVG
            text: officePushpinMap.title,
            icon: `
                <svg width="200" height="80" xmlns="http://www.w3.org/2000/svg">
                    <circle cx="6" cy="6" r="6" fill="#0000"/>
                    <text x="50%" y="35%" dominant-baseline="middle" text-anchor="middle" font-weight="bold" font-family="sans-serif" fill="#322312" font-size="14">{text}</text>
                    <text x="50%" y="10%" dominant-baseline="middle" text-anchor="middle" font-weight="bold" font-family="sans-serif" fill="#322312" font-size="14">${officePushpinMap.office}</text>
                </svg>`
            }
        );

        // append office id to the pushpin
        pushPin.metadata = {
            idOficina: officePushpinMap.id as number
        } as PushPinMetadata;

        return pushPin;

    },
    addMapEntity: function( dotNetHelper: DotNetHelper, point: OfficePushpinMap ):void{
        
        // * create the circle polygon with the colors
        var _location = new Microsoft.Maps.Location( point.lat, point.lon);
        var _color = SICEM_INCOME_MAP.colors[point.id-1];
        var polygonLayer = SICEM_INCOME_MAP.createCircle(_location, 10, _color );
        SICEM_INCOME_MAP.map!.layers.insert(polygonLayer);


        // * create the pushpin 
        var _pushPinText = SICEM_INCOME_MAP.createPushpin( point );

        // add the click event
        Microsoft.Maps.Events.addHandler(_pushPinText, "click", ()=>{
            dotNetHelper.invokeMethodAsync("PushPinClick", point.id );
        })
        SICEM_INCOME_MAP.map!.entities.insert(_pushPinText, point.id);

    },
    colors: [ "#c0392b22", "#f6b53f22", "#6faab022", "#22995422", "#22319922", "#3EC6B622", "#e9618822"] as Array<string>,
};

export function initialize(dotNetHelper: DotNetHelper, elementId: string, center: Microsoft.Maps.Location) {
    SICEM_INCOME_MAP.map = new Microsoft.Maps.Map( elementId, {
        enableCORS: true,
        center: center,
        mapTypeId: Microsoft.Maps.MapTypeId.road,
        zoom: 8,
        showDashboard: false, 
        showMapTypeSelector: false, 
        showScalebar: true,
        disableScrollWheelZoom: false,
        disablePanning: false,
        disableZooming: false,
        maxBounds: new Microsoft.Maps.LocationRect(center, 20, 15)
    });

    // load modules
    Microsoft.Maps.loadModule(['Microsoft.Maps.SpatialMath', 'Microsoft.Maps.Contour']);

    dotNetHelper.invokeMethodAsync("IncomeMapLoaded");
}

export function loadPoints( dotNetHelper: DotNetHelper, points: Array<OfficePushpinMap>) {

    SICEM_INCOME_MAP.points = points;
    
    // * create polygons and pushpines for each office record
    SICEM_INCOME_MAP.map!.layers.clear();
    SICEM_INCOME_MAP.points.forEach( (element: OfficePushpinMap) => {
        
        if( element.lat == 0 ){
            return;
        }

        SICEM_INCOME_MAP.addMapEntity( dotNetHelper, element);

    });

}

export function updatePoint(dotNetHelper: DotNetHelper, point: OfficePushpinMap ){

    // * attempt to remove the old circle layer
    var layer = null;
    var _layers = SICEM_INCOME_MAP.map!.layers as Microsoft.Maps.LayerCollection; 
    for (var i = 0; i < _layers.length; i++) {
        if (_layers[i].metadata && _layers[i].metadata.id === point.id) {
            layer = _layers[i];
        }
    }
    if(layer){
        SICEM_INCOME_MAP.map!.layers.remove(layer);
    }

    
    // * remove the old pushpin
    SICEM_INCOME_MAP.map!.entities.removeAt(point.id);

    
    // add the entities
    SICEM_INCOME_MAP.addMapEntity( dotNetHelper, point);
    
}