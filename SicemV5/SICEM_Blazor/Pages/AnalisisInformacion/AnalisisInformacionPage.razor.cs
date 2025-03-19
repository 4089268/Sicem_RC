using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Navigations;
using Syncfusion.Blazor.Grids;
using MatBlazor;
using SICEM_Blazor.Data.KML;
using SICEM_Blazor.Models;
using SICEM_Blazor.Services;
using SICEM_Blazor.Data;

namespace SICEM_Blazor.Pages.AnalisisInformacion {

    public partial class AnalisisInformacionPage {

        [Inject]
        private IJSRuntime JSRuntime {get; set;}

        [Inject]
        private NavigationManager NavigationManager {get; set;}

        private SfGrid<CatPadron> DataGrid {get;set;}
        private List<CatPadron> DatosGrid {get;set;}
        private bool Cargando {get;set;} = false;
        private bool MostrarResultados {get;set;} = false;

        private Dictionary<int, string> CatOficinas {get;set;}
        private Dictionary<int,string> CatEstatus {get;set;}
        private Dictionary<int,string> CatTipoCalculo {get;set;}
        private Dictionary<int,string> CatSetvicios {get;set;}
        private Dictionary<int,string> CatTarifas {get;set;}
        private Dictionary<int,string> CatAnomalias {get;set;}
        private Dictionary<int,string> CatGiro {get;set;}
        private Dictionary<int,string> CatClaseUsuaro {get;set;}
        
        private AnalisysInfoFilter FiltroBusqueda {get;set;} = new AnalisysInfoFilter();
        private bool DatosGenerales_visible = false;
        private string CuentaActual = "";
        private int OficinaActual = 0;

        private bool MostrarUbicaciones = false, NotificarDialogVisible = false;
        private List<MapMark> MapPoints {get;set;} = new List<MapMark>();
        private List<CatPadron> UsuariosANotificar { get; set;} = new List<CatPadron>();
        
        protected override void OnInitialized()
        {
            
            // Validar si se cuenta con la opcion
            if(!sicemService.Usuario.OpcionSistemas.Select(item => item.Id).Contains(OpcionesSistema.BUSQUEDA_AVANZADA)){
                navigationManager.NavigateTo("/");
                return;
            }

            base.OnInitialized();
        }
        
        protected override async Task OnParametersSetAsync(){
            this.Cargando = true;
            await Task.Delay(200);

            
            var _oficinas = sicemService.ObtenerEnlaces().Where(item => item.Inactivo != true ).ToList();
            CatOficinas = new Dictionary<int, string>();
            _oficinas.ForEach(ofi => {
                CatOficinas.Add(ofi.Id, ofi.Oficina);
            });
            
            CatEstatus = sicemService.ObtenerCatalogoEstatus(1, "CAT_PADRON");
            CatTipoCalculo = sicemService.ObtenerCatalogoTipoCalculo(1);
            CatSetvicios = sicemService.ObtenerCatalogoServicios(1);
            CatTarifas = sicemService.ObtenerCatalogoTarifas(1);
            CatAnomalias = sicemService.ObtenerCatalogoAnomalias(1);
            CatGiro = sicemService.ObtenerCatalogoGiros(1);
            CatClaseUsuaro = sicemService.ObtenerCatalogoClaseUsuario(1);
            
            await Task.Delay(200);
            this.Cargando = false;

        }

        private async Task RealizarConsulta(AnalisysInfoFilter datosFiltro){

            if( datosFiltro == null ){
                return;
            }

            if( datosFiltro.Id_Oficinas.Count <= 0 ){
                MatToaster.Add("Seleccione una oficina", MatToastType.Info);
                return;
            }

            this.Cargando = true;
            await Task.Delay(200);

            var _oficinas = sicemService.ObtenerEnlaces().Where(item => item.Inactivo != true && datosFiltro.Id_Oficinas.Contains(item.Id) ).ToList();
            var _tmpDatos = sicemService.ObtenerAnalisisInfo(_oficinas, datosFiltro);
            this.DatosGrid = _tmpDatos;

            this.MostrarResultados = true;
            StateHasChanged();
            this.Cargando = false;
        }

        private async Task ExportarExcel_Click(){
            Cargando = true;
            await Task.Delay(100);

            var _tmpFolder = Configuration.GetValue<string>("TempFolder");
            var _exportador = new ExportarExcel<CatPadron>(DatosGrid, new Uri(_tmpFolder) );
            var _archivo = _exportador.GenerarExcel();

            if(!String.IsNullOrEmpty(_archivo)){
                var _endPointDownload = Configuration.GetSection("AppSettings").GetValue<string>("Direccion_Api");
                var _targetUrl = $"{_endPointDownload}/download/{_archivo}";
                await JSRuntime.InvokeVoidAsync("OpenNewTabUrl", _targetUrl);
            }
            
            await Task.Delay(500);
            Cargando = false;
        }

        private void MostrarEnConsultaGeneral(CatPadron e){
            this.CuentaActual = e.Id_Cuenta.ToString();
            this.OficinaActual = e.Id_Oficina;
            DatosGenerales_visible = true;
        }

        private void MostrarUbicacion_Click(CatPadron e){
            var _newItem = new MapMark();
            _newItem.Latitude = (double)e.Latitude;
            _newItem.Longitude = (double)e.Longitude;
            _newItem.Zoom = 17;
            _newItem.Descripcion = $"{e.Id_Cuenta}";
            _newItem.Subtitulo = e.RazonSocial;
            _newItem.IdPadron = e.Id_Padron;
            _newItem.IdCuenta = e.Id_Cuenta;
            _newItem.MesesAdeudo = e.MesAdeudoAct;
            _newItem.IdOficina = e.Id_Oficina;
            MapPoints.Clear();
            MapPoints.Add(_newItem);
            
            this.MostrarUbicaciones = true;
        }

        private void MostrarUbicaciones_Click(){
            if(DatosGrid.Count() <= 0){
                return;
            }

            MapPoints.Clear();
            foreach(var pad in DatosGrid){
                var _newItem = new MapMark();
                _newItem.Latitude = (double)pad.Latitude;
                _newItem.Longitude = (double)pad.Longitude;
                _newItem.Zoom = 17;
                _newItem.Descripcion = $"{pad.Id_Cuenta}";
                _newItem.Subtitulo = pad.RazonSocial;
                _newItem.IdPadron = pad.Id_Padron;
                _newItem.IdCuenta = pad.Id_Cuenta;
                _newItem.MesesAdeudo = pad.MesAdeudoAct;
                _newItem.IdOficina = pad.Id_Oficina;
                MapPoints.Add(_newItem);
            }

            this.MostrarUbicaciones = true;

        }

        private async Task NotificarWhatsapp_Click(CatPadron usuario){

            if( usuario == null){
                UsuariosANotificar = DatosGrid.Where(item => item.TieneTelefono).ToList();
            }else{
                UsuariosANotificar = new List<CatPadron>(){usuario};
            }

            NotificarDialogVisible = true;
            await Task.Delay(100);
        }

        private void ModalNotificacionOnClosed(){
            NotificarDialogVisible = false;
            UsuariosANotificar.Clear();
        }


        [JSInvokable]
        public void RedirectToHome(){
            NavigationManager.NavigateTo("/");
        }
        
    }
}
