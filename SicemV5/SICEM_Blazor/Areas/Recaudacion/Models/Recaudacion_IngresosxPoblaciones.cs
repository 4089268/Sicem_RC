using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using SICEM_Blazor.Data;

namespace SICEM_Blazor.Recaudacion.Models {
    public class RecaudacionIngresosxPoblaciones {
        public int Id_localidad { get; set; }
        public string Localidad { get; set; }
        public decimal Facturado { get; set; }
        public decimal Cobrado { get; set; }
        public int Recibos { get; set; }
        public bool EsRural { get; set; }
        public int Habitantes { get; set; }
        
        public static RecaudacionIngresosxPoblaciones FromDataReader(SqlDataReader reader){
            var newItem = new RecaudacionIngresosxPoblaciones();
            newItem.Id_localidad = ConvertUtils.ParseInteger(reader["id_localidad"].ToString());
            newItem.Localidad = reader["localidad"].ToString();
            newItem.Facturado = ConvertUtils.ParseDecimal(reader["facturado"].ToString());
            newItem.Cobrado = ConvertUtils.ParseDecimal(reader["Cobrado"].ToString());
            newItem.Recibos = ConvertUtils.ParseInteger(reader["Recibos"].ToString());
            
            try{
                newItem.EsRural = Convert.ToBoolean(reader["es_rural"]);
            }catch(IndexOutOfRangeException){
                newItem.EsRural = false;
            }

            newItem.Habitantes = Convert.ToInt32(reader["habitantes"]);

            return newItem;
        }
        
    }
}
