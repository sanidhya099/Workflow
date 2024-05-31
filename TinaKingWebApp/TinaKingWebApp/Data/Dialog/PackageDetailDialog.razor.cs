using Microsoft.AspNetCore.Components;
using TinaKingSystem.BLL;
using TinaKingSystem.ViewModels;
using Microsoft.AspNetCore.Components.Forms;
using TinaKingSystem;
using MudBlazor;

namespace TinaKingWebApp.Data.Dialog
{
    #region Methods
    public partial class PackageDetailDialog
    {
        MudForm form;

        [Parameter] public PackageView packageView { get; set; }
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }
        public string _summery { get; set; }
        public int _total { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _total = packageView.Rate * packageView.SpendTime;
        }

        private void SpendChanged(int newValue)
        {
            newValue = Math.Max(0, newValue);
            packageView.SpendTime = newValue;
            _total = packageView.Total = packageView.Rate * packageView.SpendTime;
        }

        private void RateChanged(int newValue)
        {
            newValue = Math.Max(0, newValue);
            packageView.Rate = newValue;
            _total = packageView.Total = packageView.Rate * packageView.SpendTime;
        }


        private async Task Confirm(bool confirm)
        {
            if (confirm)
            {
                await form.Validate();
                if (form.IsValid)
                {
                    MudDialog.Close(DialogResult.Ok(confirm));
                }
                else
                {
                    return;
                }
            }
            else
            {
                MudDialog.Close(DialogResult.Ok(confirm));
            }
        }
        #endregion
    }
}
