using System;
using System.Collections.Generic;
using System.Text;

namespace SICEM_Blazor.Descuentos.Models{
    public class Descuentos_Autorizo {
        public string CVE { get; set; }
        public string Autorizo { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Iva { get; set; }
        public decimal Total { get; set; }
        public int Usuarios { get; set; }

    }
}
