
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Syncfusion.Blazor.Grids;
using SICEM_Blazor.Models;
using SICEM_Blazor.Services.Whatsapp;
using SICEM_Blazor.Services;
using SICEM_Blazor.Data.Notification;
using SICEM_Blazor.Helpers;

namespace SICEM_Blazor.Shared.Dialogs;

public partial class NotificarDialog
{

    [Inject]
    public IJSRuntime JSRuntime {get;set;}
    
    [Inject]
    public WHttpService WHttpService {get;set;}
    
    [Inject]
    public SicemService SicemService {get;set;}

    [Inject]
    public IConfiguration Configuration {get;set;}
    
    [Inject]
    public ILogger<NotificarDialog> Logger {get;set;}

    [Parameter]
    public EventCallback OnClosed {get;set;}
    
    [Parameter]
    public string Titulo {get;set;} = "Notificacion Usuarios";

    [Parameter]
    public IEnumerable<CatPadron> PadronUsuarios { get; set; } = new List<CatPadron>();

    private double[] FilasTemplate = new double[]{400, 40};
    private double[] ColumnasTemplate = new double[]{300, 400, 350};
    private bool cargando = false, mostrarPre = false;
    private List<NotificacionTemplate> DatosGrid {get;set;}
    private NotificacionTemplate TemplateSeleccionado {get;set;}
    private string TextoMensaje {
        get {
            if( TemplateSeleccionado == null){
                TemplateSeleccionado = new NotificacionTemplate(){
                    Titulo = "Notificacion de envio",
                    Texto = "Mensaje de prueba",
                    FCreacion = DateTime.Now,
                    UltimaMod = DateTime.Now
                };
            }
            return TemplateSeleccionado.Texto;
        }
        set{
            if( TemplateSeleccionado == null){
                TemplateSeleccionado = new NotificacionTemplate(){
                    Titulo = "Notificacion de envio",
                    Texto = "Mensaje de prueba",
                    FCreacion = DateTime.Now,
                    UltimaMod = DateTime.Now
                };
            }
            TemplateSeleccionado.Texto = value;
        }
    }
    private System.Reflection.PropertyInfo[] propertiesCatPadron = typeof(CatPadron).GetProperties();
    private List<CatPadron> PadronUsuarios_ConTelefono {
        get {
            return PadronUsuarios.Where(item =>  !String.IsNullOrEmpty(item.Telefono1)).ToList();
        }
    }
    private CatPadron DestinatarioSeleccionado {get;set;}
    private string MesnajePrevisualizado {get; set;} = "";

    protected override async Task OnInitializedAsync() {
        await CargarTemplates();
    }
    protected override void OnAfterRender(bool firstRender) {
        if (firstRender) {
            Console.WriteLine("first render!");
            var _usuariosConTelefono = PadronUsuarios.Where(item =>  !String.IsNullOrEmpty(item.Telefono1)).ToArray();
            var _usuariosSinTelefono = PadronUsuarios.Where(item =>  String.IsNullOrEmpty(item.Telefono1)).ToArray();
            Console.WriteLine($"Total Usuarios:{PadronUsuarios.Count()} Usuarios con telefono: {_usuariosConTelefono.Count()}  Usuarios sin telefono:{_usuariosSinTelefono.Count()}");
        }
    }

    private async Task CerrarVentana_Click(){
        await OnClosed.InvokeAsync();
    }

    private async Task EnviarMensaje(){

        cargando = true;
        await Task.Delay(100);

        // * foreach user make and send the message
        foreach(var usuario in PadronUsuarios_ConTelefono)
        {
            // TODO: move this into a queue

            var tmpTelefono = usuario.Telefono1.Replace(" ", "").Trim();
            var message = MessageUtils.GenerateMessage(TemplateSeleccionado.Texto, usuario);

            var imageBase64 = await MakeImageNotification(message);
            
            var ok = await WHttpService.SendFile(tmpTelefono, imageBase64, "image/jpeg", "notification");
        }

        await Task.Delay(100);
        cargando = false;

        await OnClosed.InvokeAsync();
    }

    private void OnDataGrid_SelectionChanged(RowSelectEventArgs<NotificacionTemplate> e){
        TemplateSeleccionado = e.Data;
    }
    
    private async Task CargarTemplates(){
        this.cargando = true;
        await Task.Delay(100);

        TemplateSeleccionado = null;
        this.DatosGrid = SicemService.ObtenerTemplateNotificaciones().ToList();

        await Task.Delay(100);
        this.cargando = false;
    }

    private void OnDestinatarioSeleccionado(RowSelectEventArgs<CatPadron> e){
        this.DestinatarioSeleccionado = e.Data;
    }

    private void PrevisualizarTexto_Click(){
        if(DestinatarioSeleccionado == null){
            return;
        }
        this.MesnajePrevisualizado = MessageUtils.GenerateMessage(TemplateSeleccionado.Texto, DestinatarioSeleccionado);
        mostrarPre = true;
    }

    private async Task<string> MakeImageNotification(string message)
    {
        // var originalImage = Path.Join(AppContext.BaseDirectory, "Resources/Templates/banner-caev.png");
        var originalImage = Path.Join(AppContext.BaseDirectory, "Resources/Templates/banner-notification.png");
        var destinationImagen = Path.Join(this.Configuration["TempFolder"], Guid.NewGuid().ToString(), "notifica.jpg");
        // * ensure the directory exists
        string directoryPath = Path.GetDirectoryName(destinationImagen);
        if (!string.IsNullOrEmpty(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
        this.Logger.LogInformation("Original File:[{original}], Destination Fiel:[{destination}]", originalImage, destinationImagen);

        var appendTextToImage = new AppendTextToImage();
        await appendTextToImage.AppendText(originalImage, message, destinationImagen);
        var imageBase64 = ConvertToBase64.ConvertFile(destinationImagen);

        return imageBase64;
    }

}

class BodyNotification {
    public string phone {get;set;}
    public string message {get;set;}
}
