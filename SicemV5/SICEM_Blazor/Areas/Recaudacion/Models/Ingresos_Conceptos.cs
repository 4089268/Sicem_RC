using System;
using System.Data;
using System.Data.SqlClient;
using SICEM_Blazor.Data;

namespace SICEM_Blazor.Recaudacion.Models {
    public class Ingresos_Conceptos {

        public int Id_Oficina { get; set; }
        public string Oficina { get; set; }

        public int Id_Concepto { get; set; }
        public string Concepto { get; set; }
        
        public decimal Domestico_Sub{ get; set; }
        public decimal Domestico_Iva{ get; set; }
        public decimal Domestico_Total{ get; set; }

        public decimal Hotelero_Sub{ get; set; }
        public decimal Hotelero_Iva{ get; set; }
        public decimal Hotelero_Total{ get; set; }

        public decimal Comercial_Sub{ get; set; }
        public decimal Comercial_Iva{ get; set; }
        public decimal Comercial_Total{ get; set; }

        public decimal Industrial_Sub{ get; set; }
        public decimal Industrial_Iva{ get; set; }
        public decimal Industrial_Total{ get; set; }

        public decimal ServGen_Sub{ get; set; }
        public decimal ServGen_Iva{ get; set; }
        public decimal ServGen_Total{ get; set; }

        public decimal Subtotal { get; set; }
        public decimal Iva { get; set; }
        public decimal Total { get; set; }

        public override string ToString()
        {
            return $"{Id_Concepto}-{Concepto} Total: {Subtotal.ToString("c2")} + {Iva.ToString("c2")} = {Total.ToString("c2")} ";
        }
        public static Ingresos_Conceptos FromSqlDataReader(SqlDataReader reader){

            var item = new Ingresos_Conceptos();
            item.Id_Oficina = 0;
            item.Oficina = "";
            item.Id_Concepto = ConvertUtils.ParseInteger(reader["id_concepto"].ToString());
            item.Concepto = reader["concepto"].ToString();

            item.Subtotal = ConvertUtils.ParseDecimal(reader["subtotal"].ToString());
            item.Iva = ConvertUtils.ParseDecimal(reader["iva"].ToString());
            item.Total = ConvertUtils.ParseDecimal(reader["total"].ToString());

            item.Domestico_Sub = ConvertUtils.ParseDecimal(reader["sub_domestico"].ToString());
            item.Domestico_Iva = ConvertUtils.ParseDecimal(reader["iva_domestico"].ToString());
            item.Domestico_Total = ConvertUtils.ParseDecimal(reader["tot_domestico"].ToString());

            item.Hotelero_Sub = ConvertUtils.ParseDecimal(reader["sub_hotelero"].ToString());
            item.Hotelero_Iva = ConvertUtils.ParseDecimal(reader["iva_hotelero"].ToString());
            item.Hotelero_Total = ConvertUtils.ParseDecimal(reader["tot_hotelero"].ToString());

            item.Comercial_Sub = ConvertUtils.ParseDecimal(reader["sub_comercial"].ToString());
            item.Comercial_Iva = ConvertUtils.ParseDecimal(reader["iva_comercial"].ToString());
            item.Comercial_Total = ConvertUtils.ParseDecimal(reader["tot_comercial"].ToString());

            item.Industrial_Sub = ConvertUtils.ParseDecimal(reader["sub_industrial"].ToString());
            item.Industrial_Iva = ConvertUtils.ParseDecimal(reader["iva_industrial"].ToString());
            item.Industrial_Total = ConvertUtils.ParseDecimal(reader["tot_industrial"].ToString());

            item.ServGen_Sub = ConvertUtils.ParseDecimal(reader["sub_servGen"].ToString());
            item.ServGen_Iva = ConvertUtils.ParseDecimal(reader["iva_servGen"].ToString());
            item.ServGen_Total = ConvertUtils.ParseDecimal(reader["tot_servGen"].ToString());

            return item;
        }
    }
}