using System;
using System.Collections.Generic;

namespace SICEM_Blazor.Models.Entities.Arquos
{
    public partial class CatGiro
    {
        public decimal IdGiro { get; set; }
        public string Descripcion { get; set; }
        public bool Inactivo { get; set; }
        public decimal? RegsPadron { get; set; }
        public DateTime? RegsCounted { get; set; }
    }
}
