/// <reference path="./types/leaflet/index.d.ts" />

import L from "./types/leaflet/index";

interface PointInfo {
    titulo: string,
    subTitulo: string,
    latitud : number,
    longitud: number,
    idOficina: number,
    razonSocial:string,
    idCuenta: number,
    localizacion: string,
    direccion : string,
    colonia : string,
    mesesAdeudo: number
}

interface DotNetHelper {
    invokeMethodAsync(methodName: string, ...args: any[]): void;
}

interface ImageData {
    id : number,
    fileName: string,
    description: string,
    date: string
}

const SICEM_MAP = {
    pushpinColors: {
        "color1": "#29BC5F",
        "color2": "#fdd835",
        "color3": "#C63621",
    },
    map: null as L.Map|null,
    mapWrapper : null as HTMLElement|null,
    points: [] as PointInfo[],
    markers: {} as {[key:string]: L.Marker},
    initZoom: 14,
    getColor: function (meses: number): string {
        switch (meses) {
            case 0:
            case 1:
            case 2:
                return SICEM_MAP.pushpinColors['color1'];
            case 3:
            case 4:
            case 5:
                return SICEM_MAP.pushpinColors['color2'];
            default:
                return SICEM_MAP.pushpinColors['color3'];
        }
    },
    addMarker: function(point: PointInfo, dotNetHelper: DotNetHelper) {
        
        var marker = L.marker([point.latitud, point.longitud], {
            opacity:0.75,
            riseOnHover: true
        });

        marker.bindPopup(`<b>${point.razonSocial}</b> <br/> ${point.idCuenta}`).openPopup();
        
        // added the event
        marker.on('click', function(e) {
            dotNetHelper.invokeMethodAsync('PushpinClick', point);
        });
        
        // save the reference of the marker
        SICEM_MAP.markers[point.idCuenta.toString()] = marker;

        // add the marker to the map
        SICEM_MAP.map!.addLayer(marker);
    }
};

const VISOR_IMAGENES = {
    visorImagen: null as HTMLElement|null,
    visorImagenList: null as HTMLElement|null,
    vtnCloseImage: null as HTMLElement|null,
    listaImagenes: [] as Array<any>,
    ajustarPosision: function(elementParent: HTMLElement){
        var left = elementParent.offsetLeft;
        var top = elementParent.offsetTop;
        VISOR_IMAGENES.visorImagen!.style.left = (left + 10) + "px";
        VISOR_IMAGENES.visorImagen!.style.top = (top + 10) + "px";
    },
    mostrarVisor: function(visible: Boolean){
        VISOR_IMAGENES.visorImagen!.style.display = visible 
            ? "block" 
            : "none";
    },
    visorClick: function(){
        VISOR_IMAGENES.visorImagen!.classList.toggle("agrandar");
    },
    makeCarouselImageElement: function( pointInfo:PointInfo, imageData: ImageData ) : HTMLElement | null {

        var _imageSrc = `/api/Documento/${pointInfo.idOficina}/${imageData.id}`;

        const imagenElement = document.createElement("img") as HTMLImageElement;
        imagenElement.src = _imageSrc + "?w=420&h=420";
        imagenElement.classList.add("d-block");
        imagenElement.classList.add("w-100");


        var captionImg = document.createElement("div") as HTMLElement;
        captionImg.classList.add("carousel-caption");
        captionImg.classList.add("d-none");
        captionImg.classList.add("d-md-block");
        captionImg.innerHTML = `<h5>${imageData.description}</h5>
            <div class='infoCuenta'>${pointInfo.idCuenta}-${pointInfo.razonSocial}</div>
            <div class='infoCuenta'>${pointInfo.localizacion}</div>
            <div class='infoCuenta'>${pointInfo.direccion}, ${pointInfo.colonia}</div>
            <div>${imageData.date}</div>`;

        
        const carouselElement = document.createElement("div") as HTMLElement;
        carouselElement.classList.add("carousel-item");
        carouselElement.appendChild(imagenElement);
        carouselElement.appendChild(captionImg);

        return carouselElement;
    }
};

const DATA_PANEL = {
    panel: null as HTMLElement|null
};

function showPanel( show: Boolean){
    if(show){
        DATA_PANEL.panel!.classList.remove("collapse");
        SICEM_MAP.mapWrapper!.style.gridArea = "2/1/3/2";
    }else{
        DATA_PANEL.panel!.classList.add("collapse");
        SICEM_MAP.mapWrapper!.style.gridArea = "2/1/4/2";
    }
}


export function initialize(dotNetHelper: DotNetHelper, elementId: string, points: PointInfo[]) {

    const firstPoint = points[0];

    const initZoom = SICEM_MAP.initZoom;

    // save locally the points
    SICEM_MAP.points = points;

    if(!SICEM_MAP.map){
        
        // initialize map
        SICEM_MAP.map = L.map( elementId)
            .setView([firstPoint.latitud, firstPoint.longitud], initZoom);

        // set the layer of streetmaps
        L.tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', {
            maxZoom: 19,
            attribution: '&copy; <a href="http://www.openstreetmap.org/copyright">OpenStreetMap</a>'
        }).addTo(SICEM_MAP.map);

    }

    // clear the markers
    Object.keys(SICEM_MAP.markers).forEach(cuenta => {
        SICEM_MAP.map!.removeLayer( SICEM_MAP.markers[cuenta] );
    });
    SICEM_MAP.markers = {};

    // Add pushpins
    SICEM_MAP.points.forEach(point => {
        SICEM_MAP.addMarker(point, dotNetHelper);
        
    });

    // load data panel
    DATA_PANEL.panel = document.getElementById("data-panel");
    SICEM_MAP.mapWrapper = document.getElementById("map-wraper");

    dotNetHelper.invokeMethodAsync('MapLoaded');
}

export function moveMap(dotNetHelper: DotNetHelper, point: PointInfo, zoom: number|null){
    SICEM_MAP.map!.setView([point.latitud, point.longitud], zoom??17);

    var marker = SICEM_MAP.markers[point.idCuenta.toString()] as L.Marker|null;
    if(marker != null){
        marker.openPopup();
    }
}

export function initializeVisor(dotNetHelper: DotNetHelper, elementIdVisor: string, elementIdList: string, elementIdClose: string) {
    
    VISOR_IMAGENES.visorImagen = document.getElementById(elementIdVisor); // #visorImg
    VISOR_IMAGENES.visorImagenList = document.getElementById(elementIdList); // #visorImg-list
    VISOR_IMAGENES.vtnCloseImage = document.getElementById(elementIdClose); // #btn-closeImg

    VISOR_IMAGENES.visorImagenList!.addEventListener("click", VISOR_IMAGENES.visorClick);
    VISOR_IMAGENES.vtnCloseImage!.addEventListener("click", () => {
        VISOR_IMAGENES.mostrarVisor(false)
        showPanel(false);
    });
}

export function loadVisorImages( dotNetHelper: DotNetHelper, pointInfo: PointInfo, imagesList: Array<ImageData> ) {
    
    // Limpiar imagenes anteriores
    VISOR_IMAGENES.visorImagenList!.innerText = '';

    // comprobar listado de imagenes
    if( imagesList == null || imagesList.length <= 0 ){
        VISOR_IMAGENES.listaImagenes = [];
        VISOR_IMAGENES.mostrarVisor(false);
        showPanel(false);
        return;
    }
    VISOR_IMAGENES.listaImagenes = imagesList;


    // generar elementos img
    var _first = true;
    VISOR_IMAGENES.listaImagenes.forEach(imageInfo => {
        const carouselElement = VISOR_IMAGENES.makeCarouselImageElement(pointInfo, imageInfo) as HTMLElement;
        if(_first){
            carouselElement.classList.add("active");
            _first = false;
        }
        VISOR_IMAGENES.visorImagenList!.appendChild(carouselElement);
    });

    VISOR_IMAGENES.mostrarVisor(true);
    showPanel(true);

}

export function showVisorImages( dotNetHelper: DotNetHelper, show: Boolean  ) {
    VISOR_IMAGENES.mostrarVisor(show);
    showPanel(show);
}