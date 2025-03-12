/// <reference path="../wwwroot/ts/types/MicrosoftMaps/Microsoft.Maps.d.ts" />
const SICEM_MAP = {
    pushpinColors: {
        "color1": "#29BC5F",
        "color2": "#fdd835",
        "color3": "#C63621",
    },
    map: null,
    mapWrapper: null,
    points: [],
    initZoom: 19,
    mapRenderedChanged: function () {
        // if (SICEM_MAP.map) {
        //     VISOR_IMAGENES.ajustarPosision(SICEM_MAP.map);
        // }
    },
    getColor: function (meses) {
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
    }
};
const VISOR_IMAGENES = {
    visorImagen: null,
    visorImagenList: null,
    vtnCloseImage: null,
    listaImagenes: [],
    ajustarPosision: function (elementParent) {
        var left = elementParent.offsetLeft;
        var top = elementParent.offsetTop;
        VISOR_IMAGENES.visorImagen.style.left = (left + 10) + "px";
        VISOR_IMAGENES.visorImagen.style.top = (top + 10) + "px";
    },
    mostrarVisor: function (visible) {
        VISOR_IMAGENES.visorImagen.style.display = visible
            ? "block"
            : "none";
    },
    visorClick: function () {
        VISOR_IMAGENES.visorImagen.classList.toggle("agrandar");
    },
    makeCarouselImageElement: function (pointInfo, imageData) {
        var _imageSrc = `/api/Documento/${pointInfo.idOficina}/${imageData.id}`;
        const imagenElement = document.createElement("img");
        imagenElement.src = _imageSrc + "?w=420&h=420";
        imagenElement.classList.add("d-block");
        imagenElement.classList.add("w-100");
        var captionImg = document.createElement("div");
        captionImg.classList.add("carousel-caption");
        captionImg.classList.add("d-none");
        captionImg.classList.add("d-md-block");
        captionImg.innerHTML = `<h5>${imageData.description}</h5>
            <div class='infoCuenta'>${pointInfo.idCuenta}-${pointInfo.razonSocial}</div>
            <div class='infoCuenta'>${pointInfo.localizacion}</div>
            <div class='infoCuenta'>${pointInfo.direccion}, ${pointInfo.colonia}</div>
            <div>${imageData.date}</div>`;
        const carouselElement = document.createElement("div");
        carouselElement.classList.add("carousel-item");
        carouselElement.appendChild(imagenElement);
        carouselElement.appendChild(captionImg);
        return carouselElement;
    }
};
const DATA_PANEL = {
    panel: null
};
function showPanel(show) {
    if (show) {
        DATA_PANEL.panel.classList.remove("collapse");
        SICEM_MAP.mapWrapper.style.gridArea = "2/1/3/2";
    }
    else {
        DATA_PANEL.panel.classList.add("collapse");
        SICEM_MAP.mapWrapper.style.gridArea = "2/1/4/2";
    }
}
export function initialize(dotNetHelper, elementId, points) {
    const firstPoint = points[0];
    const initZoom = SICEM_MAP.initZoom;
    // save locally the points
    SICEM_MAP.points = points;
    // initialize map
    SICEM_MAP.map = new Microsoft.Maps.Map(elementId, {
        center: new Microsoft.Maps.Location(firstPoint.latitud, firstPoint.longitud),
        mapTypeId: Microsoft.Maps.MapTypeId.canvasLight,
        zoom: initZoom
    });
    // Add events
    Microsoft.Maps.Events.addHandler(SICEM_MAP.map, 'viewrendered', SICEM_MAP.mapRenderedChanged);
    // Add pushpins
    SICEM_MAP.points.forEach(point => {
        var _a;
        const pushpin = new Microsoft.Maps.Pushpin(new Microsoft.Maps.Location(point.latitud, point.longitud), {
            title: point.titulo,
            subTitle: point.subTitulo,
            color: SICEM_MAP.getColor(point.mesesAdeudo),
        });
        (_a = SICEM_MAP.map) === null || _a === void 0 ? void 0 : _a.entities.push(pushpin);
        // Add click event to the pushpin
        Microsoft.Maps.Events.addHandler(pushpin, 'click', function () {
            // SICEM_MAP.pushpinClick(point);
            dotNetHelper.invokeMethodAsync('PushpinClick', point);
        });
    });
    // load data panel
    DATA_PANEL.panel = document.getElementById("data-panel");
    SICEM_MAP.mapWrapper = document.getElementById("map-wraper");
    dotNetHelper.invokeMethodAsync('MapLoaded');
}
export function moveMap(dotNetHelper, point, zoom) {
    var _a;
    (_a = SICEM_MAP.map) === null || _a === void 0 ? void 0 : _a.setView({
        center: new Microsoft.Maps.Location(point.latitud, point.longitud),
        zoom: zoom !== null && zoom !== void 0 ? zoom : 21.5
    });
}
export function initializeVisor(dotNetHelper, elementIdVisor, elementIdList, elementIdClose) {
    VISOR_IMAGENES.visorImagen = document.getElementById(elementIdVisor); // #visorImg
    VISOR_IMAGENES.visorImagenList = document.getElementById(elementIdList); // #visorImg-list
    VISOR_IMAGENES.vtnCloseImage = document.getElementById(elementIdClose); // #btn-closeImg
    VISOR_IMAGENES.visorImagenList.addEventListener("click", VISOR_IMAGENES.visorClick);
    VISOR_IMAGENES.vtnCloseImage.addEventListener("click", () => {
        VISOR_IMAGENES.mostrarVisor(false);
        showPanel(false);
    });
}
export function loadVisorImages(dotNetHelper, pointInfo, imagesList) {
    // Limpiar imagenes anteriores
    VISOR_IMAGENES.visorImagenList.innerText = '';
    // comprobar listado de imagenes
    if (imagesList == null || imagesList.length <= 0) {
        VISOR_IMAGENES.listaImagenes = [];
        VISOR_IMAGENES.mostrarVisor(false);
        showPanel(false);
        return;
    }
    VISOR_IMAGENES.listaImagenes = imagesList;
    // generar elementos img
    var _first = true;
    VISOR_IMAGENES.listaImagenes.forEach(imageInfo => {
        const carouselElement = VISOR_IMAGENES.makeCarouselImageElement(pointInfo, imageInfo);
        if (_first) {
            carouselElement.classList.add("active");
            _first = false;
        }
        VISOR_IMAGENES.visorImagenList.appendChild(carouselElement);
    });
    VISOR_IMAGENES.mostrarVisor(true);
    showPanel(true);
}
export function showVisorImages(dotNetHelper, show) {
    VISOR_IMAGENES.mostrarVisor(show);
    showPanel(show);
}
