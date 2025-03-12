using System;
using System.Collections.Generic;

namespace SICEM_Blazor.Models.Entities.Arquos
{
    public partial class CatClasesUsuario
    {
        public decimal IdClaseUsuario { get; set; }
        public string Descripcion { get; set; }
        public bool Condicionar { get; set; }
        public bool Inactivo { get; set; }
        public string CveDescuento { get; set; }
        public decimal Porcentaje { get; set; }
        public decimal? Desctoxanticipo { get; set; }
        public decimal? RegsPadron { get; set; }
        public DateTime? RegsCounted { get; set; }
    }
}
