using System;
using System.Collections.Generic;

namespace SICEM_Blazor.Models.Entities.Arquos
{
    public partial class CatConcepto
    {
        public decimal IdConcepto { get; set; }
        public string Descripcion { get; set; }
        public decimal Importe { get; set; }
        public decimal IdTipoconcepto { get; set; }
        public bool ClaveFija { get; set; }
        public bool Credito { get; set; }
        public bool Mostrar { get; set; }
        public decimal IdSystema { get; set; }
        public decimal IdEstatus { get; set; }
        public bool Inactivo { get; set; }
        public string TipoIva { get; set; }
        public bool? CostoEstatico { get; set; }
        public decimal? IdTrabajo { get; set; }
        public string ClaveProdServ { get; set; }
        public string ClaveUnidad { get; set; }
        public bool? NoDetcfdi { get; set; }
    }
}
