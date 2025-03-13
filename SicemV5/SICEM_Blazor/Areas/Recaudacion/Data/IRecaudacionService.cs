using System;
using System.Collections.Generic;
using SICEM_Blazor.Data;
using SICEM_Blazor.Recaudacion.Models;

namespace SICEM_Blazor.Recaudacion.Data {
    public interface IRecaudacionService {

        public ResumenOficina ObtenerResumen(IEnlace enlace, DateRange dateRange);
        public Recaudacion_Analitico ObtenerAnalisisIngresos(IEnlace enlace, DateRange dateRange);
        public IEnumerable<Recaudacion_Rezago> ObtenerRezago(IEnlace enlace, DateTime desde, DateTime hasta, int sb, int sect);
        public IEnumerable<Recaudacion_IngresosDias> ObtenerIngresosPorDias(IEnlace enlace, DateTime desde, DateTime hasta, int sb, int sect);
        public IEnumerable<Recaudacion_IngresosCajas> ObtenerIngresosPorCajas(IEnlace enlace, DateTime desde, DateTime hasta, int sb, int sect);
        public IEnumerable<IngresosxConceptos> ObtenerIngresosPorConceptos(IEnlace enlace, DateTime desde, DateTime hasta, int sb, int sect);
        public IEnumerable<IngresosTipoUsuario> ObtenerIngresosPorTipoUsuarios(IEnlace enlace, DateTime desde, DateTime hasta, int sb, int sect);
        public IEnumerable<Ingresos_FormasPago> ObtenerIngresosPorFormasPago(IEnlace enlace, DateTime desde, DateTime hasta, int sb, int sect);
        public Recaudacion_PagosMayores_Response ObtenerPagosMayores(IEnlace enlace, DateTime desde, DateTime hasta, int sb, int sect, int total);
        public IEnumerable<Recaudacion_IngresosDetalle> ObtenerDetalleIngresos(IEnlace enlace, DateTime desde, DateTime hasta, int sb, int sect);
        public IEnumerable<Recaudacion_IngresosDetalleConceptos> ObtenerDetalleConceptos(IEnlace enlace, DateTime desde, DateTime hasta, int sb, int sect, int id_poblacion, int id_colonia);
        public IEnumerable<Recaudacion_Rezago_Detalle> ObtenerDetalleRezago(IEnlace enlace, DateTime desde, DateTime hasta, int sub, int sect, int mes, bool acumulativo);
        public IEnumerable<ConceptoTipoUsuario> ObtenerRecaudacionPorConceptosYTipoUsuario(IEnlace enlace, DateTime desde, DateTime hasta, int sub, int sect );
        public IEnumerable<RecaudacionIngresosxPoblaciones> ObtenerRecaudacionLocalidades(IEnlace enlace, DateTime desde, DateTime hasta, int sb, int sec);
        public IEnumerable<RecaudacionIngresosxColonias> ObtenerRecaudacionColonias(IEnlace enlace, DateTime desde, DateTime hasta, int sb, int sec, int idLocalidad);
        public IEnumerable<Recaudacion_IngresosDetalleConceptos> ObtenerIngresosConceptosPorLocalidadColonia(IEnlace enlace, DateTime desde, DateTime hasta, int sb, int sect, int idLocalidad, int idColonia);
        public IEnumerable<Ingresos_Conceptos> ObtenerIngresosPorConceptosTipoUsuarios(IEnlace enlace, DateTime desde , DateTime hasta, int sb , int sect, int idLocalidad);

    }

}
