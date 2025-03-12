using System;

namespace SICEM_Blazor.Lecturas.Models{
    public class Incidencia {
        public long Cuenta {get;set;}
        public string Localizacion {get;set;}
        public string Usuario {get;set;}
        public string Lecturista {get;set;}
        public string Lectura {get;set;}
        public string Anomalia {get;set;}
        public string Descripcion {get;set;}
        public DateTime Fecha {get;set;}
        public string Handheld {get;set;}
    }
}