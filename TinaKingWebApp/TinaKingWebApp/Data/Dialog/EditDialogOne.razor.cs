using Microsoft.AspNetCore.Components;
using TinaKingSystem.BLL;
using TinaKingSystem.ViewModels;
using Microsoft.AspNetCore.Components.Forms;

using TinaKingSystem;
using MudBlazor;


namespace TinaKingWebApp.Data.Dialog
{
    public partial class EditDialogOne
    {
        [Parameter] public DialogView Dlg { get; set; }
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }

        private async Task Confirm(bool confirm)
        {
            if (Dlg.Value == "") return;
            MudDialog.Close(DialogResult.Ok(confirm));
        }
    }
}
