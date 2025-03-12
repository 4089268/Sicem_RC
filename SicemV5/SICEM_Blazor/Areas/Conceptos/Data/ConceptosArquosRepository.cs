using System;
using System.Collections.Generic;
using System.Linq;
using SICEM_Blazor.Conceptos.Models;
using SICEM_Blazor.Data;
using SICEM_Blazor.Models.Entities.Arquos;

namespace SICEM_Blazor.Conceptos.Data {

    public class ConceptosArquosRepository : IConceptosRepository
    {
        
        public readonly IEnlace enlace;
        public ConceptosArquosRepository(IEnlace enlace){
            this.enlace = enlace;
        }

        public IEnumerable<Concepto> ObtenerConceptos()
        {
            try{
                var conceptos = new List<Concepto>();
                var _tmpData = new List<CatConcepto>();
                using(var context = new ArquosContext(enlace.GetConnectionString())){
                    _tmpData = context.CatConceptos.ToList();
                }

                var conceptosAdapter = new ConceptosAdapter();
                conceptos = _tmpData.Select(item => conceptosAdapter.FromCatConcepto(item)).ToList();
                
                return conceptos;
            }catch(Exception){
                return new Concepto[]{};
            }
        }
    }

}