using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinaKingSystem.DAL;
using TinaKingSystem.Entities;
using TinaKingSystem.ViewModels;

namespace TinaKingSystem.BLL
{
    public class ClientInputService
    {
        private readonly WFS_2590Context? _WFS_2590Context;

        public ClientInputService(WFS_2590Context WFS_2590Context)
        {
            _WFS_2590Context = WFS_2590Context;
        }

        // Create a package asynchronously
        public async Task<bool> CreatePackageAsync(PackageView packageView)
        {
            // Create a new package object from packageView
            var package = new Package
            {
                ClientID = packageView.ClientID,
                TypeOfRequest = packageView.TypeOfRequest,
                DateSubmitted = packageView.DateSubmitted,
                Deadline = packageView.Deadline,
                Priority = packageView.Priority,
                PackageNumber = packageView.PackageNumber,
                Status = packageView.Status,
            };

            // Add package to the Packages DbSet and save changes to the database
            _WFS_2590Context.Packages.Add(package);
            await _WFS_2590Context.SaveChangesAsync();

            // Return true indicating successful creation of package
            return true;
        }
    }
}
