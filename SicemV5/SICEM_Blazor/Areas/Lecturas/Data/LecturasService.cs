using System;
using System.Data;
using System.Collections.Generic;
using System.Data.SqlClient;
using SICEM_Blazor.Data;
using SICEM_Blazor.Lecturas.Models;
using Microsoft.Extensions.Logging;

namespace SICEM_Blazor.Lecturas.Data {

    public class LecturasService {

        private readonly ILogger<LecturasService> logger;

        public LecturasService(ILogger<LecturasService> logger){
            this.logger = logger;
        }

        public IEnumerable<Lecturista> ObtenerLecturasPorLecturisa(DateRange dateRange, IEnlace enlace){
            logger.LogInformation("Obteniendo lecturas del enlace {enlace}", enlace.Nombre );
            var response = new List<Lecturista>();
            using(var sqlConnection = new SqlConnection(enlace.GetConnectionString())){
                sqlConnection.Open();
                var query = StoredProcedures.LECTURASXLECTURISTAS.Replace("@dFecha", dateRange.Desde_ISO);
                var sqlCommand = new SqlCommand(query, sqlConnection);
                using(SqlDataReader reader = sqlCommand.ExecuteReader()){
                    while(reader.Read()){
                        response.Add( new Lecturista(){
                            IdLecturista = ConvertUtils.ParseInteger(reader["id_lecturista"].ToString()),
                            Nombre = reader["lecturista"].ToString(),
                            Inicio = reader.GetDateTime("inicio"),
                            Fin = reader.GetDateTime("termino"),
                            TotalLecturas = ConvertUtils.ParseInteger(reader["lecturas"].ToString()),
                            H0708 = ConvertUtils.ParseInteger(reader["07-08"].ToString()),
                            H0809 = ConvertUtils.ParseInteger(reader["08-09"].ToString()),
                            H0910 = ConvertUtils.ParseInteger(reader["09-10"].ToString()),
                            H1011 = ConvertUtils.ParseInteger(reader["10-11"].ToString()),
                            H1112 = ConvertUtils.ParseInteger(reader["11-12"].ToString()),
                            H1213 = ConvertUtils.ParseInteger(reader["12-13"].ToString()),
                            H1314 = ConvertUtils.ParseInteger(reader["13-14"].ToString()),
                            H1415 = ConvertUtils.ParseInteger(reader["14-15"].ToString()),
                            H1516 = ConvertUtils.ParseInteger(reader["15-16"].ToString()),
                            H1617 = ConvertUtils.ParseInteger(reader["16-17"].ToString()),

                        });
                    }
                }
                sqlConnection.Close();
            }
            logger.LogInformation("Consulta lecturas enlace {enlace} concluido", enlace.Nombre);
            return response;
        }

        public IEnumerable<Incidencia> ObtenerIncidencias(DateRange dateRange, IEnlace enlace){
            logger.LogInformation("Obteniendo incidencias del enlace {enlace}", enlace.Nombre );
            var response = new List<Incidencia>();
            using(var sqlConnection = new SqlConnection(enlace.GetConnectionString())){
                sqlConnection.Open();
                var query = StoredProcedures.RESUMENINCIDENCIAS.Replace("@desde", dateRange.Desde_ISO).Replace("@hasta", dateRange.Hasta_ISO);
                var sqlCommand = new SqlCommand(query, sqlConnection);
                using(SqlDataReader reader = sqlCommand.ExecuteReader()){
                    while(reader.Read()){
                        response.Add( new Incidencia(){
                            Cuenta = (int) ConvertUtils.ParseInteger(reader["cuenta"].ToString()),
                            Localizacion = reader["localizacion"].ToString(),
                            Usuario = reader["usuario"].ToString(),
                            Lecturista = reader["lecturista"].ToString(),
                            Lectura = reader["lectura"].ToString(),
                            Anomalia = reader["anomalia"].ToString(),
                            Descripcion = reader["incidencia"].ToString(),
                            Fecha = reader.GetDateTime("fecha"),
                            Handheld = reader["handheld"].ToString()
                        });
                    }
                }
                sqlConnection.Close();
            }
            logger.LogInformation("Consulta incidencias lecturas enlace {enlace} concluido", enlace.Nombre);
            return response;
        }

    }


    public class StoredProcedures {

        public static string LECTURASXLECTURISTAS = @" Select l.id_lecturista as id_lecturista,
            p._descripcion as lecturista
            ,min(l.fecha)  as inicio
            ,max(l.fecha)  as termino
            ,COUNT(l.id_padron)	as lecturas
            ,sum(case when DATENAME(HOUR, l.fecha)=7 then 1 else 0 end) as [07-08]
            ,sum(case when DATENAME(HOUR, l.fecha)=8 then 1 else 0 end) as [08-09]
            ,sum(case when DATENAME(HOUR, l.fecha)=9 then 1 else 0 end) as [09-10]
            ,sum(case when DATENAME(HOUR, l.fecha)=10 then 1 else 0 end) as [10-11]
            ,sum(case when DATENAME(HOUR, l.fecha)=11 then 1 else 0 end) as [11-12]
            ,sum(case when DATENAME(HOUR, l.fecha)=12 then 1 else 0 end) as [12-13]
            ,sum(case when DATENAME(HOUR, l.fecha)=13 then 1 else 0 end) as [13-14]
            ,sum(case when DATENAME(HOUR, l.fecha)=14 then 1 else 0 end) as [14-15]
            ,sum(case when DATENAME(HOUR, l.fecha)=15 then 1 else 0 end) as [15-16]
            ,sum(case when DATENAME(HOUR, l.fecha)=16 then 1 else 0 end) as [16-17]
        From Facturacion.opr_lecturas				l  With(NoLock) 
            Inner Join Nomina.Cat_Personal			p  With(NoLock) On p.id_personal=l.id_lecturista
        Where Convert(VarChar(8),l.fecha,112)= '@dFecha' 
        Group by id_lecturista,p._descripcion ";
    
        public static string RESUMENINCIDENCIAS = @" Select 
            pp.id_cuenta	as cuenta,
            pp.__sl__localizacion as localizacion,
            pp.razon_social as usuario,
            p._descripcion  as lecturista,
            l.lectura		as lectura,
            a.descripcion	as anomalia,
            l.observacion	as incidencia,
            l.fecha,
            l.handheld
        From Facturacion.opr_lecturas				l  With(NoLock) 
            Inner Join Padron.Cat_Padron			pp With(NoLock) On pp.id_padron=l.id_padron
            Inner Join Nomina.Cat_Personal			p  With(NoLock) On p.id_personal=l.id_lecturista
            Inner Join Facturacion.Cat_Anomalias	a  With(Nolock) On l.id_anomalia=a.id_anomalia 
        --	Inner Join Global.Sys_Usuarios			u  With(Nolock) On l.id_capturo=u.id_usuario
        Where Convert(VarChar(8),l.fecha,112) between '@desde' and '@hasta' And l.observacion!='' ";
    }
    
}