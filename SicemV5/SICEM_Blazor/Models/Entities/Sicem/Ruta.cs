using System;
using System.Collections.Generic;
using SICEM_Blazor.Data;

namespace SICEM_Blazor.Models
{
    public partial class Ruta : IEnlace
    {
        public Ruta()
        {
            ModsOficinas = new HashSet<ModsOficina>();
            RutasLocations = new HashSet<RutasLocation>();
        }

        public int Id { get; set; }
        public string Oficina { get; set; }
        public string Ruta1 { get; set; }
        public bool? Inactivo { get; set; }
        public string Servidor { get; set; }
        public string BaseDatos { get; set; }
        public string Usuario { get; set; }
        public string Contraseña { get; set; }
        public short? IdZona { get; set; }
        public bool? Desconectado { get; set; }
        public bool? Alterno { get; set; }
        public string ServidorA { get; set; }
        public string Alias { get; set; }

        public virtual ICollection<ModsOficina> ModsOficinas { get; set; }
        public virtual ICollection<RutasLocation> RutasLocations { get; set; }

        #region Implementación de IEnlace
        public string StringConection {
            get {
                var _connectionBuilder = new System.Data.SqlClient.SqlConnectionStringBuilder() {
                    DataSource = Alterno == true?this.ServidorA:this.Servidor,
                    InitialCatalog = this.BaseDatos,
                    UserID = this.Usuario,
                    Password = this.Contraseña,
                    ConnectTimeout = 900,
                    ApplicationName = "SICEM"
                };
                return _connectionBuilder.ConnectionString;
            }
        }
        public string Nombre => Oficina;
        public string GetConnectionString() => StringConection;

        #endregion
    }
}
