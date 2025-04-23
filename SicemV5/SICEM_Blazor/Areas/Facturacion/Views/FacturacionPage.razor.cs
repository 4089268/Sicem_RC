using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MatBlazor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Charts;
using Syncfusion.Blazor.Grids;
using SICEM_Blazor.Data;
using SICEM_Blazor.Models;
using SICEM_Blazor.Services;
using SICEM_Blazor.Facturacion.Models;
using SICEM_Blazor.Areas.Facturacion.Views;

namespace SICEM_Blazor.Facturacion.Views;

public partial class FacturacionPage : ComponentBase {

    [Inject]
    public IMatToaster Toaster {get; set;}
    
    [Inject]
    public FacturacionService FacturacionService1 {get;set;}
    
    [Inject]
    public SicemService SicemService {get;set;}
    
    [Inject]
    public IJSRuntime JSRuntime {get;set;}
    
    [Inject]
    public NavigationManager NavigationManager1 {get;set;}

    private SfGrid<Facturacion_Oficina> DataGrid {get;set;}
    private SfChart GraficaFacturacion {get;set;}
    private List<Facturacion_Oficina> DatosFacturacion {get;set;}
    private List<ChartItem> DatosGrafica {get;set;}
    private bool busyDialog = false;

    private DateTime Fecha1, Fecha2;
    private int Subsistema, Sector;
    private Facturacion_ConceptosVtn FacturacionConceptos {get;set;}
    private Facturacion_AnualVtn FacturacionAnualVtn {get;set;}
    private Facturacion_LocalidadesVtn FacturacionLocalidadesVtn {get; set;}
    private bool facturacionConceptos_Visible = false, FacturacionAnual_Visible = false, FacturacionLocalidad_Visible = false;
    private IEnlace EnlaceSeleccionado;


    protected override void OnInitialized() {

        // Validar si se cuenta con la opcion
        if(SicemService.Usuario != null){
            if(!SicemService.Usuario.OpcionSistemas.Select(item => item.Id).Contains(OpcionesSistema.FACTURACION)){
                NavigationManager1.NavigateTo("/");
                return;
            }
        }


        var _now = DateTime.Now.AddMonths(-1);
        this.Fecha1 = new DateTime(_now.Year, _now.Month, 1);
        this.Fecha2 = new DateTime(_now.Year, _now.Month, DateTime.DaysInMonth(_now.Year, _now.Month));
        this.Subsistema = 0;
        this.Sector = 0;
    }
    
    public void Procesar(SeleccionarFechaEventArgs e) {
        this.Fecha1 = e.Fecha1;
        this.Fecha2 = e.Fecha2;
        this.Subsistema = e.Subsistema;
        this.Sector = e.Sector;

        DatosFacturacion = new List<Facturacion_Oficina>();
        DatosGrafica = new List<ChartItem>();
        var enlaces = SicemService.ObtenerOficinasDelUsuario();
        var oficinas = SicemService.ObtenerEnlaces().Where(item => enlaces.Select(i => i.Id).Contains(item.Id)).ToArray();
        var tareas = new List<Task>();

        foreach(Ruta oficina in oficinas){
            var newItem = new Facturacion_Oficina(){
                Estatus = 0,
                Id_Oficina = oficina.Id,
                Oficina = oficina.Oficina
            };
            this.DatosFacturacion.Add(newItem);

            var newChart = new ChartItem(){
                Id = oficina.Id,
                Descripcion = oficina.Oficina,
                Valor1 = 0m,
                Valor2 = 0m,
                Valor3 = 0m,
                Valor4 = 0m,
                Valor5 = 0m
            };
            this.DatosGrafica.Add(newChart);

            tareas.Add(new Task( () => ProcesarConsulta(oficina)) );
        }

        //*** Fila total
        if(oficinas.Length > 1){
            var itemTotal = new Facturacion_Oficina(){
                Estatus = 0,
                Id_Oficina = 999,
                Oficina = "TOTAL"
            };
            this.DatosFacturacion.Add(itemTotal);
        }

        tareas.ForEach(item => item.Start());
    }
    
    private void ProcesarConsulta(Ruta oficina){
        var result = FacturacionService1.ObtenerFacturacionOficina(oficina, this.Fecha1.Year, this.Fecha1.Month, this.Subsistema, this.Sector );

        lock(DatosFacturacion){
            var itemList = this.DatosFacturacion.Where(item=> item.Id_Oficina == oficina.Id).FirstOrDefault();
            if(itemList != null){
                itemList.Estatus = result.Estatus;
                itemList.Domestico_Fact = result.Domestico_Fact;
                itemList.Domestico_Usu = result.Domestico_Usu;
                itemList.Comercial_Fact = result.Comercial_Fact;
                itemList.Comercial_Usu = result.Comercial_Usu;
                itemList.Industrial_Fact = result.Industrial_Fact;
                itemList.Industrial_Usu = result.Industrial_Usu;
                itemList.ServGener_Fact = result.ServGener_Fact;
                itemList.ServGener_Usu = result.ServGener_Usu;
                itemList.Subtotal = result.Subtotal;
                itemList.Iva = result.Iva;
                itemList.Total = result.Total;
                itemList.Usuarios = result.Usuarios;
            }
            //**** Recalcular fila total
            RecalcularTotal();
            DataGrid.Refresh();
        }

        lock(DatosGrafica){
            //**** Procesar item grafica
            var itemGraf = this.DatosGrafica.Where(item => item.Id == oficina.Id).FirstOrDefault();
            if(itemGraf != null){
                itemGraf.Valor1 = result.Domestico_Fact;
                itemGraf.Valor3 = result.Comercial_Fact;
                itemGraf.Valor4 = result.Industrial_Fact;
                itemGraf.Valor5 = result.ServGener_Fact;
            }
            GraficaFacturacion.RefreshLiveData();
        }
    }
    
    private void RecalcularTotal(){
        var filaTotal = this.DatosFacturacion.Where(item => item.Id_Oficina == 999).FirstOrDefault();
        if( filaTotal != null){
            var _tmpDatos = this.DatosFacturacion.Where(item => item.Id_Oficina > 0 && item.Id_Oficina < 999).ToList();
            filaTotal.Domestico_Fact = _tmpDatos.Sum( item => item.Domestico_Fact);
            filaTotal.Domestico_Usu = _tmpDatos.Sum( item => item.Domestico_Usu);
            filaTotal.Comercial_Fact = _tmpDatos.Sum( item => item.Comercial_Fact);
            filaTotal.Comercial_Usu = _tmpDatos.Sum( item => item.Comercial_Usu);
            filaTotal.Industrial_Fact = _tmpDatos.Sum( item => item.Industrial_Fact);
            filaTotal.Industrial_Usu = _tmpDatos.Sum( item => item.Industrial_Usu);
            filaTotal.ServGener_Fact = _tmpDatos.Sum( item => item.ServGener_Fact);
            filaTotal.ServGener_Usu = _tmpDatos.Sum( item => item.ServGener_Usu);
            filaTotal.Subtotal = _tmpDatos.Sum( item => item.Subtotal);
            filaTotal.Iva = _tmpDatos.Sum( item => item.Iva);
            filaTotal.Total = _tmpDatos.Sum( item => item.Total);
            filaTotal.Usuarios = _tmpDatos.Sum( item => item.Usuarios);
        }
    }
    
    private async Task ExportarExcel_Click(){
        var p = new ExcelExportProperties();
        p.IncludeHiddenColumn = true;
        await this.DataGrid.ExportToExcelAsync(p);
    }

    //*** Funciones Ventanas Secundarias
    private async Task FacturacionConceptos_Click(Facturacion_Oficina context){
        if(facturacionConceptos_Visible){
            return;
        }

        this.busyDialog = true;
        await Task.Delay(100);

        Ruta oficina = SicemService.ObtenerEnlaces(context.Id_Oficina).FirstOrDefault();
        var datos = FacturacionService1.ObtenerFacturacionConceptos(oficina, this.Fecha1.Year, this.Fecha1.Month, this.Subsistema, this.Sector);
        
        var catLocalidades = new Dictionary<int,string>();
        var _localidades = SicemService.ObtenerCatalogoLocalidades(oficina.Id).Where(i => i.Id_Poblacion > 0).ToList();
        catLocalidades.Add(0, "TODOS");
        foreach( var loc in _localidades){
            catLocalidades.Add(loc.Id_Poblacion, loc.Descripcion.ToUpper().Trim());
        }

        if(datos == null){
            Toaster.Add("Error al procesar la consulta, intentelo mas tarde.", MatToastType.Warning);
        }else{
            facturacionConceptos_Visible = true;
            FacturacionConceptos.Titulo = $"CONCEPTOS FACTURADOS DEL {Fecha1.ToString("dd/MM/yyyy")} AL {Fecha2.ToString("dd/MM/yyyy")}  DE LA OFICINA {context.Oficina}";
            FacturacionConceptos.Inicializar(oficina, datos, catLocalidades);
            
            StateHasChanged();
            await JSRuntime.InvokeVoidAsync("iniciarVentanaConceptos");
        }

        await Task.Delay(100);
        this.busyDialog = false;
    }
    
    private async Task FacturacionAnual_Click(Facturacion_Oficina context){
        if(FacturacionAnual_Visible || FacturacionAnualVtn == null){
            return;
        }

        this.busyDialog = true;
        await Task.Delay(100);

        var _oficina = SicemService.ObtenerEnlaces(context.Id_Oficina).FirstOrDefault();
        var _datos = FacturacionService1.ObtenerFacturacionAnual(_oficina, this.Fecha1.Year, this.Subsistema, this.Sector).ToList();
        if(_datos.Count > 0){
            FacturacionAnual_Visible = true;
            FacturacionAnualVtn.SetTitulo( $"{_oficina.Oficina} - ANALISIS DE FACTURACION {this.Fecha1.Year}" );
            FacturacionAnualVtn.Inicializar(_datos, _oficina);
        }

        await Task.Delay(100);
        this.busyDialog = false;
    }
    
    private async Task FacturacionLocalidades_Click(Facturacion_Oficina context){
        if(FacturacionLocalidad_Visible){
            return;
        }

        this.busyDialog = true;
        await Task.Delay(100);

        this.EnlaceSeleccionado = SicemService.ObtenerEnlaces(context.Id_Oficina).FirstOrDefault();
        var _datos = FacturacionService1.ObtenerFacturacionLocalidades(EnlaceSeleccionado, this.Fecha1.Year, this.Fecha1.Month, this.Subsistema, this.Sector).ToList();
        if(_datos.Count > 0){
            FacturacionLocalidad_Visible = true;
            FacturacionLocalidadesVtn.Inicializar(_datos, EnlaceSeleccionado);
        }else{
            Toaster.Add("Error al realizar la consulta", MatToastType.Warning);
        }

        await Task.Delay(100);
        this.busyDialog = false;
    }

}