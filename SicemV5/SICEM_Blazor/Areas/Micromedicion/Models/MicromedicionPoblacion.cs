using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SICEM_Blazor.Data;
using SICEM_Blazor.Data.Contracts;

namespace SICEM_Blazor.Micromedicion.Models {
    public class MicromedicionPoblacion {
        
        public int IdPoblacion {get; set;}
        public string Poblacion {get; set; }
        public bool EsRural {get; set; }
        public int Reales { get; set; }
        public int Promedios { get; set; }
        public int Medidos { get; set; }
        public int Fijos { get; set; }
        public int Total { get; set; }
        public int Habitantes { get; set; }
        
        public MicromedicionPoblacion(int id, string poblacion) {
            IdPoblacion = id;
            Poblacion = poblacion;
            EsRural = false;
            Reales = 0;
            Promedios = 0;
            Medidos = 0;
            Fijos = 0;
            Total = 0;
            Habitantes = 0;
        }

    }
}
