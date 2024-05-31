using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TinaKingSystem.DAL;
using TinaKingSystem.Entities;
using TinaKingSystem.ViewModels;

namespace TinaKingSystem.BLL
{
    public class InvoiceService
    {
        private readonly WFS_2590Context? _WFS_2590Context;

        internal InvoiceService(WFS_2590Context WFS_2590Context)
        {
            _WFS_2590Context = WFS_2590Context;
        }

        // Generates a new automatic invoice number based on the current year and month
        public async Task<string> GenerateAutomaticInvoiceNumber()
        {
            using (var transaction = await _WFS_2590Context.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable))
            {
                try
                {
                    var currentYear = DateTime.Now.Year;
                    var currentMonth = DateTime.Now.Month;
                    var invoicePrefix = "PK";

                    // Fetch and increment the count of invoices for the current year and month
                    var invoiceCountThisMonth = await _WFS_2590Context.Invoices
                                                .Where(i => i.Regist.Year == currentYear && i.Regist.Month == currentMonth)
                                                .CountAsync();

                    var nextInvoiceNumber = invoiceCountThisMonth + 1;

                    var newInvoiceNumber = $"{invoicePrefix}-{currentYear}-{currentMonth.ToString().PadLeft(2, '0')}-{nextInvoiceNumber}";

                    // Commit the transaction
                    await transaction.CommitAsync();

                    return newInvoiceNumber;
                }
                catch (Exception ex)
                {
                    // Rollback the transaction in case of an error
                    await transaction.RollbackAsync();

                    // Log the exception
                    Console.WriteLine($"Error generating automatic invoice number: {ex.Message}");
                    throw;
                }
            }
        }

        // Uploads a new invoice to the database
        public async Task<int> UploadInvoice(
            string InvoiceNo,
            int UserID,
            string Username,
            string Email,
            int PackageCount,
            double Total,
            string Status,
            DateTime Regist,
            double GST,
            string City,
            string Country,
            string PostalCode,
            string BusinessNo
        )
        {
            // Generate a new invoice number for each invoice upload
            var newInvoiceNo = await GenerateAutomaticInvoiceNumber();

            // Adjust the registration date to the next Saturday
            TimeSpan span = new TimeSpan(6 - (int)Regist.DayOfWeek, 0, 0, 0);
            DateTime dt = Regist + span;

            Invoice invoice = new Invoice
            {
                InvoiceNo = newInvoiceNo,
                ClientID = UserID,
                Username = Username,
                Email = Email,
                City = City,
                Country = Country,
                PostalCode = PostalCode,
                BusinessNo = BusinessNo,
                PackageCount = PackageCount,
                Total = Total,
                Status = Status,
                Regist = dt,
                GST = GST
            };

            try
            {
                // Add the new invoice to the database and save changes
                _WFS_2590Context.Invoices.Add(invoice);
                await _WFS_2590Context.SaveChangesAsync();

                // Return the ID of the newly added invoice
                return invoice.ID;
            }
            catch
            {
                // In a real-world application, you'd want to log this error
                return 0;
            }
        }

        // Retrieves a list of invoices with the specified status
        public async Task<List<InvoiceView>> GetInvoiceList(string status)
        {
            var invoiceList = await _WFS_2590Context.Invoices
                                    .Where(x => x.Status == status)
                                    .Select(x => new InvoiceView
                                    {
                                        ID = x.ID,
                                        InvoiceNo = x.InvoiceNo,
                                        UserID = x.ClientID,
                                        Username = x.Username,
                                        City = x.City,
                                        Email = x.Email,
                                        Country = x.Country,
                                        PostalCode = x.PostalCode,
                                        Business = x.BusinessNo,
                                        PackageCount = x.PackageCount,
                                        Total = x.Total,
                                        Status = x.Status,
                                        Regist = x.Regist,
                                        GST = x.GST
                                    }).ToListAsync();

            return invoiceList;
        }

        // Updates an invoice's status to "Send"
        public async Task<bool> InvoiceToHistory(InvoiceView invoiceView)
        {
            DateTime dtStart;
            var invoice = await _WFS_2590Context.Invoices
                .FindAsync(invoiceView.ID);
            invoice.Status = "Send";
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

        // Retrieves the last uploaded invoice
        public async Task<InvoiceView> GetLastInvoice()
        {
            var lastInvoice = await _WFS_2590Context.Invoices
                                    .OrderByDescending(x => x.Regist)
                                    .ThenByDescending(x => x.ID)
                                    .Select(x => new InvoiceView
                                    {
                                        ID = x.ID,
                                        InvoiceNo = x.InvoiceNo,
                                        City = x.City,
                                        Country = x.Country,
                                        PostalCode = x.PostalCode,
                                        Business = x.BusinessNo,
                                        UserID = x.ClientID,
                                        Username = x.Username,
                                        Email = x.Email,
                                        PackageCount = x.PackageCount,
                                        Total = x.Total,
                                        Status = x.Status,
                                        Regist = x.Regist,
                                        GST = x.GST
                                    })
                                    .FirstOrDefaultAsync();

            return lastInvoice;
        }
    }
}
