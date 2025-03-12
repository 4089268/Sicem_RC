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
    public partial class MapDataPanel {
    
        [Inject]
        private IMatToaster MatToaster {get;set;}

        [Inject]
        private SicemService SicemService {get;set;}

        [Inject]
        private ConsultaGralService ConsultaGralService {get;set;}
        
        [Parameter]
        public PointInfo PointInfo {get;set;}

        private bool cargandoDatos = true;

        private int panelIndexSelected = 0;

        private ConsultaGralResponse userData;

        private List<ConsultaGral_Ordenesresponse> datosOrdenes = new();
        private List<ModificacionABCResume> datosBitacorABC = new();
        private List<ConsultaGreal_MovimientosResponse> datosMovimientos = new();
        
        public async Task LoadPanel(PointInfo pointInfo){
            this.cargandoDatos = true;
            await Task.Delay(100);
            StateHasChanged();

            try {
                LoadData(pointInfo.IdOficina, pointInfo.IdCuenta);
            }catch(Exception err){
                Console.WriteLine("Error: " + err.Message);
            }

            this.cargandoDatos = false;
            StateHasChanged();
        }

        private void LoadData(int oficinaId, int IdCuenta){

            this.datosOrdenes = new();
            this.datosMovimientos = new();
            this.datosBitacorABC = new();

            // get id padron
            this.userData = ConsultaGralService.ConsultaGeneral(oficinaId, IdCuenta.ToString() );

            // get enlace
            var enlace = SicemService.ObtenerEnlaces(oficinaId).FirstOrDefault();
            
            // get the
            var tasks = new List<Task>();
            tasks.Add( Task.Run( () => {
                this.datosOrdenes = ConsultaGralService.ConsultaOrdenesTrabajo(enlace, userData.Id_Padron).ToList();
            }));
            tasks.Add( Task.Run( () => {
                this.datosMovimientos = ConsultaGralService.ConsultaMovimientos(enlace, userData.Id_Padron).ToList();
            }));
            tasks.Add( Task.Run( () => {
                var datosModABC = ConsultaGralService.ConsultaModificacionesABC(enlace, userData.Id_Padron).ToList();
                foreach (var item in datosModABC)
                {
                    foreach( var det in item.Detalle){
                        this.datosBitacorABC.Add( new ModificacionABCResume {
                            Id_abc = item.Id_abc,
                            Fecha = item.Fecha,
                            Id_padron = item.Id_padron,
                            Id_operador = item.Id_operador,
                            Id_sucursal = item.Id_sucursal,
                            Observacion = item.Observacion,
                            Maquina = item.Maquina,
                            Operador = item.Operador,
                            Sucursal = item.Sucursal,
                            Campo = det.Campo,
                            Valor_Ant = det.Valor_Ant,
                            Valor_Act = det.Valor_Act
                        });
                    }
                }
            }));

            Task.WaitAll( tasks.ToArray() );

        }

        public void HandlePanelSelectedIndexChanged(int newIndex){
            this.panelIndexSelected = newIndex;

            // Todo: loadData;

            StateHasChanged();
        }
    }

}   