using System;
using System.Collections.Generic;
using System.Text;

namespace SICEM_Blazor.Descuentos.Models{
    public class Descuentos_Conceptos {
        public int Id_Concepto { get; set; }
        public string Descripcion { get; set; }
        public decimal Conc_Con_Iva { get; set; }
        public decimal Iva { get; set; }
        public decimal Aplicado_Con_Iva { get; set; }
        public decimal Conc_Sin_Iva { get; set; }
        public decimal Total_Aplicado { get; set; }
        public int Usuarios { get; set; }

    }

}
