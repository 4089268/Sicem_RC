using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;
using SICEM_Blazor.Data;
using SICEM_Blazor.Data.Contracts;
using SICEM_Blazor.Eficiencia.Models;
using SICEM_Blazor.Eficiencia.Data;
using SICEM_Blazor.Models;
using Aspose.Cells.Slicers;

namespace SICEM_Blazor.Services {
    public class EficienciaService : IEficienciaService {

        private readonly ILogger<EficienciaService> logger;
        private int CommandConnectionTimeout = 900;

        public EficienciaService(ILogger<EficienciaService> l){
            this.logger = l;
        }

        public EficienciaResumen ObtenerResumenEnlace(IEnlace enlace, int anio, int mes, int sb, int sec, bool soloPropios = true ){
            var result = new EficienciaResumen(){
                Enlace = enlace,
                Estatus = ResumenOficinaEstatus.Completado
            };

            try {
                using( var sqlConnection = new SqlConnection(enlace.GetConnectionString())){
                    sqlConnection.Open();
                    var _query = string.Format("Exec [sicem].[eficiencia_importe] @cAlias = 'MENSUAL_IMP_GLOBAL', @nSub = {0}, @nSec = {1}, @xAf = {2}, @xMf = {3}, @cPropios = {4}", sb, sec, anio, mes, soloPropios ? 1 : 0);

                    var command = new SqlCommand(_query, sqlConnection);
                    command.CommandTimeout = CommandConnectionTimeout;
                    using(SqlDataReader reader = command.ExecuteReader()){
                        if(reader.Read()){
                            result.Af =  Convert.ToInt32(reader["af"]);
                            result.Mf = Convert.ToInt32(reader["mmf"]);
                            result.Mes = reader["mf"].ToString();
                            result.Facturado = ConvertUtils.ParseDecimal( reader["facturado"].ToString());
                            result.Refacturado = ConvertUtils.ParseDecimal( reader["refacturado"].ToString());
                            result.Anticipado = ConvertUtils.ParseDecimal( reader["anticipado"].ToString());
                            result.Descontado = ConvertUtils.ParseDecimal( reader["descontado"].ToString());
                            result.Cobrado = ConvertUtils.ParseDecimal( reader["cobrado"].ToString());
                            result.Porcentaje = ConvertUtils.ParseDouble( reader["porc"].ToString());
                            try {
                                
                                result.PorcentajeCapa = ConvertUtils.ParseDouble( reader["porc_capa"].ToString());
                                result.CobroCapa = ConvertUtils.ParseDecimal( reader["cobrado1"].ToString());
                            }catch(Exception err){
                                Console.WriteLine(err);
                                Console.WriteLine(enlace.Nombre);
                            }
                        }
                    }
                    sqlConnection.Close();
                }
            }catch(Exception err){
                logger.LogError(err, $"Error al obtener la eficiencia comercial del enlace {enlace.Nombre}");
                result.Estatus = ResumenOficinaEstatus.Error;
            }
            return result;
        }

        public IEnumerable<EficienciaResumen> ObtenerResumenAnual(IEnlace enlace, int anio, int sb, int sec, bool soloPropios ){
            var result = new List<EficienciaResumen>();
            try{
                using( var sqlConnection = new SqlConnection(enlace.GetConnectionString())){
                    sqlConnection.Open();
                    var _query = string.Format("Exec [sicem].[eficiencia_importe] @cAlias = 'MENSUAL_IMP_GLOBAL', @nSub = {0}, @nSec = {1}, @xAf = {2}, @xMf = 0, @cPropios = {3}", sb, sec, anio, soloPropios ? 1 : 0);

                    var command = new SqlCommand(_query, sqlConnection);
                    command.CommandTimeout = CommandConnectionTimeout;
                    using(SqlDataReader reader = command.ExecuteReader()){
                        while(reader.Read()){
                            var item = new EficienciaResumen(){
                                Enlace = enlace
                            };
                            item.Af = Convert.ToInt32(reader["af"]);
                            item.Mf = Convert.ToInt32(reader["mmf"]);
                            item.Mes = reader["mf"].ToString();
                            item.Facturado = ConvertUtils.ParseDecimal( reader["facturado"].ToString());
                            item.Refacturado = ConvertUtils.ParseDecimal( reader["refacturado"].ToString());
                            item.Anticipado = ConvertUtils.ParseDecimal( reader["anticipado"].ToString());
                            item.Descontado = ConvertUtils.ParseDecimal( reader["descontado"].ToString());
                            item.Cobrado = ConvertUtils.ParseDecimal( reader["cobrado"].ToString());
                            item.Porcentaje = ConvertUtils.ParseDouble( reader["porc"].ToString());
                            try {
                                item.PorcentajeCapa = ConvertUtils.ParseDouble( reader["porc_capa"].ToString());
                                item.CobroCapa = ConvertUtils.ParseDecimal( reader["cobrado1"].ToString());
                            }catch(Exception err ){
                                Console.WriteLine(err);
                            }
                            result.Add(item);
                        }
                    }
                    sqlConnection.Close();
                }
                return result;

            }catch(Exception err){
                logger.LogError(err, $"Error al obtener la eficiencia comercial anual del enlace {enlace.Nombre}");
                return new List<EficienciaResumen>();
            }
        }

        public IEnumerable<EficienciaLocalidad> EficienciaPorLocalidades(IEnlace enlace, int anio, int mes, int sb, int sec, bool soloPropios = true ){
            var results = new List<EficienciaLocalidad>();

            try {
                using( var sqlConnection = new SqlConnection(enlace.GetConnectionString())){
                    sqlConnection.Open();
                    var _query = string.Format("Exec [sicem].[eficiencia_importe] @cAlias = 'MENSUAL_IMP_GLOBAL_POBLACIONES', @nSub = {0}, @nSec = {1}, @xAf = {2}, @xMf = {3}, @cPropios = {4}", sb, sec, anio, mes, soloPropios ? 1 : 0);

                    var command = new SqlCommand(_query, sqlConnection);
                    command.CommandTimeout = CommandConnectionTimeout;
                    using(SqlDataReader reader = command.ExecuteReader()){
                        while(reader.Read()){
                            results.Add( EficienciaLocalidad.fromDataReader(reader) );
                        }
                    }
                    sqlConnection.Close();
                }
            }catch(Exception err){
                this.logger.LogError(err, "Error al obtener la eficiancia por localidad del enlace {enlace} ", enlace.Nombre);
                return Array.Empty<EficienciaLocalidad>();
            }
            
            // Add total row
            var totalRow = new EficienciaLocalidad(){
                IdPoblacion = 999,
                Poblacion = " TOTAL",
                EsRural = false,
                Facturado = results.Sum(item => item.Facturado),
                Anticipado = results.Sum(item => item.Anticipado),
                Descuentos = results.Sum(item => item.Descuentos),
                Cobrado = results.Sum(item => item.Cobrado),
                Refacturacion = results.Sum(item => item.Refacturacion),
                CobroCapa = results.Sum(item => item.CobroCapa),
                PorcentajeCapa = results.Average(item => item.PorcentajeCapa),
                EfiCome = 0,
                EfiConagua = 0
            };

            try {
                totalRow.EfiConagua = (double) (totalRow.Cobrado / totalRow.Facturado ) * 100;
                totalRow.EfiCome = (double) ( (totalRow.Anticipado + totalRow.Descuentos + totalRow.Cobrado) / (totalRow.Facturado + totalRow.Refacturacion) ) * 100;
            }
            catch (System.Exception) {}

            results.Add(totalRow);
            return results;
        }

        public IEnumerable<EficienciaImpPoblacionTarifa> ObtenerEficienciaPoblacionesTarifa(IEnlace enlace, int anio, int mes, int sb, int sec, int id_poblacion, bool soloPropios = true ){
            var result = new List<EficienciaImpPoblacionTarifa>();

            try {

                using( var sqlConnection = new SqlConnection(enlace.GetConnectionString())){
                    sqlConnection.Open();
                    var _query = string.Format("Exec [sicem].[eficiencia_importe] @cAlias='MENSUAL_IMP_POBLACIONES_TARIFAS', @nSub = {0}, @nSec = {1}, @xAf = {2}, @xMf = {3}, @cPropios = {4}", sb, sec, anio, mes, soloPropios ? 1 : 0);

                    var command = new SqlCommand(_query, sqlConnection);
                    command.CommandTimeout = CommandConnectionTimeout;
                    using(SqlDataReader reader = command.ExecuteReader()){
                        while(reader.Read()){
                            var newItem = new EficienciaImpPoblacionTarifa();
                            newItem.Af = Convert.ToInt32( reader["af"] );
                            newItem.Mf = Convert.ToInt32( reader["mmf"] );
                            newItem.Mes = reader["mf"].ToString();
                            newItem.Facturado = ConvertUtils.ParseDecimal( reader["facturado"].ToString());
                            newItem.Refacturado = ConvertUtils.ParseDecimal( reader["refacturado"].ToString());
                            newItem.Anticipado = ConvertUtils.ParseDecimal( reader["anticipado"].ToString());
                            newItem.Descontado = ConvertUtils.ParseDecimal( reader["descontado"].ToString());
                            newItem.Cobrado = ConvertUtils.ParseDecimal( reader["cobrado"].ToString());
                            newItem.Porcentaje = ConvertUtils.ParseDouble( reader["porc"].ToString());
                            newItem.PorcentajeCNA = ConvertUtils.ParseDouble( reader["porc_cna"].ToString());
                            newItem.Id_Poblacion = ConvertUtils.ParseInteger( reader["id_poblacion"].ToString());
                            newItem.Poblacion = reader["_poblacion"].ToString();
                            newItem.Id_Tarifa = ConvertUtils.ParseInteger( reader["id_tarifa"].ToString());
                            newItem.Tarifa = reader["_tarifa"].ToString();
                            try {
                                newItem.CobroCapa = ConvertUtils.ParseDecimal( reader["cobrado1"].ToString());
                                newItem.PorcentajeCapa = ConvertUtils.ParseDouble( reader["porc_capa"].ToString());
                            }catch(Exception){}
                            result.Add(newItem);
                        }
                    }
                    sqlConnection.Close();
                }

                return result.Where( item => item.Id_Poblacion == id_poblacion).ToList();

            }catch(Exception err){
                logger.LogError(err, "Error al obtener la eficiencia comercial volumen por poblaciones del enlace {enlace}", enlace.Nombre);
                return null;
            }
        }
        
        public IEnumerable<EficienciaImpTarifa> ObtenerEficienciaTarifas(IEnlace enlace, int anio, int mes, int sb, int sec, bool soloPropios = true ){
            var result = new List<EficienciaImpTarifa>();

            try {
                using( var sqlConnection = new SqlConnection(enlace.GetConnectionString())){
                    sqlConnection.Open();
                    var _query = string.Format("Exec [sicem].[eficiencia_importe] @cAlias='MENSUAL_IMP_GLOBAL_TARIFAS', @nSub = {0}, @nSec = {1}, @xAf = {2}, @xMf = {3}, @cPropios = {4}", sb, sec, anio, mes, soloPropios ? 1 : 0);

                    var command = new SqlCommand(_query, sqlConnection);
                    command.CommandTimeout = CommandConnectionTimeout;
                    using(SqlDataReader reader = command.ExecuteReader()){
                        while(reader.Read()){
                            var newItem = new EficienciaImpTarifa();
                            newItem.Af = Convert.ToInt32( reader["af"] );
                            newItem.Mf = Convert.ToInt32( reader["mmf"] );
                            newItem.Mes = reader["mf"].ToString();
                            newItem.Facturado = ConvertUtils.ParseDecimal( reader["facturado"].ToString());
                            newItem.Refacturado = ConvertUtils.ParseDecimal( reader["refacturado"].ToString());
                            newItem.Anticipado = ConvertUtils.ParseDecimal( reader["anticipado"].ToString());
                            newItem.Descontado = ConvertUtils.ParseDecimal( reader["descontado"].ToString());
                            newItem.Cobrado = ConvertUtils.ParseDecimal( reader["cobrado"].ToString());
                            newItem.Porcentaje = ConvertUtils.ParseDouble( reader["porc"].ToString());
                            newItem.PorcentajeCNA = ConvertUtils.ParseDouble( reader["porc_cna"].ToString());
                            newItem.Id_Tarifa = ConvertUtils.ParseInteger( reader["id_tarifa"].ToString());
                            newItem.Tarifa = reader["_tarifa"].ToString();
                            try {
                                newItem.CobroCapa = ConvertUtils.ParseDecimal( reader["cobrado1"].ToString());
                                newItem.PorcentajeCapa = ConvertUtils.ParseDouble( reader["porc_capa"].ToString());
                            }
                            catch (System.Exception) { }
                            result.Add(newItem);
                        }
                    }
                    sqlConnection.Close();
                }

                return result;

            }catch(Exception err){
                logger.LogError(err, $"Error al obtener la eficiencia comercial volumen por tarifas del enlace {enlace.Nombre}");
                return null;
                
            }
        }



        public Eficiencia_Colonia[] ObtenerEficienciaColonias(int Id_Oficina, int Ano, int Mes, int Sb, int Sect, int Tipo)
        {
            throw new NotImplementedException();
        }

        public Eficiencia_Conceptos[] ObtenerEficienciaConceptos(int Id_Oficina, int Ano, int Mes, int Sb, int Sect)
        {
            throw new NotImplementedException();
        }

        public Eficiencia_Detalle[] ObtenerEficienciaDetalleColonia(int Id_Oficina, int Ano, int Mes, int Sb, int Sect, int Tipo, int IdColonia, int IdLocalidad)
        {
            throw new NotImplementedException();
        }

        public Eficiencia_Detalle[] ObtenerEficienciaDetalleSector(int Id_Oficina, int Ano, int Mes, int Sb, int Sect, int Tipo)
        {
            throw new NotImplementedException();
        }

        public dynamic[] ObtenerEficienciaPorOficinas(Ruta[] oficinas, DateTime fecha1, DateTime fecha2, bool agregarTotal = true)
        {
            throw new NotImplementedException();
        }

        public Eficiencia_Sectores_Resp ObtenerEficienciaSectores(int Id_Oficina, int Ano, int Mes, int Sb)
        {
            throw new NotImplementedException();
        }


        // Eficiencias por volumen
        public EficienciaResumenVolumen ObtenerResumenVolumenEnlace(IEnlace enlace, int anio, int mes, int sb, int sec, bool soloPropios = true ){
            var result = new EficienciaResumenVolumen(){
                Enlace = enlace,
                Estatus = ResumenOficinaEstatus.Completado
            };

            try {
                using( var sqlConnection = new SqlConnection(enlace.GetConnectionString())){
                    sqlConnection.Open();
                    var _query = string.Format("Exec [sicem].[eficiencia_volumen] @cAlias='MENSUAL_VOL_GLOBAL', @nSub = {0}, @nSec = {1}, @xAf = {2}, @xMf = {3}, @cPropios = {4}", sb, sec, anio, mes, soloPropios ? 1 : 0);

                    var command = new SqlCommand(_query, sqlConnection);
                    command.CommandTimeout = CommandConnectionTimeout;
                    using(SqlDataReader reader = command.ExecuteReader()){
                        if(reader.Read()){
                            result.Af = Convert.ToInt32( reader["af"] );
                            result.Mf = Convert.ToInt32( reader["mmf"] );
                            result.Mes = reader["mf"].ToString();
                            result.Facturado = ConvertUtils.ParseInteger( reader["facturado"].ToString());
                            result.Refacturado = ConvertUtils.ParseInteger( reader["refacturado"].ToString());
                            result.Anticipado = ConvertUtils.ParseInteger( reader["anticipado"].ToString());
                            result.Descontado = ConvertUtils.ParseInteger( reader["descontado"].ToString());
                            result.Cobrado = ConvertUtils.ParseInteger( reader["cobrado"].ToString());
                            result.Porcentaje = ConvertUtils.ParseDouble( reader["porc"].ToString());
                            result.PorcentajeCNA = ConvertUtils.ParseDouble( reader["porc_cna"].ToString());
                        }
                    }				
                    sqlConnection.Close();
                }
            }catch(Exception err){
                logger.LogError(err, $"Error al obtener la eficiencia comercial del enlace {enlace.Nombre}");
                result.Estatus = ResumenOficinaEstatus.Error;
            }
            return result;
        }

        public IEnumerable<EficienciaResumenVolumen> ObtenerResumenVolumenAnual(IEnlace enlace, int anio, int sb, int sec, bool soloPropios = true ){
            var result = new List<EficienciaResumenVolumen>();
            try{
                using( var sqlConnection = new SqlConnection(enlace.GetConnectionString())){
                    sqlConnection.Open();
                    var _query = string.Format("Exec [sicem].[eficiencia_volumen] @cAlias = 'MENSUAL_VOL_GLOBAL', @nSub = {0}, @nSec = {1}, @xAf = {2}, @xMf = 0, @cPropios = {3}", sb, sec, anio, soloPropios ? 1 : 0);

                    var command = new SqlCommand(_query, sqlConnection);
                    command.CommandTimeout = CommandConnectionTimeout;
                    using(SqlDataReader reader = command.ExecuteReader()){
                        while(reader.Read()){
                            var item = new EficienciaResumenVolumen(){
                                Enlace = enlace
                            };
                            item.Af = Convert.ToInt32(reader["af"]);
                            item.Mf = Convert.ToInt32(reader["mmf"]);
                            item.Mes = reader["mf"].ToString();
                            item.Facturado = ConvertUtils.ParseInteger( reader["facturado"].ToString());
                            item.Refacturado = ConvertUtils.ParseInteger( reader["refacturado"].ToString());
                            item.Anticipado = ConvertUtils.ParseInteger( reader["anticipado"].ToString());
                            item.Descontado = ConvertUtils.ParseInteger( reader["descontado"].ToString());
                            item.Cobrado = ConvertUtils.ParseInteger( reader["cobrado"].ToString());
                            item.Porcentaje = ConvertUtils.ParseDouble( reader["porc"].ToString());
                            result.Add(item);
                        }
                    }				
                    sqlConnection.Close();
                }
                return result;

            }catch(Exception err){
                logger.LogError(err, $"Error al obtener la eficiencia comercial anual del enlace {enlace.Nombre}");
                return new List<EficienciaResumenVolumen>();
            }
        }

        public IEnumerable<EficienciaVolumenTarifa> ObtenerResumenVolumenTarifas(IEnlace enlace, int anio, int mes, int sb, int sec, bool soloPropios = true ){
            var result = new List<EficienciaVolumenTarifa>();

            try {
                using( var sqlConnection = new SqlConnection(enlace.GetConnectionString())){
                    sqlConnection.Open();
                    var _query = string.Format("Exec [sicem].[eficiencia_volumen] @cAlias='MENSUAL_VOL_GLOBAL_TARIFAS', @nSub = {0}, @nSec = {1}, @xAf = {2}, @xMf = {3}, @cPropios = {4}", sb, sec, anio, mes, soloPropios ? 1 : 0);

                    var command = new SqlCommand(_query, sqlConnection);
                    command.CommandTimeout = CommandConnectionTimeout;
                    using(SqlDataReader reader = command.ExecuteReader()){
                        while(reader.Read()){
                            var newItem = new EficienciaVolumenTarifa();
                            newItem.Af = Convert.ToInt32( reader["af"] );
                            newItem.Mf = Convert.ToInt32( reader["mmf"] );
                            newItem.Mes = reader["mf"].ToString();
                            newItem.Facturado = ConvertUtils.ParseInteger( reader["facturado"].ToString());
                            newItem.Refacturado = ConvertUtils.ParseInteger( reader["refacturado"].ToString());
                            newItem.Anticipado = ConvertUtils.ParseInteger( reader["anticipado"].ToString());
                            newItem.Descontado = ConvertUtils.ParseInteger( reader["descontado"].ToString());
                            newItem.Cobrado = ConvertUtils.ParseInteger( reader["cobrado"].ToString());
                            newItem.Porcentaje = ConvertUtils.ParseDouble( reader["porc"].ToString());
                            newItem.PorcentajeCNA = ConvertUtils.ParseDouble( reader["porc_cna"].ToString());
                            newItem.Id_Tarifa = ConvertUtils.ParseInteger( reader["id_tarifa"].ToString());
                            newItem.Tarifa = reader["_tarifa"].ToString();
                            result.Add(newItem);
                        }
                    }				
                    sqlConnection.Close();
                }

                return result;

            }catch(Exception err){
                logger.LogError(err, $"Error al obtener la eficiencia comercial volumen por tarifas del enlace {enlace.Nombre}");
                return null;
                
            }
        }
        
        public IEnumerable<EficienciaVolumenPoblacion> ObtenerResumenVolumenPoblaciones(IEnlace enlace, int anio, int mes, int sb, int sec, bool soloPropios = true ){
            var result = new List<EficienciaVolumenPoblacion>();

            try {
                using( var sqlConnection = new SqlConnection(enlace.GetConnectionString())){
                    sqlConnection.Open();
                    var _query = string.Format("Exec [sicem].[eficiencia_volumen] @cAlias='MENSUAL_VOL_GLOBAL_POBLACIONES', @nSub = {0}, @nSec = {1}, @xAf = {2}, @xMf = {3}, @cPropios = {4}", sb, sec, anio, mes, soloPropios ? 1 : 0);

                    var command = new SqlCommand(_query, sqlConnection);
                    command.CommandTimeout = CommandConnectionTimeout;
                    using(SqlDataReader reader = command.ExecuteReader()){
                        while(reader.Read()){
                            var newItem = new EficienciaVolumenPoblacion();
                            newItem.Af = Convert.ToInt32( reader["af"] );
                            newItem.Mf = Convert.ToInt32( reader["mmf"] );
                            newItem.Mes = reader["mf"].ToString();
                            newItem.Facturado = ConvertUtils.ParseInteger( reader["facturado"].ToString());
                            newItem.Refacturado = ConvertUtils.ParseInteger( reader["refacturado"].ToString());
                            newItem.Anticipado = ConvertUtils.ParseInteger( reader["anticipado"].ToString());
                            newItem.Descontado = ConvertUtils.ParseInteger( reader["descontado"].ToString());
                            newItem.Cobrado = ConvertUtils.ParseInteger( reader["cobrado"].ToString());
                            newItem.Porcentaje = ConvertUtils.ParseDouble( reader["porc"].ToString());
                            newItem.PorcentajeCNA = ConvertUtils.ParseDouble( reader["porc_cna"].ToString());
                            newItem.Id_Poblacion = ConvertUtils.ParseInteger( reader["id_poblacion"].ToString());
                            newItem.Poblacion = reader["_poblacion"].ToString();
                            result.Add(newItem);
                        }
                    }				
                    sqlConnection.Close();
                }

                return result;

            }catch(Exception err){
                logger.LogError(err, "Error al obtener la eficiencia comercial volumen por poblaciones del enlace {enlace}", enlace.Nombre);
                return null;
            }
        }
        
        public IEnumerable<EficienciaVolumenPoblacionTarifa> ObtenerResumenVolumenPoblacionesTarifa(IEnlace enlace, int anio, int mes, int sb, int sec, int id_poblacion, bool soloPropios = true ){
            var result = new List<EficienciaVolumenPoblacionTarifa>();

            try {

                using( var sqlConnection = new SqlConnection(enlace.GetConnectionString())){
                    sqlConnection.Open();
                    var _query = string.Format("Exec [sicem].[eficiencia_volumen] @cAlias='MENSUAL_VOL_POBLACIONES_TARIFAS', @nSub = {0}, @nSec = {1}, @xAf = {2}, @xMf = {3}, @cPropios = {4}", sb, sec, anio, mes, soloPropios ? 1 : 0);

                    var command = new SqlCommand(_query, sqlConnection);
                    command.CommandTimeout = CommandConnectionTimeout;
                    using(SqlDataReader reader = command.ExecuteReader()){
                        while(reader.Read()){
                            var newItem = new EficienciaVolumenPoblacionTarifa();
                            newItem.Af = Convert.ToInt32( reader["af"] );
                            newItem.Mf = Convert.ToInt32( reader["mmf"] );
                            newItem.Mes = reader["mf"].ToString();
                            newItem.Facturado = ConvertUtils.ParseInteger( reader["facturado"].ToString());
                            newItem.Refacturado = ConvertUtils.ParseInteger( reader["refacturado"].ToString());
                            newItem.Anticipado = ConvertUtils.ParseInteger( reader["anticipado"].ToString());
                            newItem.Descontado = ConvertUtils.ParseInteger( reader["descontado"].ToString());
                            newItem.Cobrado = ConvertUtils.ParseInteger( reader["cobrado"].ToString());
                            newItem.Porcentaje = ConvertUtils.ParseDouble( reader["porc"].ToString());
                            newItem.PorcentajeCNA = ConvertUtils.ParseDouble( reader["porc_cna"].ToString());
                            newItem.Id_Poblacion = ConvertUtils.ParseInteger( reader["id_poblacion"].ToString());
                            newItem.Poblacion = reader["_poblacion"].ToString();
                            newItem.Id_Tarifa = ConvertUtils.ParseInteger( reader["id_tarifa"].ToString());
                            newItem.Tarifa = reader["_tarifa"].ToString();
                            result.Add(newItem);
                        }
                    }				
                    sqlConnection.Close();
                }

                return result.Where( item => item.Id_Poblacion == id_poblacion).ToList();

            }catch(Exception err){
                logger.LogError(err, "Error al obtener la eficiencia comercial volumen por poblaciones del enlace {enlace}", enlace.Nombre);
                return null;
            }
        }
        

        // Eficiencia por usuarios
        public EficienciResumenUsuario ObtenerResumenUsuariosEnlace(IEnlace enlace, int anio, int mes, int sb, int sec, bool soloPropios = true ){
            var result = new EficienciResumenUsuario(){
                Enlace = enlace,
                Estatus = ResumenOficinaEstatus.Completado
            };

            try {
                using( var sqlConnection = new SqlConnection(enlace.GetConnectionString())){
                    sqlConnection.Open();
                    var _query = string.Format("Exec [sicem].[eficiencia_usuario] @cAlias='MENSUAL_USU_GLOBAL', @nSub = {0}, @nSec = {1}, @xAf = {2}, @xMf = {3}, @cPropios = {4}", sb, sec, anio, mes, soloPropios ? 1 : 0);

                    var command = new SqlCommand(_query, sqlConnection);
                    command.CommandTimeout = CommandConnectionTimeout;
                    using(SqlDataReader reader = command.ExecuteReader()){
                        if(reader.Read()){
                            result.Af = Convert.ToInt32( reader["af"] );
                            result.Mf = Convert.ToInt32( reader["mmf"] );
                            result.Mes = reader["mf"].ToString();
                            result.Facturado = ConvertUtils.ParseInteger( reader["facturado"].ToString());
                            result.Refacturado = ConvertUtils.ParseInteger( reader["refacturado"].ToString());
                            result.Anticipado = ConvertUtils.ParseInteger( reader["anticipado"].ToString());
                            result.Descontado = ConvertUtils.ParseInteger( reader["descontado"].ToString());
                            result.Cobrado = ConvertUtils.ParseInteger( reader["cobrado"].ToString());
                            result.Porcentaje = ConvertUtils.ParseDouble( reader["porc"].ToString());
                            result.PorcentajeCNA = ConvertUtils.ParseDouble( reader["porc_cna"].ToString());
                        }
                    }				
                    sqlConnection.Close();
                }
            }catch(Exception err){
                logger.LogError(err, $"Error al obtener la eficiencia comercial del enlace {enlace.Nombre}");
                result.Estatus = ResumenOficinaEstatus.Error;
            }
            return result;
        }

        public IEnumerable<EficienciResumenUsuario> ObtenerResumenUsuariosAnual(IEnlace enlace, int anio, int sb, int sec, bool soloPropios = true ){
            var result = new List<EficienciResumenUsuario>();
            try{
                using( var sqlConnection = new SqlConnection(enlace.GetConnectionString())){
                    sqlConnection.Open();
                    var _query = string.Format("Exec [sicem].[eficiencia_usuario] @cAlias = 'MENSUAL_USU_GLOBAL', @nSub = {0}, @nSec = {1}, @xAf = {2}, @xMf = 0, @cPropios = {3}", sb, sec, anio, soloPropios ? 1 : 0);

                    var command = new SqlCommand(_query, sqlConnection);
                    command.CommandTimeout = CommandConnectionTimeout;
                    using(SqlDataReader reader = command.ExecuteReader()){
                        while(reader.Read()){
                            var item = new EficienciResumenUsuario(){
                                Enlace = enlace
                            };
                            item.Af = Convert.ToInt32(reader["af"]);
                            item.Mf = Convert.ToInt32(reader["mmf"]);
                            item.Mes = reader["mf"].ToString();
                            item.Facturado = ConvertUtils.ParseInteger( reader["facturado"].ToString());
                            item.Refacturado = ConvertUtils.ParseInteger( reader["refacturado"].ToString());
                            item.Anticipado = ConvertUtils.ParseInteger( reader["anticipado"].ToString());
                            item.Descontado = ConvertUtils.ParseInteger( reader["descontado"].ToString());
                            item.Cobrado = ConvertUtils.ParseInteger( reader["cobrado"].ToString());
                            item.Porcentaje = ConvertUtils.ParseDouble( reader["porc"].ToString());
                            result.Add(item);
                        }
                    }				
                    sqlConnection.Close();
                }
                return result;

            }catch(Exception err){
                logger.LogError(err, $"Error al obtener la eficiencia comercial anual del enlace {enlace.Nombre}");
                return new List<EficienciResumenUsuario>();
            }
        }

        public IEnumerable<EficienciaUsuarioTarifa> ObtenerResumenUsuariosTarifas(IEnlace enlace, int anio, int mes, int sb, int sec, bool soloPropios = true ){
            var result = new List<EficienciaUsuarioTarifa>();

            try {
                using( var sqlConnection = new SqlConnection(enlace.GetConnectionString())){
                    sqlConnection.Open();
                    var _query = string.Format("Exec [sicem].[eficiencia_usuario] @cAlias='MENSUAL_USU_GLOBAL_TARIFAS', @nSub = {0}, @nSec = {1}, @xAf = {2}, @xMf = {3}, @cPropios = {4}", sb, sec, anio, mes, soloPropios ? 1 : 0);

                    var command = new SqlCommand(_query, sqlConnection);
                    command.CommandTimeout = CommandConnectionTimeout;
                    using(SqlDataReader reader = command.ExecuteReader()){
                        while(reader.Read()){
                            var newItem = new EficienciaUsuarioTarifa();
                            newItem.Af = Convert.ToInt32( reader["af"] );
                            newItem.Mf = Convert.ToInt32( reader["mmf"] );
                            newItem.Mes = reader["mf"].ToString();
                            newItem.Facturado = ConvertUtils.ParseInteger( reader["facturado"].ToString());
                            newItem.Refacturado = ConvertUtils.ParseInteger( reader["refacturado"].ToString());
                            newItem.Anticipado = ConvertUtils.ParseInteger( reader["anticipado"].ToString());
                            newItem.Descontado = ConvertUtils.ParseInteger( reader["descontado"].ToString());
                            newItem.Cobrado = ConvertUtils.ParseInteger( reader["cobrado"].ToString());
                            newItem.Porcentaje = ConvertUtils.ParseDouble( reader["porc"].ToString());
                            newItem.PorcentajeCNA = ConvertUtils.ParseDouble( reader["porc_cna"].ToString());
                            newItem.Id_Tarifa = ConvertUtils.ParseInteger( reader["id_tarifa"].ToString());
                            newItem.Tarifa = reader["_tarifa"].ToString();
                            result.Add(newItem);
                        }
                    }				
                    sqlConnection.Close();
                }

                return result;

            }catch(Exception err){
                logger.LogError(err, $"Error al obtener la eficiencia comercial volumen por tarifas del enlace {enlace.Nombre}");
                return null;
                
            }
        }
        
        public IEnumerable<EficienciaUsuarioPoblacion> ObtenerResumenUsuariosPoblaciones(IEnlace enlace, int anio, int mes, int sb, int sec, bool soloPropios = true ){
            var result = new List<EficienciaUsuarioPoblacion>();

            try {
                using( var sqlConnection = new SqlConnection(enlace.GetConnectionString())){
                    sqlConnection.Open();
                    var _query = string.Format("Exec [sicem].[eficiencia_usuario] @cAlias='MENSUAL_USU_GLOBAL_POBLACIONES', @nSub = {0}, @nSec = {1}, @xAf = {2}, @xMf = {3}, @cPropios = {4}", sb, sec, anio, mes, soloPropios ? 1 : 0);

                    var command = new SqlCommand(_query, sqlConnection);
                    command.CommandTimeout = CommandConnectionTimeout;
                    using(SqlDataReader reader = command.ExecuteReader()){
                        while(reader.Read()){
                            var newItem = new EficienciaUsuarioPoblacion();
                            newItem.Af = Convert.ToInt32( reader["af"] );
                            newItem.Mf = Convert.ToInt32( reader["mmf"] );
                            newItem.Mes = reader["mf"].ToString();
                            newItem.Facturado = ConvertUtils.ParseInteger( reader["facturado"].ToString());
                            newItem.Refacturado = ConvertUtils.ParseInteger( reader["refacturado"].ToString());
                            newItem.Anticipado = ConvertUtils.ParseInteger( reader["anticipado"].ToString());
                            newItem.Descontado = ConvertUtils.ParseInteger( reader["descontado"].ToString());
                            newItem.Cobrado = ConvertUtils.ParseInteger( reader["cobrado"].ToString());
                            newItem.Porcentaje = ConvertUtils.ParseDouble( reader["porc"].ToString());
                            newItem.PorcentajeCNA = ConvertUtils.ParseDouble( reader["porc_cna"].ToString());
                            newItem.Id_Poblacion = ConvertUtils.ParseInteger( reader["id_poblacion"].ToString());
                            newItem.Poblacion = reader["_poblacion"].ToString();
                            result.Add(newItem);
                        }
                    }				
                    sqlConnection.Close();
                }

                return result;

            }catch(Exception err){
                logger.LogError(err, "Error al obtener la eficiencia comercial volumen por poblaciones del enlace {enlace}", enlace.Nombre);
                return null;
            }
        }
        
        public IEnumerable<EficienciaUsuarioPoblacionTarifa> ObtenerResumenUsuariosPoblacionesTarifa(IEnlace enlace, int anio, int mes, int sb, int sec, int id_poblacion, bool soloPropios = true ){
            var result = new List<EficienciaUsuarioPoblacionTarifa>();

            try {

                using( var sqlConnection = new SqlConnection(enlace.GetConnectionString())){
                    sqlConnection.Open();
                    var _query = string.Format("Exec [sicem].[eficiencia_usuario] @cAlias='MENSUAL_USU_POBLACIONES_TARIFAS', @nSub = {0}, @nSec = {1}, @xAf = {2}, @xMf = {3}, @cPropios = {4}", sb, sec, anio, mes, soloPropios ? 1 : 0);

                    var command = new SqlCommand(_query, sqlConnection);
                    command.CommandTimeout = CommandConnectionTimeout;
                    using(SqlDataReader reader = command.ExecuteReader()){
                        while(reader.Read()){
                            var newItem = new EficienciaUsuarioPoblacionTarifa();
                            newItem.Af = Convert.ToInt32( reader["af"] );
                            newItem.Mf = Convert.ToInt32( reader["mmf"] );
                            newItem.Mes = reader["mf"].ToString();
                            newItem.Facturado = ConvertUtils.ParseInteger( reader["facturado"].ToString());
                            newItem.Refacturado = ConvertUtils.ParseInteger( reader["refacturado"].ToString());
                            newItem.Anticipado = ConvertUtils.ParseInteger( reader["anticipado"].ToString());
                            newItem.Descontado = ConvertUtils.ParseInteger( reader["descontado"].ToString());
                            newItem.Cobrado = ConvertUtils.ParseInteger( reader["cobrado"].ToString());
                            newItem.Porcentaje = ConvertUtils.ParseDouble( reader["porc"].ToString());
                            newItem.PorcentajeCNA = ConvertUtils.ParseDouble( reader["porc_cna"].ToString());
                            newItem.Id_Poblacion = ConvertUtils.ParseInteger( reader["id_poblacion"].ToString());
                            newItem.Poblacion = reader["_poblacion"].ToString();
                            newItem.Id_Tarifa = ConvertUtils.ParseInteger( reader["id_tarifa"].ToString());
                            newItem.Tarifa = reader["_tarifa"].ToString();
                            result.Add(newItem);
                        }
                    }				
                    sqlConnection.Close();
                }

                return result.Where( item => item.Id_Poblacion == id_poblacion).ToList();

            }catch(Exception err){
                logger.LogError(err, "Error al obtener la eficiencia comercial volumen por poblaciones del enlace {enlace}", enlace.Nombre);
                return null;
            }
        }
        
    }
}