using System;

namespace SICEM_Blazor.Models{
    public class Ordenes_Capturacion{        
        public string Id {get;set;}
        public string Descripcion{get;set;}
        public int  Pendientes {get;set;}
        public int  En_Ejecucion {get;set;}
        public int  Realizadas {get;set;}
        public int  Canceladas {get;set;}
        public int  Ejecutadas {get;set;}
        public int  No_Ejecutadas {get;set;}        
        public int  Total {get;set;}  
    }
}