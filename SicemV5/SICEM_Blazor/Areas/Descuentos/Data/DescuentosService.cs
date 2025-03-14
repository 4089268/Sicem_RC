using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SICEM_Blazor.Data;
using SICEM_Blazor.Models;
using SICEM_Blazor.Recaudacion.Models;
using SICEM_Blazor.Descuentos.Models;
using Microsoft.Extensions.Logging;

namespace SICEM_Blazor.Services {
    public class DescuentosService {
        private readonly IConfiguration appSettings;
        private readonly SicemService sicemService;
        private readonly ILogger<DescuentosService> logger;
        public DescuentosService(IConfiguration c, SicemService s, ILogger<DescuentosService> l) {
            this.appSettings = c;
            this.sicemService = s;
            this.logger = l;
        }

        
        public Descuentos_Resumen ObtenerDescuentosResumen(IEnlace enlace, DateTime fecha1, DateTime fecha2)
        {
            throw new NotImplementedException();
            // var respuesta = new Descuentos_Resumen();

            // try {
            //     using(var xConnecton = new SqlConnection(enlace.GetConnectionString())) {
            //         xConnecton.Open();
            //         using(var xCommand = new SqlCommand()) {
            //             xCommand.Connection = xConnecton;
            //             xCommand.CommandText = $"EXEC [SICEM].[Descuentos] 'RESUMEN','{fecha1.ToString("yyyyMMdd")}','{fecha2.ToString("yyyyMMdd")}'";
            //             var tmpRes = new List<IngresosDia>();
            //             var tmpDataSet = new DataSet();
            //             new SqlDataAdapter(xCommand).Fill(tmpDataSet);
            //             //****** Resumen 
            //             var xRow = tmpDataSet.Tables[0].Rows[0];
            //             respuesta.Conc_Con_Iva = decimal.Parse(xRow["Conc_Con_Iva"].ToString());
            //             respuesta.Iva = decimal.Parse(xRow["Iva"].ToString());
            //             respuesta.Apli_Con_Iva = decimal.Parse(xRow["Aplicado_Con_Iva"].ToString());
            //             respuesta.Conc_Sin_Iva = decimal.Parse(xRow["Conc_Sin_Iva"].ToString());
            //             respuesta.Total = decimal.Parse(xRow["Total_Aplicado"].ToString());
            //             respuesta.Usuarios = int.Parse(xRow["Usuarios"].ToString());

            //             var tmpList = new List<string>();
            //             tmpList.Add($"Agua;{decimal.Parse(xRow["Agua"].ToString())}");
            //             tmpList.Add($"Drenaje;{decimal.Parse(xRow["Dren"].ToString())}");
            //             tmpList.Add($"Saneamiento;{decimal.Parse(xRow["Sane"].ToString())}");
            //             tmpList.Add($"Rezago Agua;{decimal.Parse(xRow["Rez_Agua"].ToString())}");
            //             tmpList.Add($"Rezago Drenaje;{decimal.Parse(xRow["Rez_Dren"].ToString())}");
            //             tmpList.Add($"Rezago Saneamiento;{decimal.Parse(xRow["Rez_Sane"].ToString())}");
            //             tmpList.Add($"Recargos;{decimal.Parse(xRow["Recargos"].ToString())}");
            //             tmpList.Add($"Conexion;{decimal.Parse(xRow["Conexion"].ToString())}");
            //             tmpList.Add($"Reconexion;{decimal.Parse(xRow["Reconec"].ToString())}");
            //             tmpList.Add($"Otros;{decimal.Parse(xRow["Otros"].ToString())}");
            //             respuesta.Conceptos = tmpList.ToArray<string>();

            //             //****** Agrupar por Tarifas
            //             var xList1 = new List<Descuentos_Resumen_Item>();
            //             foreach(DataRow xr in tmpDataSet.Tables[1].Rows) {
            //                 xList1.Add(
            //                     new Descuentos_Resumen_Item {
            //                         Id = int.Parse(xr["id_tarifa"].ToString()),
            //                         Descripcion = xr["descrip"].ToString(),
            //                         Total = decimal.Parse(xr["total"].ToString()),
            //                         NTotal = int.Parse(xr["c"].ToString())
            //                     }
            //                 );
            //             }
            //             respuesta.Tarifas = xList1.ToArray<Descuentos_Resumen_Item>();

            //             //****** Agrupar por Calculos
            //             var xList2 = new List<Descuentos_Resumen_Item>();
            //             foreach(DataRow xr in tmpDataSet.Tables[2].Rows) {
            //                 xList2.Add(
            //                     new Descuentos_Resumen_Item {
            //                         Id = int.Parse(xr["id_calculo"].ToString()),
            //                         Descripcion = xr["descripcion"].ToString(),
            //                         Total = decimal.Parse(xr["total"].ToString()),
            //                         NTotal = int.Parse(xr["c"].ToString())
            //                     }
            //                 );
            //             }
            //             respuesta.Calculos = xList2.ToArray<Descuentos_Resumen_Item>();

            //         }
            //     }
            //     return respuesta;
            // }catch(Exception err){
            //     logger.LogError(err, $">> Erro al obtener el resumen de descuentos de {enlace.Nombre}");
            //     return null;
            // }
        }
        public Descuentos_Totales ObtenerDescuentosOficina(IEnlace enlace, DateTime fecha1, DateTime fecha2, int sb, int sect) {
            var response = new Descuentos_Totales();
            response.Enlace = enlace;
            try {
                using(var xConnecton = new SqlConnection(enlace.GetConnectionString())) {
                    xConnecton.Open();
                    var xCommand = new SqlCommand();
                    xCommand.Connection = xConnecton;
                    xCommand.CommandText = string.Format("EXEC [SICEM].[Descuentos] 'SOLO_TOTAL','{0}','{1}'", fecha1.ToString("yyyyMMdd"), fecha2.ToString("yyyyMMdd"));
                    using(var xReader = xCommand.ExecuteReader()) {
                        if(xReader.Read()) {
                            decimal tmpdata = 0m;
                            response.Conc_Con_Iva = decimal.TryParse(xReader["Conc_Con_Iva"].ToString(), out tmpdata) ? tmpdata : 0m;
                            response.Iva = decimal.TryParse(xReader["Iva"].ToString(), out tmpdata) ? tmpdata : 0m;
                            response.Aplicado_Con_Iva = decimal.TryParse(xReader["Aplicado_Con_Iva"].ToString(), out tmpdata) ? tmpdata : 0m;
                            response.Conc_Sin_Iva = decimal.TryParse(xReader["Conc_Sin_Iva"].ToString(), out tmpdata) ? tmpdata : 0m;
                            response.Total_Aplicado = decimal.TryParse(xReader["Total_Aplicado"].ToString(), out tmpdata) ? tmpdata : 0m;
                            response.Usuarios = int.TryParse(xReader["Usuarios"].ToString(), out int tmpData2) ? tmpData2 : 0;
                        }
                    }
                }
                response.Estatus = Data.Contracts.ResumenOficinaEstatus.Completado;
            }
            catch(Exception err) {
                response.Estatus = Data.Contracts.ResumenOficinaEstatus.Error;
                logger.LogError(err, $">> Error al obtener descuentos oficina {enlace.Id} {enlace.Nombre}");
            }
            return response;
        }

        public IEnumerable<Descuentos_Conceptos> ObtenerDescuentosConceptos(IEnlace enlace, DateTime fecha1, DateTime fecha2) {
            var respuesta = new List<Descuentos_Conceptos>();
            using(var xConnecton = new SqlConnection(enlace.GetConnectionString() )) {
                xConnecton.Open();
                using(var xCommand = new SqlCommand()) {
                    xCommand.Connection = xConnecton;
                    xCommand.CommandText = $"EXEC [SICEM].[Descuentos] 'POR_CONCEPTOS','{fecha1.ToString("yyyyMMdd")}','{fecha2.ToString("yyyyMMdd")}'";
                    using(var xReader = xCommand.ExecuteReader()) {
                        while(xReader.Read()) {
                            try {
                                var tmpItem = new Descuentos_Conceptos();
                                tmpItem.Id_Concepto = int.Parse(xReader.GetValue("Id_Concepto").ToString());
                                tmpItem.Descripcion = xReader.GetValue("Descripcion").ToString();
                                tmpItem.Conc_Con_Iva = decimal.Parse(xReader.GetValue("Conc_Con_Iva").ToString());
                                tmpItem.Iva = decimal.Parse(xReader.GetValue("Iva").ToString());
                                tmpItem.Aplicado_Con_Iva = decimal.Parse(xReader.GetValue("Aplicado_Con_Iva").ToString());
                                tmpItem.Conc_Sin_Iva = decimal.Parse(xReader.GetValue("Conc_Sin_Iva").ToString());
                                tmpItem.Total_Aplicado = decimal.Parse(xReader.GetValue("Total_Aplicado").ToString());
                                tmpItem.Usuarios = int.Parse(xReader.GetValue("Usuarios").ToString());
                                respuesta.Add(tmpItem);
                            }
                            catch(Exception) { }
                        }
                    }
                }
            }
            return respuesta.ToArray();
        }
        public IEnumerable<Descuentos_Autorizo> ObtenerDescuentosAutorizo(IEnlace enalce, DateTime fecha1, DateTime fecha2) {
            var respuesta = new List<Descuentos_Autorizo>();            
            using(var xConnecton = new SqlConnection(enalce.GetConnectionString() )) {
                xConnecton.Open();
                using(var xCommand = new SqlCommand()) {
                    xCommand.Connection = xConnecton;
                    xCommand.CommandText = $"EXEC [SICEM].[Descuentos] 'POR_AUTORIZO_GLOBAL','{fecha1.ToString("yyyyMMdd")}','{fecha2.ToString("yyyyMMdd")}'";

                    using(var xReader = xCommand.ExecuteReader()) {
                        while(xReader.Read()) {
                            respuesta.Add(new Descuentos_Autorizo {
                                CVE = xReader.GetString("CVE"),
                                Autorizo = xReader.GetString("AUTORIZO").ToString(),
                                Subtotal = decimal.Parse(xReader.GetValue("SUBTOTAL").ToString()),
                                Iva = decimal.Parse(xReader.GetValue("IVA").ToString()),
                                Total = decimal.Parse(xReader.GetValue("TOTAL").ToString()),
                                Usuarios = int.Parse(xReader.GetValue("USUARIOS").ToString())
                            });
                        }
                    }
                }
            }
            return respuesta.ToArray();
        }
        public Descuentos_Autorizo_Detalle[] ObtenerDescuentosAutorizoDetalle(IEnlace enlace, DateTime fecha1, DateTime fecha2, string cve) {
            var respuesta = new List<Descuentos_Autorizo_Detalle>();
            using(var xConnecton = new SqlConnection(enlace.GetConnectionString() )) {
                xConnecton.Open();
                using(var xCommand = new SqlCommand()) {
                    xCommand.Connection = xConnecton;
                    xCommand.CommandText = $"EXEC [SICEM].[Descuentos] 'POR_AUTORIZO_DETALLE' ,'{fecha1.ToString("yyyyMMdd")}','{fecha2.ToString("yyyyMMdd")}', '{cve}'";
                    using(var xReader = xCommand.ExecuteReader()) {
                        while(xReader.Read()) {
                            respuesta.Add(new Descuentos_Autorizo_Detalle {
                                Id_Abono = xReader.GetValue("id_abono").ToString(),
                                Cuenta = long.Parse(xReader.GetValue("CUENTA").ToString()),
                                Usuarios = xReader.GetValue("USUARIO").ToString(),
                                Colonia = "",
                                Tipo_Usuario = "",
                                Fecha = DateTime.Parse(xReader.GetValue("FECHA").ToString()),
                                Cve = xReader.GetValue("CVE").ToString(),
                                Autorizo = xReader.GetValue("AUTORIZO").ToString(),
                                Justifica = xReader.GetValue("Justifica").ToString(),
                                Agua = 0m,
                                Drenaje = 0m,
                                Saneamiento = 0m,
                                Rez_Agua = 0m,
                                Rez_Drenaje = 0m,
                                Rez_Saneamiento = 0m,
                                Otros = 0m,
                                Recargos = 0m,
                                Subtotal = decimal.Parse(xReader.GetValue("SUBTOTAL").ToString()),
                                Iva = decimal.Parse(xReader.GetValue("IVA").ToString()),
                                Total = decimal.Parse(xReader.GetValue("TOTAL").ToString())
                            });
                        }
                    }
                }
            }
            return respuesta.ToArray();
        }

    }
}
