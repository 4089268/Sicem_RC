using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Charts;
using Syncfusion.Blazor.Inputs;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MatBlazor;
using SICEM_Blazor.Data.Contracts;
using SICEM_Blazor.Services;
using SICEM_Blazor.Models;
using SICEM_Blazor.Data;
using SICEM_Blazor.Shared.Dialogs;


namespace SICEM_Blazor.Pages.ConsultaGeneral;

public partial class ConsultaGeneralPage
{
    [Inject]
    public IMatToaster Toaster {get;set;}
    
    [Inject]
    public IJSRuntime JSRuntime {get;set;}
    
    [Inject]
    public ConsultaGralService consultaGralService {get;set;}
    
    [Inject]
    public SicemService sicemService {get;set;}
    
    [Inject]
    public NavigationManager navigationManager {get;set;}


    [Parameter]
    public string IdCuenta { get; set; }
    
    [Parameter]
    public int IdOficinaParam { get; set; }
    
    [Parameter]
    public int IdOficina { get; set; }

    private SfGrid<ConsultaGralResponse_saldoItem> dataGrid;
    private SfChart grafConsumos;
    private SfNumericTextBox<long?> tb_search;
    
    private BusquedaCuenta modal_BusquedaCuenta;
    private ConsultaGeral_Movimientos vtn_Movimientos;
    private ConsultaGeral_ModificacionesABS vtn_ModificacionesABS;
    private ConsultaGral_Ordenes vnt_Ordenes;
    private ConsultaGral_Anticipos vnt_Anticipos;
    private ConsultaGral_ImagenesVtn vnt_Imagenes;

    public ConsultaGralResponse datosConsultaGral = new ConsultaGralResponse();
    public ConsultaGralResponse_saldoItem[] SaldoActualArray { get; set; }
    public ConsumoItem[] ItemsHistorialConsum { get; set; }
    long? nCuenta = null;
    bool Busy = true;
    

    //*** Componente Mapa
    private bool mostrarMapa = false;
    private List<MapMark> MarkerDataSource { get; set; } = new List<MapMark>();

    private Ruta[] catOficinas = new Ruta[] { };

    private bool DesBotones = true;
    private bool MostrarModal_BuscarCuentas { get; set; } = false;
    private bool MostrarModalMovimientos { get; set; } = false;
    private bool MostrarModalModifABC { get; set; } = false;
    private bool MostrarModal_Ordenes { get; set; } = false;
    private bool MostrarModal_Anticipos { get; set; } = false;
    private bool MostrarModal_Imagenes { get; set; } = false;

    /*********** Eventos Override ***********/
    protected override async Task OnInitializedAsync() {
        
        // Validar si se cuenta con la opcion
        if(!sicemService.Usuario.OpcionSistemas.Select(item => item.Id).Contains(OpcionesSistema.CONSULTA_GENERAL)){
            navigationManager.NavigateTo("/");
            return;
        }

        await Task.Delay(100);

        var enlaces = sicemService.ObtenerOficinasDelUsuario();
        var tmpOfis = sicemService.ObtenerEnlaces().Where(item => enlaces.Select(i => i.Id).Contains(item.Id)).ToArray();

        if (tmpOfis.Length > 0) {
            this.catOficinas = tmpOfis;
            this.IdOficina = catOficinas.First().Id;
            this.Busy = false;
        }
    }
    protected override async Task OnParametersSetAsync(){
        if (IdCuenta != null) {
            if (IdCuenta.Length > 1) {
                this.nCuenta = long.Parse(IdCuenta);
                this.IdOficina = IdOficinaParam;
                await Tb_Cuenta_KeyUp(new KeyboardEventArgs { Key = "Enter" });
            }
        }
    }


    /*********** Eventos UI ***********/
    public async Task Tb_Cuenta_KeyUp(KeyboardEventArgs e) {
        if (e.Key == "Enter") {
            this.datosConsultaGral = new ConsultaGralResponse();
            this.SaldoActualArray = null;
            this.ItemsHistorialConsum = null;
            this.DesBotones = true;

            //*** Comprobar cuenta capturada
            if (this.nCuenta == null || this.IdOficina <= 0) {
                if (this.nCuenta == null) {
                    await JSRuntime.InvokeVoidAsync("shake", "#tb_ncuenta");
                    Toaster.Add("El Numero de Contrato esta vacio.", MatToastType.Info, "");
                }
                else {
                    await JSRuntime.InvokeVoidAsync("shake", "#cb_oficinas");
                    Toaster.Add("Seleccione una oficina.", MatToastType.Info, "");
                }
                return;
            }

            this.Busy = true;
            await Task.Delay(200);


            //*** Realizar contusulta general
            try {
                this.datosConsultaGral = consultaGralService.ConsultaGeneral(IdOficina, this.nCuenta.ToString());
                if (datosConsultaGral == null) {
                    datosConsultaGral = new ConsultaGralResponse();
                    Toaster.Add("El numero de cuenta no existe.", MatToastType.Warning, "");
                    await Task.Delay(200);
                    this.Busy = false;
                    return;
                }
            }
            catch (Exception err) {
                Toaster.Add(err.Message, MatToastType.Warning, "");
                await Task.Delay(200);
                this.Busy = false;
                return;
            }


            //*** Mostrar saldo actual (Grid)
            var tmpSaldoItems = this.datosConsultaGral.SaldoActual.ToArray<ConsultaGralResponse_saldoItem>();
            if (tmpSaldoItems.Length > 0) {
                this.SaldoActualArray = tmpSaldoItems;
            }
            else {
                this.SaldoActualArray = null; ;
            }

            //*** Cargar el Historial de consumos
            var tmpHistItems = this.datosConsultaGral.HistorialConsumos.OrderBy(item => item.Fecha).ToArray<ConsumoItem>();
            if (tmpHistItems.Length > 0) {
                ItemsHistorialConsum = tmpHistItems;
            }
            else {
                ItemsHistorialConsum = null;
            }
            await grafConsumos.RefreshAsync();


            //*** Activar botones de las ventanas secundarias
            this.DesBotones = false;

            await Task.Delay(200);
            this.Busy = false;
        }
    }
    private async Task ComboBoxOficinas_SelectionChanged(int val) {
        this.IdOficina = val;
        await Task.Delay(100);
    }
    private async Task SearchAccountClick()
    {
        await Tb_Cuenta_KeyUp(new KeyboardEventArgs { Key = "Enter" });
    }

    /******* Funciones Modales *******/
    private async Task MostrarBusquedaAvz() {
        if (!this.MostrarModal_BuscarCuentas) {
            this.MostrarModal_BuscarCuentas = true;
            Ruta tmpOfi = sicemService.ObtenerEnlaces(this.IdOficina).FirstOrDefault();
            await modal_BusquedaCuenta.Inicializar(tmpOfi);
        }
    }
    private async Task CerrarVentana_BusquedaCuenta(long? IdCuenta) {
        this.MostrarModal_BuscarCuentas = false;

        if (IdCuenta != null) {
            nCuenta = IdCuenta;
            await Tb_Cuenta_KeyUp(new KeyboardEventArgs { Key = "Enter" });
        }
    }

    private async Task BotonMovimienos_click(Microsoft.AspNetCore.Components.Web.MouseEventArgs e) {
        if (datosConsultaGral.Id_Padron <= 0) { return; }
        if (MostrarModalMovimientos) { return; }

        this.Busy = true;
        await Task.Delay(100);
        var tmpData = consultaGralService.ConsultaGral_Movimientos(IdOficina, datosConsultaGral.Id_Padron);
        if (tmpData == null) {
            Toaster.Add("Error al obtener el historial de movimientos, intente más tarde.", MatToastType.Danger, "");
            await Task.Delay(100);
            this.Busy = false;
            return;
        }
        if (tmpData.Count <= 0) {
            Toaster.Add("No hay movimientos registrados para esta cuenta.", MatToastType.Info, "");
            await Task.Delay(100);
            this.Busy = false;
            return;
        }

        var ofi = sicemService.ObtenerEnlaces(this.IdOficina).FirstOrDefault();
        this.MostrarModalMovimientos = true;
        vtn_Movimientos.Titulo = $"{ofi.Oficina} - CONSULTA MOVIMIENTOS - {datosConsultaGral.Id_Cuenta}";
        await vtn_Movimientos.Inicializar(ofi, tmpData.ToArray());

        await Task.Delay(100);
        this.Busy = false;

    }
    private async Task BotonModifABC_click(Microsoft.AspNetCore.Components.Web.MouseEventArgs e) {
        if (datosConsultaGral.Id_Padron <= 0) { return; }
        if (MostrarModalModifABC) { return; }

        this.Busy = true;
        await Task.Delay(100);

        var ofi = sicemService.ObtenerEnlaces(this.IdOficina).FirstOrDefault();
        var tmpData = consultaGralService.ConsultaGral_ModificacionesABC(IdOficina, datosConsultaGral.Id_Padron.ToString());

        if (tmpData == null) {
            Toaster.Add("Error al obtener el historial de modificaciones, intente más tarde.", MatToastType.Danger, "");
            await Task.Delay(100);
            this.Busy = false;
            return;
        }

        if (tmpData.Count <= 0) {
            Toaster.Add("No hay modificaciones registradas para esta cuenta.", MatToastType.Info, "");
            await Task.Delay(100);
            this.Busy = false;
            return;
        }

        this.MostrarModalModifABC = true;
        vtn_ModificacionesABS.Titulo = $"{ofi.Oficina} - MODIFICACIONES ABC - {datosConsultaGral.Id_Cuenta}";
        await vtn_ModificacionesABS.Inicializar(tmpData);

        await Task.Delay(100);
        this.Busy = false;

    }
    private async Task BotonOrdenes_click(Microsoft.AspNetCore.Components.Web.MouseEventArgs e) {
        if (datosConsultaGral.Id_Padron <= 0) { return; }
        if (MostrarModal_Ordenes) { return; }

        this.Busy = true;
        await Task.Delay(100);

        var tmpData = consultaGralService.ConsultaGral_Ordenes(IdOficina, datosConsultaGral.Id_Padron.ToString());

        if (tmpData == null) {
            Toaster.Add("Error al obtener el historial de ordenes de trabajado, intente más tarde.", MatToastType.Danger, "");
            await Task.Delay(100);
            this.Busy = false;
            return;
        }

        if (tmpData.Count <= 0) {
            Toaster.Add("No hay ordenes de trabajado para esta cuenta.", MatToastType.Info, "");
            await Task.Delay(100);
            this.Busy = false;
            return;
        }

        var ofi = sicemService.ObtenerEnlaces(this.IdOficina).FirstOrDefault();
        this.MostrarModal_Ordenes = true;
        vnt_Ordenes.Titulo = $"{ofi.Oficina} - ORDENES DE TRABAJO - {datosConsultaGral.Id_Cuenta}";
        await vnt_Ordenes.Inicializar(tmpData);

        await Task.Delay(100);
        this.Busy = false;

    }
    private async Task BotonAnticipos_click(Microsoft.AspNetCore.Components.Web.MouseEventArgs e) {
        if (datosConsultaGral.Id_Padron <= 0) { return; }
        if (MostrarModal_Anticipos) { return; }

        this.Busy = true;
        await Task.Delay(100);

        var tmpData = consultaGralService.ConsultaGral_Anticipos(IdOficina, datosConsultaGral.Id_Padron.ToString());

        if (tmpData == null) {
            Toaster.Add("No hay anticipos registrados a esta cuenta.", MatToastType.Info, "");
            await Task.Delay(100);
            this.Busy = false;
            return;
        }
        if (tmpData.Anticipos.Count <= 0) {
            Toaster.Add("No hay anticipos registrados a esta cuenta.", MatToastType.Info, "");
            await Task.Delay(100);
            this.Busy = false;
            return;
        }

        var ofi = sicemService.ObtenerEnlaces(this.IdOficina).FirstOrDefault();
        this.MostrarModal_Anticipos = true;
        vnt_Ordenes.Titulo = $"{ofi.Oficina} - ANTICIPOS - {datosConsultaGral.Id_Cuenta}";
        await vnt_Anticipos.Inicializar(tmpData);

        await Task.Delay(100);
        this.Busy = false;

    }
    private async Task BotonImagenes_click(Microsoft.AspNetCore.Components.Web.MouseEventArgs e) {
        if (datosConsultaGral.Id_Padron >= 1) {

            if (!MostrarModal_Imagenes) {
                this.Busy = true;
                await Task.Delay(200);

                var tmpData = consultaGralService.ConsultaGral_Imagenes(IdOficina, datosConsultaGral.Id_Padron.ToString());

                if (tmpData == null) {
                    Toaster.Add("Error al tratar de realizar la consulta, intente mas tarde", type: MatToastType.Danger);
                }
                else {
                    if (tmpData.ToArray().Length > 0) {
                        this.MostrarModal_Imagenes = true;
                        vnt_Imagenes.OficinaActual = IdOficina;
                        await vnt_Imagenes.Inicializar(tmpData);
                    }
                    else {
                        Toaster.Add("No hay documentos para este usuario", MatToastType.Info, "Sin Datos", null);
                    }
                }
                this.Busy = false;
            }
        }
    }
    private void MostrarUbicacion_Click(){
        if(!datosConsultaGral.TieneUbicacion){
            return;
        }

        //*** Mostrar datos en el mapa
        double latitud = Double.Parse(this.datosConsultaGral.Latitud);
        double longitud = Double.Parse(this.datosConsultaGral.Longitud);
        MarkerDataSource.Clear();
        MarkerDataSource.Add(new MapMark(){
            Latitude = latitud,
            Longitude = longitud,
            Descripcion = $"{this.datosConsultaGral.Id_Cuenta} {this.datosConsultaGral.Razon_social} {latitud}, {longitud}",
            Zoom = 17,
            IdOficina = IdOficina,
            IdPadron = datosConsultaGral.Id_Padron,
            IdCuenta = datosConsultaGral.Id_Cuenta,
            MesesAdeudo =  Convert.ToInt32(datosConsultaGral.MesesAdeudo)
        });
        mostrarMapa = true;
    }

}
