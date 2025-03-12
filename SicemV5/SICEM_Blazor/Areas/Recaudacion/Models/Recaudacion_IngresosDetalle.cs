using System;
using System.Collections.Generic;
using System.Text;

namespace SICEM_Blazor.Recaudacion.Models{
    public class Recaudacion_IngresosDetalle {
        public DateTime Fecha {get;set;}
        public string Folio {get;set;}
        public string Cuenta {get;set;}
        public string Usuario{get;set;}
        public decimal Cobrado {get;set;}
        public string Caja {get;set;}
        public DateTime Fecha_aplicacion {get;set;}
        public int Hrs_dif {get;set;}
    }
}
