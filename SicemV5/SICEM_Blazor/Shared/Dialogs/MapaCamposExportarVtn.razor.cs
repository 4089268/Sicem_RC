using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MatBlazor;
using Syncfusion.Blazor.Grids;

namespace SICEM_Blazor.Shared.Dialogs {
    
    public partial class MapaCamposExportarVtn {
        
        [Inject]
        private IMatToaster MatToaster {get;set;} = default!;

        [Parameter]
        public EventCallback<List<string>> OnClosed {get;set;}

        private List<Dictionary<string, string>> catCamposExportar;
        private List<Dictionary<string, string>> camposAExportar;

        private Dictionary<string, string> CampoSeleccionado;

        private readonly List<double> gridFilas = new(){.8, 40 };
        private readonly List<double> gridColumnas = new(){200, 30 , 200};
        private SfGrid<Dictionary<string, string>> DataGrid1 {get;set;}
        private SfGrid<Dictionary<string, string>> DataGrid2 {get;set;}

        protected override void OnInitialized(){
            this.catCamposExportar = new List<Dictionary<string, string>>(){
                new (){ { "Nombre", "RazonSocial" }, { "Desc", "Razon Social"} },
                new (){ { "Nombre", "Direccion"}, { "Desc", "Direccion"} },
                new (){ { "Nombre", "Colonia"}, { "Desc", "Colonia"} },
                new (){ { "Nombre", "CodigoPostal"}, { "Desc", "Codigo Postal"} },
                new (){ { "Nombre", "CallePpal"}, { "Desc", "Calle Principal"} },
                new (){ { "Nombre", "Ruta"}, { "Desc", "Ruta"} },
                new (){ { "Nombre", "Localizacion"}, { "Desc", "Localizacion"} },
                new (){ { "Nombre", "Sb"}, { "Desc", "Subsistema"} },
                new (){ { "Nombre", "Sector"}, { "Desc", "Sector"} },
                new (){ { "Nombre", "Manzana"}, { "Desc", "Manzana"} },
                new (){ { "Nombre", "Lote"}, { "Desc", "Lote"} },
                new (){ { "Nombre", "Giro"}, { "Desc", "Giro"} },
                new (){ { "Nombre", "Clase_Usuario"}, { "Desc", "Clase Usuario"} },
                new (){ { "Nombre", "Estatus"}, { "Desc", "Estatus"} },
                new (){ { "Nombre", "MesAdeudoAct"}, { "Desc", "Meses Adeudo Act"} },
                new (){ { "Nombre", "Servicio"}, { "Desc", "Servicio"} },
                new (){ { "Nombre", "Tipo_Usuario"}, { "Desc", "Tipo Usuario"} },
                new (){ { "Nombre", "Situacion"}, { "Desc", "Situacion Actual"} },
                new (){ { "Nombre", "AnomaliaAct"}, { "Desc", "Anomalia Actual"} },
                new (){ { "Nombre", "ConsumoAct"}, { "Desc", "Consumo Actual"} },
                new (){ { "Nombre", "Diametro"}, { "Desc", "Diametro"} },
                new (){ { "Nombre", "Total"}, { "Desc", "Saldo Actual"} }
            };
            this.camposAExportar  = new List<Dictionary<string, string>>(){
                new () { { "Nombre", "Id_Cuenta"}, { "Desc", "Num Contrato"} }
            };
        }

        private async Task CerrarVentana_Click(bool param){
            if(param){
                if(camposAExportar.Count <= 0 ){
                    MatToaster.Add("No selecciono ningun campo a exportar", MatToastType.Warning );
                    return;
                }
                var _listaCampos = camposAExportar.Select(item => item["Nombre"].ToString()).ToList<string>();
                await OnClosed.InvokeAsync(_listaCampos);
            }else{
                await OnClosed.InvokeAsync(null);
            }
        }

        public async Task RowSelectHandler(RowSelectEventArgs<Dictionary<string, string>> args)
        {
            this.CampoSeleccionado = args.Data;
            await DataGrid2.ClearRowSelectionAsync();
        }
        public void RecordDoubleClickHandler(RecordDoubleClickEventArgs<Dictionary<string, string>> args)
        {
            var campo = args.RowData;
            if(!camposAExportar.Contains(campo)){
                camposAExportar.Add(campo);
                catCamposExportar.Remove(campo);

                DataGrid1.Refresh();
                DataGrid2.Refresh();
            }
        }

        public async Task RowSelectHandler2(RowSelectEventArgs<Dictionary<string, string>> args)
        {
            this.CampoSeleccionado = args.Data;
            await DataGrid1.ClearRowSelectionAsync();
        }
        public void RecordDoubleClickHandler2(RecordDoubleClickEventArgs<Dictionary<string, string>> args)
        {
            var campo = args.RowData;
            if(!catCamposExportar.Contains(campo)){
                catCamposExportar.Add(campo);
                camposAExportar.Remove(campo);

                DataGrid1.Refresh();
                DataGrid2.Refresh();
            }
        }

        public void ButtonLeft2Right_Click(){
            if(CampoSeleccionado != null){
                if(!camposAExportar.Contains(CampoSeleccionado)){
                    catCamposExportar.Remove(CampoSeleccionado);
                    camposAExportar.Add(CampoSeleccionado);
                    DataGrid1.Refresh();
                    DataGrid2.Refresh();
                }
            }

        }
        public void ButtonRight2Left_Click(){
            if(CampoSeleccionado != null){
                if(!catCamposExportar.Contains(CampoSeleccionado)){
                    camposAExportar.Remove(CampoSeleccionado);
                    catCamposExportar.Add(CampoSeleccionado);
                    DataGrid1.Refresh();
                    DataGrid2.Refresh();
                }
            }
        }

    }


}