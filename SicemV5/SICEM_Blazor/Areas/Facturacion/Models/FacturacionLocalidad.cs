using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using SICEM_Blazor.Data;

namespace SICEM_Blazor.Facturacion.Models {
    
    public class FacturacionLocalidad {
        public int IdLocalidad {get; set;}
        public string Localidad {get; set;}
        public decimal Agua {get; set;}
        public decimal Drenaje {get; set;}
        public decimal Saneamiento {get; set;}
        public decimal Actu {get; set;}
        public decimal Otros {get; set;}
        public decimal Subtotal {get; set;}
        public decimal Iva {get; set;}
        public decimal Total {get;set;}
        public int Usuarios {get;set;}
        public int M3Consumidos {get;set;}
        public int M3Facturados {get;set;}
        public bool EsRural {get;set;}
        public int Habitantes {get;set;}

        public decimal TotalPropios {
            get {
                return Agua + Drenaje + Saneamiento;
            }
        }


        public static FacturacionLocalidad FromDataReader(SqlDataReader reader){
            var result = new FacturacionLocalidad();
            result.IdLocalidad = ConvertUtils.ParseInteger(reader["id_localidad"].ToString());
            result.Localidad = reader["localidad"].ToString();
            result.Agua = ConvertUtils.ParseDecimal(reader["agua"].ToString());
            result.Drenaje = ConvertUtils.ParseDecimal(reader["dren"].ToString());
            result.Saneamiento = ConvertUtils.ParseDecimal(reader["sane"].ToString());
            result.Actu = ConvertUtils.ParseDecimal(reader["actu"].ToString());
            result.Otros = ConvertUtils.ParseDecimal(reader["otros"].ToString());
            result.Subtotal = ConvertUtils.ParseDecimal(reader["subtotal"].ToString());
            result.Iva = ConvertUtils.ParseDecimal(reader["iva"].ToString());
            result.Total = ConvertUtils.ParseDecimal(reader["total"].ToString());
            result.Usuarios = ConvertUtils.ParseInteger(reader["usuarios"].ToString());
            result.M3Consumidos = ConvertUtils.ParseInteger(reader["m3_consumidos"].ToString());
            result.M3Facturados = ConvertUtils.ParseInteger(reader["m3_facturados"].ToString());

            try {
                result.EsRural = Convert.ToBoolean( reader["es_rural"]);
            }
            catch (System.Exception) {
                result.EsRural = false;
            }    

            try {
                result.Habitantes = Convert.ToInt32( reader["habitantes"] );
            }
            catch (System.Exception) {
                result.Habitantes = -1;
            }    
            
            return result;
        }							
    }
    
}