using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace TinaKingWebApp.Data.Dialog
{
    public partial class DeleteConfirmDialog
    {
        [CascadingParameter]
        MudDialogInstance MudDialog { get; set; }
        [Parameter] public string PackageNumber { get; set; }
        [Parameter] public string Username { get; set; }

        private void ConfirmDelete(bool confirm)
        {
            MudDialog.Close(DialogResult.Ok(confirm));
        }
    }
}
