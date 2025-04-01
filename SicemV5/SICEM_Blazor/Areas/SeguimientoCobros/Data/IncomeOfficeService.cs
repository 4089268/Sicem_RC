using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SICEM_Blazor.Data;
using SICEM_Blazor.SeguimientoCobros.Models;
using SICEM_Blazor.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;


namespace SICEM_Blazor.SeguimientoCobros.Data {

    public class IncomeOfficeService {
        private readonly SicemContext sicemContext;
        private readonly ILogger<IncomeOfficeService> logger;
        private readonly TimeSpan lifeTimeCancelation = TimeSpan.FromSeconds(12);

        public IncomeOfficeService(SicemContext context, ILogger<IncomeOfficeService> logger){
            this.sicemContext = context;
            this.logger = logger;
        }


        /// <summary>
        /// return the coordinates of the office
        /// </summary>
        /// <param name="enlace"></param>
        /// <returns></returns>
        /// <exception cref="TimeoutException"></exception>
        public OfficePushpinMap GetPushpinOfOffice(IEnlace enlace)
        {
            try
            {
                var officesPushpin = new OfficePushpinMap(enlace.Id, enlace.Nombre);

                // * get the location of the office
                using(var sqlConnection = new SqlConnection(this.sicemContext.Database.GetConnectionString()))
                {
                    sqlConnection.Open();
                    var command = new SqlCommand("Select latitude, longitude From [dbo].[RutasLocation] where id_ruta = @rutaId", sqlConnection);
                    command.Parameters.AddWithValue("@rutaId", enlace.Id);
                    using var reader = command.ExecuteReader();
                    if(reader.Read())
                    {
                        officesPushpin.Lat = Convert.ToDouble(reader["latitude"]);
                        officesPushpin.Lon = Convert.ToDouble(reader["longitude"]);
                    }
                    sqlConnection.Close();
                }

                // TODO: Get the income office
                // try
                // {
                //     var data = this.GetIncomes(enlace);
                //     officesPushpin.Bills = data.Where( item=> item.Id < 900).Sum( item => item.Bills);
                //     officesPushpin.Income = data.Where( item=> item.Id < 900).Sum( item => item.Income);
                // }
                // catch(Exception err)
                // {
                //     this.logger.LogError(err, "Error al obtner los datos");
                // }

                return officesPushpin;
            }
            catch(OperationCanceledException)
            {
                throw new TimeoutException("Operation timed out after 6 seconds.");
            }

        }

        public ICollection<OfficePushpinMap> GetIncomes(IEnlace enlace){
            var results = new List<OfficePushpinMap>();
            using(var sqlConnection = new SqlConnection(enlace.GetConnectionString()) ){
                sqlConnection.Open();
                var command = new SqlCommand( "[SICEM].[usp_Cobranza_en_vivo]", sqlConnection);
                command.CommandType = CommandType.StoredProcedure;
                using(SqlDataReader reader = command.ExecuteReader()){
                    while(reader.Read()){
                        var id = ConvertUtils.ParseInteger( reader["id_sucursal"].ToString() );
                        var oficina = reader["sucursal"].ToString();
                        results.Add( new OfficePushpinMap(id, oficina){
                            Lat = Convert.ToDouble(reader["latitud"]),
                            Lon = Convert.ToDouble(reader["longitud"]),
                            Bills = ConvertUtils.ParseInteger( reader["recibos"].ToString() ),
                            Income = ConvertUtils.ParseDecimal( reader["cobrado"].ToString() ),
                        });
                    }
                }
                sqlConnection.Close();
            }
            // return results;
            return results;
        }

        /// <summary>
        /// consulta el ingreso por cajas dividido en horarios de una sucursal
        /// </summary>
        /// <param name="enalce"></param>
        /// <param name="sucursal"></param>
        /// <returns></returns>
        public ICollection<IngresoHorario> IngresosPorHorario(IEnlace enlace, Sicem_Sucursal sucursal){
            try{

                var response = new List<IngresoHorario>();
                using( var conexion = new SqlConnection(enlace.GetConnectionString())){
                    conexion.Open();
                    var command = new SqlCommand("[SICEM].[usp_cobros_horario]", conexion)
                    {
                        CommandType = CommandType.StoredProcedure,
                        CommandTimeout = (int)TimeSpan.FromMinutes(10).TotalSeconds
                    };
                    command.Parameters.AddWithValue("@id_sucursal", sucursal.Id_Sucursal);
                    using(SqlDataReader reader = command.ExecuteReader()){
                        while(reader.Read()){
                            
                            var id = reader["id_cajero"].ToString();
                            var caja = reader["caja"].ToString();

                            var ingresoHorario = new IngresoHorario( sucursal.Id_Sucursal, sucursal.Descripcion, id, caja){
                                Recibos = ConvertUtils.ParseInteger( reader["recibos"].ToString() ),
                                Cobrado = ConvertUtils.ParseDecimal( reader["cobrado"].ToString() ),
                                Hora0708 = ConvertUtils.ParseDecimal( reader["07-08"].ToString() ),
                                Hora0809 = ConvertUtils.ParseDecimal( reader["08-09"].ToString() ),
                                Hora0910 = ConvertUtils.ParseDecimal( reader["09-10"].ToString() ),
                                Hora1011 = ConvertUtils.ParseDecimal( reader["10-11"].ToString() ),
                                Hora1112 = ConvertUtils.ParseDecimal( reader["11-12"].ToString() ),
                                Hora1213 = ConvertUtils.ParseDecimal( reader["12-13"].ToString() ),
                                Hora1314 = ConvertUtils.ParseDecimal( reader["13-14"].ToString() ),
                                Hora1415 = ConvertUtils.ParseDecimal( reader["14-15"].ToString() ),
                                Hora1516 = ConvertUtils.ParseDecimal( reader["15-16"].ToString() ),
                                Hora1617 = ConvertUtils.ParseDecimal( reader["16-17"].ToString() ),
                                Hora1718 = ConvertUtils.ParseDecimal( reader["17-18"].ToString() ),
                                Hora1819 = ConvertUtils.ParseDecimal( reader["18-19"].ToString() ),
                                Hora1920 = ConvertUtils.ParseDecimal( reader["19-20"].ToString() )
                            };

                            response.Add(ingresoHorario);
                        }
                    }
                    conexion.Close();
                }
                return response;

            }catch(Exception err){
                this.logger.LogError(err, "Error al obtener ingresos por cajas del enlace {enlace}", enlace.Nombre);
                return null;
            }
        }

    }
}