using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SICEM_Blazor.Data;
using SICEM_Blazor.Models;
using SICEM_Blazor.Models.Entities.Arquos;

namespace SICEM_Blazor.Services {
    public class SucursalesService {

        private readonly SicemContext sicemContext;
        private readonly ILogger<SucursalesService> logger;

        public SucursalesService(SicemContext sicemContext, ILogger<SucursalesService> logger ){
            this.sicemContext = sicemContext;
            this.logger = logger;
        }

        public IEnumerable<IEnlace> ObtenerOficinas(){
            try{
                return this.sicemContext.Rutas.ToList();
            }catch(Exception err){
                logger.LogError(err, "Error at attempting to get the ofifice catalog: {message}", err.Message );
                return null;
            }
        }
        
        /// <summary>
        /// Return the poblaciones of the office
        /// </summary>
        /// <param name="oficina_id"></param>
        /// <returns></returns>
        /// <exception cref="TimeoutException"></exception> 
        public IEnumerable<CatSucursale> ObtenerSucursales(long oficina_id){
            try{
                // * Get office
                var ruta = this.sicemContext.Rutas.Where(x => x.Id == oficina_id).FirstOrDefault()
                    ?? throw new Exception($"Ruta ID {oficina_id} not found");

                // * Prepared time limit for 5 seconds
                using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(6));

                // * Return all the data
                var task = Task.Run<IEnumerable<CatSucursale>>( () =>{
                    using var arquosDbContext = new ArquosContext(ruta.GetConnectionString());
                    return arquosDbContext.CatSucursales.ToList();
                });

                task.Wait( cancellationTokenSource.Token );
                return task.Result;

            }
            catch(OperationCanceledException){
                throw new TimeoutException("Operation timed out after 6 seconds.");
            }
        }

        public void ModificarSucursal(long oficina_id, CatSucursale sucursal){
            try
            {
                // * Get office
                var ruta = this.sicemContext.Rutas.Where(x => x.Id == oficina_id).FirstOrDefault()
                    ?? throw new KeyNotFoundException($"Ruta ID {oficina_id} not found");
                
                // * Get the poblacion
                using var arquosDbContext = new ArquosContext(ruta.GetConnectionString());
                var _sucursal = arquosDbContext.CatSucursales.Where( item => item.IdSucursal == sucursal.IdSucursal).FirstOrDefault()
                    ?? throw new KeyNotFoundException($"Sucursal ID {sucursal.IdSucursal} not found");

                // * Update the values
                // _sucursal.Latitud = sucursal.Latitud;
                // _sucursal.Longitud = sucursal.Longitud;
                _sucursal.Inactivo = sucursal.Inactivo;
                arquosDbContext.CatSucursales.Update(_sucursal);
                arquosDbContext.SaveChanges();
                
            }catch(Exception err){
                this.logger.LogError(err, "Error al actualizar la sucursal: {message}", err.Message );
            }
        }
    }   

}