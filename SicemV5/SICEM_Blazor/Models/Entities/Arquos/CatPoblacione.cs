using System;
using System.Collections.Generic;

namespace SICEM_Blazor.Models.Entities.Arquos
{
    public partial class CatPoblacione
    {
        public decimal IdPoblacion { get; set; }
        public string Descripcion { get; set; }
        public decimal? IdSucursal { get; set; }
        public byte? Sb { get; set; }
        public string Sectores { get; set; }
        public bool Inactivo { get; set; }
        public string Direccion { get; set; }
        public string Firma { get; set; }
        public bool? PorcentajeDrenaje { get; set; }
        public bool? DrenajeDeCuota { get; set; }
        public decimal? IdZonapoblacion { get; set; }
        public decimal? RegsPadron { get; set; }
        public DateTime? RegsCounted { get; set; }
        public decimal? Agrupador { get; set; }

        public virtual CatSucursale IdSucursalNavigation { get; set; }
    }
}
