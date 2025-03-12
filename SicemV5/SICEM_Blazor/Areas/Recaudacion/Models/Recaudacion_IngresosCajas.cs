using System;
using System.Collections.Generic;
using System.Text;

namespace SICEM_Blazor.Recaudacion.Models {
    public class Recaudacion_IngresosCajas {
        public int Id_Sucursal {get;set;}
        public string Sucursal { get; set; }
        public string CveCaja { get; set; }
        public string Caja { get; set; }
        public decimal Facturado { get; set; }        
        public decimal Cobrado { get; set; } 
        public int Recibos { get; set; }
        
    }
}
