using System;
using SICEM_Blazor.Data;
using SICEM_Blazor.Data.Contracts;

namespace SICEM_Blazor.Eficiencia.Models {

    public class EficienciResumenUsuario : IResumenOficina {
    
        public IEnlace Enlace {get;set;}
        public int Af {get;set;}
        public int Mf {get;set;}
        public string Mes {get;set;}
        public int Facturado {get;set;}
        public int Refacturado {get; set;}
        public int Anticipado {get; set;}
        public int Descontado {get; set;}
        public int Cobrado {get; set;}
        public double Porcentaje {get; set;}
        public double PorcentajeCNA {get; set;}
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

        public ResumenOficinaEstatus Estatus { get; set; }
        public int Id {get => Enlace==null?999:Enlace.Id; }
        public string Oficina {get => Enlace==null?"TOTAL":Enlace.Nombre; }

        public string Periodo {
            get => $"{Mes}-{Af}";
        }

        public EficienciResumenUsuario(){
            Estatus = ResumenOficinaEstatus.Pendiente;
        }
        public EficienciResumenUsuario(IEnlace enlace){
            Estatus = ResumenOficinaEstatus.Pendiente;
            Enlace = enlace;
        }
    }

}