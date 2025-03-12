using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace SICEM_Blazor.Areas.Padron.Models
{
    public class PoblacionResumeResponse
    {
        public string Poblacion { get; set; }
        public bool EsRural { get; set; }
        public int Com { get; set; }
        public int Dom { get; set; }
        public int Hot { get; set; }
        public int Ind { get; set; }
        public int Gen { get; set; }
        public int Otros { get; set; }
        public int Activo { get; set; }
        public int SalaEspera { get; set; }
        public int Inactivo { get; set; }
        public int Total { get; set; }
        public int Habitantes { get; set; }
        
        public override string ToString()
        {
            return $"Poblacion: {Poblacion}, EsRural: {EsRural}, Com: {Com}, Dom: {Dom}, Hot: {Hot}, Ind: {Ind}, Gen: {Gen}, Otros: {Otros}, Activo: {Activo}, Inactivo: {Inactivo}, Total: {Total}";
        }

        public static PoblacionResumeResponse FromDataReader(SqlDataReader reader) {
            var poblacionResumeResponse = new PoblacionResumeResponse
            {
                Poblacion = reader["poblacion"].ToString(),
                EsRural = Convert.ToBoolean(reader["es_rural"]),
                Com = Convert.ToInt32(reader["com"]),
                Dom = Convert.ToInt32(reader["dom"]),
                Hot = Convert.ToInt32(reader["hot"]),
                Ind = Convert.ToInt32(reader["ind"]),
                Gen = Convert.ToInt32(reader["gen"]),
                Otros = Convert.ToInt32(reader["otros"]),
                Activo = Convert.ToInt32(reader["activo"]),
                Inactivo = Convert.ToInt32(reader["inactivo"]),
                Total = Convert.ToInt32(reader["total"]),
                Habitantes = Convert.ToInt32(reader["habitantes"])
            };

            try {
                poblacionResumeResponse.SalaEspera = Convert.ToInt32(reader["sala_espera"]);
            }
            catch (System.Exception){
                poblacionResumeResponse.SalaEspera = 0;
            }

            return poblacionResumeResponse;
        }
    }
}