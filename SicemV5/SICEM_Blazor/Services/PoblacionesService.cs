using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SICEM_Blazor.Data;
using SICEM_Blazor.Models;
using SICEM_Blazor.Models.Entities.Arquos;
using System.Threading;
using MatBlazor;

namespace SICEM_Blazor.Services {
    public class PoblacionesService {

        private readonly SicemContext sicemContext;
        private readonly ILogger<PoblacionesService> logger;

        public PoblacionesService(SicemContext sicemContext, ILogger<PoblacionesService> logger ){
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
        public IEnumerable<CatPoblacione> ObtenerPoblaciones( long oficina_id){
            try{
                // * Get office
                var ruta = this.sicemContext.Rutas.Where(x => x.Id == oficina_id).FirstOrDefault()
                    ?? throw new Exception($"Ruta ID {oficina_id} not found");

                // * Prepared time limit for 5 seconds
                using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(6));

                // * Return all the data
                var task = Task.Run<IEnumerable<CatPoblacione>>( () =>{
                    using var arquosDbContext = new ArquosContext(ruta.GetConnectionString());
                    return arquosDbContext.CatPoblaciones.ToList();
                });

                task.Wait( cancellationTokenSource.Token );
                return task.Result;

            }
            catch(OperationCanceledException){
                throw new TimeoutException("Operation timed out after 6 seconds.");
            }
        }

        public void ModificarPoblacion(long oficina_id, CatPoblacione poblacion)
        {
            throw new NotImplementedException();
            // try{

            //     // * Get office
            //     var ruta = this.sicemContext.Rutas.Where(x => x.Id == oficina_id).FirstOrDefault()
            //         ?? throw new KeyNotFoundException($"Ruta ID {oficina_id} not found");
                
            //     // * Get the poblacion
            //     using var arquosDbContext = new ArquosContext(ruta.GetConnectionString());
            //     var _poblacion = arquosDbContext.CatPoblaciones.Where( item => item.IdPoblacion == poblacion.IdPoblacion).FirstOrDefault()
            //         ?? throw new KeyNotFoundException($"Poblacion ID {poblacion.IdPoblacion} not found");

            //     // * Update the values
            //     _poblacion.EsRural = poblacion.EsRural;
            //     _poblacion.Habitantes = poblacion.Habitantes;
            //     _poblacion.IterEntidad = poblacion.IterEntidad;
            //     _poblacion.IterMunicipio = poblacion.IterMunicipio;
            //     _poblacion.IterLocalidad = poblacion.IterLocalidad;
            //     arquosDbContext.CatPoblaciones.Update( _poblacion);
            //     arquosDbContext.SaveChanges();
                
            // }catch(Exception err){
            //     this.logger.LogError(err, "Error al actualizar la poblacion: {message}", err.Message );
            // }
        }
    }

}