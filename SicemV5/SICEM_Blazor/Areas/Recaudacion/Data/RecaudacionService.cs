using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using SICEM_Blazor.Data;
using SICEM_Blazor.Data.Contracts;
using SICEM_Blazor.Services;
using SICEM_Blazor.Recaudacion.Models;
using SICEM_Blazor.Models;

namespace SICEM_Blazor.Recaudacion.Data {

    public class RecaudacionService : IRecaudacionService {

        private readonly IConfiguration appSettings;
        private readonly SicemService sicemService;
        private readonly ILogger<IRecaudacionService> logger;
        
        public RecaudacionService(IConfiguration c, SicemService s, ILogger<IRecaudacionService> l) {
            this.appSettings = c;
            this.sicemService = s;
            this.logger = l;
        }

        public ResumenOficina ObtenerResumen(IEnlace enlace, DateRange dateRange)
        {
            var response = new ResumenOficina(enlace);
            try
            {
                using(var sqlConnection = new SqlConnection(enlace.GetConnectionString())) {
                    sqlConnection.Open();
                    var sqlCommand = new SqlCommand("[Sicem].[Recaudacion]", sqlConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    sqlCommand.Parameters.AddWithValue("@cAlias", "TOTALES");
                    sqlCommand.Parameters.AddWithValue("@cFecha1", dateRange.Desde_ISO);
                    sqlCommand.Parameters.AddWithValue("@cFecha2", dateRange.Hasta_ISO);
                    sqlCommand.Parameters.AddWithValue("@xSb", dateRange.Subsistema);
                    sqlCommand.Parameters.AddWithValue("@xSec", dateRange.Sector);
                    
                    using(var sqlDataReader = sqlCommand.ExecuteReader())
                    {
                        if(sqlDataReader.Read())
                        {
                            response = ResumenOficina.FromDataReader(enlace, sqlDataReader);
                        }
                    }
                    sqlConnection.Close();
                }
            }
            catch(Exception err)
            {
                logger.LogError(err, "Error al obtener resumen recaudacion enlace '{enlace}': {message}", enlace.Nombre, err.Message);
                response.Estatus = ResumenOficinaEstatus.Error;
            }
            return response;
        }

        public Recaudacion_Analitico ObtenerAnalisisIngresos(IEnlace enlace, DateRange dateRange)
        {
            var respuesta = new Recaudacion_Analitico();
            try
            {
                
                // * Analitico
                using(var sqlConnection = new SqlConnection(enlace.GetConnectionString()))
                {
                    sqlConnection.Open();
                    var sqlCommand1 = new SqlCommand("[SICEM].[Recaudacion]", sqlConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    sqlCommand1.Parameters.AddWithValue("@cAlias", "ANALITICO");
                    sqlCommand1.Parameters.AddWithValue("@cFecha1", dateRange.Desde_ISO);
                    sqlCommand1.Parameters.AddWithValue("@cFecha2", dateRange.Hasta_ISO);
                    sqlCommand1.Parameters.AddWithValue("@xSb", dateRange.Subsistema);
                    sqlCommand1.Parameters.AddWithValue("@xSec", dateRange.Sector);

                    var xDataSet = new DataSet();
                    var xDataAdapter = new SqlDataAdapter(sqlCommand1);
                    xDataAdapter.Fill(xDataSet);

                    //*** Analitico Mensual
                    var tmpMensual = new List<Recaudacion_AnaliticoMensual>();
                    var tmpTableMensual = xDataSet.Tables[0];
                    foreach(DataRow xRow in tmpTableMensual.Rows) {
                        var tmpItem = new Recaudacion_AnaliticoMensual();
                        tmpItem.Oficina = xRow.Field<string>("Oficina");
                        tmpItem.Año = xRow.Field<int>("Año");
                        tmpItem.Ene = xRow.Field<decimal>("Ene");
                        tmpItem.Feb = xRow.Field<decimal>("Feb");
                        tmpItem.Mar = xRow.Field<decimal>("Mar");
                        tmpItem.Abr = xRow.Field<decimal>("Abr");
                        tmpItem.May = xRow.Field<decimal>("May");
                        tmpItem.Jun = xRow.Field<decimal>("Jun");
                        tmpItem.Jul = xRow.Field<decimal>("Jul");
                        tmpItem.Ago = xRow.Field<decimal>("Ago");
                        tmpItem.Sep = xRow.Field<decimal>("Sep");
                        tmpItem.Oct = xRow.Field<decimal>("Oct");
                        tmpItem.Nov = xRow.Field<decimal>("Nov");
                        tmpItem.Dic = xRow.Field<decimal>("Dic");
                        tmpItem.Total = xRow.Field<decimal>("Total");
                        tmpMensual.Add(tmpItem);
                    }
                    respuesta.Analitico_Mensual = tmpMensual.ToArray();

                    //*** Analitico Quincenal
                    var tmpQuincenal = new List<Recaudacion_AnaliticoQuincenal>();
                    var tmpTableQuincenal = xDataSet.Tables[1];
                    foreach(DataRow xRow in tmpTableQuincenal.Rows) {
                        var tmpItem = new Recaudacion_AnaliticoQuincenal();
                        tmpItem.Oficina = xRow.Field<string>("Oficina");
                        tmpItem.Año = xRow.Field<int>("Año");
                        tmpItem.Ene_1 = xRow.Field<decimal>("Ene_1");
                        tmpItem.Ene_2 = xRow.Field<decimal>("Ene_2");
                        tmpItem.Feb_1 = xRow.Field<decimal>("Feb_1");
                        tmpItem.Feb_2 = xRow.Field<decimal>("Feb_2");
                        tmpItem.Mar_1 = xRow.Field<decimal>("Mar_1");
                        tmpItem.Mar_2 = xRow.Field<decimal>("Mar_2");
                        tmpItem.Abr_1 = xRow.Field<decimal>("Abr_1");
                        tmpItem.Abr_2 = xRow.Field<decimal>("Abr_2");
                        tmpItem.May_1 = xRow.Field<decimal>("May_1");
                        tmpItem.May_2 = xRow.Field<decimal>("May_2");
                        tmpItem.Jun_1 = xRow.Field<decimal>("Jun_1");
                        tmpItem.Jun_2 = xRow.Field<decimal>("Jun_2");
                        tmpItem.Jul_1 = xRow.Field<decimal>("Jul_1");
                        tmpItem.Jul_2 = xRow.Field<decimal>("Jul_2");
                        tmpItem.Ago_1 = xRow.Field<decimal>("Ago_1");
                        tmpItem.Ago_2 = xRow.Field<decimal>("Ago_2");
                        tmpItem.Sep_1 = xRow.Field<decimal>("Sep_1");
                        tmpItem.Sep_2 = xRow.Field<decimal>("Sep_2");
                        tmpItem.Oct_1 = xRow.Field<decimal>("Oct_1");
                        tmpItem.Oct_2 = xRow.Field<decimal>("Oct_2");
                        tmpItem.Nov_1 = xRow.Field<decimal>("Nov_1");
                        tmpItem.Nov_2 = xRow.Field<decimal>("Nov_2");
                        tmpItem.Dic_1 = xRow.Field<decimal>("Dic_1");
                        tmpItem.Dic_2 = xRow.Field<decimal>("Dic_2");
                        tmpItem.Total = xRow.Field<decimal>("Total");
                        tmpQuincenal.Add(tmpItem);
                    }
                    respuesta.Analitico_Quincenal = tmpQuincenal.ToArray();

                    //*** Analitico Semanal
                    var tmpSemanal = new List<Recaudacion_AnaliticoSemanal>();
                    var tmpTableSemanal = xDataSet.Tables[2];
                    foreach(DataRow xRow in tmpTableSemanal.Rows) {
                        var tmpItem = new Recaudacion_AnaliticoSemanal();
                        tmpItem.Oficina = xRow.Field<string>("Oficina");
                        tmpItem.Año = xRow.Field<int>("Año");
                        tmpItem.Ene_1 = xRow.Field<decimal>("Ene_1");
                        tmpItem.Ene_2 = xRow.Field<decimal>("Ene_2");
                        tmpItem.Ene_3 = xRow.Field<decimal>("Ene_3");
                        tmpItem.Ene_4 = xRow.Field<decimal>("Ene_4");

                        tmpItem.Feb_1 = xRow.Field<decimal>("Feb_1");
                        tmpItem.Feb_2 = xRow.Field<decimal>("Feb_2");
                        tmpItem.Feb_3 = xRow.Field<decimal>("Feb_3");
                        tmpItem.Feb_4 = xRow.Field<decimal>("Feb_4");

                        tmpItem.Mar_1 = xRow.Field<decimal>("Mar_1");
                        tmpItem.Mar_2 = xRow.Field<decimal>("Mar_2");
                        tmpItem.Mar_3 = xRow.Field<decimal>("Mar_3");
                        tmpItem.Mar_4 = xRow.Field<decimal>("Mar_4");

                        tmpItem.Abr_1 = xRow.Field<decimal>("Abr_1");
                        tmpItem.Abr_2 = xRow.Field<decimal>("Abr_2");
                        tmpItem.Abr_3 = xRow.Field<decimal>("Abr_3");
                        tmpItem.Abr_4 = xRow.Field<decimal>("Abr_4");

                        tmpItem.May_1 = xRow.Field<decimal>("May_1");
                        tmpItem.May_2 = xRow.Field<decimal>("May_2");
                        tmpItem.May_3 = xRow.Field<decimal>("May_3");
                        tmpItem.May_4 = xRow.Field<decimal>("May_4");

                        tmpItem.Jun_1 = xRow.Field<decimal>("Jun_1");
                        tmpItem.Jun_2 = xRow.Field<decimal>("Jun_2");
                        tmpItem.Jun_3 = xRow.Field<decimal>("Jun_3");
                        tmpItem.Jun_4 = xRow.Field<decimal>("Jun_4");

                        tmpItem.Jul_1 = xRow.Field<decimal>("Jul_1");
                        tmpItem.Jul_2 = xRow.Field<decimal>("Jul_2");
                        tmpItem.Jul_3 = xRow.Field<decimal>("Jul_3");
                        tmpItem.Jul_4 = xRow.Field<decimal>("Jul_4");

                        tmpItem.Ago_1 = xRow.Field<decimal>("Ago_1");
                        tmpItem.Ago_2 = xRow.Field<decimal>("Ago_2");
                        tmpItem.Ago_3 = xRow.Field<decimal>("Ago_3");
                        tmpItem.Ago_4 = xRow.Field<decimal>("Ago_4");

                        tmpItem.Sep_1 = xRow.Field<decimal>("Sep_1");
                        tmpItem.Sep_2 = xRow.Field<decimal>("Sep_2");
                        tmpItem.Sep_3 = xRow.Field<decimal>("Sep_3");
                        tmpItem.Sep_4 = xRow.Field<decimal>("Sep_4");

                        tmpItem.Oct_1 = xRow.Field<decimal>("Oct_1");
                        tmpItem.Oct_2 = xRow.Field<decimal>("Oct_2");
                        tmpItem.Oct_3 = xRow.Field<decimal>("Oct_3");
                        tmpItem.Oct_4 = xRow.Field<decimal>("Oct_4");

                        tmpItem.Nov_1 = xRow.Field<decimal>("Nov_1");
                        tmpItem.Nov_2 = xRow.Field<decimal>("Nov_2");
                        tmpItem.Nov_3 = xRow.Field<decimal>("Nov_3");
                        tmpItem.Nov_4 = xRow.Field<decimal>("Nov_4");

                        tmpItem.Dic_1 = xRow.Field<decimal>("Dic_1");
                        tmpItem.Dic_2 = xRow.Field<decimal>("Dic_2");
                        tmpItem.Dic_3 = xRow.Field<decimal>("Dic_3");
                        tmpItem.Dic_4 = xRow.Field<decimal>("Dic_4");

                        tmpItem.Total = xRow.Field<decimal>("Total");
                        tmpSemanal.Add(tmpItem);
                    }
                    respuesta.Analitico_Semanal = tmpSemanal.ToArray();
                    sqlConnection.Close();
                }

                // *** Analitico Rezago
                using(var sqlConnection = new SqlConnection(enlace.GetConnectionString())) {
                    sqlConnection.Open();
                    var sqlCommand = new SqlCommand("[SICEM].[Recaudacion]", sqlConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    sqlCommand.Parameters.AddWithValue("@cAlias", "ANALITICO_REZAGO");
                    sqlCommand.Parameters.AddWithValue("@cFecha1", dateRange.Desde_ISO);
                    sqlCommand.Parameters.AddWithValue("@cFecha2", dateRange.Hasta_ISO);
                    sqlCommand.Parameters.AddWithValue("@xSb", dateRange.Subsistema);
                    sqlCommand.Parameters.AddWithValue("@xSec", dateRange.Sector);

                    var xDataSet = new DataSet();
                    var xDataAdapter = new SqlDataAdapter(sqlCommand);
                    xDataAdapter.Fill(xDataSet);

                    //*** Mensual
                    var tmpMensual = new List<Recaudacion_AnaliticoMensual>();
                    var tmpTableMensual = xDataSet.Tables[0];
                    foreach(DataRow xRow in tmpTableMensual.Rows) {
                        var tmpItem = new Recaudacion_AnaliticoMensual();
                        tmpItem.Oficina = xRow.Field<string>("Oficina");
                        tmpItem.Año = xRow.Field<int>("Año");
                        tmpItem.Ene = xRow.Field<decimal>("Ene");
                        tmpItem.Feb = xRow.Field<decimal>("Feb");
                        tmpItem.Mar = xRow.Field<decimal>("Mar");
                        tmpItem.Abr = xRow.Field<decimal>("Abr");
                        tmpItem.May = xRow.Field<decimal>("May");
                        tmpItem.Jun = xRow.Field<decimal>("Jun");
                        tmpItem.Jul = xRow.Field<decimal>("Jul");
                        tmpItem.Ago = xRow.Field<decimal>("Ago");
                        tmpItem.Sep = xRow.Field<decimal>("Sep");
                        tmpItem.Oct = xRow.Field<decimal>("Oct");
                        tmpItem.Nov = xRow.Field<decimal>("Nov");
                        tmpItem.Dic = xRow.Field<decimal>("Dic");
                        tmpItem.Total = xRow.Field<decimal>("Total");
                        tmpMensual.Add(tmpItem);
                    }
                    respuesta.AnaliticoRez_Mensual = tmpMensual.ToArray();

                    //*** Quincenal
                    var tmpQuincenal = new List<Recaudacion_AnaliticoQuincenal>();
                    var tmpTableQuincenal = xDataSet.Tables[1];
                    foreach(DataRow xRow in tmpTableQuincenal.Rows) {
                        var tmpItem = new Recaudacion_AnaliticoQuincenal();
                        tmpItem.Oficina = xRow.Field<string>("Oficina");
                        tmpItem.Año = xRow.Field<int>("Año");
                        tmpItem.Ene_1 = xRow.Field<decimal>("Ene_1");
                        tmpItem.Ene_2 = xRow.Field<decimal>("Ene_2");
                        tmpItem.Feb_1 = xRow.Field<decimal>("Feb_1");
                        tmpItem.Feb_2 = xRow.Field<decimal>("Feb_2");
                        tmpItem.Mar_1 = xRow.Field<decimal>("Mar_1");
                        tmpItem.Mar_2 = xRow.Field<decimal>("Mar_2");
                        tmpItem.Abr_1 = xRow.Field<decimal>("Abr_1");
                        tmpItem.Abr_2 = xRow.Field<decimal>("Abr_2");
                        tmpItem.May_1 = xRow.Field<decimal>("May_1");
                        tmpItem.May_2 = xRow.Field<decimal>("May_2");
                        tmpItem.Jun_1 = xRow.Field<decimal>("Jun_1");
                        tmpItem.Jun_2 = xRow.Field<decimal>("Jun_2");
                        tmpItem.Jul_1 = xRow.Field<decimal>("Jul_1");
                        tmpItem.Jul_2 = xRow.Field<decimal>("Jul_2");
                        tmpItem.Ago_1 = xRow.Field<decimal>("Ago_1");
                        tmpItem.Ago_2 = xRow.Field<decimal>("Ago_2");
                        tmpItem.Sep_1 = xRow.Field<decimal>("Sep_1");
                        tmpItem.Sep_2 = xRow.Field<decimal>("Sep_2");
                        tmpItem.Oct_1 = xRow.Field<decimal>("Oct_1");
                        tmpItem.Oct_2 = xRow.Field<decimal>("Oct_2");
                        tmpItem.Nov_1 = xRow.Field<decimal>("Nov_1");
                        tmpItem.Nov_2 = xRow.Field<decimal>("Nov_2");
                        tmpItem.Dic_1 = xRow.Field<decimal>("Dic_1");
                        tmpItem.Dic_2 = xRow.Field<decimal>("Dic_2");
                        tmpItem.Total = xRow.Field<decimal>("Total");
                        tmpQuincenal.Add(tmpItem);
                    }
                    respuesta.AnaliticoRez_Quincenal = tmpQuincenal.ToArray();

                    //*** Semanal
                    var tmpSemanal = new List<Recaudacion_AnaliticoSemanal>();
                    var tmpTableSemanal = xDataSet.Tables[2];
                    foreach(DataRow xRow in tmpTableSemanal.Rows) {
                        var tmpItem = new Recaudacion_AnaliticoSemanal();
                        tmpItem.Oficina = xRow.Field<string>("Oficina");
                        tmpItem.Año = xRow.Field<int>("Año");
                        tmpItem.Ene_1 = xRow.Field<decimal>("Ene_1");
                        tmpItem.Ene_2 = xRow.Field<decimal>("Ene_2");
                        tmpItem.Ene_3 = xRow.Field<decimal>("Ene_3");
                        tmpItem.Ene_4 = xRow.Field<decimal>("Ene_4");

                        tmpItem.Feb_1 = xRow.Field<decimal>("Feb_1");
                        tmpItem.Feb_2 = xRow.Field<decimal>("Feb_2");
                        tmpItem.Feb_3 = xRow.Field<decimal>("Feb_3");
                        tmpItem.Feb_4 = xRow.Field<decimal>("Feb_4");

                        tmpItem.Mar_1 = xRow.Field<decimal>("Mar_1");
                        tmpItem.Mar_2 = xRow.Field<decimal>("Mar_2");
                        tmpItem.Mar_3 = xRow.Field<decimal>("Mar_3");
                        tmpItem.Mar_4 = xRow.Field<decimal>("Mar_4");

                        tmpItem.Abr_1 = xRow.Field<decimal>("Abr_1");
                        tmpItem.Abr_2 = xRow.Field<decimal>("Abr_2");
                        tmpItem.Abr_3 = xRow.Field<decimal>("Abr_3");
                        tmpItem.Abr_4 = xRow.Field<decimal>("Abr_4");

                        tmpItem.May_1 = xRow.Field<decimal>("May_1");
                        tmpItem.May_2 = xRow.Field<decimal>("May_2");
                        tmpItem.May_3 = xRow.Field<decimal>("May_3");
                        tmpItem.May_4 = xRow.Field<decimal>("May_4");

                        tmpItem.Jun_1 = xRow.Field<decimal>("Jun_1");
                        tmpItem.Jun_2 = xRow.Field<decimal>("Jun_2");
                        tmpItem.Jun_3 = xRow.Field<decimal>("Jun_3");
                        tmpItem.Jun_4 = xRow.Field<decimal>("Jun_4");

                        tmpItem.Jul_1 = xRow.Field<decimal>("Jul_1");
                        tmpItem.Jul_2 = xRow.Field<decimal>("Jul_2");
                        tmpItem.Jul_3 = xRow.Field<decimal>("Jul_3");
                        tmpItem.Jul_4 = xRow.Field<decimal>("Jul_4");

                        tmpItem.Ago_1 = xRow.Field<decimal>("Ago_1");
                        tmpItem.Ago_2 = xRow.Field<decimal>("Ago_2");
                        tmpItem.Ago_3 = xRow.Field<decimal>("Ago_3");
                        tmpItem.Ago_4 = xRow.Field<decimal>("Ago_4");

                        tmpItem.Sep_1 = xRow.Field<decimal>("Sep_1");
                        tmpItem.Sep_2 = xRow.Field<decimal>("Sep_2");
                        tmpItem.Sep_3 = xRow.Field<decimal>("Sep_3");
                        tmpItem.Sep_4 = xRow.Field<decimal>("Sep_4");

                        tmpItem.Oct_1 = xRow.Field<decimal>("Oct_1");
                        tmpItem.Oct_2 = xRow.Field<decimal>("Oct_2");
                        tmpItem.Oct_3 = xRow.Field<decimal>("Oct_3");
                        tmpItem.Oct_4 = xRow.Field<decimal>("Oct_4");

                        tmpItem.Nov_1 = xRow.Field<decimal>("Nov_1");
                        tmpItem.Nov_2 = xRow.Field<decimal>("Nov_2");
                        tmpItem.Nov_3 = xRow.Field<decimal>("Nov_3");
                        tmpItem.Nov_4 = xRow.Field<decimal>("Nov_4");

                        tmpItem.Dic_1 = xRow.Field<decimal>("Dic_1");
                        tmpItem.Dic_2 = xRow.Field<decimal>("Dic_2");
                        tmpItem.Dic_3 = xRow.Field<decimal>("Dic_3");
                        tmpItem.Dic_4 = xRow.Field<decimal>("Dic_4");

                        tmpItem.Total = xRow.Field<decimal>("Total");
                        tmpSemanal.Add(tmpItem);
                    }
                    respuesta.AnaliticoRez_Semanal = tmpSemanal.ToArray();
                    sqlConnection.Close();
                }

                return respuesta;
            }
            catch(Exception err) {
                logger.LogError(err, $">> Error al obtener ingresos Analitico enlace:{enlace.Nombre}");
                return null;
            }
        }
        
        public IEnumerable<Recaudacion_Rezago> ObtenerRezago(IEnlace enlace, DateTime desde, DateTime hasta, int sb, int sect) {
            var respuesta = new List<Recaudacion_Rezago>();
            try {
                using(var xConnecton = new SqlConnection(enlace.GetConnectionString())) {
                    xConnecton.Open();
                    var xCommand = new SqlCommand();
                    xCommand.Connection = xConnecton;
                    xCommand.CommandText = string.Format("EXEC [SICEM].[Recaudacion] 'INGRESOS_REZAGO','{0}','{1}'", desde.ToString("yyyyMMdd"), hasta.ToString("yyyyMMdd"));
                    using(var xReader = xCommand.ExecuteReader()) {
                        while(xReader.Read()) {
                            var tmpResRow = new Recaudacion_Rezago {
                                Mes = xReader["mes"].ToString(),
                                Usuarios = int.Parse(xReader.GetValue("usuarios").ToString()),
                                Rez_agua = xReader.GetFieldValue<decimal>("rez_agua"),
                                Rez_dren = xReader.GetFieldValue<decimal>("rez_dren"),
                                Rez_sane = xReader.GetFieldValue<decimal>("rez_sane"),
                                Rez_otros = xReader.GetFieldValue<decimal>("rez_otros"),
                                Rez_recargos = xReader.GetFieldValue<decimal>("rez_recargos"),
                                Subtotal = xReader.GetFieldValue<decimal>("subtotal"),
                                Iva = xReader.GetFieldValue<decimal>("iva"),
                                Total = xReader.GetFieldValue<decimal>("total")
                            };
                            respuesta.Add(tmpResRow);
                        }
                    }
                }
                return respuesta.ToArray();
            }
            catch(Exception err){
                logger.LogError(err, $">> Error al obtener el ingresos rezago enlace:{enlace.Nombre}");
                return null;
            }
        }
        
        public IEnumerable<IngresosDia> ObtenerIngresosPorDias(IEnlace enlace, DateRange dateRange)
        {
            var respuesta = new List<IngresosDia>();
            try
            {
                using(var sqlConnection = new SqlConnection(enlace.GetConnectionString()))
                {
                    sqlConnection.Open();
                    var sqlCommand = new SqlCommand("[SICEM].[Recaudacion]", sqlConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    sqlCommand.Parameters.AddWithValue("@cAlias", "TOTALES_X_DIA");
                    sqlCommand.Parameters.AddWithValue("@cFecha1", dateRange.Desde_ISO);
                    sqlCommand.Parameters.AddWithValue("@cFecha2", dateRange.Hasta_ISO);
                    sqlCommand.Parameters.AddWithValue("@xSb", dateRange.Subsistema);
                    sqlCommand.Parameters.AddWithValue("@xSec", dateRange.Sector);
                    using(var reader = sqlCommand.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            respuesta.Add(IngresosDia.FromDataReader(reader));
                        }
                    }
                }
            }
            catch(Exception err)
            {
                logger.LogError(err, "Error al obtener los ingresos por dia enlace '{enlace}': {message}", enlace.Nombre, err.Message);
                return null;
            }
            return respuesta.ToArray();
        }

        public IEnumerable<Recaudacion_IngresosCajas> ObtenerIngresosPorCajas(IEnlace enlace, DateTime desde, DateTime hasta, int sb, int sect) {
            var respuesta = new List<Recaudacion_IngresosCajas>();
            try {
                using(var xConnecton = new SqlConnection(enlace.GetConnectionString())) {
                    xConnecton.Open();
                    var xCommand = new SqlCommand();
                    xCommand.Connection = xConnecton;
                    xCommand.CommandText = string.Format("Exec [Sicem_QRoo].[Ingresos_04] @xfec1 = '{0}', @xfec2 = '{1}', @xSb = {2}, @xSec = {3}", desde.ToString("yyyyMMdd"), desde.ToString("yyyyMMdd"), sb, sect);
                    using(var xReader = xCommand.ExecuteReader()) {
                        while(xReader.Read()) {
                            var tmpInt = 0;
                            var tmpDecimal = 0m;
                            var tmpResRow = new Recaudacion_IngresosCajas {
                                Id_Sucursal = int.Parse(xReader.GetValue("id_sucursal").ToString()),
                                Caja = xReader.GetFieldValue<string>("caja"),
                                Facturado = decimal.TryParse(xReader.GetValue("facturado").ToString(), out tmpDecimal)?tmpDecimal:0m,
                                Cobrado = decimal.TryParse(xReader.GetValue("cobrado").ToString(), out tmpDecimal)?tmpDecimal:0m,
                                Recibos = int.TryParse(xReader.GetValue("recibos").ToString(), out tmpInt)?tmpInt:0,
                                CveCaja = xReader.GetValue("cve_caja").ToString(),
                                Sucursal = xReader.GetValue("sucursal").ToString()
                            };
                            respuesta.Add(tmpResRow);
                        }
                    }
                }
            }
            catch(Exception err){
                logger.LogError(err, $">> Error al obtener ingresos por cajas enlace:{enlace.Nombre}");
            }
            return respuesta.ToArray();
        }
        public IEnumerable<IngresosxConceptos> ObtenerIngresosPorConceptos(IEnlace enlace, DateTime desde, DateTime hasta, int sb, int sect){
            throw new NotImplementedException("Funcione reemplazada por ObtenerIngresosPorConceptosTipoUsuarios");
        }
        public IEnumerable<Ingresos_Conceptos> ObtenerIngresosPorConceptosTipoUsuarios(IEnlace enlace, DateTime desde, DateTime hasta, int sb , int sect, int idLocalidad = 0){
            var result = new List<Ingresos_Conceptos>();
            try {

                using(var sqlConnection = new SqlConnection(enlace.GetConnectionString())){
                    sqlConnection.Open();
                    var _query = $"[Sicem_QRoo].[Ingresos_03] @xfec1 = '{desde.ToString("yyyyMMdd")}', @xfec2 = '{hasta.ToString("yyyyMMdd")}', @idLocalidad = {idLocalidad}";
                    var _commad = new SqlCommand(_query, sqlConnection);
                    using(var reader = _commad.ExecuteReader()){
                        while(reader.Read()){
                            result.Add(Ingresos_Conceptos.FromSqlDataReader(reader));
                        }
                    }
                    sqlConnection.Close();
                }
                return result;
            }catch(Exception err){
                logger.LogError(err, "Error al tratar de obtener los ingresos de la oficina: " + enlace.Nombre);
                return new Ingresos_Conceptos[]{};
            }
        }
        public IEnumerable<IngresosTipoUsuario> ObtenerIngresosPorTipoUsuarios(IEnlace enlace, DateTime desde, DateTime hasta, int sb, int sect){
            try {
                var respuesta = new List<IngresosTipoUsuario>();
                using(var xConnecton = new SqlConnection(enlace.GetConnectionString())) {
                    xConnecton.Open();
                    var xCommand = new SqlCommand();
                    xCommand.Connection = xConnecton;
                    xCommand.CommandText = string.Format("EXEC [SICEM].[Recaudacion] 'POR_CONCEPTOS_TARIFAS','{0}','{1}',{2}, {3}", desde.ToString("yyyyMMdd"), hasta.ToString("yyyyMMdd"), sb, sect);
                    using(var xReader = xCommand.ExecuteReader()) {
                        while(xReader.Read()) {
                            var tmpResRow = new IngresosTipoUsuario {
                                Id_TipoCalculo = 0,
                                Id_Concepto = xReader.GetFieldValue<int>("Id_Concepto"),
                                Descripcion = xReader.GetFieldValue<string>("Descripcion"),

                                Dom_Sbt = xReader.GetFieldValue<decimal>("Sbt1"),
                                Dom_IVA = xReader.GetFieldValue<decimal>("IVA1"),
                                Dom_Tot = xReader.GetFieldValue<decimal>("Tot1"),

                                Hot_Sbt = xReader.GetFieldValue<decimal>("Sbt2"),
                                Hot_IVA = xReader.GetFieldValue<decimal>("IVA2"),
                                Hot_Tot = xReader.GetFieldValue<decimal>("Tot2"),

                                Com_Sbt = xReader.GetFieldValue<decimal>("Sbt3"),
                                Com_IVA = xReader.GetFieldValue<decimal>("IVA3"),
                                Com_Tot = xReader.GetFieldValue<decimal>("Tot3"),

                                Ind_Sbt = xReader.GetFieldValue<decimal>("Sbt4"),
                                Ind_IVA = xReader.GetFieldValue<decimal>("IVA4"),
                                Ind_Tot = xReader.GetFieldValue<decimal>("Tot4"),

                                Pub_Sbt = xReader.GetFieldValue<decimal>("Sbt5"),
                                Pub_IVA = xReader.GetFieldValue<decimal>("IVA5"),
                                Pub_Tot = xReader.GetFieldValue<decimal>("Tot5"),

                                Subtotal = xReader.GetFieldValue<decimal>("Subtotal"),
                                IVA = xReader.GetFieldValue<decimal>("IVA"),
                                Total = xReader.GetFieldValue<decimal>("Total")
                            };
                            respuesta.Add(tmpResRow);
                        }
                    }
                    xConnecton.Close();
                }

                //*** Medido
                using(var xConnecton = new SqlConnection(enlace.GetConnectionString())) {
                    xConnecton.Open();
                    var xCommand = new SqlCommand();
                    xCommand.Connection = xConnecton;
                    xCommand.CommandText = string.Format("EXEC [SICEM].[Recaudacion] 'POR_CONCEPTOS_TARIFAS_MEDIDOS','{0}','{1}',{2}, {3}", desde.ToString("yyyyMMdd"), hasta.ToString("yyyyMMdd"), sb, sect);
                    using(var xReader = xCommand.ExecuteReader()) {
                        while(xReader.Read()) {
                            var tmpResRow = new IngresosTipoUsuario {
                                Id_TipoCalculo = 1,
                                Id_Concepto = xReader.GetFieldValue<int>("Id_Concepto"),
                                Descripcion = xReader.GetFieldValue<string>("Descripcion"),

                                Dom_Sbt = xReader.GetFieldValue<decimal>("Sbt1"),
                                Dom_IVA = xReader.GetFieldValue<decimal>("IVA1"),
                                Dom_Tot = xReader.GetFieldValue<decimal>("Tot1"),

                                Hot_Sbt = xReader.GetFieldValue<decimal>("Sbt2"),
                                Hot_IVA = xReader.GetFieldValue<decimal>("IVA2"),
                                Hot_Tot = xReader.GetFieldValue<decimal>("Tot2"),

                                Com_Sbt = xReader.GetFieldValue<decimal>("Sbt3"),
                                Com_IVA = xReader.GetFieldValue<decimal>("IVA3"),
                                Com_Tot = xReader.GetFieldValue<decimal>("Tot3"),

                                Ind_Sbt = xReader.GetFieldValue<decimal>("Sbt4"),
                                Ind_IVA = xReader.GetFieldValue<decimal>("IVA4"),
                                Ind_Tot = xReader.GetFieldValue<decimal>("Tot4"),

                                Pub_Sbt = xReader.GetFieldValue<decimal>("Sbt5"),
                                Pub_IVA = xReader.GetFieldValue<decimal>("IVA5"),
                                Pub_Tot = xReader.GetFieldValue<decimal>("Tot5"),

                                Subtotal = xReader.GetFieldValue<decimal>("Subtotal"),
                                IVA = xReader.GetFieldValue<decimal>("IVA"),
                                Total = xReader.GetFieldValue<decimal>("Total")
                            };
                            respuesta.Add(tmpResRow);
                        }
                    }

                    xConnecton.Close();
                }

                //*** Promedio
                using(var xConnecton = new SqlConnection(enlace.GetConnectionString())) {
                    xConnecton.Open();
                    var xCommand = new SqlCommand();
                    xCommand.Connection = xConnecton;
                    xCommand.CommandText = string.Format("EXEC [SICEM].[Recaudacion] 'POR_CONCEPTOS_TARIFAS_PROMEDIOS','{0}','{1}',{2}, {3}", desde.ToString("yyyyMMdd"), hasta.ToString("yyyyMMdd"), sb, sect);
                    using(var xReader = xCommand.ExecuteReader()) {
                        while(xReader.Read()) {
                            var tmpResRow = new IngresosTipoUsuario {
                                Id_TipoCalculo = 2,
                                Id_Concepto = xReader.GetFieldValue<int>("Id_Concepto"),
                                Descripcion = xReader.GetFieldValue<string>("Descripcion"),

                                Dom_Sbt = xReader.GetFieldValue<decimal>("Sbt1"),
                                Dom_IVA = xReader.GetFieldValue<decimal>("IVA1"),
                                Dom_Tot = xReader.GetFieldValue<decimal>("Tot1"),

                                Hot_Sbt = xReader.GetFieldValue<decimal>("Sbt2"),
                                Hot_IVA = xReader.GetFieldValue<decimal>("IVA2"),
                                Hot_Tot = xReader.GetFieldValue<decimal>("Tot2"),

                                Com_Sbt = xReader.GetFieldValue<decimal>("Sbt3"),
                                Com_IVA = xReader.GetFieldValue<decimal>("IVA3"),
                                Com_Tot = xReader.GetFieldValue<decimal>("Tot3"),

                                Ind_Sbt = xReader.GetFieldValue<decimal>("Sbt4"),
                                Ind_IVA = xReader.GetFieldValue<decimal>("IVA4"),
                                Ind_Tot = xReader.GetFieldValue<decimal>("Tot4"),

                                Pub_Sbt = xReader.GetFieldValue<decimal>("Sbt5"),
                                Pub_IVA = xReader.GetFieldValue<decimal>("IVA5"),
                                Pub_Tot = xReader.GetFieldValue<decimal>("Tot5"),

                                Subtotal = xReader.GetFieldValue<decimal>("Subtotal"),
                                IVA = xReader.GetFieldValue<decimal>("IVA"),
                                Total = xReader.GetFieldValue<decimal>("Total")
                            };
                            respuesta.Add(tmpResRow);
                        }
                    }

                    xConnecton.Close();
                }

                //*** Fijo
                using(var xConnecton = new SqlConnection(enlace.GetConnectionString())) {
                    xConnecton.Open();
                    var xCommand = new SqlCommand();
                    xCommand.Connection = xConnecton;
                    xCommand.CommandText = string.Format("EXEC [SICEM].[Recaudacion] 'POR_CONCEPTOS_TARIFAS_FIJOS','{0}','{1}',{2}, {3}", desde.ToString("yyyyMMdd"), hasta.ToString("yyyyMMdd"), sb, sect);
                    using(var xReader = xCommand.ExecuteReader()) {
                        while(xReader.Read()) {
                            var tmpResRow = new IngresosTipoUsuario {
                                Id_TipoCalculo = 3,
                                Id_Concepto = xReader.GetFieldValue<int>("Id_Concepto"),
                                Descripcion = xReader.GetFieldValue<string>("Descripcion"),

                                Dom_Sbt = xReader.GetFieldValue<decimal>("Sbt1"),
                                Dom_IVA = xReader.GetFieldValue<decimal>("IVA1"),
                                Dom_Tot = xReader.GetFieldValue<decimal>("Tot1"),

                                Hot_Sbt = xReader.GetFieldValue<decimal>("Sbt2"),
                                Hot_IVA = xReader.GetFieldValue<decimal>("IVA2"),
                                Hot_Tot = xReader.GetFieldValue<decimal>("Tot2"),

                                Com_Sbt = xReader.GetFieldValue<decimal>("Sbt3"),
                                Com_IVA = xReader.GetFieldValue<decimal>("IVA3"),
                                Com_Tot = xReader.GetFieldValue<decimal>("Tot3"),

                                Ind_Sbt = xReader.GetFieldValue<decimal>("Sbt4"),
                                Ind_IVA = xReader.GetFieldValue<decimal>("IVA4"),
                                Ind_Tot = xReader.GetFieldValue<decimal>("Tot4"),

                                Pub_Sbt = xReader.GetFieldValue<decimal>("Sbt5"),
                                Pub_IVA = xReader.GetFieldValue<decimal>("IVA5"),
                                Pub_Tot = xReader.GetFieldValue<decimal>("Tot5"),

                                Subtotal = xReader.GetFieldValue<decimal>("Subtotal"),
                                IVA = xReader.GetFieldValue<decimal>("IVA"),
                                Total = xReader.GetFieldValue<decimal>("Total")
                            };
                            respuesta.Add(tmpResRow);
                        }
                    }

                    xConnecton.Close();
                }

                return respuesta.ToArray();

            }catch(Exception err) {
                logger.LogError(err, $">> Error al obtener los ingresos por tipo de usuario enlace:{enlace.Nombre}");
                return null;
            }
        }
        public IEnumerable<Ingresos_FormasPago> ObtenerIngresosPorFormasPago(IEnlace enlace, DateTime desde, DateTime hasta, int sb, int sect) {
            try {
                var _result = new List<Ingresos_FormasPago>();
                using(var xConnecton = new SqlConnection(enlace.GetConnectionString())) {
                    xConnecton.Open();
                    var _query = string.Format("Exec [WEB].[usp_Recaudacion] @cAlias='FORMA_PAGO', @xano=0, @xmes=0, @xfec1='{0}', @xfec2='{1}', @sb={2}, @sector={3}", desde.ToString("yyyyMMdd"), hasta.ToString("yyyyMMdd"), sb, sect);
                    using(var xCommand = new SqlCommand(_query, xConnecton)){
                        using(SqlDataReader reader = xCommand.ExecuteReader()) {
                            while(reader.Read()){
                                var _newItem = new Ingresos_FormasPago();
                                var tmpInt = 0;
                                var tmpDec = 0m;
                                _newItem.Orden = int.TryParse(reader["orden"].ToString(), out tmpInt) ? tmpInt : 0;
                                _newItem.Id = int.TryParse(reader["id"].ToString(), out tmpInt) ? tmpInt : 0;
                                _newItem.Forma_Pago = reader["forma_pago"].ToString().Trim();
                                _newItem.Cobrado = decimal.TryParse(reader["cobrado"].ToString(), out tmpDec) ? tmpDec : 0m;
                                _newItem.Arqueo = decimal.TryParse(reader["arqueo"].ToString(), out tmpDec) ? tmpDec : 0m;
                                _newItem.Diferencia = decimal.TryParse(reader["dif"].ToString(), out tmpDec) ? tmpDec : 0m;
                                _result.Add(_newItem);
                            }
                        }
                    }
                    xConnecton.Close();
                }
                return _result.ToArray();
            }catch(Exception err){
                logger.LogError(err, $">> Error al obtener ingresos por formas de pago enlace:{enlace.Nombre}");
                return null;
            }
        }
        public Recaudacion_PagosMayores_Response ObtenerPagosMayores(IEnlace enlace, DateTime desde, DateTime hasta, int sb, int sect, int total){
            try {
                var respuesta = new Recaudacion_PagosMayores_Response();
                var tmp_listaPagosMayores = new List<Recaudacion_PagosMayores>();
                var tmp_listaPagosM_Items = new List<Recaudacion_PagosMayores_Items>();
                var tmpDataSet = new DataSet();
                using(var xConnecton = new SqlConnection(enlace.GetConnectionString())) {
                    xConnecton.Open();
                    var xCommand = new SqlCommand();
                    xCommand.Connection = xConnecton;
                    xCommand.CommandText = string.Format("Exec [Sicem].[Recaudacion] @cAlias = 'TOP_VENTAS', @cFecha1 = '{0}', @cFecha2 = '{1}', @xParam = {2}", desde.ToString("yyyyMMdd"), hasta.ToString("yyyyMMdd"), total);
                    var xDataAdapter = new SqlDataAdapter(xCommand);
                    xDataAdapter.Fill(tmpDataSet);
                    xConnecton.Close();
                }

                if(tmpDataSet.Tables.Count < 2) {
                    Console.WriteLine($">> Error al obtener ObtenerPagosMayores \n\tError: El dataset no contiene 2 tablas.");
                    return null;
                }
                else {
                    foreach(DataRow xRow in tmpDataSet.Tables[0].Rows) {
                        var newItem = new Recaudacion_PagosMayores {
                            Id_Padron = int.Parse(xRow["id_padron"].ToString()),
                            Id_Cuenta = int.Parse(xRow["id_Cuenta"].ToString()),
                            Fecha = DateTime.Parse(xRow["fecha"].ToString()),
                            Subtotal = decimal.Parse(xRow["subtotal"].ToString()),
                            Iva = decimal.Parse(xRow["iva"].ToString()),
                            Total = decimal.Parse(xRow["total"].ToString()),
                            Sucursal = xRow["sucursal"].ToString(),
                            Id_Venta = xRow["id_venta"].ToString(),
                            Nombre = xRow["nombre"].ToString(),
                            Direccion = xRow["direccion"].ToString(),
                            Id_Publico = int.Parse(xRow["id_publico"].ToString())
                        };
                        tmp_listaPagosMayores.Add(newItem);
                    }
                    foreach(DataRow xRow in tmpDataSet.Tables[1].Rows) {
                        var newItem = new Recaudacion_PagosMayores_Items {
                            Id_Venta = xRow["id_venta"].ToString(),
                            Concepto = xRow["concepto"].ToString(),
                            Subtotal = decimal.Parse(xRow["subtotal"].ToString()),
                            Iva = decimal.Parse(xRow["iva"].ToString()),
                            Total = decimal.Parse(xRow["total"].ToString())
                        };
                        tmp_listaPagosM_Items.Add(newItem);
                    }
                    respuesta = new Recaudacion_PagosMayores_Response {
                        PagosMayores = tmp_listaPagosMayores.ToArray<Recaudacion_PagosMayores>(),
                        PagosMayores_Detalle = tmp_listaPagosM_Items.ToArray<Recaudacion_PagosMayores_Items>()
                    };
                }
                        
                return respuesta;

            }catch(Exception err){
                logger.LogError(err, $">> Error al obtener ObtenerPagosMayores \n\tError: {err.Message}\n\tStacktrace: {err.StackTrace}");
                return null;
            }
        }
        public IEnumerable<Recaudacion_IngresosDetalle> ObtenerDetalleIngresos(IEnlace enlace, DateTime desde, DateTime hasta, int sb, int sect){
            var respuesta = new List<Recaudacion_IngresosDetalle>();
            try {
                using(var xConnecton = new SqlConnection(enlace.GetConnectionString())) {
                    xConnecton.Open();
                    var xCommand = new SqlCommand();
                    xCommand.Connection = xConnecton;
                    xCommand.CommandText = string.Format("EXEC [SICEM].[Recaudacion] 'DETALLE','{0}','{1}'", desde.ToString("yyyyMMdd"), hasta.ToString("yyyyMMdd"));
                    using(var xReader = xCommand.ExecuteReader()) {
                        while(xReader.Read()) {
                            var tmpResRow = new Recaudacion_IngresosDetalle();
                            tmpResRow.Fecha = DateTime.Parse(xReader.GetValue("fecha").ToString());
                            tmpResRow.Folio = xReader.GetValue("folio").ToString();
                            tmpResRow.Cuenta = xReader.GetValue("cuenta").ToString();
                            tmpResRow.Usuario = xReader.GetValue("usuario").ToString();
                            tmpResRow.Cobrado = decimal.Parse(xReader.GetValue("cobrado").ToString());
                            tmpResRow.Caja = xReader.GetValue("caja").ToString();
                            tmpResRow.Fecha_aplicacion = DateTime.Parse(xReader.GetValue("fecha_aplicacion").ToString());
                            tmpResRow.Hrs_dif = int.Parse(xReader.GetValue("hrs_dif").ToString());
                            respuesta.Add(tmpResRow);
                        }
                    }
                    xConnecton.Close();
                }
            }catch(Exception err) {
                logger.LogError(err, $">> Error al consutlar el detalle de ingresos, enlace:{enlace.Nombre}\n\t{err.Message}");
                return null;
            }
            return respuesta.ToArray();
        }
        public IEnumerable<Recaudacion_IngresosDetalleConceptos> ObtenerDetalleConceptos(IEnlace enlace, DateTime desde, DateTime hasta, int sb, int sect, int id_poblacion, int id_colonia){
            var respuesta = new List<Recaudacion_IngresosDetalleConceptos>();
            //*** Rezago Total, xTipo = 0
            using(var xConnecton = new SqlConnection(enlace.GetConnectionString())) {
                xConnecton.Open();
                var xCommand = new SqlCommand();
                xCommand.Connection = xConnecton;
                xCommand.CommandText = string.Format("EXEC [SICEM].[Recaudacion] 'POR_POBLACION_COLONIA_CONCEPTOS','{0}','{1}',{2}, {3}, {4}, {5}", desde.ToString("yyyyMMdd"), hasta.ToString("yyyyMMdd"), sb, sect, id_poblacion, id_colonia);
                using(var xReader = xCommand.ExecuteReader()) {
                    while(xReader.Read()) {
                        var tmpResRow = new Recaudacion_IngresosDetalleConceptos();
                        tmpResRow.Descripcion = xReader.GetValue("Descripcion").ToString();
                        tmpResRow.Concepto_Con_Iva = decimal.Parse(xReader.GetValue("Conc_Con_Iva").ToString());
                        tmpResRow.Iva = decimal.Parse(xReader.GetValue("Iva").ToString());
                        tmpResRow.Aplicado_Con_Iva = decimal.Parse(xReader.GetValue("Aplicado_Con_Iva").ToString());
                        tmpResRow.Concepto_Sin_Iva = decimal.Parse(xReader.GetValue("Conc_Sin_Iva").ToString());
                        tmpResRow.Total_Aplicado = decimal.Parse(xReader.GetValue("Total_Aplicado").ToString());
                        tmpResRow.Usuarios = int.Parse(xReader.GetValue("Usuarios").ToString());
                        tmpResRow.Id_Concepto = int.Parse(xReader.GetValue("Id_Concepto").ToString());
                        respuesta.Add(tmpResRow);
                    }
                }
                xConnecton.Close();
            }
            return respuesta;
        }
        public IEnumerable<Recaudacion_Rezago_Detalle> ObtenerDetalleRezago(IEnlace enlace, DateTime desde, DateTime hasta, int sub, int sect, int mes, bool acumulativo){
            var respuesta = new List<Recaudacion_Rezago_Detalle>();
            try {
                using(var xConnecton = new SqlConnection(enlace.GetConnectionString())) {
                    xConnecton.Open();
                    var xCommand = new SqlCommand();
                    xCommand.Connection = xConnecton;
                    xCommand.CommandText = string.Format("EXEC [SICEM].[Recaudacion] 'INGRESOS_REZAGO_DETALLE', @cFecha1 = '{0}', @cFecha2 = '{1}', @xParam = {2}", desde.ToString("yyyyMMdd"), hasta.ToString("yyyyMMdd"), mes);
                    using(var xReader = xCommand.ExecuteReader()) {
                        while(xReader.Read()) {
                            var tmpResRow = new Recaudacion_Rezago_Detalle();
                            tmpResRow.Cuenta = xReader.GetValue("id_cuenta").ToString();
                            tmpResRow.Usuario = xReader.GetValue("razon_social").ToString();
                            tmpResRow.Tarifa = xReader.GetValue("_tipousuario").ToString();
                            tmpResRow.Id_Tarifa = int.Parse(xReader.GetValue("id_tarifa").ToString());
                            tmpResRow.Meses_Adeudo = int.Parse(xReader.GetValue("meses_adeudo").ToString());
                            tmpResRow.Agua = decimal.Parse(xReader.GetValue("rez_agua").ToString());
                            tmpResRow.Dren = decimal.Parse(xReader.GetValue("rez_dren").ToString());
                            tmpResRow.Trat = decimal.Parse(xReader.GetValue("rez_trat").ToString());
                            tmpResRow.Otros = decimal.Parse(xReader.GetValue("rez_otros").ToString());
                            tmpResRow.Recar = decimal.Parse(xReader.GetValue("rez_recar").ToString());
                            tmpResRow.Subtotal = decimal.Parse(xReader.GetValue("subtotal").ToString());
                            tmpResRow.Iva = decimal.Parse(xReader.GetValue("iva").ToString());
                            tmpResRow.Total = decimal.Parse(xReader.GetValue("total").ToString());
                            respuesta.Add(tmpResRow);
                        }
                    }
                    xConnecton.Close();
                }
                return respuesta.ToArray();
            }
            catch(Exception err) {
                logger.LogError(err, $">> Error al procesar la consulta, enlace:{enlace.Nombre}");
                return null;
            }
        }
        public IEnumerable<ConceptoTipoUsuario> ObtenerRecaudacionPorConceptosYTipoUsuario(IEnlace enlace, DateTime desde, DateTime hasta, int sub, int sect ){
            var _result = new List<ConceptoTipoUsuario>();
            try{
                using(var connection = new SqlConnection(enlace.GetConnectionString())){
                    connection.Open();
                    var _query = string.Format("Exec [WEB].[usp_Recaudacion] @cAlias = 'CONCEPTOS_TARIFAS', @xano = {0}, @xmes = {1}, @xfec1 = '{2}', @xfec2 = '{3}', @sb = {4}, @sector = {5}", 0, 0, desde.ToString("yyyyMMdd"), hasta.ToString("yyyyMMdd"), sub, sect);
                    
                    var _command = new SqlCommand(_query, connection);
                    using(SqlDataReader reader = _command.ExecuteReader()){
                        while(reader.Read()){
                            _result.Add(ConceptoTipoUsuario.FromSqlReader(reader));
                        }
                    }
                    
                    // Actualizar el tipo Concepto
                    var _catTiposConceptos = ObtenerCatalogoTiposConceptos(enlace);
                    foreach(var c in _result){
                        if(_catTiposConceptos.ContainsKey(c.Id_Tipo)){
                            c.TipoConcepto = _catTiposConceptos[c.Id_Tipo];
                        }else{
                            c.TipoConcepto = "--INDEFINIDO--";
                        }
                    }
                    connection.Close();
                }


                // Agregar fila total a cada grupo
                var idTipos = _result.Where(item => item.Id_Tipo > 0).GroupBy( item => item.Id_Tipo).Select(g => g.Key).ToList();
                foreach(int id in idTipos){
                    var _tmpData = _result.Where(item => item.Id_Tipo == id);
                    var conceptoTipoUsuario = new ConceptoTipoUsuario();
                    conceptoTipoUsuario.Id_Concepto = 999;
                    conceptoTipoUsuario.Descripcion = $"Total {_tmpData.First().TipoConcepto}".ToUpper();
                    conceptoTipoUsuario.DomesticoSubTot = _tmpData.Sum(item => item.DomesticoSubTot);
                    conceptoTipoUsuario.DomesticoIVA = _tmpData.Sum(item => item.DomesticoIVA);
                    conceptoTipoUsuario.DomesticoUsu = _tmpData.Sum(item => item.DomesticoUsu);
                    conceptoTipoUsuario.HoteleroSubTot = _tmpData.Sum(item => item.HoteleroSubTot);
                    conceptoTipoUsuario.HoteleroIVA = _tmpData.Sum(item => item.HoteleroIVA);
                    conceptoTipoUsuario.HoteleroUsu = _tmpData.Sum(item => item.HoteleroUsu);
                    conceptoTipoUsuario.ComercialSubTot = _tmpData.Sum(item => item.ComercialSubTot);
                    conceptoTipoUsuario.ComercialIVA = _tmpData.Sum(item => item.ComercialIVA);
                    conceptoTipoUsuario.ComercialUsu = _tmpData.Sum(item => item.ComercialUsu);
                    conceptoTipoUsuario.IndustrialSubTot = _tmpData.Sum(item => item.IndustrialSubTot);
                    conceptoTipoUsuario.IndustrialIVA = _tmpData.Sum(item => item.IndustrialIVA);
                    conceptoTipoUsuario.IndustrialUsu = _tmpData.Sum(item => item.IndustrialUsu);
                    conceptoTipoUsuario.GeneralIVA = _tmpData.Sum(item => item.GeneralIVA);
                    conceptoTipoUsuario.GeneralSubTot = _tmpData.Sum(item => item.GeneralSubTot);
                    conceptoTipoUsuario.GeneralUsu = _tmpData.Sum(item => item.GeneralUsu);
                    conceptoTipoUsuario.Subtotal = _tmpData.Sum(item => item.Subtotal);
                    conceptoTipoUsuario.IVA = _tmpData.Sum(item => item.IVA);
                    conceptoTipoUsuario.Total = _tmpData.Sum(item => item.Total);
                    conceptoTipoUsuario.Usuarios = _tmpData.Sum(item => item.Usuarios);
                    conceptoTipoUsuario.Id_Tipo = _tmpData.First().Id_Tipo;
                    conceptoTipoUsuario.TipoConcepto = _tmpData.First().TipoConcepto;
                    _result.Add(conceptoTipoUsuario);
                }


                return _result;
            }catch(Exception err){
                logger.LogError(err, " Error al tratar de obtener los ingresos por conceptos y tipos de usuarios del enlace: " + enlace.Nombre);
                return null;
            }
        }

        public IEnumerable<RecaudacionIngresosxPoblaciones> ObtenerRecaudacionLocalidades(IEnlace enlace, DateTime desde, DateTime hasta, int sb, int sec){
            var result = new List<RecaudacionIngresosxPoblaciones>();
            try{
                using(var sqlConnection = new SqlConnection(enlace.GetConnectionString())){
                    sqlConnection.Open();
                    var _query =$"Exec [Sicem_QRoo].[Ingresos_05] @xAlias = 'LOCALIDADES', @xfec1 = '{desde.ToString("yyyyMMdd")}', @xfec2 ={hasta.ToString("yyyyMMdd")}, @xSb = {sb}, @xSec = {sec}";
                    var _command = new SqlCommand(_query, sqlConnection);
                    _command.CommandTimeout = (int)TimeSpan.FromMinutes(15).TotalSeconds;
                    using( var reader = _command.ExecuteReader()){
                        while(reader.Read()){
                            result.Add( RecaudacionIngresosxPoblaciones.FromDataReader(reader));
                        }
                    }
                    sqlConnection.Close();
                }
                return result;

            }catch(Exception err){
                logger.LogError(err, $"Error al obtener la facturacion por localidades de {enlace.Nombre}");
                return new List<RecaudacionIngresosxPoblaciones>();
            }
        }

        public IEnumerable<RecaudacionIngresosxColonias> ObtenerRecaudacionColonias(IEnlace enlace, DateTime desde, DateTime hasta, int sb, int sec, int idLocalidad){
            var result = new List<RecaudacionIngresosxColonias>();
            try{
                using(var sqlConnection = new SqlConnection(enlace.GetConnectionString())){
                    sqlConnection.Open();
                    var _query =$"Exec [Sicem_QRoo].[Ingresos_05] @xAlias = 'COLONIAS', @xfec1 = '{desde.ToString("yyyyMMdd")}', @xfec2 ={hasta.ToString("yyyyMMdd")}, @xSb = {sb}, @xSec = {sec}, @nParam = {idLocalidad}";
                    var _command = new SqlCommand(_query, sqlConnection);
                    _command.CommandTimeout = (int)TimeSpan.FromMinutes(15).TotalSeconds;
                    using( var reader = _command.ExecuteReader()){
                        while(reader.Read()){
                            result.Add( RecaudacionIngresosxColonias.FromDataReader(reader));
                        }
                    }
                    sqlConnection.Close();
                }
                return result;

            }catch(Exception err){
                logger.LogError(err, $"Error al obtener la facturacion por localidades de {enlace.Nombre}");
                return new List<RecaudacionIngresosxColonias>();
            }
        }
        public IEnumerable<Recaudacion_IngresosDetalleConceptos> ObtenerIngresosConceptosPorLocalidadColonia(IEnlace enlace, DateTime desde, DateTime hasta, int sb, int sect, int idLocalidad, int idColonia){
            var result = new List<Recaudacion_IngresosDetalleConceptos>();

            var tmpIdLocalidad = idLocalidad>=999?0:idLocalidad;
            var tmpIdColonia = idColonia>=999?0:idColonia;
            try{
                using(var sqlConnection = new SqlConnection(enlace.GetConnectionString())){
                    sqlConnection.Open();
                    var _query =$"Exec [SICEM_QROO].[Ingresos_06] @xfec1 = '{desde.ToString("yyyyMMdd")}', @xfec2 = '{hasta.ToString("yyyyMMdd")}', @xLocalidad = {idLocalidad}, @xColonia = {tmpIdColonia}";
                    var _command = new SqlCommand(_query, sqlConnection);
                    _command.CommandTimeout = (int)TimeSpan.FromMinutes(15).TotalSeconds;
                    using( var reader = _command.ExecuteReader()){
                        while(reader.Read()){
                            result.Add( Recaudacion_IngresosDetalleConceptos.FromSqlDataReader(reader));
                        }
                    }
                    sqlConnection.Close();
                }
                return result;

            }catch(Exception err){
                logger.LogError(err, $"Error al obtener la facturacion por localidades de {enlace.Nombre}");
                return new List<Recaudacion_IngresosDetalleConceptos>();
            }
        }

        private Dictionary<int, string> ObtenerCatalogoTiposConceptos(IEnlace enlace){
            var _result = new Dictionary<int,string>();
            _result.Add(0, "z--*--");
            using(var connection = new SqlConnection(enlace.GetConnectionString())){
                connection.Open();
                var _query = "Select id_TipoConcepto as id, descripcion From [Padron].[Cat_TiposConcepto] Where id_TipoConcepto > 0";
                var _command = new SqlCommand(_query, connection);
                using(var reader = _command.ExecuteReader()){
                    while(reader.Read()){
                        _result.Add(ConvertUtils.ParseInteger(reader["id"].ToString()), reader["descripcion"].ToString().ToUpper());
                    }
                }
                connection.Close();
            }
            return _result;
        }

    }
}
