using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SICEM_Blazor.Data.KML;
using SICEM_Blazor.Models;

namespace SICEM_Blazor.Pages.AnalisisInformacion {
    public class AnalisisInfoMapJsInterop : IAsyncDisposable
    {

        /// <summary>
        /// reference of the js module
        /// </summary>
        private readonly Lazy<Task<IJSObjectReference>> moduleTask;

        public AnalisisInfoMapJsInterop(IJSRuntime jSRuntime){
            moduleTask = new Lazy<Task<IJSObjectReference>>(() => jSRuntime.InvokeAsync<IJSObjectReference>("import", "./js/sicem/analisisInfoMap.js").AsTask() );
        }

        /// <summary>
        /// inicializa el mapa y mustra las ubicaciones pasadas como argumento
        /// </summary>
        /// <param name="objRef"></param>
        /// <param name="elementId"></param>
        /// <param name="points"></param>
        /// <returns></returns>
        public async Task InitializedMapAsync(object objRef, string elementId, IEnumerable<PointInfo> points) {
            var module = await moduleTask.Value;
            await module.InvokeVoidAsync("initialize", objRef, elementId, points);
        }

        /// <summary>
        /// mueve el visor a un punto y con un zoom
        /// </summary>
        /// <param name="objRef"></param>
        /// <param name="point"></param>
        /// <param name="zoom"></param>
        /// <returns></returns>
        public async Task MoveMap(object objRef, PointInfo point, double? zoom){
            var module = await moduleTask.Value;
            await module.InvokeVoidAsync("moveMap", objRef, point);
        }

        /// <summary>
        /// Inisializa el visor de imagenes referenciando los elementos html y agregando los eventos necesarios.
        /// </summary>
        /// <param name="objRef"></param>
        /// <param name="elementIdVisor"></param>
        /// <param name="elementIdList"></param>
        /// <param name="elementIdClose"></param>
        /// <returns></returns>
        public async Task InitializeVisorAsync(object objRef, string elementIdVisor,string elementIdList, string elementIdClose){
            var module = await moduleTask.Value;
            await module.InvokeVoidAsync("initializeVisor", objRef, elementIdVisor, elementIdList, elementIdClose);
        }

        /// <summary>
        /// muestra el visor y lista las imagenes pasadas como argumento
        /// </summary>
        /// <param name="objRef"></param>
        /// <param name="pointInfo"></param>
        /// <param name="imagesList"></param>
        /// <returns></returns>        
        public async Task LoadVisorImages(object objRef, PointInfo pointInfo, ImageData[] imagesList){
            var module = await moduleTask.Value;
            await module.InvokeVoidAsync("loadVisorImages", objRef, pointInfo, imagesList );
        }

        /// <summary>
        /// muestra u oculta el visor
        /// </summary>
        /// <param name="objRef"></param>
        /// <param name="show"></param>
        /// <returns></returns>
        public async Task ShowVisorImages(object objRef, bool show){
            var module = await moduleTask.Value;
            await module.InvokeVoidAsync("showVisorImages", objRef, show);
        }
        

        public async ValueTask DisposeAsync()
        {
            if( moduleTask.IsValueCreated){
                var module = await moduleTask.Value;
                await module.DisposeAsync();
            }
        }
    }

}   