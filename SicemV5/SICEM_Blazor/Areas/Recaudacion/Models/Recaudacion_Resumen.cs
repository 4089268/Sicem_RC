using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SICEM_Blazor.Data;

namespace SICEM_Blazor.Recaudacion.Models {
    public class Recaudacion_Resumen {
        public int Estatus { get;  set; }
        public IEnlace Enlace { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Iva { get; set; }
        public decimal Total { get; set; }
        public decimal Cobrado { get; set; }
        public int UsuariosFact{ get; set; }
        public decimal UsuariosCobrado { get; set; }
        public decimal PorCobrado { get; set; }
        public int RecibosPropios { get; set; }
        public int RecibosOtros { get; set; }

        public int Id {
            get {
                if(Enlace != null){
                    return Enlace.Id;
                }else{
                    return 0;
                }
            }
        }
        public string Oficina {
            get {
                if(Enlace != null){
                    return Enlace.Nombre;
                }else{
                    return "";
                }
            }
        }

        public Recaudacion_Resumen() {
            Estatus = 0;
            SubTotal = 0m;
            Iva = 0m;
            Total = 0m;
            Cobrado = 0m;
            UsuariosFact = 0;
            UsuariosCobrado = 0;
            PorCobrado = 0m;
            RecibosPropios = 0;
            RecibosOtros = 0;
        }

    }
}
