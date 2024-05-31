using Microsoft.AspNetCore.Components;
using TinaKingSystem.BLL;
using TinaKingSystem.ViewModels;
using Microsoft.AspNetCore.Components.Forms;

using TinaKingSystem;
using MudBlazor;


namespace TinaKingWebApp.Data.Dialog
{
    public partial class EditPackageDialog
    {
        [Parameter]
        public PackageView Package { get; set; }
        [Inject]
        PackageService PackageService { get; set; }

        [CascadingParameter] MudDialogInstance MudDialog { get; set; }

        private async Task SaveChanges()
        {
            await PackageService.UpdatePackage(Package);
            MudDialog.Close(DialogResult.Ok(Package));
        }
    }
}
