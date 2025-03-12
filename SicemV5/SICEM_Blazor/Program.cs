using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
//using SICEM_Blazor.Services;
//using SICEM_Blazor.Models;

namespace SICEM_Blazor {
    public class Program {
        public static void Main(string[] args) {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => {
                    webBuilder.UseStartup<Startup>();
                });

        //public void Pruebas_Services(IConfiguration c){

        //    var sicemService = new SicemService(c);
        //    var recaudacionSerivide = new RecaudacionService(c, sicemService);

        //    var datos = recaudacionSerivide.ObtenerRecaudacionIngresosxDias(1, DateTime.Now, DateTime.Now, 0, 0);
        //    foreach(var fila in datos) {
        //        Console.WriteLine($">> {fila.Fecha} Pagos Total:{fila.UsuariosTotal.ToString("n0")}  Ingresos Total:{fila.IngresosTotal.ToString("c2")} ");
        //    }
        //    Console.WriteLine($">> Total Filas {datos.Count()}");
        //    Console.ReadKey();
        //}

    }
}
