using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SICEM_Blazor.Data.KML;
using SICEM_Blazor.Models;
using SICEM_Blazor.Pages.AnalisisInformacion;
using MatBlazor;

namespace SICEM_Blazor.Shared.Dialogs {

    public partial class MapaComponent {

        [Inject]
        public IJSRuntime JSRuntime {get;set;} = default!;
        
        [Inject]
        private AnalisisInfoMapJsInterop AnalisisInfoMapJsInterop { get; set; } = default!;
        
        [Inject]
        private IMatToaster MatToaster { get; set; } = default!;

        [Parameter]
        public EventCallback OnClosed {get;set;}
        
        [Parameter]
        public string Titulo {get;set;} = "Visor mapa";
        
        [Parameter]
        public List<MapMark> MarkerDataSource { get; set; } = new List<MapMark>();
        
        private DotNetObjectReference<MapaComponent> objRef;
        private MapMark PositionSelected;
        public List<MapMark> MarkerDataSourceItems = new List<MapMark>();
        
        protected override void OnInitialized() {
            PositionSelected = MarkerDataSource.FirstOrDefault();
            objRef = DotNetObjectReference.Create(this);
            Console.WriteLine("Initialized!");
        }

        protected override async Task OnAfterRenderAsync(bool firstRender) {
            if (firstRender) {
                await ShowPushpinesOnMap(MarkerDataSource);
                StateHasChanged();
            }
        }

        private async Task ShowPushpinesOnMap(IEnumerable<MapMark> dataSource){
            var dataPoints = new List<PointInfo>();
            foreach( MapMark markData in dataSource){
                dataPoints.Add( PointInfo.FromMarkData(markData));
            }
            await AnalisisInfoMapJsInterop.InitializedMapAsync(objRef, "map", dataPoints);
        }

        private async Task HandleCerrarVentanaClick(){
            await OnClosed.InvokeAsync();
        }
        

        #region JSInvokable methods
        [JSInvokable]
        public async void MapLoaded(){
            await Task.Delay(100);
            Console.WriteLine("Map loaded");
            //await AnalisisInfoMapJsInterop.InitializeVisorAsync(objRef, "visorImg", "visorImg-list", "btn-closeImg");
        }

        [JSInvokable]
        public async Task PushpinClick(PointInfo pointInfo){
            Console.WriteLine($"Pushpin click; {pointInfo.IdCuenta} - {pointInfo.Titulo} ", MatToastType.Info);
            await Task.Delay(100);
        }
        #endregion

    }

}