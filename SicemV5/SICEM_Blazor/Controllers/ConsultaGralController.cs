using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SICEM_Blazor.Models;
using SICEM_Blazor.Services;

namespace SICEM_Blazor.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ConsultaGralController : ControllerBase
    {

        private readonly ConsultaGralService consultaGralService;
        private readonly ILogger<ConsultaGralController> logger;

        public ConsultaGralController(ILogger<ConsultaGralController> logger, ConsultaGralService cs) {
            this.logger = logger;
            this.consultaGralService = cs;
        }

        [HttpGet]
        [Route("{idOficina:int}/ordenes/{idPadron}")]
        public ActionResult<IEnumerable<ConsultaGral_Ordenesresponse>> OrdenesOrdenes([FromRoute] int idOficina, [FromRoute] string idPadron)
        {
            try{
                return consultaGralService.ConsultaGral_Ordenes(idOficina, idPadron);
            }catch(Exception err){
                logger.LogError(err, "Error at attempting to get the orders of padron '{padron}' of the office '{enlace}'", idPadron, idOficina);
                return Conflict();
            };
        }

        [HttpGet]
        [Route("{idOficina:int}/bitacora/{idPadron}")]
        public ActionResult<IEnumerable<ConsultaGral_ModificacionesABCResponse>> ObtenerBitacoraABC([FromRoute] int idOficina, [FromRoute] string idPadron)
        {
            try {
                return consultaGralService.ConsultaGral_ModificacionesABC(idOficina, idPadron);
            } catch(Exception err) {
                logger.LogError(err, "Error at attempting to get the modifications of padron '{padron}' of the office '{enlace}'", idPadron, idOficina);
                return Conflict();
            }
        }

        [HttpGet]
        [Route("{idOficina:int}/movimientos/{idPadron:int}")]
        public ActionResult<IEnumerable<ConsultaGreal_MovimientosResponse>> ObtenerMovimientos([FromRoute] int idOficina, [FromRoute] int idPadron)
        {
            try {
                return consultaGralService.ConsultaGral_Movimientos(idOficina, idPadron);
            } catch(Exception err) {
                logger.LogError(err, "Error at attempting to get the modifications of padron '{padron}' of the office '{enlace}'", idPadron, idOficina);
                return Conflict();
            }
        }

    }
}
