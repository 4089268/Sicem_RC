using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace SICEM_Blazor.Eficiencia.Models{
    public class EficienciaLocalidad {
        public int IdPoblacion { get; set; }
        public string Poblacion { get; set; }
        public bool EsRural { get; set; }
        public decimal Facturado { get; set; }
        public decimal Anticipado { get; set; }
        public decimal Descuentos { get; set; }
        public decimal Cobrado { get; set; }
        public decimal Refacturacion { get; set; }
        public double EfiCome { get; set; }
        public double EfiConagua { get; set; } = 0;
        public double PorcentajeCapa { get; set; } = 0;
        public decimal CobroCapa { get; set; }
        public double EfiComePer { get => EfiCome / 100; }
        public double EfiConaguaPer { get => EfiConagua / 100;  }
        public double EfiCapa { get => PorcentajeCapa / 100;  }

        public static EficienciaLocalidad fromDataReader(SqlDataReader sqlDataReader){
            var data = new EficienciaLocalidad();
            data.IdPoblacion =  Convert.ToInt32( sqlDataReader["id_poblacion"] );
            data.Poblacion = sqlDataReader["_poblacion"].ToString();
            // data.EsRural = Convert.ToBoolean(sqlDataReader["es_rural"]);
            data.EsRural = false;
            data.Facturado = Convert.ToDecimal(sqlDataReader["facturado"]);
            data.Anticipado = Convert.ToDecimal(sqlDataReader["anticipado"]);
            data.Descuentos = Convert.ToDecimal(sqlDataReader["descontado"]);
            data.Cobrado = Convert.ToDecimal(sqlDataReader["cobrado"]);
            data.Refacturacion = Convert.ToDecimal(sqlDataReader["refacturado"]);
            data.EfiCome = Convert.ToDouble(sqlDataReader["porc"]);
            data.EfiConagua = Convert.ToDouble(sqlDataReader["porc_cna"]);
            try {
                data.CobroCapa = Convert.ToDecimal(sqlDataReader["cobrado1"]);
                data.PorcentajeCapa = Convert.ToDouble(sqlDataReader["porc_capa"]);
            }catch(Exception){}
            return data;
        }
    }
}
