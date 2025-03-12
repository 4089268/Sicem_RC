using System;
using System.Collections.Generic;
using System.Text;

namespace SICEM_Blazor.Descuentos.Models{
    public class Descuentos_Resumen {

        public decimal Conc_Con_Iva{ get; set; }
        public decimal Iva { get; set; }
        public decimal Apli_Con_Iva { get; set; }
        public decimal Conc_Sin_Iva { get; set; }
        public decimal Total { get; set; }
        public int Usuarios { get; set; }

        public string[] Conceptos { get; set; }

        public Descuentos_Resumen_Item[] Tarifas { get; set; }

        public Descuentos_Resumen_Item[] Calculos { get; set; }

    }
    public class Descuentos_Resumen_Item {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public decimal Total { get; set; }
        public int NTotal { get; set; }
    }

}
