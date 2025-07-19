using System;
using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Components;
using MatBlazor;
using SICEM_Blazor.SeguimientoCobros.Models;
using SICEM_Blazor.SeguimientoCobros.Data;
using SICEM_Blazor.Helpers;
using SICEM_Blazor.Models;
using SICEM_Blazor.Data;
using SICEM_Blazor.Services;
using SICEM_Blazor.Recaudacion.Data;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;

namespace SICEM_Blazor.SeguimientoCobros.Views {

    public partial class IngresoCajasPanel{

        [Inject]
        private IncomeOfficeService IncomeOfficeService {get;set;}
        
        [Inject]
        private SicemService SicemService {get;set;}
        
        [Inject]
        private IMatToaster Toaster {get;set;}
        
        [Inject]
        private IRecaudacionService RecaudacionService {get;set;}

        [CascadingParameter]
        public Index IndexPage {get;set;}
        
        [Parameter]
        public IEnlace Enlace {get;set;}

        public bool CargandoLocalidades {get;set;} = true;
        
        private List<Recaudacion.Models.RecaudacionIngresosxPoblaciones> RecaudacionLocalidades {get;set;} = new();

        private List<IngresoHorario> IngresosHorarios {get;set;} = new();

        private CancellationTokenSource cancellationTokenSource = new();
        

        public async Task LoadData()
        {
            this.CargandoLocalidades = true;
            IngresosHorarios.Clear();
            await Task.Delay(100);
            StateHasChanged();

            // * cancel the previous tasks
            cancellationTokenSource.Cancel();
            cancellationTokenSource = new();

            // * get sucursales of the enlace
            var sucursales = SicemService.ObtenerCatalogoSucursales(Enlace.Id);

            var ingresosHorarios = new List<IngresoHorario>();

            var cancellationToken = cancellationTokenSource.Token;

            // * get incomes for each sucursal
            var tasks = new List<Task>();
            foreach( var sucursal in sucursales)
            {
                tasks.Add( Task.Run( ()=>
                {
                    if( cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }

                    var ingresosCaja = IncomeOfficeService.IngresosPorHorario(Enlace, sucursal, DateTime.Now);
                    if(ingresosCaja != null && !cancellationToken.IsCancellationRequested)
                    {
                        lock(ingresosHorarios)
                        {
                            ingresosHorarios.AddRange(ingresosCaja);
                        }
                    }
                }, cancellationToken ));
            }
            
            // wait for all task to complete
            try
            {
                await Task.WhenAll(tasks);
            }
            catch(OperationCanceledException){}

            // * update the current datacontext
            IngresosHorarios = ingresosHorarios;

            this.CargandoLocalidades = false;
            StateHasChanged();
        }

        public void HandleClosePanelInfo(){
            IndexPage.ClosePanelInfo();
        }
    }

}
