using System;
using SICEM_Blazor.Data;
using SICEM_Blazor.Data.Contracts;

namespace SICEM_Blazor.Eficiencia.Models {

    public class EficienciaResumen : IResumOficina {
    
        public IEnlace Enlace {get;set;}
        public int Af {get;set;}
        public int Mf {get;set;}
        public string Mes {get;set;}
        public decimal Facturado {get;set;}
        public decimal Refacturado {get; set;}
        public decimal Anticipado {get; set;}
        public decimal Descontado {get; set;}
        public decimal Cobrado {get; set;}
        public double Porcentaje {get; set;}
        public double PorcentajeCapa {get; set;} = 0;
        public decimal CobroCapa {get; set;} = 0;
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
                    return (double)(Cobrado/Facturado);
                }
                else{
                    return 0;
                }
            }
        }
        public string Periodo {
            get => $"{Mes} {Af}";
        }
        public double EficienciaCapa {
            get {
                if( PorcentajeCapa > 0){
                    return PorcentajeCapa / 100;
                }else{
                    return 0;
                }
            }
        }

        public ResumenOficinaEstatus Estatus { get; set; }
        public int Id {get => Enlace==null?999:Enlace.Id; }
        public string Oficina {get => Enlace==null?"TOTAL":Enlace.Nombre; }

        public EficienciaResumen(){
            Estatus = ResumenOficinaEstatus.Pendiente;
        }
        public EficienciaResumen(IEnlace enlace){
            Estatus = ResumenOficinaEstatus.Pendiente;
            Enlace = enlace;
        }
    }

}