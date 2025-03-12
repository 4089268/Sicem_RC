using System;
using System.Collections.Generic;

namespace SICEM_Blazor.Padron.Models {
    public class ResumenColonia {
        public string Colonia {get;set;}
        public int Usuarios {get;set;}
        public decimal Subtotal {get;set;}
        public decimal IVA {get;set;}
        public decimal Total {get;set;}

    }

}