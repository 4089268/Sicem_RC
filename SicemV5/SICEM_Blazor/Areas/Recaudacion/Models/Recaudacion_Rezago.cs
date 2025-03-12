using System;
using System.Collections.Generic;
using System.Text;

namespace SICEM_Blazor.Recaudacion.Models {
    public class Recaudacion_Rezago {
        public string Mes { get; set; }
        public int Usuarios { get; set; }
        public decimal Rez_agua { get; set; }
        public decimal Rez_dren { get; set; }
        public decimal Rez_sane { get; set; }
        public decimal Rez_otros { get; set; }
        public decimal Rez_recargos { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Iva { get; set; }
        public decimal Total { get; set; }

    }
}
