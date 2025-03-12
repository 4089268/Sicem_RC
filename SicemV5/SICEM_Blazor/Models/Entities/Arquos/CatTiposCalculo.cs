using System;
using System.Collections.Generic;

namespace SICEM_Blazor.Models.Entities.Arquos
{
    public partial class CatTiposCalculo
    {
        public CatTiposCalculo()
        {
            CatAnomaliases = new HashSet<CatAnomalias>();
        }

        public decimal IdCalculo { get; set; }
        public string Descripcion { get; set; }
        public bool Inactivo { get; set; }
        public decimal? RegsPadron { get; set; }
        public DateTime? RegsCounted { get; set; }

        public virtual ICollection<CatAnomalias> CatAnomaliases { get; set; }
    }
}
