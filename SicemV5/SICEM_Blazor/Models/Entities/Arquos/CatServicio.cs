using System;
using System.Collections.Generic;

namespace SICEM_Blazor.Models.Entities.Arquos
{
    public partial class CatServicio
    {
        public CatServicio()
        {
            CatColonia = new HashSet<CatColonia>();
        }

        public decimal IdServicio { get; set; }
        public string Descripcion { get; set; }
        public bool Inactivo { get; set; }
        public decimal? RegsPadron { get; set; }
        public DateTime? RegsCounted { get; set; }

        public virtual ICollection<CatColonia> CatColonia { get; set; }
    }
}
