using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Data;
using System.Threading.Tasks;
using TinaKingSystem.Entities;
using TinaKingSystem.ViewModels;


namespace TinaKingSystem.BLL
{
    public class UploadFileService
    {
        private string connectionString = "DefaultEndpointsProtocol=https;AccountName=tinakingwebapp;AccountKey=nv/Jgs3+WKOnjE5ORTthUw9umu9QchLj7T03JIr7Lq+h8OzFyM/AalE/Wov0+l8nfmhoMf7ph0ME+AStRwWWvg==;EndpointSuffix=core.windows.net";
        private string containerName = "tinakingstorage";

        public async Task<string> UploadFileAsync(IBrowserFile file)
        {
            try
            {
                var container = new BlobContainerClient(connectionString, containerName);
                await container.CreateIfNotExistsAsync();

                var blob = container.GetBlobClient(file.Name);
                await using var stream = file.OpenReadStream(maxAllowedSize: 2147483648);
                await blob.UploadAsync(stream, overwrite: true);

                return blob.Uri.ToString(); 
            }
            catch (Exception ex)
            {
                throw new Exception($"Error uploading file: {ex.Message}", ex);
            }
        }
    }
}
