using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text;
using SICEM_Blazor.Data;

namespace SICEM_Blazor.Recaudacion.Models
{
    public class IngresosDia
    {
        public string FechaString { get; set; }
        public decimal IngresoPropios { get; set; }
        public int UsuariosProios { get; set; }
        public decimal IngresoOtros { get; set; }
        public int UsuariosOtros { get; set; }
        public decimal TotalImpore { get; set; }
        public decimal Cobrado { get; set; }
        public int TotalUsuarios { get; set; }

        public DateTime? Fecha
        {
            get => DateTime.TryParseExact(this.FechaString, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime f )
                ? f
                : null;
        }

        public string FechaLetras
        {
            get => Fecha?.ToString("dd MMMM yyyy") ?? "";
        }


        public static IngresosDia FromDataReader(IDataReader reader)
        {
            return new IngresosDia()
            {
                FechaString = reader["dias"].ToString(),
                IngresoPropios = ConvertUtils.ParseDecimal(reader["i1"]),
                UsuariosProios = ConvertUtils.ParseInteger(reader["u1"]),
                IngresoOtros = ConvertUtils.ParseDecimal(reader["i2"]),
                UsuariosOtros = ConvertUtils.ParseInteger(reader["u2"]),
                TotalImpore = ConvertUtils.ParseDecimal(reader["importe"]),
                Cobrado = ConvertUtils.ParseDecimal(reader["cobrado"]),
                TotalUsuarios = ConvertUtils.ParseInteger(reader["usuarios"])
            };
        }
    }
}
