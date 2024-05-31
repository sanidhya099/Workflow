using Microsoft.AspNetCore.Components;
using TinaKingSystem.BLL;
using TinaKingSystem.ViewModels;
using Microsoft.AspNetCore.Components.Forms;

using TinaKingSystem;
using MudBlazor;
using Syncfusion.Blazor.Inputs;
using Microsoft.AspNetCore.Routing.Constraints;
using System.Data;
using TinaKingSystem.Entities;

namespace TinaKingWebApp.Pages.MainPages
{
    public class UploadFile
    {
        public string FileName { get; set; }
        public string Url { get; set; }

        public UploadFile(string fileName, string url)
        {
            this.FileName = fileName;
            this.Url = url;
        }
    }

    public partial class ClientInput
    {
        #region Properties
        [Inject] private NavigationManager NavigationManager { get; set; }
        [Inject] public ClientInputService ClientInputService { get; set; }
        [Inject] public PackageService PackageService { get; set; }
        [Inject] public ClientDataService ClientDataService { get; set; }
        [Inject] UploadFileService UploadFileService { get; set; }
        [Inject] EventService EventService { get; set; }
        [Inject] public ISnackbar Snackbar { get; set; }

        private List<FileUploadModel> selectedFilesModels = new List<FileUploadModel>();

        public List<UploadFile> uploadFiles = new List<UploadFile>();

        private bool isUploading = false;
        private bool isUpload = false;
        private string uploadStatus;
        public string Username { get; set; }

        private IReadOnlyList<IBrowserFile> selectedFiles;
        public string FirstName { get; set; }

        public PackageView PackageView { get; set; } = new PackageView
        {
            DateSubmitted = DateTime.Today,
            Deadline = DateTime.Today,
            TypeOfRequest = "",
        };

        #endregion

        #region Methods

        public async Task SubmitForm()
        {
            // check package number
            PackageView.ClientID = this.ClientID;
            if (PackageService.IsExistPackageNo(PackageView) == true)
            {
                Snackbar.Add("Package number already exist. Please input different package number.", Severity.Warning);
                return;
            }

            if (PackageView.TypeOfRequest == "" || PackageView.TypeOfRequest == null)
            {
                Snackbar.Add("Please choose the Type of Request.", Severity.Warning);
                return;
            }
            if (PackageView.Priority == 0 || PackageView.Priority == null)
            {
                Snackbar.Add("Please choose the Priority Level.", Severity.Warning);
                return;
            }
            if (PackageView.PackageNumber == "" || PackageView.PackageNumber == null)
            {
                Snackbar.Add("Please input the Package Number.", Severity.Warning);
                return;
            }
            if (PackageView.Deadline <= PackageView.DateSubmitted)
            {
                Snackbar.Add("Please input the submitted data or deadline", Severity.Warning);
                return;
            }

            if (isUpload == false)
            {
                await UploadFiles();
                if (isUpload == false)
                {
                    Snackbar.Add("Failed to upload files.", Severity.Warning);
                    return;
                }
            }
       
            Console.WriteLine($"ClientID {ClientID}");
            try
            {
                foreach (var file in uploadFiles)
                {
                    PackageView.DocumentUrls.Add(file.Url);
                    PackageView.DocumentNames.Add(file.FileName);
                }

                var success = await PackageService.RegisterPackageAsync(PackageView);

                await EventService.UploadEvent(
                    ClientID,
                    "Client",
                    "Package",
                    PackageView.PackageID,
                    Username,
                    PackageView.PackageNumber,
                    ""
                    );
                if (success)
                {
                    Snackbar.Add("Package has been created successfully!", Severity.Success);
                    NavigationManager.NavigateTo("/PreviousPackages/" + PackageView.ClientID, forceLoad: true);
                }

            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error during package registration: {ex.Message}");
                Snackbar.Add($"Error: {ex.Message}", Severity.Error);

            }
        }

        public void ResetForm()
        {
            //PackageView = new PackageView();
            NavigationManager.NavigateTo("/ClientInput", forceLoad: true);
        }

        public async Task GoToPreviousPackages()
        {
            NavigationManager.NavigateTo("/PreviousPackages", forceLoad: true);
        }

        private void HandleFiles(InputFileChangeEventArgs e)
        {
            selectedFiles = e.GetMultipleFiles();
            uploadStatus = $"{selectedFiles.Count} file(s) selected for upload.";
        }

        private async Task UploadFiles()
        {
            try
            {
                isUploading = true;
                foreach (var file in selectedFiles)
                {
                    var url = await UploadFileService.UploadFileAsync(file);
                    uploadFiles.Add(new UploadFile(Path.GetFileNameWithoutExtension(file.Name), url));
                }
                uploadStatus = "All files have been uploaded successfully.";
                isUpload = true;
            }
            catch (Exception ex)
            {
                uploadStatus = $"Error during file upload: {ex.Message}";
            }
            finally
            {
                isUploading = false;
                StateHasChanged();
            }
        }
        #endregion
    }
    
    public class FileUploadModel
    {
        public IBrowserFile File { get; set; }
        public int UploadProgress { get; set; }
    }
}