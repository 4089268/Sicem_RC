using System;

namespace SICEM_Blazor.Models {
    public class Ordenes_Detalle{        
        public string Orden {get;set;}
        public long Cuenta {get;set;}
        public string Nombre {get;set;}
        public int Id_Trabajo {get;set;}
        public string Trabajo {get;set;}
        public string Fecha_Genero {get;set;}
        public string Fecha_Realizo {get;set;}
        public int Id_Estatus {get;set;}
        public string Estatus {get;set;}
        public string Colonia {get;set;}
        public int Id_Colonia {get;set;}
        public string Tarifa {get;set;}
        public int Id_Tarifa {get;set;}
        public string Id_Genero {get;set;}
        public string Genero {get;set;}
        public string Motivo {get;set;}
        public string Resultado {get;set;}
        public int Id_Departamento {get;set;}
        public string Departamento {get;set;}
        public int  Id_Realizo {get;set;}
        public string Realizo {get;set;}


        private Ordenes_PagoRealizadoItem _pagoRealizado;
        public void SetPagoRealizado(Ordenes_PagoRealizadoItem val){
            _pagoRealizado = val;
        }
        public decimal Adeudo_Orden { get => _pagoRealizado == null ? 0 :_pagoRealizado.Adeudo_Orden; }
        public decimal Importe_Pagado { get => _pagoRealizado == null ? 0 :_pagoRealizado.Importe_Pagado; }
        public DateTime? Fecha_Pago { get => _pagoRealizado == null ? null : _pagoRealizado.Fecha_Pago; }
        public int Dias {get  => _pagoRealizado == null ? 0 :_pagoRealizado.Dias; }

    }

}