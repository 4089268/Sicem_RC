using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using SICEM_Blazor.Data;

namespace SICEM_Blazor.Recaudacion.Models
{
    public class Ingresos_FormasPago
    {
        public int Orden { get; set; }
        public int Id { get; set; }
        public string Forma_Pago { get; set; }
        public decimal Cobrado{ get; set; }
        public int Cobros { get; set; }

        public static Ingresos_FormasPago FromDataReader(IDataReader reader)
        {
            var item = new Ingresos_FormasPago()
            {
                Orden = 0,
                Id = ConvertUtils.ParseInteger(reader["id_formapago"]),
                Forma_Pago = reader["formapago"].ToString(),
                Cobrado = ConvertUtils.ParseDecimal(reader["cobrado"]),
                Cobros = ConvertUtils.ParseInteger(reader["num_cobros"])
            };
            return item;
        }
    }
}
