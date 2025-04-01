using System;
using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MatBlazor;
using Syncfusion.Blazor.Grids;
using BlazorBootstrap;
using SICEM_Blazor.SeguimientoCobros.Models;
using SICEM_Blazor.SeguimientoCobros.Data;
using SICEM_Blazor.Helpers;
using SICEM_Blazor.Models;
using SICEM_Blazor.Data;
using SICEM_Blazor.Services;
using SICEM_Blazor.Recaudacion.Data;
using Syncfusion.Blazor.RichTextEditor;

namespace SICEM_Blazor.SeguimientoCobros.Views {

    public partial class Index : IDisposable {

        [Inject]
        private IncomeMapJsInterop IncomeMapJsInterop {get;set;}
        
        [Inject]
        private IncomeOfficeService IncomeOfficeService {get;set;}
        
        [Inject]
        private SicemService SicemService {get;set;}
        
        [Inject]
        private IMatToaster Toaster {get;set;}
        
        [Inject]
        private IOptions<BingMapsSettings> BingMapsSettings {get;set;}
        
        [Inject]
        private IRecaudacionService RecaudacionService {get;set;}

        private DotNetObjectReference<Index> objRef;
        public IngresoCajasPanel IngresoCajasPanel {get;set;}
        private List<OfficePushpinMap> IncomeData {get; set;} = new List<OfficePushpinMap>();
        private List<OfficePushpinMap> IncomeDataGrid {
            get {
                var _data = IncomeData.Where(item  => item.Id < 900 ).ToList();
                _data.Add( new OfficePushpinMap(999, " TOTAL"){
                    Income = IncomeData.Where(item  => item.Id < 900 ).Sum( item => item.Income),
                    Bills = IncomeData.Where(item  => item.Id < 900 ).Sum( item => item.Bills)
                });
                return _data;
            }
        }
        
        public bool ShowPanelInfo {get;set;} = false;
        public IEnlace EnlaceSeleccionado {get;set;}

        private SfGrid<OfficePushpinMap> dataGrid;
        private List<IEnlace> offices = new List<IEnlace>();
        public string[] palettes = new String[] { "#c0392b", "#F6B53F", "#6FAAB0", "#229954", "#223199", "#3EC6B6", "#E96188" };
        
        private MapMark centerMap = new MapMark();
        private bool isBusy = true, showDrawer= false;
        private Task<IEnumerable<OfficePushpinMap>> officePushpinesTask;
        private UpdateIncomeService updateIncomeService;
        private MapMark centerPosition = new MapMark
        {
            Latitude = 27.905077,
            Longitude = -101.274589
        };

        #region Chart properties
        private PieChart pieChart = default!;
        private PieChartOptions pieChartOptions = default!;
        private ChartData chartData = default!;
        private string[] backgroundColors;
        
        /// <summary>
        /// for verify if is a changed in the data before update the chart
        /// </summary>
        private decimal previousTotal = 0;

        #endregion

        [JSInvokable("MapLoaded")]
        public void OnMapLoaded(){
            //isMapLoaded = true;
            Console.WriteLine("Map loaded!!");
            //Toaster.Add( "Income map loaded", MatToastType.Info);
        }
        
        [JSInvokable("PushpinClick")]
        public async Task PushPinClick(int id)
        {
            Console.WriteLine($"Push with id '{id}' clicked");
            await Task.Delay(100);

            // this.EnlaceSeleccionado = offices.Where(item => item.Id == id).FirstOrDefault();
            // await Task.Delay(100);
            // ShowPanelInfo = true;
            // StateHasChanged();

            // if( IngresoCajasPanel != null){
            //     await IngresoCajasPanel.LoadData();
            // }
        }

        protected override void OnInitialized()
        {
            objRef = DotNetObjectReference.Create(this);

            // get the offices of the user
            this.offices = this.SicemService.ObtenerOficinasDelUsuario().ToList();

            // start task of offices pushpins
            this.officePushpinesTask = GetOfficesPushpins();

        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if(firstRender)
            {
                await InitializeMapAsync();
                // await InitializedChart();
                StateHasChanged();
            }
        }


        public async Task InitializeMapAsync()
        {

            // load the map
            await IncomeMapJsInterop.InitializedAsync(this.objRef, "map", centerPosition.Latitude, centerPosition.Longitude);
            
            this.isBusy = false;

            // get the offices initial pushpins
            this.IncomeData = (await officePushpinesTask).ToList();

            // * draw the points (TESTING)
            await IncomeMapJsInterop.UpdateMarks(this.objRef, this.IncomeData.ToArray());
            
            // set the pushpin for each elemen
            // foreach(var officePushpin in this.IncomeData)
            // {
            //     RefreIncomesData(officePushpin);
            // }

            // TODO: start service of income update
            // updateIncomeService = new UpdateIncomeService(IncomeOfficeService);
            // updateIncomeService.Start( this.offices, RefreIncomesData );

            this.showDrawer = true;
        }


        /// <summary>
        /// Retorna un listado con las ubicaciones de las oficinas 
        /// </summary>
        private async Task<IEnumerable<OfficePushpinMap>> GetOfficesPushpins(){
            
            var tasks = new List<Task>();

            // Initialized the tasks
            var _officesMap = new List<OfficePushpinMap>();
            foreach(var item in this.offices)
            {
                var task = Task.Run(() =>
                {
                    try
                    {
                        var data = this.IncomeOfficeService.GetPushpinOfOffice(item);
                        _officesMap.Add(data);
                    }
                    catch (Exception err)
                    {
                        Toaster.Add($"Error al obtenerla recaudacion de la oficina {item.Nombre}", MatToastType.Danger);
                        Console.WriteLine($"Error oficina {item.Nombre}: {err.Message} {err.StackTrace}" );
                    }
                });
                tasks.Add(task);
            }
            
            // Await the completion of all task
            await Task.WhenAll(tasks);

            return _officesMap;
        }

        /// <summary>
        /// Actualiza el datagrid y las graficas
        /// </summary>
        private void RefreIncomesData(OfficePushpinMap officePushpinMap)
        {
            try
            {
                // // actualizar mapa
                // * var t = Task.Run( async () => await IncomeMapJsInterop.UpdatePoint( this.objRef, officePushpinMap) );
                
                // * update datagrid record
                var refdata = IncomeData.Where(item => item.Id == officePushpinMap.Id).FirstOrDefault();
                if(refdata != null)
                {
                    refdata.Bills = officePushpinMap.Bills;
                    refdata.Income = officePushpinMap.Income;
                }
                dataGrid.Refresh();


                // * verify if is a changed in the data
                var _total = IncomeData.Where(item => item.Id < 999).Sum( item => item.Income);

                // * Use InvokeAsync to ensure StateHasChanged is called on the main thread
                InvokeAsync( async() =>
                {
                    // attempt to update the chart
                    if( _total != previousTotal)
                    {
                        previousTotal = _total;
                        await UpdateChartData(IncomeData);
                    }
                    StateHasChanged();
                });

                
                // actualizar grafica
                //var charItem = this.incomeDataGraph.Where( item => item.Id == officePushpinMap.Id ).FirstOrDefault();
                //if(charItem != null){
                //    charItem.Valor1 = officePushpinMap.Income;
                //    myChart.Refresh(false);
                //}

                // t.Wait();

            }
            catch(Exception err)
            {
                Console.WriteLine(err.Message);
                Console.WriteLine(err.StackTrace);
            }
        }

        public void Dispose()
        {
            try {
                this.updateIncomeService.Dispose();
            }catch(Exception){}
        }

        public void ClosePanelInfo()
        {
            ShowPanelInfo = false;
            StateHasChanged();
        }

        public async Task OnGridSelectionChanged(RowSelectEventArgs<OfficePushpinMap> args)
        {
            if( args.Data.Id < 999){
                this.EnlaceSeleccionado = offices.Where(item => item.Id == args.Data.Id).FirstOrDefault();
                await Task.Delay(100);
                ShowPanelInfo = true;
                StateHasChanged();
                if( IngresoCajasPanel != null){
                    await IngresoCajasPanel.LoadData();
                }
            }
        }

        #region Chart methods
        public async Task InitializedChart()
        {
            backgroundColors = new[]{ "#c0392b", "#f6b53f", "#6faab0", "#229954", "#223199", "#3EC6B6", "#e96188" };

            double[] _data = new double[ offices.Count ];
            Array.Fill(_data, 0);
            foreach (var income in IncomeData)
            {
                _data[ income.Id-1] = Convert.ToDouble(income.Income);
            }

            // initi previous total
            previousTotal = IncomeData.Where( item => item.Id < 999).Sum( item => item.Income);

            string[] _labels = new string[ offices.Count ];
            foreach(var office in offices){
                _labels[office.Id - 1] = office.Nombre;
            }

            // prepared datasets
            var datasets = new List<IChartDataset>(){
                new PieChartDataset() {
                    Label = "Recaudacion",
                    Data = _data.ToList(),
                    BackgroundColor = backgroundColors.ToList()
                }
            };
        
            // prepared chart data 
            chartData = new ChartData {
                Labels = _labels.ToList(),
                Datasets = datasets
            };

            pieChartOptions = new PieChartOptions {
                Responsive = true
            };
            pieChartOptions.Plugins.Title!.Text = "Seguimiento de cobros";
            pieChartOptions.Plugins.Title.Display = true;

            // initi chart
            if(pieChart is not null)
            {
                await pieChart.InitializeAsync(chartData, pieChartOptions);
            }
        }

        public async Task UpdateChartData(IEnumerable<OfficePushpinMap> officePushpinMaps)
        {
            if( pieChart is null || chartData is null || chartData.Datasets is null)
            {
                return;
            }

            // prepared data
            double[] _data = new double[ officePushpinMaps.Count() ];
            foreach(var incomeData in officePushpinMaps)
            {
                _data[ incomeData.Id - 1 ] = Convert.ToDouble(incomeData.Income);
            }

            // prepared datasets
            var datasets = new List<IChartDataset>()
            {
                new PieChartDataset() {
                    Label = "Recaudacion " + DateTime.Now.ToShortTimeString(),
                    Data = _data.ToList(),
                    BackgroundColor = backgroundColors.ToList()
                }
            };

            chartData.Datasets = datasets;
            
            await pieChart.UpdateAsync(chartData, pieChartOptions);
        }

        #endregion
    }

}
