using System;
using Microsoft.Extensions.DependencyInjection;

namespace SICEM_Blazor.SeguimientoCobros.Data
{
    public static class IncomeMapServiceCollectionExtension
    {

        public static void AddSicemIncomeOfficeServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IncomeOfficeService>();
            serviceCollection.AddScoped<IncomeMapJsInterop>();
        }
        
    }
}