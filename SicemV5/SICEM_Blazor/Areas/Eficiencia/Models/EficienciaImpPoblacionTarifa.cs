using System;
using SICEM_Blazor.Data;
using SICEM_Blazor.Data.Contracts;

namespace SICEM_Blazor.Eficiencia.Models {

    public class EficienciaImpPoblacionTarifa {
    
        public int Id_Poblacion {get;set;}
        public string Poblacion {get;set;}
        public int Id_Tarifa {get;set;}
        public string Tarifa {get;set;}
        public int Af {get;set;}
        public int Mf {get;set;}
        public string Mes {get;set;}
        public decimal Facturado {get;set;}
        public decimal Refacturado {get; set;}
        public decimal Anticipado {get; set;}
        public decimal Descontado {get; set;}
        public decimal Cobrado {get; set;}
        public double Porcentaje {get; set;}
        public double PorcentajeCNA {get; set;}
        public double PorcentajeCapa {get; set;} = 0;
        public decimal CobroCapa {get; set;} = 0m;
        public double EficienciaPorcentaje {
            get {
                if( Porcentaje > 0){
                    return Porcentaje / 100;
                }else{
                    return 0;
                }
            }
        }
        public double EficienciaCNA {
            get {
                if( Facturado > 0){
                    return PorcentajeCNA / 100;
                }
                else{
                    return 0;
                }
            }
        }
        public double EfiCapa { get => PorcentajeCapa / 100; }
        public string Periodo {
            get => $"{Mes}-{Af}";
        }
        
    }

}