using System;
using System.Collections.Generic;

namespace SICEM_Blazor.Data {
    public interface IUsuario {
        public string Id {get;}
        public string Nombre { get; }
        public string Usuario { get; }
        public IEnumerable<IEnlace> Enlaces { get; }
        public IEnumerable<IOpcionSistema> OpcionSistemas { get; }
        
        public bool Administrador { get; }

        public void SetEnlaces(IEnumerable<IEnlace> enlaces);
        public void SetOpciones(IEnumerable<IOpcionSistema> opciones);
        public string GetCadEnlaces();

    }
}