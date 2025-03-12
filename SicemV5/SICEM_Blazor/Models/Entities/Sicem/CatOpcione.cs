using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using SICEM_Blazor.Data;

namespace SICEM_Blazor.Models
{
    public partial class CatOpcione : IOpcionSistema
    {
        public decimal? IdOpcion { get; set; }
        public string Descripcion { get; set; }
        public bool? Inactivo { get; set; }


        [NotMapped]
        public int Id {
            get {
                return (int)(IdOpcion??0m);
            }
            set{
                IdOpcion = (decimal)value;
            }
        }
    }
}
