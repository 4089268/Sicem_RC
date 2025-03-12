using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SICEM_Blazor.Models;

namespace SICEM_Blazor.Pages.AnalisisInformacion
{
    public class PointInfo
    {
        public string Titulo {get;set;}
        public string SubTitulo {get;set;}
        public decimal Latitud {get;set;}
        public decimal Longitud {get;set;}
        public int IdOficina {get;set;}
        public string RazonSocial {get;set;}
        public int IdCuenta {get;set;}
        public string Localizacion {get;set;}
        public string Direccion {get;set;}
        public string Colonia {get;set;}
        public int MesesAdeudo { get; set; } = 0;

        public static PointInfo FromMarkData(MapMark markData){
            var pointInfo = new PointInfo
            {
                Titulo = markData.Descripcion,
                SubTitulo = markData.Subtitulo,
                Latitud = (decimal)markData.Latitude,
                Longitud = (decimal)markData.Longitude,
                IdOficina = markData.IdOficina,
                RazonSocial = markData.Subtitulo,
                IdCuenta = (int)markData.IdCuenta,
                MesesAdeudo = markData.MesesAdeudo,
                Localizacion = "markData.Padron.Localizacion",
                Direccion = "markData.Padron.Direccion",
                Colonia = "markData.Padron.Colonia"
            };
            return pointInfo;
        }
        
        public static PointInfo FromCatPadron(CatPadron catPadron){
            var pointInfo = new PointInfo
            {
                Titulo = catPadron.RazonSocial,
                SubTitulo = catPadron.Id_Cuenta.ToString(),
                Latitud = (decimal)catPadron.Latitude,
                Longitud = (decimal)catPadron.Longitude,
                IdOficina = catPadron.Id_Oficina,
                RazonSocial = catPadron.RazonSocial,
                IdCuenta = (int)catPadron.Id_Cuenta,
                MesesAdeudo = catPadron.MesAdeudoAct,
                Localizacion = catPadron.Localizacion,
                Direccion = catPadron.Direccion,
                Colonia = catPadron.Colonia
            };
            return pointInfo;
        }
        
    }

}