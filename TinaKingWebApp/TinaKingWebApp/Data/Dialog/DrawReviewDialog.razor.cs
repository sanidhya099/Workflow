using Microsoft.AspNetCore.Components;
using TinaKingSystem.BLL;
using TinaKingSystem.ViewModels;
using Microsoft.AspNetCore.Components.Forms;

using TinaKingSystem;
using MudBlazor;
using Microsoft.Identity.Client;
using System.Data;
using TinaKingSystem.Entities;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;



namespace TinaKingWebApp.Data.Dialog
{
    public partial class DrawReviewDialog
    {
        #region Properties
        MudForm form;
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }
        [Inject] public ISnackbar Snackbar { get; set; }
        [Inject] public AuthenticationStateProvider AuthenticationStateProvider { get; set; }
        [Inject] public PackageService PackageService { get; set; }
        [Inject] public DrawingService DrawingService { get; set; }
        [Inject] public EmployeeService EmployeeService { get; set; }
        [Inject] public EventService EventService { get; set; }

        [Parameter] public PackageView packageView { get; set; }
        [Parameter] public int UserID { get; set; }
        [Parameter] public string Role { get; set; }
        [Parameter] public string UserName { get; set; }

        public DrawingView drawingView { get; set; } = new DrawingView();
        private List<string> SummeryList = new List<string>();
        #endregion

        #region Methods
        protected override async Task OnInitializedAsync()
        {
            base.OnInitializedAsync();

            drawingView = DrawingService.GetByPackageID(packageView.PackageID);
            if (drawingView == null) {
                drawingView = new DrawingView();
                drawingView.PackageID = packageView.PackageID;
            } else {
                drawingView.DetailInfo = DrawingItem.FromString(drawingView.Detail);
            }
        }

        public void OnUpdate()
        {
            SummeryList = drawingView.GetSummery();
        }

        private async Task Confirm(bool confirm)
        {
            if (confirm == false) {
                MudDialog.Close(DialogResult.Cancel());
                return;
            }

            if (await DrawingService.UploadAsync(drawingView) == false) {
                Snackbar.Add("failed to connect database", Severity.Warning);
            }
            await PackageService.UploadReviewInfo(packageView.PackageID, drawingView.GetSummeryString());

            await EventService.UploadEvent(
                UserID,
                Role,
                "Review",
                packageView.PackageID,
                UserName,
                packageView.PackageNumber,
                ""
                );


            MudDialog.Close(DialogResult.Ok(confirm));
        }
        #endregion

    }
}
