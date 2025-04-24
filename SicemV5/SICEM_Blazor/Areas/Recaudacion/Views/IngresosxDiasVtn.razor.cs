using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Charts;
using Microsoft.AspNetCore.Components;
using MatBlazor;
using SICEM_Blazor.Data.Contracts;
using SICEM_Blazor.Recaudacion.Models;
using SICEM_Blazor.Recaudacion.Data;
using SICEM_Blazor.Services;
using SICEM_Blazor.Models;
using SICEM_Blazor.Areas.Recaudacion.Views;
using SICEM_Blazor.Data;

namespace SICEM_Blazor.Recaudacion.Views;

public partial class IngresosxDiasVtn
{
    [Inject]
    public IRecaudacionService RecaudacionService {get;set;}

    [Inject]
    public IMatToaster Toaster {get;set;}

    [Parameter]
    public bool MostrarVentana { get; set; }
    
    [Parameter]
    public EventCallback CerrarModal { get; set; }
    
    [Parameter]
    public DateTime Fecha1 { get; set; }
    
    [Parameter]
    public DateTime Fecha2 { get; set; }
    
    [Parameter]
    public int Subsistema { get; set; }
    
    [Parameter]
    public int Sector { get; set; }
    
    public IEnlace Enlace { get; set; }
    public string Titulo = "INGRESOS DIAS";
    public SfGrid<IngresosDia> DataGrid;
    public List<IngresosDia> DatosIngresos { get; set; }

    bool busyDialog = false, ventanDetalle_visible = false, VtnConceptosUsuarios_Visible = false, VtnFormasPago_Visible = false, VtnCajas_visible = false;
    private Recaudacion_IngresosxDias_detalle vtn_diasDetalle;
    private Recaudacion_ConceptosyTiposUsuariosVtn VtnConceptosUsuarios;
    private Recaudacion_FormasPago_View VtnFormasPago;
    private Recaudacion_IngresosxCajas VtnCajas;


    // * Funciones Generales
    public void Inicializar(IEnlace enlace, IEnumerable<IngresosDia> datos)
    {
        this.Enlace = enlace;
        this.DatosIngresos = datos.ToList();
    }

    private async Task Cerrar_Modal()
    {
        if(DatosIngresos != null){
            DatosIngresos.Clear();
        }
        await CerrarModal.InvokeAsync("");
    }

    private async Task GenerarExcel()
    {
        await this.DataGrid.ExcelExport(new ExcelExportProperties {
            FileName = string.Format("sicem_ingresosxDias_{0}.xlsx", Guid.NewGuid().ToString().Replace("-", ""))
        });
    }

    private async Task VerDetalleDias(DateTime? fecha)
    {
        
        if (ventanDetalle_visible) {
            return;
        }
        if(fecha == null) {
            return;
        }
        this.busyDialog = true;
        await Task.Delay(200);

        var tmpData = RecaudacionService.ObtenerDetalleIngresos(Enlace, fecha??DateTime.Now, fecha?? DateTime.Now, this.Subsistema, this.Sector);
        
        if (tmpData.Count() > 0) {
            this.ventanDetalle_visible = true;
            vtn_diasDetalle.Inicializar(Enlace, tmpData, fecha ?? DateTime.Now);
        }
        
        await Task.Delay(200);
        busyDialog = false;
    }

    private async Task IngresosPorConceptosTiposUsuarios(IngresosDia data)
    {
        this.busyDialog = true;
        await Task.Delay(100);

        var _date = Convert.ToDateTime(data.Fecha);
        Fecha1 = _date;
        Fecha2 = _date;
        this.StateHasChanged();

        try
        {
            var _datos = RecaudacionService.ObtenerRecaudacionPorConceptosYTipoUsuario(Enlace, _date, _date, this.Subsistema, this.Sector).ToList();
            if( _datos == null){
                Toaster.Add("Error al tratar de obtener los ingresos por conceptos", MatToastType.Danger);
            }else{
                if(_datos.Count() <= 0){
                    Toaster.Add("No hay datos disponibles para este periodo", MatToastType.Info);
                }else{
                    VtnConceptosUsuarios_Visible = true;
                    await Task.Delay(100);
                    VtnConceptosUsuarios.Inicializar(Enlace, _datos);
                }
            }
        }catch(Exception)
        {
            Toaster.Add("Error al tratar de obtener los ingresos por conceptos", MatToastType.Danger);
        }

        await Task.Delay(100);
        this.busyDialog = false;
    }

    private async Task IngresosPorCajas(IngresosDia data)
    {
        this.busyDialog = true;
        await Task.Delay(100);

        var _date = Convert.ToDateTime(data.Fecha);
        Fecha1 = _date;
        Fecha2 = _date;
        this.StateHasChanged();

        var tmpData = RecaudacionService.ObtenerIngresosPorCajas(Enlace, _date, _date, this.Subsistema, this.Sector);
        if (tmpData == null) {
            Toaster.Add("Hubo un error al procesar la petición, inténtelo mas tarde.", MatToastType.Warning);
        }
        else {
            if (tmpData.Count() > 0) {
                VtnCajas_visible = true;
                VtnCajas.Titulo = $"{Enlace.Nombre.ToUpper()} - INGRESOS POR CAJAS DEL {_date.Day} DE {_date.ToString("MMMM").ToUpper()} DEL {_date.Year}";
                VtnCajas.Inicializar(Enlace, tmpData);
            }
            else {
                Toaster.Add("No hay datos disponibles para mostrar.", MatToastType.Info);
            }
        }

        await Task.Delay(100);
        this.busyDialog = false;
    
    }

    private async Task IngresosPorFormasDePago(IngresosDia data)
    {
        this.busyDialog = true;
        await Task.Delay(100);

        var _date = Convert.ToDateTime(data.Fecha);
        Fecha1 = _date;
        Fecha2 = _date;
        this.StateHasChanged();

        var _result = RecaudacionService.ObtenerIngresosPorFormasPago(Enlace, _date, _date, this.Subsistema, this.Sector);
        if (_result == null) {
            // * Mostrar mensaje error
            Toaster.Add("Erro al procesar la consulta, intente mas tarde.", type: MatToastType.Danger);
        }
        else {
            if (_result.Count() <= 0) {
                // * Mostrar mensaje no hay datos
                Toaster.Add("No hay datos disponibles.", type: MatToastType.Info);
            }
            else {
                // * Mostrar ventana secundaria
                this.VtnFormasPago_Visible = true;
                VtnFormasPago.Titulo = $"{Enlace.Nombre} - INGRESOS POR FORMAS DE PAGO DEL {_date.Day} DE {_date.ToString("MMMM").ToUpper()} DEL {_date.Year}";
                this.VtnFormasPago.Inicializar(Enlace, _result);
            }
        }

        await Task.Delay(100);
        this.busyDialog = false;
    }

}