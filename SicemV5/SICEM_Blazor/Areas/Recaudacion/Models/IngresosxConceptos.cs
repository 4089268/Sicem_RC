using System;
using System.Collections.Generic;
using System.Text;

namespace SICEM_Blazor.Recaudacion.Models {
    public class IngresosxConceptos {
        public int Id_Concepto { get; set; }
        public string Descripcion { get; set; }
        public decimal Tot1 { get; set; }
        public decimal IVA1 { get; set; }
        public int Usu1 { get; set; }
        public decimal Tot2 { get; set; }
        public decimal IVA2 { get; set; }
        public int Usu2 { get; set; }
        public decimal Tot3 { get; set; }
        public decimal IVA3 { get; set; }
        public int Usu3 { get; set; }
        public decimal Tot4 { get; set; }
        public decimal IVA4 { get; set; }
        public int Usu4 { get; set; }
        public decimal IVA5 { get; set; }
        public decimal Tot5 { get; set; }
        public int Usu5 { get; set; }
        public decimal Subtotal { get; set; }
        public decimal IVA { get; set; }
        public decimal Total { get; set; }
        public int Usuarios { get; set; }
        public int Id_TipoUsuario { get; set; }

    }
}