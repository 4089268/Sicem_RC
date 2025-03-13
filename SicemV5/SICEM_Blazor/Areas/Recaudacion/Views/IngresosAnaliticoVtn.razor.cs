using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Charts;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MatBlazor;
using SICEM_Blazor.Data.Contracts;
using SICEM_Blazor.Recaudacion.Models;
using SICEM_Blazor.Recaudacion.Data;
using SICEM_Blazor.Services;
using SICEM_Blazor.Models;
using SICEM_Blazor.Areas.Recaudacion.Views;
using SICEM_Blazor.Data;

namespace SICEM_Blazor.Recaudacion.Views;

public partial class IngresosAnaliticoVtn
{
    [Inject]
    private IJSRuntime JSRuntime { get; set; }

    [Parameter]
    public bool MostrarVentana { get; set;}
    
    [Parameter]
    public EventCallback CerrarModal { get; set; }
    
    [Parameter]
    public DateTime Fecha1 {get;set;}
    
    [Parameter]
    public DateTime Fecha2 {get;set;}
    
    [Parameter]
    public int Subsistema {get;set;}
    
    [Parameter]
    public int Sector {get;set;}
    
    [Parameter]
    public IEnlace EnlaceActual { get; set; }

    public string Titulo = "ANALITICO INGRESOS";

    public Recaudacion_Analitico DatosAnalitico;

    SfGrid<Recaudacion_AnaliticoMensual> dataGrid_analitico_mensual;
    SfGrid<Recaudacion_AnaliticoQuincenal> dataGrid_analitico_quincenal;
    SfGrid<Recaudacion_AnaliticoSemanal> dataGrid_analitico_semanal;

    SfGrid<Recaudacion_AnaliticoMensual> dataGrid_analiticoRez_mensual;
    SfGrid<Recaudacion_AnaliticoQuincenal> dataGrid_analiticoRez_quincenal;
    SfGrid<Recaudacion_AnaliticoSemanal> dataGrid_analiticoRez_semanal;

    public List<ChartItem> itemsGrafica_analitico = new List<ChartItem>();
    public List<ChartItem> itemsGrafica_analiticoRez = new List<ChartItem>();

    MatTabGroup tabGroup;
    private string subtitulo1 = "Analitico Mensual";
    private string subtitulo2 = "Analitico Rezago Mensual";
    private bool[,] visibles = new bool[,]{{true,false,false},{true,false,false}};
    private int rowHeight = 22;


    public async Task Inicializar(IEnlace e, Recaudacion_Analitico datos){
        this.EnlaceActual = e;
        this.DatosAnalitico = datos;
        ProcesarDatos();
        await Task.Delay(100);
    }
    private async Task Cerrar_Modal() {
        this.DatosAnalitico = new Recaudacion_Analitico();
        this.DatosAnalitico.Analitico_Mensual = new Recaudacion_AnaliticoMensual[]{};
        this.DatosAnalitico.AnaliticoRez_Mensual = new Recaudacion_AnaliticoMensual[]{};
        this.DatosAnalitico.Analitico_Quincenal = new Recaudacion_AnaliticoQuincenal[]{};
        this.DatosAnalitico.AnaliticoRez_Quincenal = new Recaudacion_AnaliticoQuincenal[]{};
        this.DatosAnalitico.Analitico_Semanal = new Recaudacion_AnaliticoSemanal[]{};
        this.DatosAnalitico.AnaliticoRez_Semanal = new Recaudacion_AnaliticoSemanal[]{};

        await CerrarModal.InvokeAsync("");
    }

    private void ProcesarDatos(){
        try {
            GenerarDatosGrafica(11);
        }
        catch(Exception err) {
            Console.WriteLine($">> Error al genera las graficas de analitico rezago:\n\t{err.Message}");
        }
        try {
            GenerarDatosGrafica(21);
        }
        catch (Exception err) {
            Console.WriteLine($">> Error al genera las graficas de analitico rezago:\n\t{err.Message}");
        }

    }
    private void GrupoBotonesClick(int param){

        switch(param){
            case 11:
                this.subtitulo1 = "Analitico Mensual";
                this.visibles[0,0] = true;
                this.visibles[0,1] = false;
                this.visibles[0,2] = false;
                GenerarDatosGrafica(11);
                break;
            case 12:
                this.subtitulo1 = "Analitico Quincenal";
                this.visibles[0,0] = false;
                this.visibles[0,1] = true;
                this.visibles[0,2] = false;
                GenerarDatosGrafica(12);
                break;
            case 13:
                this.subtitulo1 = "Analitico Semanal";
                this.visibles[0,0] = false;
                this.visibles[0,1] = false;
                this.visibles[0,2] = true;
                GenerarDatosGrafica(13);
                break;

            case 21:
                this.subtitulo2 = "Analitico Rezago Mensual";
                this.visibles[1, 0] = true;
                this.visibles[1, 1] = false;
                this.visibles[1, 2] = false;
                GenerarDatosGrafica(21);
                break;
            case 22:
                this.subtitulo2 = "Analitico Rezago Quincenal";
                this.visibles[1, 0] = false;
                this.visibles[1, 1] = true;
                this.visibles[1, 2] = false;
                GenerarDatosGrafica(22);
                break;
            case 23:
                this.subtitulo2 = "Analitico Rezago Semanal";
                this.visibles[1, 0] = false;
                this.visibles[1, 1] = false;
                this.visibles[1, 2] = true;
                GenerarDatosGrafica(23);
                break;
        }

    }
    private void GenerarDatosGrafica(int tipo){

        //this.itemsGrafica_analitico.Clear();
        //this.itemsGrafica_analiticoRez.Clear();
        var tmpListChart = new List<ChartItem>();

        switch (tipo) {
            case 11:
            case 21:
                Recaudacion_AnaliticoMensual[] tmpList;
                if (tipo == 11) {
                    tmpList = DatosAnalitico.Analitico_Mensual.OrderByDescending(item => item.Año).ToArray();
                } else {
                    tmpList = DatosAnalitico.AnaliticoRez_Mensual.OrderByDescending(item => item.Año).ToArray();
                }

                var _chart1 = new ChartItem() { Descripcion = "Ene" };
                var _chart2 = new ChartItem() { Descripcion = "Feb" };
                var _chart3 = new ChartItem() { Descripcion = "Mar" };
                var _chart4 = new ChartItem() { Descripcion = "Abr" };
                var _chart5 = new ChartItem() { Descripcion = "May" };
                var _chart6 = new ChartItem() { Descripcion = "Jun" };
                var _chart7 = new ChartItem() { Descripcion = "Jul" };
                var _chart8 = new ChartItem() { Descripcion = "Ago" };
                var _chart9 = new ChartItem() { Descripcion = "Sep" };
                var _chart10 = new ChartItem() { Descripcion = "Oct" };
                var _chart11 = new ChartItem() { Descripcion = "Nov" };
                var _chart12 = new ChartItem() { Descripcion = "Dic" };

                for(int i = 0; i <= tmpList.Length - 1; i++){
                    typeof(ChartItem).GetProperty($"Valor{i+1}").SetValue(_chart1, tmpList[i].Ene);
                    typeof(ChartItem).GetProperty($"Valor{i+1}").SetValue(_chart2, tmpList[i].Feb);
                    typeof(ChartItem).GetProperty($"Valor{i+1}").SetValue(_chart3, tmpList[i].Mar);
                    typeof(ChartItem).GetProperty($"Valor{i+1}").SetValue(_chart4, tmpList[i].Abr);
                    typeof(ChartItem).GetProperty($"Valor{i+1}").SetValue(_chart5, tmpList[i].May);
                    typeof(ChartItem).GetProperty($"Valor{i+1}").SetValue(_chart6, tmpList[i].Jun);
                    typeof(ChartItem).GetProperty($"Valor{i+1}").SetValue(_chart7, tmpList[i].Jul);
                    typeof(ChartItem).GetProperty($"Valor{i+1}").SetValue(_chart8, tmpList[i].Ago);
                    typeof(ChartItem).GetProperty($"Valor{i+1}").SetValue(_chart9, tmpList[i].Sep);
                    typeof(ChartItem).GetProperty($"Valor{i+1}").SetValue(_chart10, tmpList[i].Oct);
                    typeof(ChartItem).GetProperty($"Valor{i+1}").SetValue(_chart11, tmpList[i].Nov);
                    typeof(ChartItem).GetProperty($"Valor{i+1}").SetValue(_chart12, tmpList[i].Dic);
                }

                tmpListChart.Add(_chart1);
                tmpListChart.Add(_chart2);
                tmpListChart.Add(_chart3);
                tmpListChart.Add(_chart4);
                tmpListChart.Add(_chart5);
                tmpListChart.Add(_chart6);
                tmpListChart.Add(_chart7);
                tmpListChart.Add(_chart8);
                tmpListChart.Add(_chart9);
                tmpListChart.Add(_chart10);
                tmpListChart.Add(_chart11);
                tmpListChart.Add(_chart12);
                break;

            case 12:
            case 22:
                Recaudacion_AnaliticoQuincenal[] tmpList12;
                if (tipo == 12) {
                    tmpList12 = DatosAnalitico.Analitico_Quincenal.OrderByDescending(item => item.Año).ToArray();
                }
                else {
                    tmpList12 = DatosAnalitico.AnaliticoRez_Quincenal.OrderByDescending(item => item.Año).ToArray();
                }

                var _chart1a = new ChartItem() { Descripcion = "Ene Q1" };
                var _chart1b = new ChartItem() { Descripcion = "Ene Q2" };
                var _chart2a = new ChartItem() { Descripcion = "Feb Q1" };
                var _chart2b = new ChartItem() { Descripcion = "Feb Q2" };
                var _chart3a = new ChartItem() { Descripcion = "Mar Q1" };
                var _chart3b = new ChartItem() { Descripcion = "Mar Q2" };
                var _chart4a = new ChartItem() { Descripcion = "Abr Q1" };
                var _chart4b = new ChartItem() { Descripcion = "Abr Q2" };
                var _chart5a = new ChartItem() { Descripcion = "May Q1" };
                var _chart5b = new ChartItem() { Descripcion = "May Q2" };
                var _chart6a = new ChartItem() { Descripcion = "Jun Q1" };
                var _chart6b = new ChartItem() { Descripcion = "Jun Q2" };
                var _chart7a = new ChartItem() { Descripcion = "Jul Q1" };
                var _chart7b = new ChartItem() { Descripcion = "Jul Q2" };
                var _chart8a = new ChartItem() { Descripcion = "Ago Q1" };
                var _chart8b = new ChartItem() { Descripcion = "Ago Q2" };
                var _chart9a = new ChartItem() { Descripcion = "Sep Q1" };
                var _chart9b = new ChartItem() { Descripcion = "Sep Q2" };
                var _chart10a = new ChartItem() { Descripcion = "Oct Q1" };
                var _chart10b = new ChartItem() { Descripcion = "Oct Q2" };
                var _chart11a = new ChartItem() { Descripcion = "Nov Q1" };
                var _chart11b = new ChartItem() { Descripcion = "Nov Q2" };
                var _chart12a = new ChartItem() { Descripcion = "Dic Q1" };
                var _chart12b = new ChartItem() { Descripcion = "Dic Q2" };


                for (int i = 0; i <= tmpList12.Length - 1; i++) {
                    typeof(ChartItem).GetProperty($"Valor{i + 1}").SetValue(_chart1a, tmpList12[i].Ene_1);
                    typeof(ChartItem).GetProperty($"Valor{i + 1}").SetValue(_chart1b, tmpList12[i].Ene_2);
                    typeof(ChartItem).GetProperty($"Valor{i + 1}").SetValue(_chart2a, tmpList12[i].Feb_1);
                    typeof(ChartItem).GetProperty($"Valor{i + 1}").SetValue(_chart2b, tmpList12[i].Feb_2);
                    typeof(ChartItem).GetProperty($"Valor{i + 1}").SetValue(_chart3a, tmpList12[i].Mar_1);
                    typeof(ChartItem).GetProperty($"Valor{i + 1}").SetValue(_chart3b, tmpList12[i].Mar_2);
                    typeof(ChartItem).GetProperty($"Valor{i + 1}").SetValue(_chart4a, tmpList12[i].Abr_1);
                    typeof(ChartItem).GetProperty($"Valor{i + 1}").SetValue(_chart4b, tmpList12[i].Abr_2);
                    typeof(ChartItem).GetProperty($"Valor{i + 1}").SetValue(_chart5a, tmpList12[i].May_1);
                    typeof(ChartItem).GetProperty($"Valor{i + 1}").SetValue(_chart5b, tmpList12[i].May_2);
                    typeof(ChartItem).GetProperty($"Valor{i + 1}").SetValue(_chart6a, tmpList12[i].Jun_1);
                    typeof(ChartItem).GetProperty($"Valor{i + 1}").SetValue(_chart6b, tmpList12[i].Jun_2);
                    typeof(ChartItem).GetProperty($"Valor{i + 1}").SetValue(_chart7a, tmpList12[i].Jul_1);
                    typeof(ChartItem).GetProperty($"Valor{i + 1}").SetValue(_chart7b, tmpList12[i].Jul_2);
                    typeof(ChartItem).GetProperty($"Valor{i + 1}").SetValue(_chart8a, tmpList12[i].Ago_1);
                    typeof(ChartItem).GetProperty($"Valor{i + 1}").SetValue(_chart8b, tmpList12[i].Ago_2);
                    typeof(ChartItem).GetProperty($"Valor{i + 1}").SetValue(_chart9a, tmpList12[i].Sep_1);
                    typeof(ChartItem).GetProperty($"Valor{i + 1}").SetValue(_chart9b, tmpList12[i].Sep_2);
                    typeof(ChartItem).GetProperty($"Valor{i + 1}").SetValue(_chart10a, tmpList12[i].Oct_1);
                    typeof(ChartItem).GetProperty($"Valor{i + 1}").SetValue(_chart10b, tmpList12[i].Oct_2);
                    typeof(ChartItem).GetProperty($"Valor{i + 1}").SetValue(_chart11a, tmpList12[i].Nov_1);
                    typeof(ChartItem).GetProperty($"Valor{i + 1}").SetValue(_chart11b, tmpList12[i].Nov_2);
                    typeof(ChartItem).GetProperty($"Valor{i + 1}").SetValue(_chart12a, tmpList12[i].Dic_1);
                    typeof(ChartItem).GetProperty($"Valor{i + 1}").SetValue(_chart12b, tmpList12[i].Dic_2);
                }

                tmpListChart.Add(_chart1a);
                tmpListChart.Add(_chart1b);
                tmpListChart.Add(_chart2a);
                tmpListChart.Add(_chart2b);
                tmpListChart.Add(_chart3a);
                tmpListChart.Add(_chart3b);
                tmpListChart.Add(_chart4a);
                tmpListChart.Add(_chart4b);
                tmpListChart.Add(_chart5a);
                tmpListChart.Add(_chart5b);
                tmpListChart.Add(_chart6a);
                tmpListChart.Add(_chart6b);
                tmpListChart.Add(_chart7a);
                tmpListChart.Add(_chart7b);
                tmpListChart.Add(_chart8a);
                tmpListChart.Add(_chart8b);
                tmpListChart.Add(_chart9a);
                tmpListChart.Add(_chart9b);
                tmpListChart.Add(_chart10a);
                tmpListChart.Add(_chart10b);
                tmpListChart.Add(_chart11a);
                tmpListChart.Add(_chart11b);
                tmpListChart.Add(_chart12a);
                tmpListChart.Add(_chart12b);
                break;

            case 13:
            case 23:

                Recaudacion_AnaliticoSemanal[] tmpList13;
                if (tipo == 13) {
                    tmpList13 = DatosAnalitico.Analitico_Semanal.OrderByDescending(item => item.Año).ToArray(); ;
                }
                else {
                    tmpList13 = DatosAnalitico.AnaliticoRez_Semanal.OrderByDescending(item => item.Año).ToArray(); ;
                }

                var _chart1ea = new ChartItem() { Descripcion = "Ene S1" };
                var _chart1eb = new ChartItem() { Descripcion = "Ene S2" };
                var _chart1ed = new ChartItem() { Descripcion = "Ene S3" };
                var _chart1ec = new ChartItem() { Descripcion = "Ene S4" };
                var _chart2ea = new ChartItem() { Descripcion = "Feb S1" };
                var _chart2eb = new ChartItem() { Descripcion = "Feb S2" };
                var _chart2ec = new ChartItem() { Descripcion = "Feb S3" };
                var _chart2ed = new ChartItem() { Descripcion = "Feb S4" };
                var _chart3ea = new ChartItem() { Descripcion = "Mar S1" };
                var _chart3eb = new ChartItem() { Descripcion = "Mar S2" };
                var _chart3ec = new ChartItem() { Descripcion = "Mar S3" };
                var _chart3ed = new ChartItem() { Descripcion = "Mar S4" };
                var _chart4ea = new ChartItem() { Descripcion = "Abr S1" };
                var _chart4eb = new ChartItem() { Descripcion = "Abr S2" };
                var _chart4ec = new ChartItem() { Descripcion = "Abr S3" };
                var _chart4ed = new ChartItem() { Descripcion = "Abr S4" };
                var _chart5ea = new ChartItem() { Descripcion = "May S1" };
                var _chart5eb = new ChartItem() { Descripcion = "May S2" };
                var _chart5ec = new ChartItem() { Descripcion = "May S3" };
                var _chart5ed = new ChartItem() { Descripcion = "May S4" };
                var _chart6ea = new ChartItem() { Descripcion = "Jun S1" };
                var _chart6eb = new ChartItem() { Descripcion = "Jun S2" };
                var _chart6ec = new ChartItem() { Descripcion = "Jun S3" };
                var _chart6ed = new ChartItem() { Descripcion = "Jun S4" };
                var _chart7ea = new ChartItem() { Descripcion = "Jul S1" };
                var _chart7eb = new ChartItem() { Descripcion = "Jul S2" };
                var _chart7ec = new ChartItem() { Descripcion = "Jul S3" };
                var _chart7ed = new ChartItem() { Descripcion = "Jul S4" };
                var _chart8ea = new ChartItem() { Descripcion = "Ago S1" };
                var _chart8eb = new ChartItem() { Descripcion = "Ago S2" };
                var _chart8ec = new ChartItem() { Descripcion = "Ago S3" };
                var _chart8ed = new ChartItem() { Descripcion = "Ago S4" };
                var _chart9ea = new ChartItem() { Descripcion = "Sep S1" };
                var _chart9eb = new ChartItem() { Descripcion = "Sep S2" };
                var _chart9ec = new ChartItem() { Descripcion = "Sep S3" };
                var _chart9ed = new ChartItem() { Descripcion = "Sep S4" };
                var _chart10ea = new ChartItem() { Descripcion = "Oct S1" };
                var _chart10eb = new ChartItem() { Descripcion = "Oct S2" };
                var _chart10ec = new ChartItem() { Descripcion = "Oct S3" };
                var _chart10ed = new ChartItem() { Descripcion = "Oct S4" };
                var _chart11ea = new ChartItem() { Descripcion = "Nov S1" };
                var _chart11eb = new ChartItem() { Descripcion = "Nov S2" };
                var _chart11ec = new ChartItem() { Descripcion = "Nov S3" };
                var _chart11ed = new ChartItem() { Descripcion = "Nov S4" };
                var _chart12ea = new ChartItem() { Descripcion = "Dic S1" };
                var _chart12eb = new ChartItem() { Descripcion = "Dic S2" };
                var _chart12ec = new ChartItem() { Descripcion = "Dic S3" };
                var _chart12ed = new ChartItem() { Descripcion = "Dic S4" };

                for (int i = 0; i <= tmpList13.Length - 1; i++) {
                    typeof(ChartItem).GetProperty($"Valor{i + 1}").SetValue(_chart1ea, tmpList13[i].Ene_1);
                    typeof(ChartItem).GetProperty($"Valor{i + 1}").SetValue(_chart1eb, tmpList13[i].Ene_2);
                    typeof(ChartItem).GetProperty($"Valor{i + 1}").SetValue(_chart1ed, tmpList13[i].Ene_3);
                    typeof(ChartItem).GetProperty($"Valor{i + 1}").SetValue(_chart1ec, tmpList13[i].Ene_4);
                    typeof(ChartItem).GetProperty($"Valor{i + 1}").SetValue(_chart2ea, tmpList13[i].Ene_1);
                    typeof(ChartItem).GetProperty($"Valor{i + 1}").SetValue(_chart2eb, tmpList13[i].Ene_2);
                    typeof(ChartItem).GetProperty($"Valor{i + 1}").SetValue(_chart2ec, tmpList13[i].Ene_3);
                    typeof(ChartItem).GetProperty($"Valor{i + 1}").SetValue(_chart2ed, tmpList13[i].Ene_4);
                    typeof(ChartItem).GetProperty($"Valor{i + 1}").SetValue(_chart3ea, tmpList13[i].Ene_1);
                    typeof(ChartItem).GetProperty($"Valor{i + 1}").SetValue(_chart3eb, tmpList13[i].Ene_2);
                    typeof(ChartItem).GetProperty($"Valor{i + 1}").SetValue(_chart3ec, tmpList13[i].Ene_3);
                    typeof(ChartItem).GetProperty($"Valor{i + 1}").SetValue(_chart3ed, tmpList13[i].Ene_4);
                    typeof(ChartItem).GetProperty($"Valor{i + 1}").SetValue(_chart4ea, tmpList13[i].Ene_1);
                    typeof(ChartItem).GetProperty($"Valor{i + 1}").SetValue(_chart4eb, tmpList13[i].Ene_2);
                    typeof(ChartItem).GetProperty($"Valor{i + 1}").SetValue(_chart4ec, tmpList13[i].Ene_3);
                    typeof(ChartItem).GetProperty($"Valor{i + 1}").SetValue(_chart4ed, tmpList13[i].Ene_4);
                    typeof(ChartItem).GetProperty($"Valor{i + 1}").SetValue(_chart5ea, tmpList13[i].Ene_1);
                    typeof(ChartItem).GetProperty($"Valor{i + 1}").SetValue(_chart5eb, tmpList13[i].Ene_2);
                    typeof(ChartItem).GetProperty($"Valor{i + 1}").SetValue(_chart5ec, tmpList13[i].Ene_3);
                    typeof(ChartItem).GetProperty($"Valor{i + 1}").SetValue(_chart5ed, tmpList13[i].Ene_4);
                    typeof(ChartItem).GetProperty($"Valor{i + 1}").SetValue(_chart6ea, tmpList13[i].Ene_1);
                    typeof(ChartItem).GetProperty($"Valor{i + 1}").SetValue(_chart6eb, tmpList13[i].Ene_2);
                    typeof(ChartItem).GetProperty($"Valor{i + 1}").SetValue(_chart6ec, tmpList13[i].Ene_3);
                    typeof(ChartItem).GetProperty($"Valor{i + 1}").SetValue(_chart6ed, tmpList13[i].Ene_4);
                    typeof(ChartItem).GetProperty($"Valor{i + 1}").SetValue(_chart7ea, tmpList13[i].Ene_1);
                    typeof(ChartItem).GetProperty($"Valor{i + 1}").SetValue(_chart7eb, tmpList13[i].Ene_2);
                    typeof(ChartItem).GetProperty($"Valor{i + 1}").SetValue(_chart7ec, tmpList13[i].Ene_3);
                    typeof(ChartItem).GetProperty($"Valor{i + 1}").SetValue(_chart7ed, tmpList13[i].Ene_4);
                    typeof(ChartItem).GetProperty($"Valor{i + 1}").SetValue(_chart8ea, tmpList13[i].Ene_1);
                    typeof(ChartItem).GetProperty($"Valor{i + 1}").SetValue(_chart8eb, tmpList13[i].Ene_2);
                    typeof(ChartItem).GetProperty($"Valor{i + 1}").SetValue(_chart8ec, tmpList13[i].Ene_3);
                    typeof(ChartItem).GetProperty($"Valor{i + 1}").SetValue(_chart8ed, tmpList13[i].Ene_4);
                    typeof(ChartItem).GetProperty($"Valor{i + 1}").SetValue(_chart9ea, tmpList13[i].Ene_1);
                    typeof(ChartItem).GetProperty($"Valor{i + 1}").SetValue(_chart9eb, tmpList13[i].Ene_2);
                    typeof(ChartItem).GetProperty($"Valor{i + 1}").SetValue(_chart9ec, tmpList13[i].Ene_3);
                    typeof(ChartItem).GetProperty($"Valor{i + 1}").SetValue(_chart9ed, tmpList13[i].Ene_4);
                    typeof(ChartItem).GetProperty($"Valor{i + 1}").SetValue(_chart10ea, tmpList13[i].Ene_1);
                    typeof(ChartItem).GetProperty($"Valor{i + 1}").SetValue(_chart10eb, tmpList13[i].Ene_2);
                    typeof(ChartItem).GetProperty($"Valor{i + 1}").SetValue(_chart10ec, tmpList13[i].Ene_3);
                    typeof(ChartItem).GetProperty($"Valor{i + 1}").SetValue(_chart10ed, tmpList13[i].Ene_4);
                    typeof(ChartItem).GetProperty($"Valor{i + 1}").SetValue(_chart11ea, tmpList13[i].Ene_1);
                    typeof(ChartItem).GetProperty($"Valor{i + 1}").SetValue(_chart11eb, tmpList13[i].Ene_2);
                    typeof(ChartItem).GetProperty($"Valor{i + 1}").SetValue(_chart11ec, tmpList13[i].Ene_3);
                    typeof(ChartItem).GetProperty($"Valor{i + 1}").SetValue(_chart11ed, tmpList13[i].Ene_4);
                    typeof(ChartItem).GetProperty($"Valor{i + 1}").SetValue(_chart12ea, tmpList13[i].Ene_1);
                    typeof(ChartItem).GetProperty($"Valor{i + 1}").SetValue(_chart12eb, tmpList13[i].Ene_2);
                    typeof(ChartItem).GetProperty($"Valor{i + 1}").SetValue(_chart12ec, tmpList13[i].Ene_3);
                    typeof(ChartItem).GetProperty($"Valor{i + 1}").SetValue(_chart12ed, tmpList13[i].Ene_4);
                }


                tmpListChart.Add(_chart1ea);
                tmpListChart.Add(_chart1eb);
                tmpListChart.Add(_chart1ed);
                tmpListChart.Add(_chart1ec);
                tmpListChart.Add(_chart2ea);
                tmpListChart.Add(_chart2eb);
                tmpListChart.Add(_chart2ec);
                tmpListChart.Add(_chart2ed);
                tmpListChart.Add(_chart3ea);
                tmpListChart.Add(_chart3eb);
                tmpListChart.Add(_chart3ec);
                tmpListChart.Add(_chart3ed);
                tmpListChart.Add(_chart4ea);
                tmpListChart.Add(_chart4eb);
                tmpListChart.Add(_chart4ec);
                tmpListChart.Add(_chart4ed);
                tmpListChart.Add(_chart5ea);
                tmpListChart.Add(_chart5eb);
                tmpListChart.Add(_chart5ec);
                tmpListChart.Add(_chart5ed);
                tmpListChart.Add(_chart6ea);
                tmpListChart.Add(_chart6eb);
                tmpListChart.Add(_chart6ec);
                tmpListChart.Add(_chart6ed);
                tmpListChart.Add(_chart7ea);
                tmpListChart.Add(_chart7eb);
                tmpListChart.Add(_chart7ec);
                tmpListChart.Add(_chart7ed);
                tmpListChart.Add(_chart8ea);
                tmpListChart.Add(_chart8eb);
                tmpListChart.Add(_chart8ec);
                tmpListChart.Add(_chart8ed);
                tmpListChart.Add(_chart9ea);
                tmpListChart.Add(_chart9eb);
                tmpListChart.Add(_chart9ec);
                tmpListChart.Add(_chart9ed);
                tmpListChart.Add(_chart10ea);
                tmpListChart.Add(_chart10eb);
                tmpListChart.Add(_chart10ec);
                tmpListChart.Add(_chart10ed);
                tmpListChart.Add(_chart11ea);
                tmpListChart.Add(_chart11eb);
                tmpListChart.Add(_chart11ec);
                tmpListChart.Add(_chart11ed);
                tmpListChart.Add(_chart12ea);
                tmpListChart.Add(_chart12eb);
                tmpListChart.Add(_chart12ec);
                tmpListChart.Add(_chart12ed);
                break;

        }

        if(tipo == 11 || tipo == 12 || tipo == 13) {
            this.itemsGrafica_analitico = tmpListChart;
        }
        else {
            this.itemsGrafica_analiticoRez = tmpListChart;
        }
    }
    private async Task GenerarExcel(int tabIndex){

        if(tabIndex == 0) {
            if(visibles[0, 0]){
                await this.dataGrid_analitico_mensual.ExcelExport(new ExcelExportProperties {
                    FileName = string.Format("sicem_analiticoMensual_{0}.xlsx", Guid.NewGuid().ToString().Replace("-", ""))
                });
            }
            if(visibles[0, 1]){
                await this.dataGrid_analitico_quincenal.ExcelExport(new ExcelExportProperties {
                    FileName = string.Format("sicem_analiticoQuincenal_{0}.xlsx", Guid.NewGuid().ToString().Replace("-", ""))
                });
            }
            if (visibles[0, 2]) {
                await this.dataGrid_analitico_semanal.ExcelExport(new ExcelExportProperties {
                    FileName = string.Format("sicem_analiticoSemanal_{0}.xlsx", Guid.NewGuid().ToString().Replace("-", ""))
                });
            }
        }

        if(tabIndex == 1) {
            if (visibles[1, 0]) {
                await this.dataGrid_analiticoRez_mensual.ExcelExport(new ExcelExportProperties {
                    FileName = string.Format("sicem_analiticoRezMensual_{0}.xlsx", Guid.NewGuid().ToString().Replace("-", ""))
                });
            }
            if (visibles[1, 1]) {
                await this.dataGrid_analiticoRez_quincenal.ExcelExport(new ExcelExportProperties {
                    FileName = string.Format("sicem_analiticoRezQuincenal_{0}.xlsx", Guid.NewGuid().ToString().Replace("-", ""))
                });
            }
            if (visibles[1, 2]) {
                await this.dataGrid_analiticoRez_semanal.ExcelExport(new ExcelExportProperties {
                    FileName = string.Format("sicem_analiticoRezSemanal_{0}.xlsx", Guid.NewGuid().ToString().Replace("-", ""))
                });
            }
        }
    }
    private async Task GenerarPdf(int tabIndex) {

        if (tabIndex == 0) {
            if (visibles[0, 0]) {
                string titulo = "Analitico Ingresos Mensual";
                List<PdfHeaderFooterContent> xconts = new List<PdfHeaderFooterContent>();
                xconts.Add( new PdfHeaderFooterContent() { Type = ContentType.Text, Value = titulo, Position = new PdfPosition() { X = 10, Y = 10 }, Style = new PdfContentStyle() { TextBrushColor = "#000000", FontSize = 13 } } );
                await this.dataGrid_analitico_mensual.PdfExport(new PdfExportProperties { PageOrientation = PageOrientation.Landscape, Header = new PdfHeader { Contents = xconts },
                    FileName = string.Format("sicem_analiticoMensual_{0}.pdf", Guid.NewGuid().ToString().Replace("-", ""))
                });
            }
            if (visibles[0, 1]) {
                string titulo = "Analitico Ingresos Quincenal";
                List<PdfHeaderFooterContent> xconts = new List<PdfHeaderFooterContent>();
                xconts.Add(new PdfHeaderFooterContent() { Type = ContentType.Text, Value = titulo, Position = new PdfPosition() { X = 10, Y = 10 }, Style = new PdfContentStyle() { TextBrushColor = "#000000", FontSize = 13 } });
                await this.dataGrid_analitico_quincenal.PdfExport(new PdfExportProperties {
                    PageOrientation = PageOrientation.Landscape,
                    Header = new PdfHeader { Contents = xconts },
                    FileName = string.Format("sicem_analiticoQuincenal_{0}.pdf", Guid.NewGuid().ToString().Replace("-", ""))
                });
            }
            if (visibles[0, 2]) {
                string titulo = "Analitico Ingresos Semanal";
                List<PdfHeaderFooterContent> xconts = new List<PdfHeaderFooterContent>();
                xconts.Add(new PdfHeaderFooterContent() { Type = ContentType.Text, Value = titulo, Position = new PdfPosition() { X = 10, Y = 10 }, Style = new PdfContentStyle() { TextBrushColor = "#000000", FontSize = 13 } });
                await this.dataGrid_analitico_semanal.PdfExport(new PdfExportProperties {
                    PageOrientation = PageOrientation.Landscape,
                    Header = new PdfHeader { Contents = xconts },
                    FileName = string.Format("sicem_analiticoSemanal_{0}.pdf", Guid.NewGuid().ToString().Replace("-", ""))
                });
            }
        }


        if (tabIndex == 1) {
            if (visibles[1, 0]) {
                string titulo = "Analitico Rezago Mensual";
                List<PdfHeaderFooterContent> xconts = new List<PdfHeaderFooterContent>();
                xconts.Add(new PdfHeaderFooterContent() { Type = ContentType.Text, Value = titulo, Position = new PdfPosition() { X = 10, Y = 10 }, Style = new PdfContentStyle() { TextBrushColor = "#000000", FontSize = 13 } });
                await this.dataGrid_analiticoRez_mensual.PdfExport(new PdfExportProperties {
                    PageOrientation = PageOrientation.Landscape,
                    Header = new PdfHeader { Contents = xconts },
                    FileName = string.Format("sicem_analiticoRezMensual_{0}.pdf", Guid.NewGuid().ToString().Replace("-", ""))
                });
            }
            if (visibles[1, 1]) {
                string titulo = "Analitico Rezago Quincenal";
                List<PdfHeaderFooterContent> xconts = new List<PdfHeaderFooterContent>();
                xconts.Add(new PdfHeaderFooterContent() { Type = ContentType.Text, Value = titulo, Position = new PdfPosition() { X = 10, Y = 10 }, Style = new PdfContentStyle() { TextBrushColor = "#000000", FontSize = 13 } });
                await this.dataGrid_analiticoRez_quincenal.PdfExport(new PdfExportProperties {
                    PageOrientation = PageOrientation.Landscape,
                    Header = new PdfHeader { Contents = xconts },
                    FileName = string.Format("sicem_analiticoRezQuincenal_{0}.pdf", Guid.NewGuid().ToString().Replace("-", ""))
                });
            }
            if (visibles[1, 2]) {
                string titulo = "Analitico Rezago Semanal";
                List<PdfHeaderFooterContent> xconts = new List<PdfHeaderFooterContent>();
                xconts.Add(new PdfHeaderFooterContent() { Type = ContentType.Text, Value = titulo, Position = new PdfPosition() { X = 10, Y = 10 }, Style = new PdfContentStyle() { TextBrushColor = "#000000", FontSize = 13 } });
                await this.dataGrid_analiticoRez_semanal.PdfExport(new PdfExportProperties {
                    PageOrientation = PageOrientation.Landscape,
                    Header = new PdfHeader { Contents = xconts },
                    FileName = string.Format("sicem_analiticoRezSemanal_{0}.pdf", Guid.NewGuid().ToString().Replace("-", ""))
                });
            }
        }
    }

}