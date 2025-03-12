using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MatBlazor;
using SICEM_Blazor.Data.KML;
using SICEM_Blazor.Models;
using SICEM_Blazor.Services;

namespace SICEM_Blazor.Pages.AnalisisInformacion {
    public partial class Map {
    
        [Inject]
        private IJSRuntime JSRuntime {get;set;}
        
        [Inject]
        private AnalisisInfoMapJsInterop AnalisisInfoMapJsInterop { get; set; } = default!;
        
        [Inject]
        private IMatToaster MatToaster {get;set;}

        [Inject]
        private SicemService sicemService {get;set;}
        
        [CascadingParameter]
        public List<CatPadron> DatosPadron {get;set;}

        private DotNetObjectReference<Map> objRef;

        [Parameter] public double MinZoom {get;set;} = 1;
        [Parameter] public double MaxZoom {get;set;} = 19;
        [Parameter] public EventCallback OnClosed {get;set;}
        [Parameter] public string Titulo {get;set;} = "Visor mapa";
        [Parameter] public List<MapMark> MarkerDataSource { get; set; } = new List<MapMark>();
        private MapMark PositionSelected;
        private bool ExportarCamposVisible = false;
        private bool ShowDetailsPanel
        {
            get => MarkerDataSource.Count > 1;
        }
        public List<MapMark> MarkerDataSourceItems = new();
        
        private string selectedOption = "";
        private PointInfo pointInfoSelected = null;
        private MapDataPanel mapDataPanel;


        protected override void OnInitialized() {
            objRef = DotNetObjectReference.Create(this);
            PositionSelected = MarkerDataSource.FirstOrDefault();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender) {
            if (firstRender) {
                this.MarkerDataSourceItems = MarkerDataSource.ToList();
                await ShowPushpinesOnMap(MarkerDataSource);
                StateHasChanged();
            }
        }

        private async Task ShowPushpinesOnMap(IEnumerable<MapMark> dataSource){
            var dataPoints = new List<PointInfo>();
            foreach( MapMark markData in dataSource){
                var pointInfo = new PointInfo();
                dataPoints.Add( PointInfo.FromMarkData(markData));
            }
            await AnalisisInfoMapJsInterop.InitializedMapAsync(objRef, "map", dataPoints);
        }

        private async Task HandleCloseWindow(){
            await OnClosed.InvokeAsync();
        }

        private async Task GenerarKML(List<string> camposExportar) {

            ExportarCamposVisible = false;
            if(camposExportar == null){
                return;
            }

            // Generar lista de punto
            var _puntos = new List<KmlPoint>();
            foreach(var mapMark in MarkerDataSource){

                // get currentPadron
                CatPadron currentPadron = this.DatosPadron.Where(item => item.Id_Cuenta == mapMark.IdCuenta).FirstOrDefault();


                // get the values of the parameters selected
                var parametersValues = new List<string>();
                var _attributes = typeof(CatPadron).GetProperties();
                foreach(var campo in camposExportar){
                    System.Reflection.PropertyInfo _propInfo = _attributes.Where(item => item.Name.Equals(campo)).FirstOrDefault();
                    if(_propInfo != null && currentPadron != null){
                        parametersValues.Add( _propInfo.GetValue(currentPadron).ToString() );
                    }
                }


                // make the title from the values
                var _titulo = string.Empty;
                if( parametersValues.Count > 0){
                    _titulo = String.Join("-", parametersValues);
                }else {
                    _titulo = mapMark.IdCuenta.ToString();
                }
                

                _puntos.Add( new KmlPoint(){
                    Titulo = _titulo,
                    Latitud = mapMark.Latitude,
                    Longitud = mapMark.Longitude
                }); 
            }

            // Generar kml
            var _kmlGenerator = new KMLGenerator(_puntos);
            var _kmlContent = _kmlGenerator.GenerarXml();

            // Guardar documento
            var _tmpPath = Path.GetTempPath();
            var _name = $"predios-{Guid.NewGuid().ToString().Replace("-","")}.kml";
            var _fileSrc = $"{_tmpPath}{_name}";
            File.WriteAllText(_fileSrc, _kmlContent, System.Text.Encoding.UTF8);

            // Descargar Archivo
            var _options = new Dictionary<string, object>();
            _options.Add("content", _kmlContent);
            _options.Add("fileName", _name);
            _options.Add("contentType", "application/vnd.google-earth.kml+xml");
            await JSRuntime.InvokeVoidAsync("DownloAdFileFromBase64", _options);
            
        }
        
        private async Task HandleMarkdataListClick(MapMark markData){
            await AnalisisInfoMapJsInterop.MoveMap( objRef,
                point: PointInfo.FromMarkData(markData),
                zoom: 22
            );

            await CargarVisorImagenes( PointInfo.FromMarkData(markData) );
        }

        private string GetColorByStatus(MapMark markData)
        {
            switch(markData.MesesAdeudo){
                case 0:
                case 1:
                case 2:
                    return "#a5d6a766";
                case 3:
                case 4:
                case 5:
                    return "#ffa72666";
                default:
                    return "#ef535066"; 
            }
        }
        
        private async Task HandleRadioButtonChanged(ChangeEventArgs e){
            
            this.selectedOption = e.Value.ToString();
            
            var _data = this.MarkerDataSource;

            switch(this.selectedOption){
                case "regulares":
                    _data = _data.Where(item => item.MesesAdeudo <= 2).ToList();
                    break;

                case "recuperables":
                    _data = _data.Where(item => item.MesesAdeudo >= 3 && item.MesesAdeudo <= 5 ).ToList();
                    break;

                case "morosos":
                    _data = _data.Where(item => item.MesesAdeudo > 5 ).ToList();
                    break;
            }

            this.MarkerDataSourceItems = _data.OrderBy( item => item.IdCuenta).ToList();
            
            await ShowPushpinesOnMap(MarkerDataSourceItems);

            StateHasChanged();
        }

        private async Task CargarVisorImagenes(PointInfo pointInfo){
            this.pointInfoSelected = pointInfo;
            await Task.Delay(100);

            // get the images of the account
            IEnumerable<ImageData> images = sicemService.ObtenerImagenes(pointInfo.IdOficina, pointInfo.IdCuenta);
            await AnalisisInfoMapJsInterop.LoadVisorImages(objRef, pointInfo, images.ToArray() );
            if(mapDataPanel == null){
                throw new Exception("Panel visor mapa es nulo");
            }
            await mapDataPanel.LoadPanel( pointInfo);
            StateHasChanged();
        }

        [JSInvokable]
        public async void MapLoaded(){
            await AnalisisInfoMapJsInterop.InitializeVisorAsync(objRef, "visorImg", "visorImg-list", "btn-closeImg");
        }

        [JSInvokable]
        public async Task PushpinClick(PointInfo pointInfo){
            await CargarVisorImagenes(pointInfo);
            //MatToaster.Add($"Pushpin click; {pointInfo.IdCuenta} - {pointInfo.Titulo} ", MatToastType.Info);
        }
        
    }

}   