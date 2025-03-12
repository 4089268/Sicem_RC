using System;
using System.Data;

using SICEM_Blazor.Data;

namespace SICEM_Blazor.Micromedicion.Models {
    public class ResumenTarifaUsuario {
        public int Id
        {
            get => Enlace == null
                ? 999
                : Enlace.Id;
        }
        public string Oficina
        {
            get => Enlace==null ? "TOTAL" : Enlace.Nombre;
        }
        public IEnlace Enlace {get;set;}

        public int IdTarifa {get;set;}
        public string Tarifa {get;set;}
        public int Reales {get;set;}
        public double RealesPorc {get;set;}
        public double RealesPorcFormat { get => (RealesPorc / 100); }
        public int Promedios {get;set;}
        public double PromediosPorc {get;set;}
        public double PromediosPorcFormat { get => (PromediosPorc / 100); }
        public int Medidos {get;set;}
        public double MedidosPorc {get;set;}
        public double MedidosPorcFormat { get => (MedidosPorc / 100); }
        public int Fijos {get;set;}
        public double FijosPorc {get;set;}
        public double FijosPorcFormat { get => (FijosPorc / 100); }
        public int Total {get;set;}

        public ResumenTarifaUsuario(IEnlace enlace)
        {
            this.Enlace = enlace;
        }

        public static ResumenTarifaUsuario FromDataReader(IEnlace enlace, IDataReader reader )
        {
            var record = new ResumenTarifaUsuario(enlace)
            {
                IdTarifa = Convert.ToInt32(reader["id_tarifa"]),
                Tarifa = reader["tarifa"].ToString(),
                Reales = Convert.ToInt32(reader["reales"]),
                RealesPorc = Convert.ToDouble(reader["rea_porc"]),
                Promedios = Convert.ToInt32(reader["promedios"]),
                PromediosPorc = Convert.ToDouble(reader["pro_porc"]),
                Medidos = Convert.ToInt32(reader["medidos"]),
                MedidosPorc = Convert.ToDouble(reader["med_porc"]),
                Fijos = Convert.ToInt32(reader["fijos"]),
                FijosPorc = Convert.ToDouble(reader["fij_porc"]),
                Total = Convert.ToInt32(reader["total"])
            };
            return record;
        }

    }
}