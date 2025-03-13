using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using SICEM_Blazor.Data;
using SICEM_Blazor.Data.Contracts;

namespace SICEM_Blazor.Recaudacion.Models {
    public class ResumenOficina : IResumenOficina
    {
        public IEnlace Enlace { get; set; }
        public decimal IngresosPropios { get; set; }
        public int RecibosPropios { get; set; }
        public decimal IngresosOtros { get; set; }
        public int RecibosOtros { get; set; }
        public decimal ImporteTotal { get; set; }
        public int Usuarios{ get; set; }
        
        public int Id {
            get => Enlace?.Id ?? 0;
        }
        public string Oficina {
            get => Enlace?.Nombre ?? "";
        }

        public ResumenOficinaEstatus Estatus {get; set;}

        public ResumenOficina(IEnlace enlace)
        {
            this.Enlace = enlace;
            this.Estatus = ResumenOficinaEstatus.Pendiente;
        }

        public static ResumenOficina FromDataReader(IEnlace enlace, IDataReader reader)
        {
            return new ResumenOficina(enlace)
            {
                IngresosPropios = ConvertUtils.ParseDecimal(reader["i1"]),
                RecibosPropios = ConvertUtils.ParseInteger(reader["u1"]),
                IngresosOtros = ConvertUtils.ParseDecimal(reader["i2"]),
                RecibosOtros = ConvertUtils.ParseInteger(reader["u2"]),
                ImporteTotal = ConvertUtils.ParseDecimal(reader["importe"]),
                Usuarios = ConvertUtils.ParseInteger(reader["usuarios"]),
                Estatus = ResumenOficinaEstatus.Completado
            };
        }

    }
}
