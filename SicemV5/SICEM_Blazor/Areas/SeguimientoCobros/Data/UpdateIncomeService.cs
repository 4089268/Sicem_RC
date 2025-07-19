using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SICEM_Blazor.Data;
using SICEM_Blazor.SeguimientoCobros.Models;

namespace SICEM_Blazor.SeguimientoCobros.Data
{
    public class UpdateIncomeService : IDisposable
    {
        
        private readonly IncomeOfficeService incomeOfficeService;
        private Action<OfficePushpinMap> callbackAction;
        private IEnumerable<IEnlace> offices;
        private List<Task> officesTask;
        private CancellationTokenSource cancellationTokenSource;
        private bool disposed = false;

        public UpdateIncomeService( IncomeOfficeService incomeOfficeService  ){
            this.incomeOfficeService = incomeOfficeService;
            this.cancellationTokenSource = new CancellationTokenSource();
        }

        public void Start( IEnumerable<IEnlace> offices, Action<OfficePushpinMap> callback)
        {
            this.offices = offices;
            this.callbackAction = callback;

            officesTask = new List<Task>();
            foreach( var office in this.offices)
            {
                officesTask.Add( Task.Run( () => FetchData(office, cancellationTokenSource.Token) ) );
            }
        }

        private void FetchData(IEnlace office, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                OfficePushpinMap incomeData = this.incomeOfficeService.GetPushpinOfOffice(office);

                // * for testing
                // var rando = new Random();
                // var randomValue = rando.NextInt64(1100,999999);
                // incomeData.Income += (decimal)randomValue;
                // * for testing

                try
                {
                    callbackAction.Invoke(incomeData);
                }
                catch(Exception) { }

                try
                {
                    Random random = new();
                    var _randomMulti = random.NextDouble() * (1.25 - 0.75) + 0.75;
                    var _delay = 10000 * _randomMulti;
                    Task.Delay( (int) _delay, cancellationToken).Wait(cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    // Handle cancellation
                    break;
                }
            }

        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if (!disposed)
            {
                if (disposing)
                {
                    // Cancel the running tasks
                    cancellationTokenSource.Cancel();

                    // Wait for all tasks to complete
                    Task.WaitAll(officesTask.ToArray());

                    // Dispose of the CancellationTokenSource
                    cancellationTokenSource.Dispose();
                }
                disposed = true;
            }
        }

        ~UpdateIncomeService() {
            Dispose(false);
        }

    }
}