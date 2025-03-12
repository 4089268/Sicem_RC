using System;
using System.Collections.Generic;
using System.Text;

namespace SICEM_Blazor.Recaudacion.Models {
    public class Recaudacion_IngresosDias{
        public string Fecha { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Iva { get; set; }
        public decimal Total { get; set; }

        public int RecibosPropios { get; set; }
        public int RecibosOtros { get; set; }
        public int RecibosTotal { get; set; }

        public DateTime? Fecha_DT { 
            get {
                try {
                    return DateTime.Parse(Fecha);
                }
                catch(Exception) {
                    return null;
                }
            }
        }

        public string Fecha_Letras { 
            get {
                if(Fecha.Contains("TOTAL")){
                    return "TOTAL";
                }
                else {
                    var _fd = Fecha_DT;
                    if(_fd == null) {
                        return "";
                    }
                    else {
                        return (_fd??DateTime.Now).ToString("dd MMMM yyyy");
                    }
                }
            }
        }

    }
}
