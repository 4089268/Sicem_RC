using System;
using System.Collections.Generic;

namespace SICEM_Blazor.Models
{
    public partial class RutasLocation
    {
        public int Id { get; set; }
        public int IdRuta { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }

        public virtual Ruta IdRutaNavigation { get; set; }
    }
}
