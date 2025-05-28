using System;
using System.Data;
using System.Text;
using SICEM_Blazor.Data;

namespace SICEM_Blazor.Recaudacion.Models
{
    public class IngresosxConceptos
    {
        public int Id_Concepto { get; set; }
        public string Descripcion { get; set; }
        public decimal Subtotal { get; set; }
        public decimal IVA { get; set; }
        public decimal Total { get; set; }
        public int Usuarios { get; set; }

        public static IngresosxConceptos FromDataReader(IDataReader reader)
        {
            var item = new IngresosxConceptos
            {
                Id_Concepto = ConvertUtils.ParseInteger(reader["id_concepto"].ToString()),
                Descripcion = reader["descripcion"].ToString(),
                Subtotal = ConvertUtils.ParseDecimal(reader["subtotal"].ToString()),
                IVA = ConvertUtils.ParseDecimal(reader["iva"].ToString()),
                Total = ConvertUtils.ParseDecimal(reader["total"].ToString()),
                Usuarios = ConvertUtils.ParseInteger(reader["usuarios"].ToString()),
            };
            return item;
        }
    }
}