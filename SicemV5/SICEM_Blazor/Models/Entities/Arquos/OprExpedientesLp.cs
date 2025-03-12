using System;
using System.Collections.Generic;

namespace SICEM_Blazor.Models.Entities.Arquos
{
    public partial class OprExpedientesLp
    {
        public string IdExpediente { get; set; }
        public decimal IdEstatus { get; set; }
        public DateTime FechaGenero { get; set; }
        public string IdGenero { get; set; }
        public decimal IdPadron { get; set; }
        public decimal IdCuenta { get; set; }
        public decimal IdPublicogeneral { get; set; }
        public string Representante { get; set; }
        public string Constructora { get; set; }
        public string Rfc { get; set; }
        public string Direccion { get; set; }
        public string Colonia { get; set; }
        public string Ciudad { get; set; }
        public string Estado { get; set; }
        public string Telefono1 { get; set; }
        public string Telefono2 { get; set; }
        public string Email { get; set; }
        public decimal IdNivelsocioeconomico { get; set; }
        public string NombreDesarrollo { get; set; }
        public string UbicacionDesarrollo { get; set; }
        public decimal NumeroTomas { get; set; }
        public decimal NumeroCuartos { get; set; }
        public decimal LpsAgua { get; set; }
        public decimal LpsDren { get; set; }
        public decimal ObrasCabezaAgua { get; set; }
        public decimal ObrasCabezaDren { get; set; }
        public decimal? CobrarLpsAgua { get; set; }
        public decimal? CobrarLpsDren { get; set; }
        public string ObservaA { get; set; }
        public DateTime? FechaCancelo { get; set; }
        public string IdCancelo { get; set; }
        public string ObservaC { get; set; }
    }
}
