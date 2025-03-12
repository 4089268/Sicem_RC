
function inicializarSicemMaps(points) {
    // {
    //     "titulo": "0",
    //     "subTitulo": "SALGADO PACHECO DE GREY TELMA",
    //     "latitud": 18.698382,
    //     "longitud": -88.385952,
    //     "idOficina": 2,
    //     "razonSocial": "SALGADO PACHECO DE GREY TELMA",
    //     "idCuenta": 0,
    //     "localizacion": "01-01-0058-00029-00-01-01",
    //     "direccion": "BOULEVARD COSTERO NORTE REG.11 MZ.6 LT.89",
    //     "colonia": "MARIO VILLANUEVA MADRID -BACALAR-"
    //     "mesesAdeudo": 0
    // }

    VISOR_IMAGENES.init();
    try {
        VISOR_IMAGENES.mostrarVisor(false);
    } catch (error) {
        console.log(error);
     }
    SICEM_MAP.init(points);
}
function moverMapa(pointInfo){
    SICEM_MAP.moverMapa(pointInfo.latitud, pointInfo.longitud, 19);
    VISOR_IMAGENES.cargarVisor(pointInfo);
}

const SICEM_MAP = {
    pushpinColors:{
        "color1":"#29BC5F",
        "color2":"#fdd835",
        "color3":"#C63621",
    },
    map: null,
    points:[],
    initZoom:19,
    init: function(points){
        
        SICEM_MAP.points = points;

        //SICEM_MAP.iniciarGoogleMaps(desc, lat, lon);
        SICEM_MAP.cargarBingMaps();

    },
    cargarGoogleMapas: function(titulo, lat, lon, subtitulo){
        var latlng = new google.maps.LatLng(lat, lon);
        var options = {
            zoom: 19,
            center: latlng,
            mapTypeId: google.maps.MapTypeId.ROADMAP
        };

        const map = new google.maps.Map(document.getElementById ("map"), options);

        const markPosition = { lat: lat, lng: lon};
        new google.maps.Marker({
            position: markPosition,
            title: titulo,
        });
    },
    cargarBingMaps: function(){
        var firstPoint = SICEM_MAP.points[0];
        var initZoom = SICEM_MAP.initZoom;

        // Si solo se recibio un pinInfo hacercar mas el ampa
        if(SICEM_MAP.points.length > 1){
            initZoom = SICEM_MAP.initZoom - 3;
        }

        // Inicializar mapa
        SICEM_MAP.map = new Microsoft.Maps.Map(document.getElementById('map'), {
            center: new Microsoft.Maps.Location(firstPoint.latitud, firstPoint.longitud),
            mapTypeId: Microsoft.Maps.MapTypeId.canvasLight,
            zoom: initZoom
        });

        // Agregar eventos
        Microsoft.Maps.Events.addHandler(SICEM_MAP.map, 'viewrendered', SICEM_MAP.mapRenderedChanged);

        // Agregar los pushpin
        SICEM_MAP.points.forEach(point => {
            var pushpin = new Microsoft.Maps.Pushpin(
                new Microsoft.Maps.Location(point.latitud, point.longitud),
                {
                    title: point.titulo,
                    subTitle: point.subTitulo,
                    color: SICEM_MAP.getColor(point.mesesAdeudo),
                }
            );
            SICEM_MAP.map.entities.push(pushpin);

            // Agregar evento de click a los pushpin
            Microsoft.Maps.Events.addHandler(pushpin, 'click', function () { SICEM_MAP.pushpinClick(point); });
        });
    },
    moverMapa: function(lat, lon, zoom){
        SICEM_MAP.map.setView({
            center: new Microsoft.Maps.Location(lat, lon),
            zoom: zoom
        });
    },
    mapRenderedChanged: function(){
        VISOR_IMAGENES.ajustarPosision(SICEM_MAP.map);
    },
    pushpinClick: function(pointInfo){
        VISOR_IMAGENES.cargarVisor(pointInfo);
    },
    getColor: function(meses){
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
    visorImagen:null,
    visorImagenList:null,
    vtnCloseImage:null,
    listaImagenes:null,
    init: function(){
        // Referenciar elementos
        // VISOR_IMAGENES.panelInfoPoint = document.getElementById("panelInfoPoint");
        VISOR_IMAGENES.visorImagen = document.getElementById("visorImg");
        VISOR_IMAGENES.visorImagenList = document.getElementById("visorImg-list");
        VISOR_IMAGENES.vtnCloseImage = document.getElementById("btn-closeImg");

        // Agergar eventos
        VISOR_IMAGENES.visorImagenList.addEventListener("click", VISOR_IMAGENES.visorClick); 
        VISOR_IMAGENES.vtnCloseImage.addEventListener("click", () => VISOR_IMAGENES.mostrarVisor(false) );
    },
    ajustarPosision: function(elementParent){
        var left = elementParent.offsetLeft;
        var top = elementParent.offsetTop;
        VISOR_IMAGENES.visorImagen.style.left = (left + 10) + "px";
        VISOR_IMAGENES.visorImagen.style.top = (top + 10) + "px";
    },
    cargarVisor: function(pointInfo){
        // Limpiar imagenes anteriores
        VISOR_IMAGENES.visorImagenList.innerText = '';

        //Obtener idImagen de la cuenta
        VISOR_IMAGENES.obtenerImagenes(pointInfo.idOficina, pointInfo.idCuenta, (tmpListaImg) =>{

            if(tmpListaImg == null){
                VISOR_IMAGENES.listaImagenes = [];
                VISOR_IMAGENES.mostrarVisor(false);
                return;
            }

            if(tmpListaImg.length <= 0){
                VISOR_IMAGENES.listaImagenes = [];
                VISOR_IMAGENES.mostrarVisor(false);
                return;
            }

            this.listaImagenes = tmpListaImg;
            

            // Genearar elementos img
            var _first = true;
            VISOR_IMAGENES.listaImagenes.forEach(imageInfo => {
                try{
                    // Load imge list
                    var _imageSrc = `/api/Documento/${pointInfo.idOficina}/${imageInfo.id}`
                    const _imagen = document.createElement("img");
                    _imagen.src = _imageSrc + "?w=420&h=420";
                    _imagen.classList.add("d-block");
                    _imagen.classList.add("w-100");


                    var _captionImg = document.createElement("div");
                    _captionImg.classList.add("carousel-caption");
                    _captionImg.classList.add("d-none");
                    _captionImg.classList.add("d-md-block");
                    _captionImg.innerHTML = `<h5>${imageInfo.descripcion}</h5>
                      <div class='infoCuenta'>${pointInfo.idCuenta}-${pointInfo.razonSocial}</div>
                      <div class='infoCuenta'>${pointInfo.localizacion}</div>
                      <div class='infoCuenta'>${pointInfo.direccion}, ${pointInfo.colonia}</div>
                      <div>${imageInfo.fecha}</div>`;

                    
                    const _divElemt = document.createElement("div");
                    _divElemt.classList.add("carousel-item");
                    if(_first){
                        _divElemt.classList.add("active");
                        _first = false;
                    }
                    _divElemt.appendChild(_imagen);
                    _divElemt.appendChild(_captionImg);
                    
                    VISOR_IMAGENES.visorImagenList.appendChild(_divElemt);
                }catch(err){
                    console.error(err);
                }
            });
            
            VISOR_IMAGENES.mostrarVisor(true);
        });
    },
    mostrarVisor: function(visible){
        if(visible){
            VISOR_IMAGENES.visorImagen.style.display = "block";
        }else{
            VISOR_IMAGENES.visorImagen.style.display = "none";
        }
    },
    obtenerImagenes: function(idOficina, idCuenta, callback){
        
        fetch(`/api/ListaImagenes/${idOficina}/${idCuenta}`)
            .then(response =>{
                if(response.ok){
                    response.json()
                        .then( listaImagenes =>{
                            callback(listaImagenes);
                        })
                        .catch( err => {
                            console.error(err);
                            callback(null);
                        });
                }else{
                    callback(null);
                }
            });
    },
    visorClick: function(){
        VISOR_IMAGENES.visorImagen.classList.toggle("agrandar");
    }
};

const VENTANA = {
    apiEndpoint: "api/consultaGral/1/movimientos/263910",
    show: function( id, name, date ){

        var panel = document.querySelector("#panel-cobros");
        if(!panel){
            VENTANA.generatePanel(name);
        }else{
            VENTANA.setTitle( name );
            VENTANA.setLoading();
        }

        // fetch( VENTANA.apiEndpoint + `/${id}?d=${date}`)
        fetch( VENTANA.apiEndpoint )
            .then( response => {
                if (!response.ok) {
                    throw new Error('Network response was not ok');
                }
                return response.json();
            })
            .then( data => {
                setTimeout(() => {
                    VENTANA.setDataTable( data );
                }, 500);
            })
            .catch(error => {
                console.error('There was a problem with the fetch operation:', error)
                VENTANA.setError(); ;
            });

    },
    handleCloseClick: function(){
        var panel = document.getElementById("panel-cobros");
        if(panel){
            panel.remove();
        }
    },
    generatePanel: function ( title ){
        var fixedDiv = document.createElement('div');
        fixedDiv.id = "panel-cobros";
        fixedDiv.innerHTML = `
            <div class="card" aria-hidden="true">
                <div class="card-body cursorMove">
                    <div class="d-flex align-items-center justify-content-between">
                        <h4 class="card-title mb-0"> ${ title }</h4>
                        <button type="button" class="btn btn-close" aria-label="Close" > <i class="fa fa-close"></i>  </button>
                    </div>
                    <table class="table"> <thead> <tr>
                    <th scope="col">ID</th>
                    <th scope="col">Folio</th>
                    <th scope="col">Estatus</th>
                    <th scope="col">Operacion</th>
                    <th scope="col">Cargo</th>
                    <th scope="col">Abono</th>
                    <th scope="col">Saldo</th>
                    <th scope="col">Fecha</th>
                    <th scope="col">Quien</th>
                    <th scope="col">Sucursal</th>
                    <th scope="col">Id Movimiento</th>
                    <th scope="col">Tipo Movimiento</th>
                    <th scope="col">Observaciones</th>
                    </tr> </thead> <tbody class="table-group-divider">
                    <tr> <td colspan="16" class="text-center pt-4">
                        <div class="spinner-border" role="status">
                            <span class="visually-hidden"></span>
                        </div>
                    </td> </tr>
                    </tbody> </table>
                </div>
            </div>
        `;
        fixedDiv.style.position = 'fixed';
        fixedDiv.style.top = '10px';
        fixedDiv.style.left = '10px';
        fixedDiv.style.width = '46 rem';
        fixedDiv.style.zIndex = 60;
        document.body.appendChild(fixedDiv);
 
        // Set event on button
        var closeButton = document.querySelector("#panel-cobros button.btn-close");
        if(closeButton){
            closeButton.onclick = VENTANA.handleCloseClick;
        }

        // Make dragable
        setTimeout(function () {
            $("#panel-cobros").draggable({ containment: "body", scroll: false, handle: ".card-body" });
        }, 100);
    },
    setTitle: function(title){
        var divTitle = document.querySelector("#panel-cobros .card-title");
        divTitle.innerHTML = title;
    },
    setDataTable: function(dataRows){
        var tableElements = [];
        dataRows.forEach(row => {
            tableElements.push(`<tr>
                <td scope="col">${row.id}</td>
                <td scope="col">${ row.folio_movto }</td>
                <td scope="col">${ row.estatus }</td>
                <td scope="col">${ row.operacion }</td>
                <td scope="col">${ row.cargo }</td>
                <td scope="col">${ row.abono }</td>
                <td scope="col">${ row.saldo }</td>
                <td scope="col">${ row.fecha }</td>
                <td scope="col">${ row.quien }</td>
                <td scope="col">${ row.sucursal }</td>
                <td scope="col">${ row.id_movto }</td>
                <td scope="col">${ row.tipomovto }</td>
                <td scope="col">${ row.observacion }</td>
            </tr>`);
        });
        var dataRowsHtml = tableElements.join('');
        var tbody = document.querySelector("#panel-cobros table tbody");
        tbody.innerHTML = dataRowsHtml;
    },
    setLoading: function(){
        var tbody = document.querySelector("#panel-cobros table tbody");
        tbody.innerHTML = ` <tr> <td colspan="13" class="text-center pt-4">
            <div class="spinner-border" role="status">
                <span class="visually-hidden"></span>
            </div> </td> </tr> `;
    },
    setError: function(){
        var tbody = document.querySelector("#panel-cobros table tbody");
        tbody.innerHTML = `<tr><td colspan="13"> <div class="alert alert-danger" role="alert">
            Error al procesar la consulta, inténtelo más tarde. </div> </td> </tr>`;
    }
}