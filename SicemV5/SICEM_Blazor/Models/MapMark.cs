using System;
using SICEM_Blazor.Data.Contracts;

namespace SICEM_Blazor.Models {
    public class MapMark : IMapPoint {
        public double Latitude {get;set;} = 20.213382;
        public double Longitude {get;set;} = -87.436819;
        public string Descripcion {get;set;} = "";
        public string Subtitulo {get;set;} = "";
        public double Zoom {get;set;} = 8.5;

        public int IdOficina {get;set;} = 0;
        public long IdPadron {get;set;} = 0;
        public long IdCuenta {get;set;} = 0;
        public int MesesAdeudo {get;set;} = 0;

    }
}