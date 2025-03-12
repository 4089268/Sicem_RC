using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using SICEM_Blazor.Data;
using SICEM_Blazor.Eficiencia.Models;
using SICEM_Blazor.Models;
using SICEM_Blazor.Services;

namespace SICEM_Blazor.Eficiencia.Data {
    public interface IEficienciaService{
        
        public EficienciaResumen ObtenerResumenEnlace(IEnlace enlace, int anio, int mes, int sb, int sec, bool soloPropios);
        public EficienciaResumenVolumen ObtenerResumenVolumenEnlace(IEnlace enlace, int anio, int mes, int sb, int sec, bool soloPropios);
        public IEnumerable<EficienciaResumen> ObtenerResumenAnual(IEnlace enlace, int anio, int sb, int sec, bool agregarTotal);

        public dynamic[] ObtenerEficienciaPorOficinas(Ruta[] oficinas, DateTime fecha1, DateTime fecha2, bool agregarTotal = true);
        public Eficiencia_Sectores_Resp ObtenerEficienciaSectores(int Id_Oficina, int Ano, int Mes, int Sb);
        public Eficiencia_Colonia[] ObtenerEficienciaColonias(int Id_Oficina, int Ano, int Mes, int Sb, int Sect, int Tipo);
        public Eficiencia_Conceptos[] ObtenerEficienciaConceptos( int Id_Oficina, int Ano, int Mes, int Sb, int Sect);
        public Eficiencia_Detalle[] ObtenerEficienciaDetalleSector(int Id_Oficina, int Ano, int Mes, int Sb, int Sect, int Tipo);
        public Eficiencia_Detalle[] ObtenerEficienciaDetalleColonia(int Id_Oficina, int Ano, int Mes, int Sb, int Sect, int Tipo, int IdColonia, int IdLocalidad);
        public IEnumerable<EficienciaLocalidad> EficienciaPorLocalidades(IEnlace enlace, int anio, int mes, int sb, int sec, bool soloPropios );
        public IEnumerable<EficienciaImpPoblacionTarifa> ObtenerEficienciaPoblacionesTarifa(IEnlace enlace, int anio, int mes, int sb, int sec, int id_poblacion, bool soloPropios );
        public IEnumerable<EficienciaImpTarifa> ObtenerEficienciaTarifas(IEnlace enlace, int anio, int mes, int sb, int sec, bool soloPropios);


        public IEnumerable<EficienciaVolumenTarifa> ObtenerResumenVolumenTarifas(IEnlace enlace, int anio, int mes, int sb, int sec, bool soloPropios);
        public IEnumerable<EficienciaResumenVolumen> ObtenerResumenVolumenAnual(IEnlace enlace, int anio, int sb, int sec, bool agregarTotal);
        public IEnumerable<EficienciaVolumenPoblacion> ObtenerResumenVolumenPoblaciones(IEnlace enlace, int anio, int mes, int sb, int sec, bool soloPropios);
        public IEnumerable<EficienciaVolumenPoblacionTarifa> ObtenerResumenVolumenPoblacionesTarifa(IEnlace enlace, int anio, int mes, int sb, int sec, int id_poblacion, bool soloPropios = true );


        public EficienciResumenUsuario ObtenerResumenUsuariosEnlace(IEnlace enlace, int anio, int mes, int sb, int sec, bool soloPropios );

        public IEnumerable<EficienciResumenUsuario> ObtenerResumenUsuariosAnual(IEnlace enlace, int anio, int sb, int sec, bool soloPropios);

        public IEnumerable<EficienciaUsuarioTarifa> ObtenerResumenUsuariosTarifas(IEnlace enlace, int anio, int mes, int sb, int sec, bool soloPropios);
        
        public IEnumerable<EficienciaUsuarioPoblacion> ObtenerResumenUsuariosPoblaciones(IEnlace enlace, int anio, int mes, int sb, int sec, bool soloPropios);
        
        public IEnumerable<EficienciaUsuarioPoblacionTarifa> ObtenerResumenUsuariosPoblacionesTarifa(IEnlace enlace, int anio, int mes, int sb, int sec, int id_poblacion, bool soloPropios);
        
    }
}