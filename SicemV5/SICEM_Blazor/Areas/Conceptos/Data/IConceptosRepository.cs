using System;
using System.Collections.Generic;
using SICEM_Blazor.Conceptos.Models;

namespace SICEM_Blazor.Conceptos.Data {
    public interface IConceptosRepository {
        public IEnumerable<Concepto> ObtenerConceptos();
    }
}