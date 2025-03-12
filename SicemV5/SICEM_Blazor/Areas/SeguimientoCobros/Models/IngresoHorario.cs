using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SICEM_Blazor.SeguimientoCobros.Models
{
    public class IngresoHorario
    {

        public int IdSucursal {get;set;}
        public string Sucursal {get;set;}
        public string Id_Cajero {get; set;} = "";
        public string Caja {get; set;} = "";

        public int Recibos {get; set;} = 0;
        public decimal Cobrado {get; set;} = 0m;
        public decimal Hora0708 {get; set;} = 0m;
        public decimal Hora0809 {get; set;} = 0m;
        public decimal Hora0910 {get; set;} = 0m;
        public decimal Hora1011 {get; set;} = 0m;
        public decimal Hora1112 {get; set;} = 0m;
        public decimal Hora1213 {get; set;} = 0m;
        public decimal Hora1314 {get; set;} = 0m;
        public decimal Hora1415 {get; set;} = 0m;
        public decimal Hora1516 {get; set;} = 0m;
        public decimal Hora1617 {get; set;} = 0m;
        public decimal Hora1718 {get; set;} = 0m;
        public decimal Hora1819 {get; set;} = 0m;
        public decimal Hora1920 {get; set;} = 0m;

        public IngresoHorario(int idSucursal, string sucursal, string idCaja, string caja){
            this.IdSucursal = idSucursal;
            this.Sucursal = sucursal;
            this.Id_Cajero = idCaja;
            this.Caja = caja;
        }
        
    }
}