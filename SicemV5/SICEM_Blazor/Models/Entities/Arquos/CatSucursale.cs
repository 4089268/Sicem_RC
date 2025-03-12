using System;
using System.Collections.Generic;

namespace SICEM_Blazor.Models.Entities.Arquos
{
    public partial class CatSucursale
    {
        public CatSucursale()
        {
            CatPoblaciones = new HashSet<CatPoblacione>();
        }

        public decimal IdSucursal { get; set; }
        public string Descripcion { get; set; }
        public decimal Sb { get; set; }
        public bool Inactivo { get; set; }
        public string DatabaseName { get; set; }
        public decimal? Agrupador { get; set; }
        public decimal? CvePoblacion { get; set; }

        public virtual ICollection<CatPoblacione> CatPoblaciones { get; set; }
    }
}
