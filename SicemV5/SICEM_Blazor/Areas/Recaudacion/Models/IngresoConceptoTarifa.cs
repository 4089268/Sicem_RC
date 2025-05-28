using System;
using System.Data;
using SICEM_Blazor.Data;

namespace SICEM_Blazor.Recaudacion.Models
{
    public class IngresoConceptoTarifa
    {

        public int IdOficina { get; set; }
        public string Oficina { get; set; }
        public int IdConcepto { get; set; }
        public string Concepto { get; set; }
        
        public int Domestico_Usu{ get; set; }
        public decimal Domestico_Total{ get; set; }


        public int Comercial_Usu{ get; set; }
        public decimal Comercial_Total{ get; set; }

        public int Industrial_Usu{ get; set; }
        public decimal Industrial_Total{ get; set; }

        public int Publico_Usu{ get; set; }
        public decimal Publico_Total{ get; set; }

        public decimal Subtotal { get; set; }
        public decimal Iva { get; set; }
        public decimal Total { get; set; }
        public int Usuarios { get; set; }

        public IngresoConceptoTarifa(IEnlace enlace, int idConcepto, string concepto)
        {
            this.IdOficina = enlace.Id;
            this.Oficina = enlace.Nombre;
            this.IdConcepto = idConcepto;
            this. Concepto = concepto;
        }

        public static IngresoConceptoTarifa FromDataReader(IEnlace enlace, IDataReader reader)

        {
            var _idConcepto = ConvertUtils.ParseInteger( reader["id_concepto"]);
            var _concepto = reader["descripcion"].ToString();

            var item = new IngresoConceptoTarifa(enlace, _idConcepto, _concepto)
            {
                Domestico_Usu = ConvertUtils.ParseInteger( reader["ctos_1"]),
                Domestico_Total = ConvertUtils.ParseDecimal( reader["subtotal_1"]),
                Comercial_Usu = ConvertUtils.ParseInteger( reader["ctos_2"]),
                Comercial_Total = ConvertUtils.ParseDecimal( reader["subtotal_2"]),
                Industrial_Usu = ConvertUtils.ParseInteger( reader["ctos_3"]),
                Industrial_Total = ConvertUtils.ParseDecimal( reader["subtotal_3"]),
                Publico_Usu = ConvertUtils.ParseInteger( reader["ctos_4"]),
                Publico_Total = ConvertUtils.ParseDecimal( reader["subtotal_4"]),
                Subtotal  = ConvertUtils.ParseDecimal( reader["subtotal"]),
                Iva  = ConvertUtils.ParseDecimal( reader["iva"]),
                Total  = ConvertUtils.ParseDecimal( reader["total"]),
                Usuarios  = ConvertUtils.ParseInteger( reader["usuarios"]),
            };

            return item;
        }
    }
}