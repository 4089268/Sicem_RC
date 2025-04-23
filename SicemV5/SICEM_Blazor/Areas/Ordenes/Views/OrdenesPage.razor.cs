using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MatBlazor;
using SICEM_Blazor.Ordenes.Models;
using SICEM_Blazor.Ordenes.Data;
using SICEM_Blazor.Areas.Ordenes.Views;
using SICEM_Blazor.Services;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Charts;
using SICEM_Blazor.Models;
using SICEM_Blazor.Data;


namespace SICEM_Blazor.Ordenes.Views;

public partial class OrdenesPage : ComponentBase {

    [Inject]
    public NavigationManager navigationManager {get;set;}
    
    [Inject]
    public IMatToaster Toaster {get;set;}
    
    [Inject]

    public OrdenesService ordenesService {get;set;}
    
    [Inject]
    public SicemService sicemService {get; set;}

    private SfGrid<Ordenes_Oficina> DataGrid { get; set; }
    private SfChart GraficaUsuarios { get; set; }

    private bool busyDialog = false;
    private List<Ordenes_Oficina> DatosGrid { get; set; }
    private List<ChartItem> DatosGrafica { get; set;}
    private DateTime f1, f2;
    private int Subsistema, Sector;

    private Ordenes_DetalleVtn vtn_detalle;
    private Ordenes_ColoniasVtn vtn_colonias;
    private Ordenes_TrabajosVtn vtn_trabajos;
    private Ordenes_RealizacionVtn vtn_realizacion;
    private Ordenes_CapturacionVtn vtn_capturacion;

    private bool vtn_detalle_visible = false, vtn_colonias_visible = false, vtn_trabajos_visible = false, vtn_realizacion_visible = false, vtn_capturacion_visible = false;

    protected override void OnInitialized() {

        // Validar si se cuenta con la opcion
        if(!sicemService.Usuario.OpcionSistemas.Select(item => item.Id).Contains(OpcionesSistema.ORDENES_TRABAJO)){
            navigationManager.NavigateTo("/");
            return;
        }

        var _now =DateTime.Now;
        this.f1 = new DateTime(_now.Year, _now.Month, 1);
        this.f2 = new DateTime(_now.Year, _now.Month, DateTime.DaysInMonth(_now.Year, _now.Month));
        this.Subsistema = 0;
        this.Sector = 0;
    }
    public void Procesar(SeleccionarFechaEventArgs e) {
        this.f1 = e.Fecha1;
        this.f2 = e.Fecha2;
        this.Subsistema = e.Subsistema;
        this.Sector = e.Sector;

        this.DatosGrid = new List<Ordenes_Oficina>();
        this.DatosGrafica = new List<ChartItem>();
        var Tareas = new List<Task>();
        IEnlace[] oficinas = sicemService.ObtenerOficinasDelUsuario().ToArray();

        foreach(var oficina in oficinas){
            var _itemGrid = new Ordenes_Oficina() {
                Estatus = 0,
                IdOficina = oficina.Id,
                Oficina = oficina.Nombre
            };
            this.DatosGrid.Add(_itemGrid);

            var _itemGrafica = new ChartItem() {
                Id = oficina.Id,
                Descripcion = oficina.Nombre
            };
            this.DatosGrafica.Add(_itemGrafica);

            Tareas.Add( new Task(()=>ProcesarConsulta(oficina)) );
        }


        if(oficinas.Count()  > 1) {
            var _itemTotal = new Ordenes_Oficina() {
                IdOficina = 999,
                Oficina = " TOTAL"
            };
            this.DatosGrid.Add(_itemTotal);
        }

        Tareas.ForEach(item => item.Start());

    }
    private void Procesar_Datos_Grafica(){
        var tmpList1 = new List<ChartItem>();
        foreach (var item in this.DatosGrid) {
            if (item.IdOficina > 0 && item.IdOficina < 999) {
                var newItem = new ChartItem();
                newItem.Id = item.IdOficina;
                newItem.Descripcion = item.Oficina;
                newItem.Valor1 = item.Pendi;
                newItem.Valor2 = item.Eneje;
                newItem.Valor3 = item.Reali;
                newItem.Valor4 = item.Cance;
                tmpList1.Add(newItem);
            }
        }
        this.DatosGrafica = tmpList1;
    }
    private void ProcesarConsulta(IEnlace oficina) {
        var tmpDatos = ordenesService.ObtenerOrdenesPorOficina(oficina, f1.ToString("yyyyMMdd"), f2.ToString("yyyyMMdd"), Subsistema, Sector);

        var _random = new Random();
        var _sleepTime = _random.Next(100,3000);
        System.Threading.Thread.Sleep(_sleepTime);
        
        lock(DatosGrid){
            var _itemGrid = DatosGrid.Where(item => item.IdOficina == oficina.Id).FirstOrDefault();
            if (_itemGrid != null) {
                _itemGrid.Estatus = tmpDatos.Estatus;
                _itemGrid.Pendi = tmpDatos.Pendi;
                _itemGrid.Eneje = tmpDatos.Eneje;
                _itemGrid.Reali = tmpDatos.Reali;
                _itemGrid.Cance = tmpDatos.Cance;
                _itemGrid.Eje = tmpDatos.Eje;
                _itemGrid.No_eje = tmpDatos.No_eje;
                _itemGrid.Total = tmpDatos.Total;
            }
            RecalcularFilaTotal();
            DataGrid.Refresh();
        }

        lock(DatosGrafica){
            var _itemGrafica = DatosGrafica.Where(item => item.Id == oficina.Id).FirstOrDefault();
            if (_itemGrafica != null) {
                _itemGrafica.Valor1 = (decimal)tmpDatos.Pendi;
                _itemGrafica.Valor2 = (decimal)tmpDatos.Eneje;
                _itemGrafica.Valor3 = (decimal)tmpDatos.Reali;
                _itemGrafica.Valor4 = (decimal)tmpDatos.Cance;
            }
            try {
                GraficaUsuarios.RefreshLiveData();
            }
            catch (Exception) { }
        }
    }
    private void RecalcularFilaTotal() {
        var _itemTotal = DatosGrid.Where(item => item.IdOficina == 999).FirstOrDefault();
        if (_itemTotal != null) {
            var _datos = DatosGrid.Where(item => item.IdOficina != 999).ToList();
            _itemTotal.Pendi = _datos.Sum(item => item.Pendi);
            _itemTotal.Eneje = _datos.Sum(item => item.Eneje);
            _itemTotal.Reali = _datos.Sum(item => item.Reali);
            _itemTotal.Cance = _datos.Sum(item => item.Cance);
            _itemTotal.Eje = _datos.Sum(item => item.Eje);
            _itemTotal.No_eje = _datos.Sum(item => item.No_eje);
            _itemTotal.Total = _datos.Sum(item => item.Total);
        }
    }


    //*** Funciones Ventanas Secundarias
    private async Task Mostrar_Vtn_Detalle(Ordenes_Oficina data, int opcion) {
        if (!vtn_detalle_visible) {
            this.busyDialog = true;
            await Task.Delay(100);

            Ordenes_Detalle[] datos = new Ordenes_Detalle[] { };
            var titulo = $"{data.Oficina} / Ordenes Generadas";
            switch (opcion) {
                case 0:
                    datos = ordenesService.ObtenerOrdenesDetalle(data.IdOficina, f1.ToString("yyyyMMdd"), f2.ToString("yyyyMMdd"), Subsistema, Sector, "0");
                    titulo = $"Ordenes Generadas del {f1.ToString("dd/MM/yyyy")} al  {f2.ToString("dd/MM/yyyy")}";
                    break;
                case 1:
                    datos = ordenesService.ObtenerOrdenesDetalle(data.IdOficina, f1.ToString("yyyyMMdd"), f2.ToString("yyyyMMdd"), Subsistema, Sector, "7");
                    titulo = $"Ordenes Pendientes del {f1.ToString("dd/MM/yyyy")} al  {f2.ToString("dd/MM/yyyy")}";
                    break;
                case 2:
                    datos = ordenesService.ObtenerOrdenesDetalle(data.IdOficina, f1.ToString("yyyyMMdd"), f2.ToString("yyyyMMdd"), Subsistema, Sector, "8");
                    titulo = $"Ordenes En Ejecucion del {f1.ToString("dd/MM/yyyy")} al  {f2.ToString("dd/MM/yyyy")}";
                    break;
                case 3:
                    datos = ordenesService.ObtenerOrdenesDetalle(data.IdOficina, f1.ToString("yyyyMMdd"), f2.ToString("yyyyMMdd"), Subsistema, Sector, "9");
                    titulo = $"Ordenes Realizadas del {f1.ToString("dd/MM/yyyy")} al  {f2.ToString("dd/MM/yyyy")}";
                    break;
                case 4:
                    datos = ordenesService.ObtenerOrdenesDetalle(data.IdOficina, f1.ToString("yyyyMMdd"), f2.ToString("yyyyMMdd"), Subsistema, Sector, "10");
                    titulo = $"Ordenes Canceladas del {f1.ToString("dd/MM/yyyy")} al  {f2.ToString("dd/MM/yyyy")}";
                    break;
                case 5:
                    datos = ordenesService.ObtenerOrdenesDetalle(data.IdOficina, f1.ToString("yyyyMMdd"), f2.ToString("yyyyMMdd"), Subsistema, Sector, "9,1");
                    titulo = $"Ordenes Ejecutadas del {f1.ToString("dd/MM/yyyy")} al  {f2.ToString("dd/MM/yyyy")}";
                    break;
                case 6:
                    datos = ordenesService.ObtenerOrdenesDetalle(data.IdOficina, f1.ToString("yyyyMMdd"), f2.ToString("yyyyMMdd"), Subsistema, Sector, "9,0");
                    titulo = $"Ordenes No Ejecutadas del {f1.ToString("dd/MM/yyyy")} al  {f2.ToString("dd/MM/yyyy")}";
                    break;
            }

            if (datos == null) {
                busyDialog = false;
                Toaster.Add("No hay datos disponibles", MatToastType.Info, "Sin Datos", null);
                return;
            }
            if (datos.Length <= 0) {
                busyDialog = false;
                Toaster.Add("No hay datos disponibles", MatToastType.Info, "Sin Datos", null);
                return;
            }

            vtn_detalle_visible = true;
            await vtn_detalle.Inicializar(datos, titulo);

            await Task.Delay(100);
            this.busyDialog = false;
        }
    }
    private async Task Mostrar_Vtn_Colonias(Ordenes_Oficina data) {
        if (!vtn_colonias_visible) {
            this.busyDialog = true;
            await Task.Delay(100);

            Ordenes_Agrupado[] datos = ordenesService.ObtenerOrdenesColonias(data.IdOficina, f1.ToString("yyyyMMdd"), f2.ToString("yyyyMMdd"), Subsistema, Sector, "");

            if (datos == null) {
                busyDialog = false;
                Toaster.Add("No hay datos disponibles", MatToastType.Info, "Sin Datos", null);
                return;
            }
            if (datos.Length <= 0) {
                busyDialog = false;
                Toaster.Add("No hay datos disponibles", MatToastType.Info, "Sin Datos", null);
                return;
            }

            var tmpOfi = sicemService.ObtenerEnlaces(data.IdOficina).FirstOrDefault();
            if(tmpOfi != null){
                vtn_colonias_visible = true;
                vtn_colonias.Titulo = $"{data.Oficina} / Ordenes agrupadas por colonias";
                vtn_colonias.OficinaActual = tmpOfi;
                await vtn_colonias.Inicializar(datos);
            }

            await Task.Delay(100);
            this.busyDialog = false;
        }
    }
    private async Task Mostrar_Vtn_Trabajos(Ordenes_Oficina data) {
        if (!vtn_trabajos_visible) {
            this.busyDialog = true;
            await Task.Delay(100);

            Ordenes_Agrupado[] datos = ordenesService.ObtenerOrdenesTrabajos(data.IdOficina, f1.ToString("yyyyMMdd"), f2.ToString("yyyyMMdd"), Subsistema, Sector);
            if (datos == null) {
                Toaster.Add("No hay datos disponibles", MatToastType.Info, "Sin Datos", null);
                await Task.Delay(100);
                busyDialog = false;
                return;
            }
            if (datos.Length <= 0) {
                Toaster.Add("No hay datos disponibles", MatToastType.Info, "Sin Datos", null);
                await Task.Delay(100);
                busyDialog = false;
                return;
            }
            var tmpOfi = sicemService.ObtenerEnlaces(data.IdOficina).FirstOrDefault();
            if(tmpOfi != null){
                vtn_trabajos_visible = true;
                vtn_trabajos.OficinaActual = tmpOfi;
                vtn_trabajos.Titulo = $"{data.Oficina} / Ordenes por Trabajos";
                await vtn_trabajos.Inicializar(datos);
            }

            await Task.Delay(100);
            this.busyDialog = false;
        }
    }
    private async Task Mostrar_Vtn_Realizacion(Ordenes_Oficina data) {
        if (!vtn_realizacion_visible) {
            this.busyDialog = true;
            await Task.Delay(100);

            Ordenes_Realizacion[] datos = ordenesService.ObtenerOrdenesRealizacion(data.IdOficina, f1.ToString("yyyyMMdd"), f2.ToString("yyyyMMdd"), Subsistema, Sector, "");

            if (datos == null) {
                busyDialog = false;
                Toaster.Add("No hay datos disponibles", MatToastType.Info, "Sin Datos", null);
                return;
            }
            if (datos.Length <= 0) {
                busyDialog = false;
                Toaster.Add("No hay datos disponibles", MatToastType.Info, "Sin Datos", null);
                return;
            }

            var tmpOfi = sicemService.ObtenerEnlaces(data.IdOficina).FirstOrDefault();
            if(tmpOfi != null){
                vtn_realizacion_visible = true;
                vtn_realizacion.OficinaActual = tmpOfi;
                vtn_realizacion.Titulo = $"{data.Oficina} / Ordenes Agrupadas por Realizacion";
                await vtn_realizacion.Inicializar(datos);
            }

            await Task.Delay(100);
            this.busyDialog = false;
        }
    }
    private async Task Mostrar_Vtn_Capturacion(Ordenes_Oficina data) {
        if (!vtn_capturacion_visible) {
            this.busyDialog = true;
            await Task.Delay(100);

            Ordenes_Capturacion[] datos = ordenesService.ObtenerOrdenesCapturacion(data.IdOficina, f1.ToString("yyyyMMdd"), f2.ToString("yyyyMMdd"), Subsistema, Sector, "");

            if (datos == null) {
                busyDialog = false;
                Toaster.Add("No hay datos disponibles", MatToastType.Info, "Sin Datos", null);
                return;
            }
            if (datos.Length <= 0) {
                busyDialog = false;
                Toaster.Add("No hay datos disponibles", MatToastType.Info, "Sin Datos", null);
                return;
            }

            var tmpOfi = sicemService.ObtenerEnlaces(data.IdOficina).FirstOrDefault();
            if(tmpOfi != null) {
                vtn_capturacion_visible = true;
                vtn_capturacion.OficinaActual = tmpOfi;
                vtn_capturacion.Titulo = $"{data.Oficina} /  Ordenes Agrupadas Por Captura";
                await vtn_capturacion.Inicializar(datos);
            }

            await Task.Delay(100);
            this.busyDialog = false;
        }
    }

    private async Task ExportarExcel_Click() {
        var p = new ExcelExportProperties {
            FileName = $"Sicem_{Guid.NewGuid().ToString().Replace("-", "")}.xlsx",
            IncludeHiddenColumn = true
        };
        await DataGrid.ExportToExcelAsync(excelExportProperties: p);
    }
    private string FormateoMoneda(decimal valor) {
        return valor.ToString("c2");
    }
}