﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Extensions.Configuration;
using SICEM_Blazor.Data;
using SICEM_Blazor.Models;
using SICEM_Blazor.Models.Entities.Arquos;
using SICEM_Blazor.Pages.AnalisisInformacion;
using Syncfusion.Blazor.Data;
using Microsoft.Extensions.Logging;

namespace SICEM_Blazor.Services {
    public class SicemService {

        public readonly IConfiguration appSettings;
        private readonly SicemContext sicemContext;
        private readonly SessionService sessionService;
        private readonly ILogger<SicemService> logger;


        public IUsuario Usuario { get; set;}
        public string IdSession {get;set;} = "";

        const string secret = "*SICEMB*";

        public SicemService(SicemContext context, IConfiguration c, SessionService s, ILogger<SicemService> lg) {
            this.sicemContext = context;
            appSettings = c;
            sessionService = s;
            this.logger = lg;
        }


        // ****** Funciones Sesiones ******
        public string Logearse(string usuario, string pass, string ipAddress = "") {
            var _cadConexion = appSettings.GetConnectionString("SICEM");

            try {
                
                //*** Validar usando base ded atos SICEM
                var _usuario = new Usuario();
                using(var xConnecton = new SqlConnection(_cadConexion)) {
                    xConnecton.Open();
                    var xCommand = new SqlCommand();
                    xCommand.Connection = xConnecton;
                    xCommand.CommandText = $"Exec [api].[logearse] @alias = 'LOGEARSE', @user = '{usuario}', @password = '{pass}', @Secret = '{secret}', @ipAddress = '{ipAddress}'";
                    using(SqlDataReader xReader = xCommand.ExecuteReader()) {
                        if(xReader.Read()) {
                            _usuario.Id = int.Parse(xReader["id"].ToString());
                        }else{
                            _usuario.Id = -1;
                        }
                    }
                    xConnecton.Close();
                }

                //*** Validar usando datos oficina
                var _usuarioExt = new UsuarioExterno();
                if(_usuario.Id < 0){
                    var _enlace = ObtenerEnlaces().Where( item => item.Oficina.ToLower() == usuario.ToLower() || item.Alias.ToLower() == usuario.ToLower() ).FirstOrDefault();
                    if(_enlace != null){
                        Console.WriteLine(">>"+_enlace.GetConnectionString());
                        using( var sqlConnection = new SqlConnection(_enlace.GetConnectionString())){
                            sqlConnection.Open();
                            var _query = $"Select id_usuario, nombre, usuario From [Global].[Sys_Usuarios] Where IsNull(inactivo,0) = 0 and id_jerarquia = 1 and usuario = '{pass}'";
                            Console.WriteLine($"oficina: {_enlace.Nombre} query:"+_query);
                            var _command = new SqlCommand(_query, sqlConnection);
                            using( var reader = _command.ExecuteReader()){
                                if(reader.Read()){
                                    _usuarioExt.Id = reader["id_usuario"].ToString();
                                    _usuarioExt.Nombre = reader["nombre"].ToString();
                                    _usuarioExt.Usuario = reader["usuario"].ToString();
                                    _usuarioExt.SetEnlaces( new IEnlace[]{_enlace});
                                }else{
                                    return "El usuario y/o contraseña son incorrectas,";
                                }
                            }
                            sqlConnection.Close();
                        }
                    }else{
                        return "El usuario y/o contraseña son incorrectas";
                    }
                }
        


                //*** Cargar usuario sicem
                if(_usuario.Id >= 0 ){
                    _usuario = sicemContext.Usuarios.Where(item => item.Id == _usuario.Id).FirstOrDefault();
                    if(_usuario != null){

                        //**** Cargar Oficinas del usuario
                        var idEnlaces = _usuario.Oficinas.Split(";").Select( item => ConvertUtils.ParseInteger(item,-1)).ToList<int>();
                        var _tmpEnlaces = ObtenerEnlaces().Where(item => idEnlaces.Contains(item.Id)).ToList();
                        _usuario.SetEnlaces( _tmpEnlaces);

                        //**** Cargar Opciones disponibles
                        var _catOpciones = ObtenerListaOpcionesDelUsuario(_usuario.Id).ToList<IOpcionSistema>();
                        _usuario.SetOpciones(_catOpciones);
                        
                        this.Usuario = _usuario;
                        
                    }else{
                        return "El usuario y/o contraseña son incorrectas.";
                    }
                }else{
                    this.Usuario = _usuarioExt;
                }

                this.IdSession = sessionService.IniciarSesion(Usuario, ipAddress);
                return null;
                
            }catch(Exception err) {
                Console.WriteLine($">> Error al logearse: \n\t{err.Message}\n{err.StackTrace}");
                return "Error al conectarse con el servidor, inténtelo mas tarde.";
            }
        }
        public bool CargarUsuarioToken(string token){
            IdSession = token;
            var _tmpUsuario = sessionService.ObtenerUsuario(token);
            
            if( _tmpUsuario == null){
                CerrarSesion();
                return false;
            }

            this.Usuario = _tmpUsuario;
            this.IdSession = token;
            return true;
        }
        public void CerrarSesion(){
            this.Usuario = null;
            sessionService.FinalizarSesion(IdSession);
        }


        // ****** Funciones Usuario ******
        public IEnumerable<IEnlace> ObtenerOficinasDelUsuario(int idUsuario = 0) {
            if(idUsuario == 0) {
                return Usuario.Enlaces.ToArray();
            }else{
                var _usuario = sicemContext.Usuarios.Where(item => item.Id == idUsuario).FirstOrDefault();
                if(_usuario == null){
                    return new IEnlace[]{};
                }else{
                    var idEnlaces = _usuario.Oficinas.Split(";").Select( item => ConvertUtils.ParseInteger(item,-1)).ToList<int>();
                    return ObtenerEnlaces().Where(item => idEnlaces.Contains(item.Id)).ToList();
                }
            }
        }
        public async Task<bool> GuardarCambiosUsuario() {
            int res = 0;
            var _idUsuario = ConvertUtils.ParseInteger(this.Usuario.Id);
            var _usuario = sicemContext.Usuarios.Where( item => item.Id == _idUsuario).FirstOrDefault();
            if(_usuario != null){
                _usuario.Oficinas = this.Usuario.GetCadEnlaces();
                res = await sicemContext.SaveChangesAsync();
            }
            if(res > 0) {
                return true;
            }
            else {
                return false;
            }
        }
        public List<CatOpcione> ObtenerListaOpcionesDelUsuario(int idUsuario = 0) {
            var _listaOpciones = new List<CatOpcione>();
            var _idsOpciones = new List<int>();
            if(idUsuario > 0) {
                _idsOpciones = sicemContext.OprOpciones.Where(item => item.IdUsuario == idUsuario).Select(item => item.IdOpcion).ToList<int>();
            }
            else {
                var _idUsuario = ConvertUtils.ParseInteger(Usuario.Id);
                _idsOpciones = sicemContext.OprOpciones.Where(item => item.IdUsuario == _idUsuario).Select(item => item.IdOpcion).ToList<int>();
            }
            _listaOpciones = sicemContext.CatOpciones.Where(item => _idsOpciones.Contains((int)item.IdOpcion)).ToList();
            return _listaOpciones;
        }
        public List<CatOpcione> ObtenerCatalogoOpciones() {
            List<CatOpcione> _result = new List<CatOpcione>();
            _result = sicemContext.CatOpciones.Where(item => item.Inactivo == false).ToList();
            return _result;
        }
        public Usuario ObtenerUsuarioToken(string token, string ipAddress){
            try{
                Usuario tmpUsuario = null;
                var session = sicemContext.OprSesiones.Where(item => item.Id.ToString() == token && item.IpAddress == ipAddress && item.Caducidad > DateTime.Now ).FirstOrDefault();
                if(session != null){
                    tmpUsuario = sicemContext.Usuarios.Where(item => item.Id == session.IdUsuario).FirstOrDefault();
                }
                return tmpUsuario;
            }catch(Exception err){
                Console.WriteLine($">>> Error al obtener el usuario por token \n\tError:{err.Message}\n\tStacktrace:{err.StackTrace}");
                return null;
            }
        }
        public void CerrarSesionDb(string token, string ipAddress){
            try{                
                var session = sicemContext.OprSesiones.Where(item => item.Id.ToString() == token && item.IpAddress == ipAddress ).FirstOrDefault();
                session.Caducidad = DateTime.Now.AddMinutes(-1);
                sicemContext.OprSesiones.Update(session);
                sicemContext.SaveChanges();
            }catch(Exception err){
                Console.WriteLine($">>> Error al tratar de cerrar la sesion del usuario token \n\tError:{err.Message}\n\tStacktrace:{err.StackTrace}");                
            }
        }


        //****** Midelware ******
        public bool ComprobarOficinaUsuario(int idOficina, out IEnlace ruta){
            var listaRutas = ObtenerOficinasDelUsuario();
            if(listaRutas.Count() <= 0) {
                ruta = null;
                return false;
            }
            if(listaRutas.Select(item => item.Id).ToArray().Contains(idOficina)){
                ruta = listaRutas.Where(item => item.Id == idOficina).FirstOrDefault();
                return true;
            }
            else {
                ruta = null;
                return false;
            }
        }


        // ****** Funciones Generales ******
        public Ruta[] ObtenerEnlaces(int id_oficina = 0) {
            Ruta[] rutas = null;
            try {
                rutas = sicemContext.Rutas.ToArray();
                if(id_oficina > 0) {
                    rutas = rutas.Where(item => item.Id == id_oficina).ToArray();
                }
            } catch(Exception err) {
                Console.WriteLine($">> Error al cargar las oficinas \n\t{err.Message}\n\tStacktrace{err.StackTrace}");
                return new Ruta[]{};
            }
            return rutas;
        }
        public ICollection<Sicem_Colonia> ObtenerCatalogoColonias(int id_oficina) {
            var oficina = ObtenerEnlaces(id_oficina).FirstOrDefault();
            if(oficina == null){
                return null;
            }
            var _result = new List<Sicem_Colonia>();
            using( var context = new ArquosContext(oficina.StringConection)){
                _result = context.CatColonias.Select( item => Sicem_Colonia.FromEntity(item) ).ToList();
            }
            return _result;
        }
        public Sicem_Sucursal[] ObtenerCatalogoSucursales(int id_oficina) {
            var tmpList = new List<Sicem_Sucursal>();
            var xEnlace = ObtenerEnlaces(id_oficina).FirstOrDefault();
            using(var xConnecton = new SqlConnection(xEnlace.StringConection)) {
                xConnecton.Open();
                var xCommand = new SqlCommand();
                xCommand.Connection = xConnecton;
                xCommand.CommandText = string.Format("Select id_sucursal, descripcion, sb, inactivo From [Global].[Cat_Sucursales]");
                using(var xReader = xCommand.ExecuteReader()) {
                    while(xReader.Read()) {
                        int tmpInt = 0;
                        var newItem = new Sicem_Sucursal {
                            Id_Sucursal = int.TryParse(xReader["id_sucursal"].ToString(), out tmpInt) ? tmpInt : 0,
                            Descripcion = xReader["descripcion"].ToString(),
                            Sb = int.TryParse(xReader["sb"].ToString(), out tmpInt) ? tmpInt : 0,
                            Inactivo = int.TryParse(xReader["inactivo"].ToString(), out tmpInt) ? tmpInt : 0
                        };
                        tmpList.Add(newItem);
                    }
                }
            }
            return tmpList.ToArray();
        }   
        public Sicem_Localidad[] ObtenerCatalogoLocalidades(int id_oficina){
            var tmpList = new List<Sicem_Localidad>();
            var _query = "Select id_poblacion, descripcion, id_sucursal, sb, sectores, inactivo From[Padron].[Cat_Poblaciones]";
            var _ruta = ObtenerEnlaces(id_oficina).FirstOrDefault();
            if(_ruta != null) {
                using(var xConeccion = new SqlConnection(_ruta.StringConection)){
                    xConeccion.Open();
                    using(var xCommand = new SqlCommand(_query, xConeccion)){
                        using(SqlDataReader xReader = xCommand.ExecuteReader()){
                            while(xReader.Read()){
                                int tmpId = 0;
                                var newItem = new Sicem_Localidad() {
                                    Id_Poblacion = int.TryParse(xReader["id_poblacion"].ToString(), out tmpId)?tmpId:0,
                                    Descripcion = xReader["descripcion"].ToString(),
                                    Id_sucursal = int.TryParse(xReader["id_sucursal"].ToString(), out tmpId) ? tmpId : 0,
                                    Sb = int.TryParse(xReader["sb"].ToString(), out tmpId) ? tmpId : 0,
                                    Sector =  int.TryParse(xReader["sectores"].ToString(), out tmpId) ? tmpId : 0,
                                    Inactivo = int.TryParse(xReader["inactivo"].ToString(), out tmpId) ? tmpId : 0,
                                };
                                tmpList.Add(newItem);
                            }
                        }
                    }
                    xConeccion.Close();
                }
            }
            return tmpList.ToArray();
        }
        public DatosUsuario ObtenerDatosUsuarios(int id_oficina,int id_padron) {
            var _response = new DatosUsuario();

            var _query = "Select id_padron, id_cuenta, razon_social, rfc, direccion, colonia, ciudad, estado, telefono1, _giro as giro,  _estatus as estatus, " +
                $" _tarifafija as tarifa, _servicio as servicio, id_medidor as medidor, mes_adeudo_act as meses_adeudo From[Padron].[vw_cat_padron] where id_padron = {id_padron}";

            var _enlace = ObtenerEnlaces(id_oficina).FirstOrDefault();
            using(var conexion = new SqlConnection(_enlace.StringConection)) {
                conexion.Open();
                using(var command = new SqlCommand(_query, conexion)){
                    using(SqlDataReader reader = command.ExecuteReader()) {
                        if(reader.Read()) {
                            _response.Id_Padron = int.TryParse(reader["id_padron"].ToString(), out int tmpP) ? tmpP : 0;
                            _response.Id_Cuenta = int.TryParse(reader["id_cuenta"].ToString(), out int tmpC) ? tmpC : 0;
                            _response.Razon_Social = reader["razon_social"].ToString();
                            _response.RFC = reader["rfc"].ToString();
                            _response.Direccion = reader["direccion"].ToString();
                            _response.Colonia = reader["colonia"].ToString();
                            _response.Ciudad = reader["ciudad"].ToString();
                            _response.Estado = reader["estado"].ToString();
                            _response.Telefono1 = reader["telefono1"].ToString();
                            _response.Giro = reader["giro"].ToString();
                            _response.Estatus = reader["estatus"].ToString();
                            _response.Tarifa = reader["tarifa"].ToString();
                            _response.Servicios = reader["servicio"].ToString();
                            _response.Medidor = reader["medidor"].ToString();
                            _response.Meses_Adeudo = int.TryParse(reader["meses_adeudo"].ToString(), out int tmpM) ? tmpM : 0;
                        }
                    }
                }
                conexion.Close();
            }
            return _response;
        }
        public string ObtenerDireccionApi(){
            return appSettings.GetValue<string>("AppSettings:Direccion_Api");
        }
        
        public Dictionary<int, string> CatalogoTarifas(int id_oficina) {
            var tmpCatalogo = new Dictionary<int, string>();
            var xEnlace = ObtenerEnlaces(id_oficina).FirstOrDefault();
            using(var xConnecton = new SqlConnection(xEnlace.StringConection)) {
                xConnecton.Open();
                var xCommand = new SqlCommand();
                xCommand.Connection = xConnecton;
                xCommand.CommandText = string.Format("Select id_tipousuario as id, descripcion From Padron.Cat_TiposUsuario Order By descripcion");
                using(var xReader = xCommand.ExecuteReader()) {
                    while(xReader.Read()) {
                        try {
                            tmpCatalogo.Add(int.Parse(xReader["id"].ToString()), xReader["descripcion"].ToString());
                        }
                        catch(Exception) { }
                    }
                }
            }
            return tmpCatalogo;
        }
        public Dictionary<int,string> ObtenerCatalogoEstatus(int id_oficina, string filtro = ""){
            var result = new Dictionary<int,string>();

            var oficina = ObtenerEnlaces(id_oficina).FirstOrDefault();
            if(oficina == null){
                return null;
            }
            var _datos = new List<SICEM_Blazor.Models.Entities.Arquos.CatEstatus>();
            try{
                using(var context = new ArquosContext(oficina.StringConection)){
                    if(filtro.Trim().Length > 1){
                        _datos = context.CatEstatuses.Where(item => item.IdEstatus == 0 || item.Tabla == filtro).ToList();
                    }else{
                        _datos = context.CatEstatuses.ToList();
                    }
                }
                _datos.ForEach(item => result.Add((int)item.IdEstatus, item.Descripcion.ToUpper() ));
                return result;
            }catch(Exception err){
                Console.WriteLine($">> Error al obtener el catalogo de esatus de la oficina {oficina.Oficina}\n\tError:{err.Message}\n\tStacktrace:{err.StackTrace}");
                return null;
            }
        }
        public Dictionary<int,string> ObtenerCatalogoTipoCalculo(int id_oficina){
            var result = new Dictionary<int,string>();

            var oficina = ObtenerEnlaces(id_oficina).FirstOrDefault();
            if(oficina == null){
                return null;
            }
            var _datos = new List<CatTiposCalculo>();
            try{
                using(var context = new ArquosContext(oficina.StringConection)){
                    _datos = context.CatTiposCalculos.ToList();
                }
                _datos.ForEach(item => result.Add((int)item.IdCalculo, item.Descripcion.ToUpper() ));
                return result;
            }catch(Exception err){
                Console.WriteLine($">> Error al obtener el catalogo tipoCalculo {oficina.Oficina}\n\tError:{err.Message}\n\tStacktrace:{err.StackTrace}");
                return null;
            }
        }
        public Dictionary<int,string> ObtenerCatalogoServicios(int id_oficina){
            var result = new Dictionary<int,string>();

            var oficina = ObtenerEnlaces(id_oficina).FirstOrDefault();
            if(oficina == null){
                return null;
            }
            var _datos = new List<CatServicio>();
            try{
                using(var context = new ArquosContext(oficina.StringConection)){
                    _datos = context.CatServicios.ToList();
                }
                _datos.ForEach(item => result.Add((int)item.IdServicio, item.Descripcion.ToUpper() ));
                return result;
            }catch(Exception err){
                Console.WriteLine($">> Error al obtener el catalogo CatServicio de la oficina {oficina.Oficina}\n\tError:{err.Message}\n\tStacktrace:{err.StackTrace}");
                return null;
            }
        }
        public Dictionary<int,string> ObtenerCatalogoTarifas(int id_oficina){
            var result = new Dictionary<int,string>();

            var oficina = ObtenerEnlaces(id_oficina).FirstOrDefault();
            if(oficina == null){
                return null;
            }
            var _datos = new List<CatTiposUsuario>();
            try{
                using(var context = new ArquosContext(oficina.StringConection)){
                    _datos = context.CatTiposUsuarios.ToList();
                }
                _datos.ForEach(item => result.Add((int)item.IdTipousuario, item.Descripcion.ToUpper() ));
                return result;
            }catch(Exception err){
                Console.WriteLine($">> Error al obtener el catalogo CatTiposUsuario de la oficina {oficina.Oficina}\n\tError:{err.Message}\n\tStacktrace:{err.StackTrace}");
                return null;
            }
        }
        public Dictionary<int,string> ObtenerCatalogoAnomalias(int id_oficina){
            var result = new Dictionary<int,string>();

            var oficina = ObtenerEnlaces(id_oficina).FirstOrDefault();
            if(oficina == null){
                return null;
            }
            var _datos = new List<CatAnomalias>();
            try{
                using(var context = new ArquosContext(oficina.StringConection)){
                    _datos = context.CatAnomaliases.ToList();
                }
                _datos.ForEach(item => result.Add((int)item.IdAnomalia, item.Descripcion.ToUpper() ));
                return result;
            }catch(Exception err){
                Console.WriteLine($">> Error al obtener el catalogo CatAnomalias de la oficina {oficina.Oficina}\n\tError:{err.Message}\n\tStacktrace:{err.StackTrace}");
                return null;
            }
        }
        public Dictionary<int,string> ObtenerCatalogoGiros(int id_oficina){
            var result = new Dictionary<int,string>();

            var oficina = ObtenerEnlaces(id_oficina).FirstOrDefault();
            if(oficina == null){
                return null;
            }
            var _datos = new List<CatGiro>();
            try{
                using(var context = new ArquosContext(oficina.StringConection)){
                    _datos = context.CatGiros.ToList();
                }
                _datos.ForEach(item => result.Add((int)item.IdGiro, item.Descripcion.ToUpper() ));
                return result;
            }catch(Exception err){
                Console.WriteLine($">> Error al obtener el catalogo CatGiro de la oficina {oficina.Oficina}\n\tError:{err.Message}\n\tStacktrace:{err.StackTrace}");
                return null;
            }
        }
        public Dictionary<int,string> ObtenerCatalogoClaseUsuario(int id_oficina){
            var result = new Dictionary<int,string>();

            var oficina = ObtenerEnlaces(id_oficina).FirstOrDefault();
            if(oficina == null){
                return null;
            }
            var _datos = new List<CatClasesUsuario>();
            try{
                using(var context = new ArquosContext(oficina.StringConection)){
                    _datos = context.CatClasesUsuarios.ToList();
                }
                _datos.ForEach(item => result.Add((int)item.IdClaseUsuario, item.Descripcion.ToUpper() ));
                return result;
            }catch(Exception err){
                Console.WriteLine($">> Error al obtener el catalogo CatClasesUsuario de la oficina {oficina.Oficina}\n\tError:{err.Message}\n\tStacktrace:{err.StackTrace}");
                return null;
            }
        }
        
        public Dictionary<int,string> CatalogoEstatusPadron(){
            var _result = new Dictionary<int, string>();
            // _result.Add(0, "SIN ESPECIFICAR");
            _result.Add(1, "ACTIVO");
            _result.Add(2, "SALA DE ESPERA");
            _result.Add(4, "BAJA DEFINITIVA");
            _result.Add(51, "CONGELADO");
            return _result;
        }


        //****** Historial Sesiones ******//
        public List<SesionInfo> ObtenerHistorialSessiones(DateTime fecha1, DateTime fecha2){
            var _result = new List<SesionInfo>();

            _result = sicemContext.OprSesiones.Where(item => (item.Inicio > fecha1 && item.Inicio < fecha2) || (item.Caducidad > fecha1 && item.Caducidad < fecha2) )
                .OrderByDescending(item => item.Inicio)
                .Select( item => SesionInfo.FromEntity(item))
                .ToList();
                
            _result.ForEach( sesion => {
                var _usu = sicemContext.Usuarios.FirstOrDefault(item => item.Id == sesion.IdUsuario );
                sesion.Usuario = _usu == null?"":_usu.Nombre;
            });

            return _result;

        }

        //****** Consulta Vw Cat Padron ******//
        //TODO: move to own service
        public List<CatPadron> ObtenerAnalisisInfo(ICollection<Ruta> oficinas, AnalisysInfoFilter filtro){
            var _result = new List<CatPadron>();
            
            var _tareas = new List<Task<List<CatPadron>>>();
            foreach(var ofi in oficinas){
                var _tmpTask = Task.Run<List<CatPadron>>( () => { return AnalisisInfoOfi2(ofi, filtro);} );
                _tareas.Add(_tmpTask);
            }

            Task.WaitAll(_tareas.ToArray());

            foreach( var _tarea in _tareas ){
                var _awaiter = _tarea.GetAwaiter();
                var _result_c = _awaiter.GetResult();
                if(_result_c != null){
                    _result.AddRange(_result_c);
                }
            }
                
            return _result;
        }
        private List<CatPadron> AnalisisInfoOfi(Ruta ofi, AnalisysInfoFilter filtro){
            Console.WriteLine($">> Iniciando Analisis Info Oficina: {ofi.Oficina}");
            var _result = new List<CatPadron>();
            try{
                var _datos = new List<VwCatPadron>();
                using( var _arquosContext = new ArquosContext(ofi.StringConection)){
                    _datos = _arquosContext.VwCatPadrons.OrderByDescending( item => item.FechaAlta ).ToList();
                }

                //****** Aplicar Filtro
                if(filtro.RazonSocial.Trim().Length > 1){
                    _datos = _datos.Where( item => item.RazonSocial.ToLower().Contains(filtro.RazonSocial.ToLower())).ToList();
                }
                if(filtro.Direccion.Trim().Length > 1){
                    _datos = _datos.Where( item => (item.Direccion + item.CallePpal + item.CalleLat1 + item.CalleLat2).ToLower().Contains(filtro.Direccion.ToLower())).ToList();
                }
                if(filtro.Colonia.Trim().Length > 1){
                    _datos = _datos.Where( item => item.Colonia.ToLower().Contains(filtro.Colonia.ToLower())).ToList();
                }

                if(filtro.Id_Estatus.Count() > 0){
                    _datos = _datos.Where( item => filtro.Id_Estatus.Contains((int)item.IdEstatus)).ToList();
                }
                if(filtro.Id_Servicios.Count() > 0){
                    _datos = _datos.Where( item => filtro.Id_Servicios.Contains((int)item.IdServicio)).ToList();
                }
                if(filtro.Id_Tarifas.Count() > 0){
                    _datos = _datos.Where( item => filtro.Id_Tarifas.Contains((int)item.IdTarifa)).ToList();
                }
                if(filtro.Id_TipoCalculo.Count() > 0){
                    _datos = _datos.Where( item => filtro.Id_TipoCalculo.Contains((int)item.IdTipocalculo)).ToList();
                }
                if(filtro.Id_Anomalias.Count() > 0){
                    _datos = _datos.Where( item => filtro.Id_Anomalias.Contains((int)item.IdAnomaliaAct)).ToList();
                }
                if(filtro.Id_Giro.Count() > 0){
                    _datos = _datos.Where( item => filtro.Id_Giro.Contains((int)item.IdGiro)).ToList();
                }
                if(filtro.Id_ClaseUsuario.Count() > 0){
                    _datos = _datos.Where( item => filtro.Id_ClaseUsuario.Contains((int)item.IdClaseusuario)).ToList();
                }

                if(filtro.ImporteTarifaAgua_Opcion > 0){
                    switch (filtro.ImporteTarifaAgua_Opcion){
                        case 1: //Mayor Que
                            _datos = _datos.Where( item => (item.ImporteFijo??0m) >= filtro.ImporteTarifaAgua_Valor1).ToList();
                            break;

                        case 2: //Menor Que
                            _datos = _datos.Where( item => (item.ImporteFijo??0m) <= filtro.ImporteTarifaAgua_Valor1).ToList();
                            break;

                        case 3: //Entre
                            _datos = _datos.Where( item => (item.ImporteFijo??0m) >= filtro.ImporteTarifaAgua_Valor1 && (item.ImporteFijo??0m) <= filtro.ImporteTarifaAgua_Valor2).ToList();
                            break;

                        case 4: //Igual
                            _datos = _datos.Where( item => (item.ImporteFijo??0m) == filtro.ImporteTarifaAgua_Valor1).ToList();
                            break;
                    }
                }
                if(filtro.ImporteTarifaDren_Opcion > 0){
                    switch (filtro.ImporteTarifaDren_Opcion){
                        case 1: //Mayor Que
                            _datos = _datos.Where( item => (item.ImporteFijoDren??0m) >= filtro.ImporteTarifaDren_Valor1).ToList();
                            break;

                        case 2: //Menor Que
                            _datos = _datos.Where( item => (item.ImporteFijoDren??0m) <= filtro.ImporteTarifaDren_Valor1).ToList();
                            break;

                        case 3: //Entre
                            _datos = _datos.Where( item => (item.ImporteFijoDren??0m) >= filtro.ImporteTarifaDren_Valor1 && (item.ImporteFijoDren??0m) <= filtro.ImporteTarifaDren_Valor2).ToList();
                            break;

                        case 4: //Igual
                            _datos = _datos.Where( item => (item.ImporteFijoDren??0m) == filtro.ImporteTarifaDren_Valor1).ToList();
                            break;
                    }
                }
                if(filtro.ImporteTarifaSane_Opcion > 0){
                    switch (filtro.ImporteTarifaSane_Opcion){
                        case 1: //Mayor Que
                            _datos = _datos.Where( item => (item.ImporteFijoSane??0m) >= filtro.ImporteTarifaSane_Valor1).ToList();
                            break;

                        case 2: //Menor Que
                            _datos = _datos.Where( item => (item.ImporteFijoSane??0m) <= filtro.ImporteTarifaSane_Valor1).ToList();
                            break;

                        case 3: //Entre
                            _datos = _datos.Where( item => (item.ImporteFijoSane??0m) >= filtro.ImporteTarifaSane_Valor1 && (item.ImporteFijoSane??0m) <= filtro.ImporteTarifaSane_Valor2).ToList();
                            break;

                        case 4: //Igual
                            _datos = _datos.Where( item => (item.ImporteFijoSane??0m) == filtro.ImporteTarifaSane_Valor1).ToList();
                            break;
                    }
                }
                if(filtro.Consumo_Opcion > 0){
                    switch (filtro.Consumo_Opcion){
                        case 1: //Mayor Que
                            _datos = _datos.Where( item => (item.ConsumoAct??0m) >= filtro.Consumo_Valor1).ToList();
                            break;

                        case 2: //Menor Que
                            _datos = _datos.Where( item => (item.ConsumoAct??0m) <= filtro.Consumo_Valor1).ToList();
                            break;

                        case 3: //Entre
                            _datos = _datos.Where( item => (item.ConsumoAct??0m) >= filtro.Consumo_Valor1 && (item.ConsumoAct??0m) <= filtro.Consumo_Valor2).ToList();
                            break;

                        case 4: //Igual
                            _datos = _datos.Where( item => (item.ConsumoAct??0m) == filtro.Consumo_Valor1).ToList();
                            break;
                    }
                }
                
                if(filtro.MesesAdeudo_Opcion > 0){
                    switch (filtro.MesesAdeudo_Opcion){
                        case 1: //Mayor Que
                            _datos = _datos.Where( item => (item.MesAdeudoAct??0m) >= filtro.MesesAdeudo_Valor1).ToList();
                            break;

                        case 2: //Menor Que
                            _datos = _datos.Where( item => (item.MesAdeudoAct??0m) <= filtro.MesesAdeudo_Valor1).ToList();
                            break;

                        case 3: //Entre
                            _datos = _datos.Where( item => (item.MesAdeudoAct??0m) >= filtro.MesesAdeudo_Valor1 && (item.MesAdeudoAct??0m) <= filtro.MesesAdeudo_Valor2).ToList();
                            break;

                        case 4: //Igual
                            _datos = _datos.Where( item => (item.MesAdeudoAct??0m) == filtro.MesesAdeudo_Valor1).ToList();
                            break;
                    }
                }
                if(filtro.LectAct_Opcion > 0){
                    switch (filtro.LectAct_Opcion){
                        case 1: //Mayor Que
                            _datos = _datos.Where( item => (item.LecturaAct??0m) >= filtro.LectAct_Valor1).ToList();
                            break;

                        case 2: //Menor Que
                            _datos = _datos.Where( item => (item.LecturaAct??0m) <= filtro.LectAct_Valor1).ToList();
                            break;

                        case 3: //Entre
                            _datos = _datos.Where( item => (item.LecturaAct??0m) >= filtro.LectAct_Valor1 && (item.LecturaAct??0m) <= filtro.LectAct_Valor2).ToList();
                            break;

                        case 4: //Igual
                            _datos = _datos.Where( item => (item.LecturaAct??0m) == filtro.LectAct_Valor1).ToList();
                            break;
                    }
                }
                if(filtro.LectAnt_Opcion > 0){
                    switch (filtro.LectAnt_Opcion){
                        case 1: //Mayor Que
                            _datos = _datos.Where( item => (item.LecturaAnt??0m) >= filtro.LectAnt_Valor1).ToList();
                            break;

                        case 2: //Menor Que
                            _datos = _datos.Where( item => (item.LecturaAnt??0m) <= filtro.LectAnt_Valor1).ToList();
                            break;

                        case 3: //Entre
                            _datos = _datos.Where( item => (item.LecturaAnt??0m) >= filtro.LectAnt_Valor1 && (item.LecturaAnt??0m) <= filtro.LectAnt_Valor2).ToList();
                            break;

                        case 4: //Igual
                            _datos = _datos.Where( item => (item.LecturaAnt??0m) == filtro.LectAnt_Valor1).ToList();
                            break;
                    }
                }
                if(filtro.Promedio_Opcion > 0){
                    switch (filtro.Promedio_Opcion){
                        case 1: //Mayor Que
                            _datos = _datos.Where( item => (item.PromedioAct??0m) >= filtro.Promedio_Valor1).ToList();
                            break;

                        case 2: //Menor Que
                            _datos = _datos.Where( item => (item.PromedioAct??0m) <= filtro.Promedio_Valor1).ToList();
                            break;

                        case 3: //Entre
                            _datos = _datos.Where( item => (item.PromedioAct??0m) >= filtro.Promedio_Valor1 && (item.PromedioAct??0m) <= filtro.Promedio_Valor2).ToList();
                            break;

                        case 4: //Igual
                            _datos = _datos.Where( item => (item.PromedioAct??0m) == filtro.Promedio_Valor1).ToList();
                            break;
                    }
                }

                if(filtro.FechaLect_Opcion > 0){
                    switch (filtro.FechaLect_Opcion){
                        case 1: //Mayor Que
                            _datos = _datos.Where( item => (DateTime.TryParse(item.FechaLecturaAct, out DateTime tmpD)?tmpD:null) > filtro.FechaLect_Valor1).ToList();
                            break;

                        case 2: //Menor Que 
                            _datos = _datos.Where( item => (DateTime.TryParse(item.FechaLecturaAct, out DateTime tmpD)?tmpD:null) < filtro.FechaLect_Valor1).ToList();
                            break;

                        case 3: //Menor Que 
                            _datos = _datos.Where( item => (DateTime.TryParse(item.FechaLecturaAct, out DateTime tmpD)?tmpD:null) >= filtro.FechaLect_Valor1 && (DateTime.TryParse(item.FechaLecturaAct, out tmpD)?tmpD:null) <= filtro.FechaLect_Valor2).ToList();
                            break;
                            
                        case 4: //Igual
                            _datos = _datos.Where( item => (DateTime.TryParse(item.FechaLecturaAct, out DateTime tmpD)?tmpD:null) == filtro.FechaLect_Valor1).ToList();
                            break;
                    }
                }
                if(filtro.FechaVenci_Opcion > 0){
                    switch (filtro.FechaLect_Opcion){
                        case 1: //Mayor Que
                            _datos = _datos.Where( item => (DateTime.TryParse(item.FechaVencimientoAct, out DateTime tmpD)?tmpD:null) > filtro.FechaVenci_Valor1).ToList();
                            break;

                        case 2: //Menor Que 
                            _datos = _datos.Where( item => (DateTime.TryParse(item.FechaVencimientoAct, out DateTime tmpD)?tmpD:null) < filtro.FechaVenci_Valor1).ToList();
                            break;

                        case 3: //Menor Que 
                                _datos = _datos.Where( item => (DateTime.TryParse(item.FechaVencimientoAct, out DateTime tmpD)?tmpD:null) >= filtro.FechaVenci_Valor1 && (DateTime.TryParse(item.FechaVencimientoAct, out tmpD)?tmpD:null) <= filtro.FechaVenci_Valor2).ToList();
                            break;
                            
                        case 4: //Igual
                            _datos = _datos.Where( item => (DateTime.TryParse(item.FechaVencimientoAct, out DateTime tmpD)?tmpD:null) == filtro.FechaVenci_Valor1).ToList();
                            break;
                    }
                }

                if(filtro.EsDraef_Opcion > 0) {
                    switch(filtro.EsDraef_Opcion) {
                        case 1:
                            _datos = _datos.Where(item => item.EsDraef == true).ToList();
                            break;
                        case 2:
                            _datos = _datos.Where(item => item.EsDraef == false).ToList();
                            break;
                    }
                }
                if(filtro.AltoConsumidor_Opcion > 0) {
                    switch(filtro.AltoConsumidor_Opcion) {
                        case 1:
                            _datos = _datos.Where(item => item.EsAltoconsumidor == true).ToList();
                            break;
                        case 2:
                            _datos = _datos.Where(item => item.EsAltoconsumidor == false).ToList();
                            break;
                    }
                }
                if(filtro.TienePozo_Opcion> 0) {
                    switch(filtro.TienePozo_Opcion) {
                        case 1:
                            _datos = _datos.Where(item => item.TienePozo == true).ToList();
                            break;
                        case 2:
                            _datos = _datos.Where(item => item.TienePozo == false).ToList();
                            break;
                    }
                }

                //****** Convertir lista
                _result = _datos.Select( item => new CatPadron(ofi, item)).ToList();
                Console.WriteLine($">> Finalizando Analisis Info Oficina: {ofi.Oficina}");
                return _result;
            }catch(Exception err){
                Console.WriteLine($">> Error analisis infomarcion oficina: {ofi.Oficina}\n\tError: {err.Message}\n\tStacktrace:{err.StackTrace}");
                return null;
            }
        }
        private List<CatPadron> AnalisisInfoOfi2(Ruta ofi, AnalisysInfoFilter filtro){
            Console.WriteLine($">> Iniciando Analisis Info Oficina: {ofi.Oficina}");
            var _result = new List<CatPadron>();
            try{
                var busquea = new BusquedaAvanzada();
                var _query = busquea.GenerarQuery(filtro);
                var _datos = busquea.EjecutarConsulta(ofi, _query).ToList();

                //****** Convertir lista
                _result = _datos.Select( item => new CatPadron(ofi, item)).ToList();
                
                Console.WriteLine($">> Finalizando Analisis Info Oficina: {ofi.Oficina}");
                return _result;
            }catch(Exception err){
                Console.WriteLine($">> Error analisis infomarcion oficina: {ofi.Oficina}\n\tError: {err.Message}\n\tStacktrace:{err.StackTrace}");
                return null;
            }
        }
        public List<ImageData> ObtenerImagenes(int idOficina, long idCuenta){
            
            // get the office 
            var enlace = ObtenerEnlaces(idOficina).First();

            var images = new List<ImageData>();

             try {
                string query = String.Format(" Select i.id_imagen, archivo, descripcion, fecha = Cast(i.fecha_insert as smalldatetime) " +
                    "FROM [{1}Media].[Global].[Opr_Imagenes] i " + 
                    "Inner Join [{1}].Padron.Cat_Padron p on i.id_padron = p.id_padron " +
                    "Where p.id_cuenta = {0} AND i.file_extension <> '.ZIP'", idCuenta, enlace.BaseDatos);
           
                using(var sqlConnection = new SqlConnection(enlace.StringConection)) {
                    sqlConnection.Open();
                    var sqlCommand = new SqlCommand( query, sqlConnection){
                        CommandTimeout = (int) TimeSpan.FromMinutes(5).TotalSeconds
                    };
                    using(SqlDataReader reader = sqlCommand.ExecuteReader()) {
                        while(reader.Read()) {
                            if(int.TryParse(reader["id_imagen"].ToString(), out int tmpid)){
                                images.Add(
                                    new ImageData(){
                                        Id = tmpid,
                                        FileName = reader["archivo"].ToString(),
                                        Description = reader["descripcion"].ToString(),
                                        Date =  Convert.ToDateTime( reader["fecha"] )
                                    }
                                );
                            }
                        }
                    }
                }

            } catch(Exception e) {
                logger.LogError(e, "Error al obtener imagenes de la cuenta {cuenta} en la ofcina {oficina}", idCuenta, idOficina );
                return null;
            }

            return images;
        }

        //****** Notificaciones ******//
        public IEnumerable<NotificacionTemplate> ObtenerTemplateNotificaciones(){
            var _tmpNots =  sicemContext.CatMessagesTemplates.ToList();
            return _tmpNots.Select(e => new NotificacionTemplate(e));
        }
        public Guid ActualizarTemplateNotificaciones(NotificacionTemplate template){
            
            //Si existe, actualizar 
            if( sicemContext.CatMessagesTemplates.ToList().Select(e => e.Id).Contains(template.UUID)){
                Console.WriteLine("Actualizar template");

                var tmpData = sicemContext.CatMessagesTemplates.Find(template.UUID);
                tmpData.Titulo = template.Titulo;
                tmpData.Mensaje = template.Texto;
                tmpData.UltimaModificacion = DateTime.Now;
                sicemContext.CatMessagesTemplates.Update(tmpData);
                sicemContext.SaveChanges();
            }else{
                Console.WriteLine("Agregar template");
                sicemContext.CatMessagesTemplates.Add(template.ToCatMessageTemplate());
                sicemContext.SaveChanges();
            }
            return template.UUID;
        }

    }

    public class OpcionesSistema {
        public static readonly int RECAUDACION = 1;
        public static readonly int DESCUENTOS = 2;
        public static readonly int CANCELACIONES = 3; // ***(Inactivo)***
        public static readonly int ANUALES = 4; // ***(Inactivo)***
        public static readonly int FACTURACION = 5;
        public static readonly int FACTURACION_ELECTRONICA = 6; // ***(Inactivo)***
        public static readonly int POLIZAS = 7; // ***(Inactivo)***
        public static readonly int CONTROL_REZAGO = 8; // MOROSIDAD ***(Cambio a )***
        public static readonly int PADRON_USUARIOS = 9;
        public static readonly int ORDENES_TRABAJO = 10;
        public static readonly int BUSQUEDA_AVANZADA = 11;
        public static readonly int CONSULTA_GENERAL = 12;
        public static readonly int EFICIENCIA_COMERCIAL = 13;
        public static readonly int MICROMEDICION = 14;
        public static readonly int ATENCION_USUARIOS = 15;
        public static readonly int SIMULADOR_TARIFARIO = 16; 
        public static readonly int CONSULTA_UBITOMA = 17;
    }

}