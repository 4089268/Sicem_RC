using System;


namespace SICEM_Blazor.Models {
 public class NotificacionTemplate {
        public Guid UUID {get;}
        public string Titulo {get;set;}
        public string Texto {get;set;}
        public DateTime FCreacion {get;set;}
        public DateTime UltimaMod {get;set;}

        public NotificacionTemplate(){
            UUID = Guid.NewGuid();
        }
        public NotificacionTemplate(CatMessagesTemplate e){
            UUID = e.Id;
            Titulo = e.Titulo;
            Texto = e.Mensaje;
            FCreacion = e.FechaCreacion;
            UltimaMod = e.UltimaModificacion;
        }

        public CatMessagesTemplate ToCatMessageTemplate(){
            var item = new CatMessagesTemplate();
            item.Id = UUID;
            item.Titulo = Titulo;
            item.Mensaje = Texto;
            item.FechaCreacion = FCreacion;
            item.UltimaModificacion = DateTime.Now;
            return item;
        }
    }
}