using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SICEM_Blazor.Data.Filtros;
using SICEM_Blazor.Services;
using SICEM_Blazor.Models;
using SICEM_Blazor.Recaudacion.Data;


namespace SICEM_Blazor.Controllers{

    [ApiController]
    [Route("api/[controller]")]
    [ServiceFilter(typeof(LogeadoFilter))]
    public class RecaudacionController: ControllerBase{
        private readonly SicemService sicemService;
        private readonly IRecaudacionService recaudacionService;
        
        public RecaudacionController( SicemService s, IRecaudacionService r){
            this.sicemService = s;
            this.recaudacionService = r;
        }

        [HttpGet]
        [Route("/api/recaudacion/{idOficina:int}")]        
        public IActionResult ConsultaIngresos_1(int idOficina, [FromQuery] int a, [FromQuery] int m, [FromQuery] int sb = 0, [FromQuery] int se = 0 ){
            var usuario = (Usuario) HttpContext.Items["usuario"];
            var oficinas = (int[]) HttpContext.Items["oficinas"];

            if(!oficinas.Contains(idOficina)){
                return Unauthorized();
            }

            var oficina = sicemService.ObtenerEnlaces(idOficina).FirstOrDefault();
            var _fecha = new DateTime(a, m, 1);
            var result = recaudacionService.ObtenerResumen(oficina, _fecha, _fecha, sb, se);
            return Ok(result);
        }

        
        [HttpGet]
        [Route("/api/recaudacion/dias/{idOficina:int}")]
        public IActionResult IngresosPorDias(int idOficina, [FromQuery] int a, [FromQuery] int m, [FromQuery] int sb = 0, [FromQuery] int se = 0 ){
            var usuario = (Usuario) HttpContext.Items["usuario"];
            var oficinas = (int[]) HttpContext.Items["oficinas"];

            if(!oficinas.Contains(idOficina)){
                return Unauthorized();
            }
            var oficina = sicemService.ObtenerEnlaces(idOficina).FirstOrDefault();
            var _fecha = new DateTime(a, m, 1);
            var result = recaudacionService.ObtenerIngresosPorDias(oficina, _fecha, _fecha, sb, se);
            return Ok(result);
        }

        [HttpGet]
        [Route("/api/recaudacion/cajas/{idOficina:int}")]
        public IActionResult IngresosPorCajas(int idOficina, [FromQuery] int a, [FromQuery] int m, [FromQuery] int sb = 0, [FromQuery] int se = 0 ){
            var usuario = (Usuario) HttpContext.Items["usuario"];
            var oficinas = (int[]) HttpContext.Items["oficinas"];

            if(!oficinas.Contains(idOficina)){
                return Unauthorized();
            }
            var oficina = sicemService.ObtenerEnlaces(idOficina).FirstOrDefault();
            var fecha1 = new DateTime(a, m, 1);
            var fecha2 = new DateTime(a, m, DateTime.DaysInMonth(a,m));
            var result = recaudacionService.ObtenerIngresosPorCajas(oficina, fecha1, fecha2, sb, se);
            return Ok(result);
        }

        [HttpGet]
        [Route("/api/recaudacion/conceptos/{idOficina:int}")]
        public IActionResult IngresosPorConceptos(int idOficina, [FromQuery] int a, [FromQuery] int m, [FromQuery] int sb = 0, [FromQuery] int se = 0 ){
            var usuario = (Usuario) HttpContext.Items["usuario"];
            var oficinas = (int[]) HttpContext.Items["oficinas"];

            if(!oficinas.Contains(idOficina)){
                return Unauthorized();
            }

            var oficina = sicemService.ObtenerEnlaces(idOficina).FirstOrDefault();
            var fecha1 = new DateTime(a, m, 1);
            var fecha2 = new DateTime(a, m, DateTime.DaysInMonth(a,m));
            var result = recaudacionService.ObtenerIngresosPorConceptos(oficina, fecha1, fecha2, sb, se);
            return Ok(result);
        }

        [HttpGet]
        [Route("/api/recaudacion/tiposUsuarios/{idOficina:int}")]
        public IActionResult IngresosPorTiposUsuario(int idOficina, [FromQuery] int a, [FromQuery] int m, [FromQuery] int sb = 0, [FromQuery] int se = 0 ){
            var usuario = (Usuario) HttpContext.Items["usuario"];
            var oficinas = (int[]) HttpContext.Items["oficinas"];

            if(!oficinas.Contains(idOficina)){
                return Unauthorized();
            }

            var oficina = sicemService.ObtenerEnlaces(idOficina).FirstOrDefault();
            var fecha1 = new DateTime(a, m, 1);
            var fecha2 = new DateTime(a, m, DateTime.DaysInMonth(a,m));
            var result = recaudacionService.ObtenerIngresosPorTipoUsuarios(oficina, fecha1, fecha2, sb, se);
            return Ok(result);
        }

        [HttpGet]
        [Route("/api/recaudacion/poblaciones/{idOficina:int}")]
        public IActionResult IngresosPorPoblaciones(int idOficina, [FromQuery] int a, [FromQuery] int m, [FromQuery] int sb = 0, [FromQuery] int se = 0 ){
            var usuario = (Usuario) HttpContext.Items["usuario"];
            var oficinas = (int[]) HttpContext.Items["oficinas"];

            if(!oficinas.Contains(idOficina)){
                return Unauthorized();
            }

            var oficina = sicemService.ObtenerEnlaces(idOficina).FirstOrDefault();
            var fecha1 = new DateTime(a, m, 1);
            var fecha2 = new DateTime(a, m, DateTime.DaysInMonth(a,m));
            var result = recaudacionService.ObtenerRecaudacionLocalidades(oficina, fecha1, fecha2, sb, se);
            return Ok(result);
        }

        [HttpGet]
        [Route("/api/recaudacion/tiposPago/{idOficina:int}")]
        public IActionResult IngresosPorTiposPago(int idOficina, [FromQuery] int a, [FromQuery] int m, [FromQuery] int sb = 0, [FromQuery] int se = 0 ){
            var usuario = (Usuario) HttpContext.Items["usuario"];
            var oficinas = (int[]) HttpContext.Items["oficinas"];

            if(!oficinas.Contains(idOficina)){
                return Unauthorized();
            }

            var oficina = sicemService.ObtenerEnlaces(idOficina).FirstOrDefault();
            var fecha1 = new DateTime(a, m, 1);
            var fecha2 = new DateTime(a, m, DateTime.DaysInMonth(a,m));
            var result = recaudacionService.ObtenerIngresosPorFormasPago(oficina, fecha1, fecha2, sb, se);
            return Ok(result);
        }

        [HttpGet]
        [Route("/api/recaudacion/altosIngresos/{idOficina:int}")]
        public IActionResult IngresosAltos(int idOficina, [FromQuery] int a, [FromQuery] int m, [FromQuery] int sb = 0, [FromQuery] int se = 0, [FromQuery] int t = 10 ){
            var usuario = (Usuario) HttpContext.Items["usuario"];
            var oficinas = (int[]) HttpContext.Items["oficinas"];

            if(!oficinas.Contains(idOficina)){
                return Unauthorized();
            }

            var oficina = sicemService.ObtenerEnlaces(idOficina).FirstOrDefault();
            var fecha1 = new DateTime(a, m, 1);
            var fecha2 = new DateTime(a, m, DateTime.DaysInMonth(a,m));
            var result = recaudacionService.ObtenerPagosMayores(oficina, fecha1, fecha2, sb, se, t);
            return Ok(result);
        }

        [HttpGet]
        [Route("/api/recaudacion/analitico/{idOficina:int}")]        
        public IActionResult Analitico(int idOficina, [FromQuery] int a, [FromQuery] int m, [FromQuery] int sb = 0, [FromQuery] int se = 0 ){
            var usuario = (Usuario) HttpContext.Items["usuario"];
            var oficinas = (int[]) HttpContext.Items["oficinas"];

            if(!oficinas.Contains(idOficina)){
                return Unauthorized();
            }

            var oficina = sicemService.ObtenerEnlaces(idOficina).FirstOrDefault();
            var fecha1 = new DateTime(a, m, 1);
            var fecha2 = new DateTime(a, m, DateTime.DaysInMonth(a,m));
            var result = recaudacionService.ObtenerAnalisisIngresos(oficina, fecha1, fecha2,sb, se);
            return Ok(result);
        }

        [HttpGet]
        [Route("/api/recaudacion/rezago/{idOficina:int}")]
        public IActionResult Rezago(int idOficina, [FromQuery] int a, [FromQuery] int m, [FromQuery] int sb = 0, [FromQuery] int se = 0){
            var usuario = (Usuario) HttpContext.Items["usuario"];
            var oficinas = (int[]) HttpContext.Items["oficinas"];

            if(!oficinas.Contains(idOficina)){
                return Unauthorized();
            }

            var oficina = sicemService.ObtenerEnlaces(idOficina).FirstOrDefault();
            var fecha1 = new DateTime(a, m, 1);
            var fecha2 = new DateTime(a, m, DateTime.DaysInMonth(a,m));
            var result = recaudacionService.ObtenerRezago(oficina, fecha1, fecha2, sb, se);
            return Ok(result);
        }

    }
}