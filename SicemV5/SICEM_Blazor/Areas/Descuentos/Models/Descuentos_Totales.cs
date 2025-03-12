using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SICEM_Blazor.Data;
using SICEM_Blazor.Data.Contracts;

namespace SICEM_Blazor.Descuentos.Models{
    public class Descuentos_Totales {
        public IEnlace Enlace {get;set;}
        public ResumenOficinaEstatus Estatus { get; set; }
        public int Id_Oficina { get => Enlace == null?999:Enlace.Id;}
        public string Oficina { get => Enlace == null?"TOTAL":Enlace.Nombre; }
        public decimal Conc_Con_Iva { get; set; }
        public decimal Iva { get; set; }
        public decimal Aplicado_Con_Iva { get; set; }
        public decimal Conc_Sin_Iva { get; set; }
        public decimal Total_Aplicado { get; set; }
        public int Usuarios { get; set; }

        public Descuentos_Totales() {
            Estatus = 0;
            Conc_Con_Iva = 0m;
            Iva = 0m;
            Aplicado_Con_Iva = 0m;
            Conc_Sin_Iva = 0m;
            Total_Aplicado = 0m;
            Usuarios = 0;
        }

    }
}
