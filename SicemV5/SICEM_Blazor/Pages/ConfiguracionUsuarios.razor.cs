using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Syncfusion.Blazor.Grids;
using MatBlazor;
using SICEM_Blazor.Models;
using SICEM_Blazor.Services;
using SICEM_Blazor.Shared.Dialogs;

namespace SICEM_Blazor.Pages {
    public partial class ConfiguracionUsuarios {

        [Inject]
        private SicemService sicemService {get;set;}
        [Inject]
        private UsersSicemService UsersSicemService {get;set;}
        
        [Inject]
        private IMatToaster Toaster {get;set;}

        private SfGrid<Usuario> dataGrid { get; set; }
        private List<Usuario> catUsuarios { get; set; }
        private bool isBusy = false;
        private string stringSearch = "";
        private ModificarUsuario ModificarUsuarioVtn { get; set; }
        private bool ModificarUsuarioVtn_Visible { get; set; } = false;

        protected override void OnInitialized() {
            this.catUsuarios = UsersSicemService.ObtenerListadoUsuarios().ToList();
        }


        private async Task AgregarUsuarioClick(MouseEventArgs e){
            if (ModificarUsuarioVtn_Visible){
                return;
            }
            ModificarUsuarioVtn_Visible = true;
            ModificarUsuarioVtn.ShowDialog();
            await Task.Delay(100);
        }
        private async Task HandleModificarUsuarioClick(MouseEventArgs e, Usuario usuario){
            if (ModificarUsuarioVtn_Visible) {
                return;
            }
            ModificarUsuarioVtn_Visible = true;
            ModificarUsuarioVtn.ShowDialog(usuario);
            await Task.Delay(100);
        }

        private async Task ModificarUsuario_Closed(){
            ModificarUsuarioVtn_Visible = false;
            this.isBusy = true;
            await Task.Delay(100);

            this.catUsuarios = UsersSicemService.ObtenerListadoUsuarios().ToList();

            await Task.Delay(100);
            this.isBusy = false;
        }

        private async Task InputSearch_KeyUp(KeyboardEventArgs e){
            if(e.Key == "Enter"){
                await dataGrid.SearchAsync(stringSearch);
            }
        }

        private async Task HandleExportarUsuariosClick(MouseEventArgs e){
            if(this.dataGrid == null){
                return;
            }
            await dataGrid.ExportToExcelAsync();
        }

        private async Task HandleChangeUserStatus(MouseEventArgs e, Usuario usuario){
            try {
                this.UsersSicemService.ChangeUserStatus(usuario.Id);
            }
            catch (System.Exception)
            {
                Toaster.Add("Error al actualizar el estatus del usuario", MatToastType.Danger);
            }
            await Task.Delay(100);
            this.catUsuarios = UsersSicemService.ObtenerListadoUsuarios().ToList();
            StateHasChanged();

        }

    }

}