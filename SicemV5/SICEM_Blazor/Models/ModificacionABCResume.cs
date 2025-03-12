using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SICEM_Blazor.Models
{
    public class ModificacionABCResume
    {
        public int Id_abc {get;set;}
        public DateTime Fecha {get;set;}
        public int Id_padron { get; set; }
        public string Id_operador { get; set; }
        public int Id_sucursal { get; set; }
        public string Observacion { get; set; }
        public string Maquina { get; set; }
        public string Operador { get; set; }
        public string Sucursal { get; set; }
        public string Campo { get; set; }
        public string Valor_Ant { get; set; }
        public string Valor_Act { get; set; }
    }
}