using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using SICEM_Blazor.Data;

namespace SICEM_Blazor.Recaudacion.Models
{
    public class RecaudacionIngresosxPoblaciones
    {
        public int Id_localidad { get; set; }
        public string Localidad { get; set; }
        public int IdColonia { get; set; }
        public decimal Cobrado { get; set; }
        public int Recibos { get; set; }
        
        public static RecaudacionIngresosxPoblaciones FromDataReader(SqlDataReader reader)
        {
            var newItem = new RecaudacionIngresosxPoblaciones()
            {
                Id_localidad = ConvertUtils.ParseInteger(reader["id_localidad"]),
                Localidad = reader["localidad"].ToString(),
                IdColonia = ConvertUtils.ParseInteger(reader["colonia"]),
                Cobrado = ConvertUtils.ParseDecimal(reader["Cobrado"]),
                Recibos = ConvertUtils.ParseInteger(reader["Recibos"])
            };
            return newItem;
        }
        
    }
}
