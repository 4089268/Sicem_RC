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

public partial class RecaudacionPage
{
    [Inject]
    public NavigationManager NavigationManager1 {get;set;}

    [Inject]
    public IMatToaster Toaster {get;set;}
    
    [Inject]
    public IRecaudacionService RecaudacionService {get;set;}
    
    [Inject]
    public SicemService SicemService1 {get;set;}

    private SfGrid<ResumenOficina> dataGrid { get; set; }
    private SfChart graficaIngresos { get; set; }
    private SfChart graficaUsuarios { get; set; }
    private bool busyDialog = false;
    private DateTime f1, f2;
    private int subsistema, sector;
    private List<ResumenOficina> datosRecaudacion = new();
    private List<ChartItem> datosGraficaIngresos = new();
    private List<ChartItem> datosGraficaUsuarios = new();

    private IngresosAnaliticoVtn VtnAnalitico;
    private bool VtnAnaliticoVisible = false;

    private Recaudacion_IngresosRezago VtnRezago;
    private bool VtnRezago_visible = false;

    private IngresosxDiasVtn VtnDias;
    private bool vtnDiasVisible = false;

    private Recaudacion_IngresosxCajas VtnCajas;
    private bool VtnCajas_visible = false;

    private Recaudacion_IngresosConceptos VtnConceptos;
    private bool VtnConceptos_visible = false;

    private Recaudacion_IngresosTipoUsuario VtnTiposUsuarios;
    private bool VtnTiposUsuarios_Visible = false;

    private Recaudacion_LocalidadesVtn VtnPoblaciones;
    private bool VtnPoblaciones_Visible = false;

    private Recaudacion_IngresosPagosMayoresVtn VtnPagosMayores;
    private bool VtnPagosMayores_Visible = false;

    private Recaudacion_FormasPago_View VtnFormasPago;
    private bool VtnFormasPago_Visible = false;

    private RecaudacionConceptosTarifaVtn VtnRConceptos;
    private bool VtnRConceptos_Visible = false;


    protected override void OnInitialized()
    {
        var _now = DateTime.Now;
        this.f1 = new DateTime(_now.Year, _now.Month, 1);
        this.f2 = new DateTime(_now.Year, _now.Month, DateTime.DaysInMonth(_now.Year, _now.Month));
        this.subsistema = 0;
        this.sector = 0;
    }

    public void Procesar(SeleccionarFechaEventArgs e)
    {
        this.f1 = e.Fecha1;
        this.f2 = e.Fecha2;
        this.subsistema = e.Subsistema;
        this.sector = e.Sector;

        IEnlace[] enlaces = SicemService1.ObtenerOficinasDelUsuario().ToArray();

        // * Preparar filas
        datosRecaudacion = new List<ResumenOficina>();
        var tareas = new List<Task>();

        // * Prepara Filas
        foreach(var enlace in enlaces)
        {
            var oficinaModel = new ResumenOficina(enlace);
            oficinaModel.Enlace = enlace;
            datosRecaudacion.Add(oficinaModel);

            // * Agregar tarea
            tareas.Add(new Task(() => ConsultarOficina(enlace)) );
        }

        // * Agregar fila total
        if (enlaces.Length > 1)
        {
            var oficinaModel = new ResumenOficina( new Ruta{Id = 999, Oficina = "TOTAL"})
            {
                Estatus = ResumenOficinaEstatus.Completado
            };
            datosRecaudacion.Add(oficinaModel);
        }

        // * Iniciar tareas
        foreach(var tarea in tareas)
        {
            tarea.Start();
        }

    }

    private void ConsultarOficina(IEnlace enlace)
    {
        // * Realizar consulta
        ResumenOficina tmpDatos = RecaudacionService.ObtenerResumen(enlace, new DateRange(f1, f2, subsistema, sector));

        var _random = new Random();
        var sleep = _random.Next(3000);
        System.Threading.Thread.Sleep(sleep);

        // * Refrescar datos grid
        lock(datosRecaudacion)
        {
            // * Actualizar fila grid
            var item = datosRecaudacion.Where(item => item.Id == enlace.Id).FirstOrDefault();
            if (item != null) {
                if (tmpDatos.Estatus == ResumenOficinaEstatus.Completado)
                {
                    item.Estatus = tmpDatos.Estatus;
                    item.IngresosPropios = tmpDatos.IngresosPropios;
                    item.RecibosPropios = tmpDatos.RecibosPropios;
                    item.IngresosOtros = tmpDatos.IngresosOtros;
                    item.RecibosOtros = tmpDatos.RecibosOtros;
                    item.ImporteTotal = tmpDatos.ImporteTotal;
                    item.Usuarios = tmpDatos.Usuarios;
                }
                else
                {
                    item.Estatus = ResumenOficinaEstatus.Error;
                }
            }

            RecalcularFilaTotal();
            dataGrid.Refresh();

            // * check if all tasks are completed
            if(datosRecaudacion.All(item => item.Estatus != ResumenOficinaEstatus.Pendiente))
            {
                Console.WriteLine("Generando graficas...");
                CalculateChart();
            }
        }
    }

    private void RecalcularFilaTotal()
    {
        //*** Recalcular fila total
        var itemTotal = datosRecaudacion.Where(item => item.Id == 999).FirstOrDefault();
        if (itemTotal != null) {
            var _tmpData = datosRecaudacion.Where(item => item.Id != 999).ToList();
            itemTotal.IngresosPropios = _tmpData.Sum(item => item.IngresosPropios);
            itemTotal.RecibosPropios = _tmpData.Sum(item => item.RecibosPropios);
            itemTotal.IngresosOtros = _tmpData.Sum(item => item.IngresosOtros);
            itemTotal.RecibosOtros = _tmpData.Sum(item => item.RecibosOtros);
            itemTotal.ImporteTotal = _tmpData.Sum(item => item.ImporteTotal);
            itemTotal.Usuarios = _tmpData.Sum(item => item.Usuarios);
        }
    }

    private void CalculateChart()
    {
        datosGraficaIngresos = this.datosRecaudacion.Where(item => item.Id < 900).Select( item => new ChartItem {
            Id = item.Id,
            Descripcion = item.Oficina,
            Valor1 = item.ImporteTotal,
        }).ToList();

        datosGraficaUsuarios = this.datosRecaudacion.Where(item => item.Id < 900).Select( item => new ChartItem {
            Id = item.Id,
            Descripcion = item.Oficina,
            Valor1 = item.Usuarios,
        }).ToList();

        graficaIngresos.Refresh();
        graficaUsuarios.Refresh();
    }

    private async Task ExportarExcelClick()
    {
        var _options = new ExcelExportProperties(){
            IncludeHiddenColumn = true
        };
        await this.dataGrid.ExportToExcelAsync(_options);
    }

    #region Funciones Ventanas Secundarias
    private async Task AnaliticoClick(ResumenOficina data)
    {
        if (VtnAnaliticoVisible) {
            return;
        }
        this.busyDialog = true;
        await Task.Delay(200);
        var tmpdata = RecaudacionService.ObtenerAnalisisIngresos(data.Enlace, new DateRange(f1, f2, subsistema, sector));
        if (tmpdata == null) {
            Toaster.Add("Hubo un error al procesar la peticion, intentelo mas tarde.", MatToastType.Warning);
        }
        else {
            VtnAnaliticoVisible = true;
            VtnAnalitico.Titulo = $"{data.Enlace.Nombre.ToUpper()} - INGRESOS ANALITICO";
            await VtnAnalitico.Inicializar(data.Enlace, tmpdata);
        }
        await Task.Delay(200);
        this.busyDialog = false;
    }
    
    private async Task Rezago_Click(ResumenOficina data)
    {
        if (VtnRezago_visible) {
            return;
        }
        this.busyDialog = true;
        await Task.Delay(200);
        var tmpData = RecaudacionService.ObtenerRezago(data.Enlace, this.f1, this.f2, this.subsistema, this.sector);
        if (tmpData == null) {
            Toaster.Add("Hubo un error al procesar la petición, inténtelo mas tarde.", MatToastType.Warning);
        }
        else {
            VtnRezago_visible = true;
            // VtnRezago.Titulo = $"{data.Enlace.Nombre.ToUpper()} - ANALISIS DE REZAGO";
            VtnRezago.Titulo = $"ANALISIS DE REZAGO";
            await VtnRezago.Inicializar(data.Enlace, tmpData);
        }
        await Task.Delay(200);
        this.busyDialog = false;
    }

    private async Task IngresosDias_Click(ResumenOficina data)
    {
        if(vtnDiasVisible)
        {
            return;
        }
        this.busyDialog = true;
        await Task.Delay(200);
        var tmpData = this.RecaudacionService.ObtenerIngresosPorDias(data.Enlace, new DateRange(this.f1, this.f2, this.subsistema, this.sector));
        if (tmpData == null)
        {
            Toaster.Add("Hubo un error al procesar la petición, inténtelo mas tarde.", MatToastType.Warning);
        }
        else
        {
            vtnDiasVisible = true;
            VtnDias.Titulo = $"{data.Enlace.Nombre.ToUpper()} - INGRESOS DIAS";
            VtnDias.Inicializar(data.Enlace, tmpData);
        }
        await Task.Delay(200);
        this.busyDialog = false;
    }
    
    private async Task IngresosCajas_Click(ResumenOficina data)
    {
        if (VtnCajas_visible) {
            return;
        }
        this.busyDialog = true;
        await Task.Delay(200);
        var tmpData = RecaudacionService.ObtenerIngresosPorCajas(data.Enlace, this.f1, this.f2, this.subsistema, this.sector);
        if (tmpData == null) {
            Toaster.Add("Hubo un error al procesar la petición, inténtelo mas tarde.", MatToastType.Warning);
        }
        else {
            if (tmpData.Count() > 0) {
                VtnCajas_visible = true;
                VtnCajas.Titulo = $"{data.Enlace.Nombre.ToUpper()} - INGRESOS POR CAJAS";
                VtnCajas.Inicializar(data.Enlace, tmpData);
            }
            else {
                Toaster.Add("No hay datos disponibles para mostrar.", MatToastType.Info);
            }
        }
        await Task.Delay(200);
        this.busyDialog = false;
    }
    
    private async Task IngresosConceptos_Click(ResumenOficina data)
    {
        if (VtnConceptos_visible) {
            return;
        }
        this.busyDialog = true;
        await Task.Delay(100);

        var tmpData = RecaudacionService.ObtenerIngresosPorConceptos(data.Enlace, this.f1, this.f2, this.subsistema, this.sector);
        if (tmpData == null) {
            Toaster.Add("Hubo un error al procesar la petición, inténtelo mas tarde.", MatToastType.Warning);
        }
        else {
            if (tmpData.Count() > 0) {
                VtnConceptos_visible = true;
                VtnConceptos.Titulo = $"{data.Enlace.Nombre.ToUpper()} - INGRESOS POR CONCEPTOS";
                VtnConceptos.Inicializar(data.Enlace, tmpData);
            }
            else {
                Toaster.Add("No hay datos disponibles para mostrar.", MatToastType.Info);
            }
        }
        await Task.Delay(100);
        this.busyDialog = false;
    }
    
    private async Task IngresosTipoUsuarios_Click(ResumenOficina data)
    {
        this.busyDialog = true;
        await Task.Delay(100);
        if (!VtnTiposUsuarios_Visible) {
            var tmpData = RecaudacionService.ObtenerIngresosPorTipoUsuarios(data.Enlace, this.f1, this.f2, this.subsistema, this.sector);
            if (tmpData == null) {
                Toaster.Add("Hubo un error al procesar la petición, inténtelo mas tarde.", MatToastType.Warning);
            }
            else {
                if (tmpData.Count() > 0) {
                    VtnTiposUsuarios_Visible = true;
                    VtnTiposUsuarios.Titulo = $"{data.Enlace.Nombre.ToUpper()} - INGRESOS TIPO DE USUARIO";
                    VtnTiposUsuarios.Inicializar(data.Enlace, tmpData);
                }
                else {
                    Toaster.Add("No hay datos disponibles para mostrar.", MatToastType.Info);
                }
            }
        }
        await Task.Delay(100);
        this.busyDialog = false;
    }
    
    private async Task IngresosPoPoblacion_Click(ResumenOficina data)
    {
        this.busyDialog = true;
        await Task.Delay(100);
        if (!VtnPoblaciones_Visible) {
            var tmpData = RecaudacionService.ObtenerRecaudacionLocalidades(data.Enlace, this.f1, this.f2, this.subsistema, this.sector);
            if (tmpData == null) {
                Toaster.Add("Hubo un error al procesar la peticion, intentelo mas tarde.", MatToastType.Warning);
            }
            else {
                if (tmpData.Count() > 0) {
                    VtnPoblaciones_Visible = true;
                    VtnPoblaciones.Titulo = $"{data.Enlace.Nombre} - INGRESOS POR POBLACIONES";
                    VtnPoblaciones.Inicializar(tmpData, data.Enlace);
                }
                else {
                    Toaster.Add("No hay datos disponibles para mostrar.", MatToastType.Info);
                }
            }
        }
        await Task.Delay(100);
        this.busyDialog = false;
    }
    
    private async Task IngresosAltos_Click(ResumenOficina data)
    {
        if (VtnPagosMayores_Visible) {
            return;
        }
        VtnPagosMayores_Visible = true;
        await VtnPagosMayores.Inicializar(data.Enlace);
    }
    
    private async Task IngresosFormasdePago_Click(ResumenOficina data)
    {
        if (VtnFormasPago_Visible) {
            return;
        }

        this.busyDialog = true;
        await Task.Delay(100);

        var _result = RecaudacionService.ObtenerIngresosPorFormasPago(data.Enlace, this.f1, this.f2, this.subsistema, this.sector);
        if (_result == null) {
            //**** Mostrar mensaje error
            Toaster.Add("Erro al procesar la consulta, intente mas tarde.", type: MatToastType.Danger);
        }
        else {
            if (_result.Count() <= 0) {
                //**** Mostrar mensaje no hay datos
                Toaster.Add("No hay datos disponibles.", type: MatToastType.Info);
            }
            else {
                //**** Mostrar ventana secundaria
                this.VtnFormasPago_Visible = true;
                VtnFormasPago.Titulo = $"{data.Enlace.Nombre} - INGRESOS POR FORMAS DE PAGO";
                this.VtnFormasPago.Inicializar(data.Enlace, _result);
            }
        }

        await Task.Delay(100);
        this.busyDialog = false;
        return;
    }

    private async Task IngresosPorConceptosTiposUsuarios(ResumenOficina data)
    {
        this.busyDialog = true;
        await Task.Delay(100);

        var _datos = RecaudacionService.ObtenerIngresosPorConceptosTipoUsuarios(data.Enlace, this.f1, this.f2, this.subsistema, this.sector, 0).ToList();
        if(_datos == null)
        {
            Toaster.Add("Error al tratar de obtener los ingresos por conceptos", MatToastType.Danger);
        }
        else
        {
            if(_datos.Count() <= 0)
            {
                Toaster.Add("No hay datos disponibles para este periodo", MatToastType.Info);
            }
            else
            {
                // Generar catalogo de localidades
                var catLocalidades = new Dictionary<int,string>();
                var _localidades = SicemService1.ObtenerCatalogoLocalidades(data.Enlace.Id).Where(i => i.Id_Poblacion > 0).ToList();
                catLocalidades.Add(0, "TODOS");
                foreach( var loc in _localidades)
                {
                    catLocalidades.Add(loc.Id_Poblacion, loc.Descripcion.ToUpper().Trim());
                }
                
                // Inicializar ventana secundaria
                VtnRConceptos_Visible = true;
                VtnRConceptos.Inicializar(data.Enlace, _datos, catLocalidades);
            }
        }

        await Task.Delay(100);
        this.busyDialog = false;
    }
    #endregion
}