using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TinaKingSystem.BLL;
using TinaKingSystem.DAL;
using TinaKingSystem.Entities;
using TinaKingSystem.ViewModels;

public class PackageService
{
    private readonly WFS_2590Context _WFS_2590Context;

    public PackageService(WFS_2590Context WFS_2590Context)
    {
        _WFS_2590Context = WFS_2590Context;
    }


    public async Task<List<PackageView>> GetClientPackagesAsync(int ClientID)
    {
        var packageClientViews = await _WFS_2590Context.Packages
            .Where(p => p.ClientID == ClientID && p.ApproveState != "A")
            .Include(p => p.Client)
            .Select(p => new PackageView
            {
                PackageID = p.PackageID,
                ClientID = p.ClientID,
                ClientName = p.Client.FirstName,
                PackageNumber = p.PackageNumber,
                TypeOfRequest = p.TypeOfRequest,
                DateSubmitted = p.DateSubmitted,
                Deadline = p.Deadline,
                EndDate = p.EndDate == null ? new DateTime() : (DateTime)p.EndDate,
                Status = p.Status,
                Information = p.Information,
                Priority = p.Priority ?? default(int),
                DocumentNames = Package.Convert(p.FileNames),
                DocumentUrls = Package.Convert(p.Urls),
                str_documents = p.FileNames.Replace("|", ", "),
                Rate = p.Rate == null ? 0 : (int)p.Rate,
                SpendTime = p.Spend == null ? 0 : (int)p.Spend,
                Summery = p.Summery,
                ApproveState = p.ApproveState,
            }).OrderByDescending(p => p.Deadline)
            .ToListAsync();

        return packageClientViews;
    }
    public async Task<List<PackageView>> GetClientFinishedPackagesAsync(int ClientID)
    {
        var packageClientViews = await _WFS_2590Context.Packages
            .Where(p => p.ClientID == ClientID && p.ApproveState == "A")
            .Include(p => p.Client)
            .Select(p => new PackageView
            {
                PackageID = p.PackageID,
                ClientID = p.ClientID,
                ClientName = p.Client.FirstName,
                PackageNumber = p.PackageNumber,
                TypeOfRequest = p.TypeOfRequest,
                DateSubmitted = p.DateSubmitted,
                Deadline = p.Deadline,
                EndDate = p.EndDate == null ? new DateTime() : (DateTime)p.EndDate,
                Status = p.Status,
                Information = p.Information,
                Priority = p.Priority ?? default(int),
                DocumentNames = Package.Convert(p.FileNames),
                DocumentUrls = Package.Convert(p.Urls),
                str_documents = p.FileNames.Replace("|", ", "),
                Rate = p.Rate == null ? 0 : (int)p.Rate,
                SpendTime = p.Spend == null ? 0 : (int)p.Spend,
                Summery = p.Summery,
                ApproveState = p.ApproveState,
            }).OrderByDescending(p => p.Deadline)
            .ToListAsync();

        return packageClientViews;
    }
    public async Task<List<PackageView>> GetPackages()
    {
        try
        {
            var packageViews = await _WFS_2590Context.Packages
                .Include(p => p.Client)
                .Where(p => p.ApproveState != "D" && (p.InvoiceID == null || p.InvoiceID == 0))
                .Select(p => new PackageView
                {
                    PackageID = p.PackageID,
                    ClientID = p.ClientID,
                    PackageNumber = p.PackageNumber,
                    ClientName = p.Client.FirstName,
                    Email = p.Client.Email,
                    TypeOfRequest = p.TypeOfRequest,
                    DateSubmitted = p.DateSubmitted,
                    Deadline = p.Deadline,
                    EndDate = p.EndDate == null ? new DateTime(0) : (DateTime)p.EndDate,
                    Status = p.Status,
                    Information = p.Information,
                    Priority = p.Priority ?? default(int),
                    DocumentNames = Package.Convert(p.FileNames),
                    DocumentUrls = Package.Convert(p.Urls),
                    str_documents = p.FileNames.Replace("|", ", "),
                    Rate = p.Rate == null ? 0 : (int)p.Rate,
                    SpendTime = p.Spend == null ? 0 : (int)p.Spend,
                    Summery = p.Summery,
                    ApproveState = p.ApproveState,
                }).ToListAsync();

            return packageViews;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving: {ex.Message}");
            return new List<PackageView>();
        }
    }

    public async Task<List<PackageView>> GetFinishedPackages()
    {
        try
        {
            var packageViews = await _WFS_2590Context.Packages
                .Include(p => p.Client)
                .Where(p => p.ApproveState == "A") 
                .Select(p => new PackageView
                {
                    PackageID = p.PackageID,
                    ClientID = p.ClientID,
                    PackageNumber = p.PackageNumber,
                    ClientName = p.Client.FirstName,
                    Email = p.Client.Email,
                    TypeOfRequest = p.TypeOfRequest,
                    DateSubmitted = p.DateSubmitted,
                    Deadline = p.Deadline,
                    EndDate = p.EndDate == null ? new DateTime(0) : (DateTime)p.EndDate,
                    Status = p.Status,
                    Information = p.Information,
                    Priority = p.Priority ?? default(int),
                    DocumentNames = Package.Convert(p.FileNames),
                    DocumentUrls = Package.Convert(p.Urls),
                    str_documents = p.FileNames.Replace("|", ", "),
                    Rate = p.Rate == null ? 0 : (int)p.Rate,
                    SpendTime = p.Spend == null ? 0 : (int)p.Spend,
                    Summery = p.Summery,
                    ApproveState = p.ApproveState,
                }).ToListAsync();

            return packageViews;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving: {ex.Message}");
            return new List<PackageView>();
        }
    }

    public async Task<bool> RegisterPackageAsync(PackageView packageView)
    {
        try
        {

            var package = new Package
            {
                ClientID = packageView.ClientID,
                PackageNumber = packageView.PackageNumber,
                Status = string.IsNullOrEmpty(packageView.Status) ? "Open" : packageView.Status,
                TypeOfRequest = packageView.TypeOfRequest,
                DateSubmitted = packageView.DateSubmitted,
                Deadline = packageView.Deadline,
                EndDate = packageView.EndDate,
                Priority = packageView.Priority,
                Information = packageView.Information,
                FileNames = Package.Convert(packageView.DocumentNames),
                Urls = Package.Convert(packageView.DocumentUrls),
                ApproveState = "R",
            };


            _WFS_2590Context.Packages.Add(package);
            await _WFS_2590Context.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return false;
        }

    }


    public async Task<PackageView> GetPackageById(string packageId)
    {
        var package = await _WFS_2590Context.Packages
            .Where(x => x.PackageID == Int32.Parse(packageId))
            .Select(p => new PackageView
            {
                PackageID = p.PackageID,
                ClientID = p.ClientID,
                PackageNumber = p.PackageNumber,
                ClientName = p.Client.FirstName,
                Email = p.Client.Email,
                TypeOfRequest = p.TypeOfRequest,
                DateSubmitted = p.DateSubmitted,
                Deadline = p.Deadline,
                EndDate = p.EndDate == null ? new DateTime(0) : (DateTime)p.EndDate,
                Status = p.Status,
                Information = p.Information,
                Priority = p.Priority ?? default(int),
                DocumentNames = Package.Convert(p.FileNames),
                DocumentUrls = Package.Convert(p.Urls),
                str_documents = p.FileNames.Replace("|", ", "),
                Rate = p.Rate == null ? 0 : (int)p.Rate,
                SpendTime = p.Spend == null ? 0 : (int)p.Spend,
                Summery = p.Summery,
                ApproveState = p.ApproveState,
            })
            .FirstOrDefaultAsync();

        if (package == null)
        {
            throw new KeyNotFoundException("Package not found.");
        }

        return package;
    }
    public async Task<bool> UpdatePackage(PackageView packageView)
    {
        var package = await _WFS_2590Context.Packages.FindAsync(packageView.PackageID);

        if (package == null)
        {
            return false;
        }

        package.Deadline = packageView.Deadline;
        package.Priority = packageView.Priority;

        try
        {
            await _WFS_2590Context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public async Task<bool> UpdateMoreInformation(PackageView packageView)
    {
        var package = await _WFS_2590Context.Packages.FindAsync(packageView.PackageID);

        if (package == null)
        {
            return false;
        }

        package.Rate = packageView.Rate;
        package.Spend = packageView.SpendTime;
        package.Summery = packageView.Summery;

        try
        {
            await _WFS_2590Context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }


    public async Task<bool> DeletePackageAsync(int packageId, string reason)
    {
        try
        {
            var package = await _WFS_2590Context.Packages.FindAsync(packageId);
            if (package == null)
            {
                return false; 
            }
            package.ApproveState = "D";
            package.Reason = reason;
            package.DateRemoved = DateTime.Now;
            await _WFS_2590Context.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public async Task<bool> UpdateRate(int packageId, double rate)
    {
        try
        {
            var package = await _WFS_2590Context.Packages.FindAsync(packageId);
            await _WFS_2590Context.SaveChangesAsync();
            return true; 
        }
        catch (Exception ex)
        {
            return false; 
        }
    }
    public async Task<bool> UpdateTotal(int packageId, double total)
    {
        try
        {
            var package = await _WFS_2590Context.Packages.FindAsync(packageId);
            await _WFS_2590Context.SaveChangesAsync();
            return true; 
        }
        catch (Exception ex)
        {
            return false; 
        }
    }
    public async Task<bool> UpdateNote(int packageId, string note)
    {
        try
        {
            var package = await _WFS_2590Context.Packages.FindAsync(packageId);
            await _WFS_2590Context.SaveChangesAsync();
            return true; 
        }
        catch (Exception ex)
        {
            return false; 
        }
    }


    public async Task<bool> UpdateComment(int packageId, string comment)
    {
        try
        {
            var package = await _WFS_2590Context.Packages.FindAsync(packageId);
            await _WFS_2590Context.SaveChangesAsync();
            return true; 
        }
        catch (Exception ex)
        {
            return false; 
        }
    }

    public async Task<bool> UploadReviewInfo(int id, string summery)
    {
        try
        {
            var package = await _WFS_2590Context.Packages.FindAsync(id);
            if (package.ApproveState == "" || package.ApproveState == "R" || package.ApproveState == null)
                package.ApproveState = "V";
            package.Summery = summery;
            await _WFS_2590Context.SaveChangesAsync();
            return true; 
        }
        catch (Exception ex)
        {
            return false; 
        }
    }

    public async Task<bool> SetPackageFlag(PackageView p, string f)
    {
        try
        {
            var package = await _WFS_2590Context.Packages.FindAsync(p.PackageID);
            package.ApproveState = f;
            if (f == "A")
                package.EndDate = DateTime.Now;
            await _WFS_2590Context.SaveChangesAsync();
            return true; 
        }
        catch (Exception ex)
        {
            return false; 
        }
    }
    public async Task<string> GetWPSNo(int PackageID)
    {
        try
        {
            var WP = await _WFS_2590Context.WPs
                .Where(x => x.PackageID == PackageID)
                .FirstOrDefaultAsync();
            if (WP == null) return "";
            return WP.WPSNo;
        }
        catch (Exception ex)
        {
            return "";
        }
    }

    public async Task<List<InvoiceView>> GetInvoiceList()
    {
        try
        {
            var InvoiceList = await _WFS_2590Context.Packages
                .Where(x => x.ApproveState == "A" && (x.InvoiceID == null || x.InvoiceID == 0))
                .GroupBy(x => x.ClientID)
                .Select(g => new InvoiceView
                {
                    ID = 0,
                    InvoiceNo = "",
                    UserID = g.Max(d => d.ClientID),
                    Username = g.Max(d => d.Client.Username),
                    Email = g.Max(d => d.Client.Email),
                    PackageCount = g.Count(),
                    Total = g.Sum(d => (d.Rate != null ? (int)d.Rate : 0) * (d.Spend == null ? 0 : (int)d.Spend)),
                    Status = "Draft",
                    Regist = DateTime.Now,
                    GST = g.Sum(d => (d.Rate != null ? (int)d.Rate : 0) * (d.Spend == null ? 0 : (int)d.Spend)) * 0.05
                })
                .ToListAsync();
            return InvoiceList;
        }
        catch (Exception ex)
        {
            return new List<InvoiceView>();
        }
    }

    public async Task<List<PackageView>> GetPackagesForInvoiceAsync(
        int InvoiceID,
        int UserID,
        string Status
    )
    {
        var PackageList = new List<PackageView>();
        try
        {
            if (Status == "Draft")
            {
                PackageList = await _WFS_2590Context.Packages
                    .Where(x => x.ApproveState == "A" && (x.InvoiceID == null || x.InvoiceID == 0) && x.ClientID == UserID)
                    .Select(p => new PackageView
                    {
                        PackageID = p.PackageID,
                        ClientID = p.ClientID,
                        PackageNumber = p.PackageNumber,
                        ClientName = p.Client.FirstName,
                        Email = p.Client.Email,
                        TypeOfRequest = p.TypeOfRequest,
                        DateSubmitted = p.DateSubmitted,
                        Deadline = p.Deadline,
                        EndDate = p.EndDate == null ? new DateTime(0) : (DateTime)p.EndDate,
                        Status = p.Status,
                        Information = p.Information,
                        Priority = p.Priority ?? default(int),
                        DocumentNames = Package.Convert(p.FileNames),
                        DocumentUrls = Package.Convert(p.Urls),
                        str_documents = p.FileNames.Replace("|", ", "),
                        Rate = p.Rate == null ? 0 : (int)p.Rate,
                        SpendTime = p.Spend == null ? 0 : (int)p.Spend,
                        Summery = p.Summery,
                        ApproveState = p.ApproveState,
                    })
                    .ToListAsync();
            }
            else
            {
                PackageList = await _WFS_2590Context.Packages
                    .Where(x => x.InvoiceID == InvoiceID)
                    .Select(p => new PackageView
                    {
                        PackageID = p.PackageID,
                        ClientID = p.ClientID,
                        PackageNumber = p.PackageNumber,
                        ClientName = p.Client.FirstName,
                        TypeOfRequest = p.TypeOfRequest,
                        DateSubmitted = p.DateSubmitted,
                        Deadline = p.Deadline,
                        EndDate = p.EndDate == null ? new DateTime(0) : (DateTime)p.EndDate,
                        Status = p.Status,
                        Information = p.Information,
                        Priority = p.Priority ?? default(int),
                        DocumentNames = Package.Convert(p.FileNames),
                        DocumentUrls = Package.Convert(p.Urls),
                        str_documents = p.FileNames.Replace("|", ", "),
                        Rate = p.Rate == null ? 0 : (int)p.Rate,
                        SpendTime = p.Spend == null ? 0 : (int)p.Spend,
                        Summery = p.Summery,
                        ApproveState = p.ApproveState,
                    })
                    .ToListAsync();
            }
            return PackageList;
        }
        catch (Exception ex)
        {
            return new List<PackageView>();
        }
    }

    public async Task<bool> SetPackageInvoice(List<PackageView> l, int InvoiceID, bool force)
    {
        try
        {
            foreach (PackageView p in l)
            {
                if (p.Selected == false && force == false) continue;

                var package = await _WFS_2590Context.Packages.FindAsync(p.PackageID);

                if (p.Selected == true) package.InvoiceID = InvoiceID;
                if (p.Selected == false) package.InvoiceID = null;
            }

            await _WFS_2590Context.SaveChangesAsync();
            return true; 
        }
        catch (Exception ex)
        {
            return false; 
        }
    }

    public bool IsExistPackageNo(PackageView packageView)
    {
        return _WFS_2590Context.Packages
            .Where(x => (x.PackageID != packageView.PackageID
                        && x.PackageNumber == packageView.PackageNumber
                        && x.ClientID == packageView.ClientID))
            .ToList().Count > 0;
    }
}