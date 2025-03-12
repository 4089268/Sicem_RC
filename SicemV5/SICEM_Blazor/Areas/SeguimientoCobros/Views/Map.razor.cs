using System;
using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MatBlazor;
using Syncfusion.Blazor.Charts;
using Syncfusion.Blazor.Grids;
using SICEM_Blazor.SeguimientoCobros.Models;
using SICEM_Blazor.SeguimientoCobros.Data;
using SICEM_Blazor.Helpers;
using SICEM_Blazor.Models;
using SICEM_Blazor.Data;
using SICEM_Blazor.Services;


namespace SICEM_Blazor.SeguimientoCobros.Views {
    

    public partial class Map {
        [Inject]
        private IJSRuntime JSRuntime {get;set;}
        
        [Inject]
        private IMatToaster Toaster {get;set;}
        
        [Inject]
        private IOptions<BingMapsSettings> BingMapsSettings {get;set;}

        private Task<IJSObjectReference> _module;
        private Task<IJSObjectReference> Module => _module ??= JSRuntime.InvokeAsync<IJSObjectReference>("import", "./js/sicem/IncomeMap.js").AsTask();
        private MapMark centerMap = new MapMark();
        public static bool isMapLoaded = false;

        [JSInvokable("OnMapLoadedTest")]
        public void OnMapLoadedTest(){
            isMapLoaded = true;
        }

        protected override void OnInitialized(){
            isMapLoaded = false;
        }
        
        protected override async Task OnAfterRenderAsync( bool firstRender) {
            if(firstRender){
                var module = await Module;
                var apiKey = BingMapsSettings.Value.Key;
                await module.InvokeVoidAsync("loadMap", apiKey, "initMap" );

                await InitializeMapAsync();
            }
        }

        public async Task InitializeMapAsync(){
            var c = 0;
            while(!isMapLoaded){
                await Task.Delay(500);
                c++;
                if( c > 666){
                    throw new Exception("Timeout error at waiting for the script loaded");
                }
            }
            var module = await Module;
            await module.InvokeVoidAsync("initializeMap", "#map", centerMap.Latitude, centerMap.Longitude, centerMap.Zoom );
        }

    }

}