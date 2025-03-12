using System;
using System.Collections.Generic;

namespace SICEM_Blazor.Models.Entities.Arquos
{
    public partial class CatEstatus
    {
        public decimal IdEstatus { get; set; }
        public string Descripcion { get; set; }
        public string Tabla { get; set; }
        public decimal? RegsPadron { get; set; }
        public DateTime? RegsCounted { get; set; }
    }
}
