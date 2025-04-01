using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SICEM_Blazor.Data.KML;
using SICEM_Blazor.Models;
using SICEM_Blazor.SeguimientoCobros.Models;

namespace SICEM_Blazor.SeguimientoCobros.Data {
    public class IncomeMapJsInterop: IAsyncDisposable
    {

        /// <summary>
        /// reference of the js module
        /// </summary>
        private readonly Lazy<Task<IJSObjectReference>> moduleTask;

        public IncomeMapJsInterop(IJSRuntime jSRuntime)
        {
            // moduleTask = new Lazy<Task<IJSObjectReference>>(() => jSRuntime.InvokeAsync<IJSObjectReference>("import", "./js/sicem/incomeMap.js").AsTask() );
            moduleTask = new Lazy<Task<IJSObjectReference>>(() => jSRuntime.InvokeAsync<IJSObjectReference>("import", "./js/sicem/seguimientoCobroMap.js").AsTask() );
        }

        /// <summary>
        /// inicializa el mapa y centra el mapa en la ubicacion
        /// </summary>
        /// <param name="objRef"></param>
        /// <param name="elementId"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        public async Task InitializedAsync(object objRef, string elementId, double latitude, double longitude) {
            var module = await moduleTask.Value;
            await module.InvokeVoidAsync("initialize", objRef, elementId, new { latitude, longitude });
        }

        /// <summary>
        /// agrega los polygonos y pushpines al mapa en base a los pasados como parametros
        /// </summary>
        /// <param name="objRef"></param>
        /// <param name="points"></param>
        /// <returns></returns>
        public async Task UpdateMarks(object objRef, OfficePushpinMap[] points){
            var module = await moduleTask.Value;
            await module.InvokeVoidAsync("updateMarks", objRef, points);
        }

        // public async Task UpdatePoint(object objRef, OfficePushpinMap point){
        //     var module = await moduleTask.Value;
        //     await module.InvokeVoidAsync("updatePoint", objRef, point);
        // }

        public async ValueTask DisposeAsync()
        {
            if( moduleTask.IsValueCreated){
                var module = await moduleTask.Value;
                await module.DisposeAsync();
            }
        }
    }

}