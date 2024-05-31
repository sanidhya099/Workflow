using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.IO;
using TinaKingSystem.ViewModels;

namespace TinaKingSystem.InvoiceFunction
{
    public static class Extensions
    {
        public static float ToDpi(this float centimeter)
        {
            var inch = centimeter / 2.54;
            return (float)(inch * 72);
        }
    }

    public class ClaPDF
    {
        [Inject]
        public PackageService PackageService { get; set; }

        public async Task Generate(IJSRuntime js, InvoiceView invoice, List<PackageView> packageList)
        {
            byte[] data = ReportPDF(invoice, packageList);
            await js.InvokeVoidAsync("jsDownloadFile", invoice.InvoiceNo + ".pdf", Convert.ToBase64String(data));
        }

        private byte[] ReportPDF(InvoiceView invoice, List<PackageView> packageList)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (Document document = new Document(PageSize.A4, 50f, 50f, 25f, 25f))
                {
                    PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
                    writer.CloseStream = false;
                    document.Open();

                    AddCompanyHeading(document);
                    AddInvoiceHeader(document, invoice);
                    AddInvoiceTable(document, packageList, invoice);

                    document.Close();
                    writer.Close();
                }
                return memoryStream.ToArray();
            }
        }

        private void AddCompanyHeading(Document document)
        {
            Font companyNameFont = FontFactory.GetFont("Arial", 18, Font.BOLD);

            PdfPTable headingTable = new PdfPTable(1);
            headingTable.WidthPercentage = 100;

            PdfPCell companyNameCell = new PdfPCell(new Phrase("TINAKING INTEGRITY CONSULTING SERVICES INC.\n\n", companyNameFont))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                Border = Rectangle.NO_BORDER,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            headingTable.AddCell(companyNameCell);

            document.Add(headingTable);
        }

        private void AddInvoiceHeader(Document document, InvoiceView invoice)
        {
            Font headerFont = FontFactory.GetFont("Arial", 10, Font.NORMAL);
            Font boldFont = FontFactory.GetFont("Arial", 10, Font.BOLD);
            Font titleFont = FontFactory.GetFont("Arial", 14, Font.BOLD);
            BaseColor lightGrey = new BaseColor(240, 240, 240);

            PdfPTable headerTable = new PdfPTable(new float[] { 1, 2 });
            headerTable.WidthPercentage = 100;
            headerTable.SpacingAfter = 10;  // Adds a bit of space after the table

            Phrase invoiceInfoPhrase = new Phrase();
            invoiceInfoPhrase.Add(new Chunk("Invoice Information\n", titleFont));
            invoiceInfoPhrase.Add(new Chunk($"Invoice Number: {invoice.InvoiceNo}\n", boldFont));
            invoiceInfoPhrase.Add(new Chunk($"Invoice Date: {invoice.Regist.ToString("dd MMM yyyy")}\n\n", boldFont));
            //invoiceInfoPhrase.Add(new Chunk($"Customer: {invoice.Username}\n", boldFont));
            invoiceInfoPhrase.Add(new Chunk("240 Southridge NW,\nEdmonton, AB\nCanada\nT6H 4M9\n", headerFont));

            PdfPCell invoiceInfoCell = new PdfPCell(invoiceInfoPhrase)
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                BackgroundColor = lightGrey,
                Border = Rectangle.NO_BORDER,
                Padding = 8,
                PaddingRight = 12
            };
            headerTable.AddCell(invoiceInfoCell);

            Phrase bankInfoPhrase = new Phrase();
            bankInfoPhrase.Add(new Chunk("Bank Information\n", titleFont));
            bankInfoPhrase.Add(new Chunk("Bank: Royal Bank Of Canada\n" +
                "Bank Swift Code: 92020\n" +
                "Bank Acc No.: 234-4321233\n" +
                "Business Number: \n" +
                "GST #: 524879644\n", headerFont));

            PdfPCell bankInfoCell = new PdfPCell(bankInfoPhrase)
            {
                HorizontalAlignment = Element.ALIGN_RIGHT,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                // Differentiates this section
                Border = Rectangle.NO_BORDER,
                Padding = 8
            };
            headerTable.AddCell(bankInfoCell);

            // Add a line separator
            LineSeparator separator = new LineSeparator();
            separator.LineWidth = 1;
            PdfPCell separatorCell = new PdfPCell(new Phrase(new Chunk(separator)));
            separatorCell.Colspan = 2; // Spans across all columns
            separatorCell.Border = Rectangle.NO_BORDER;
            headerTable.AddCell(separatorCell);

            Phrase contactInfoPhrase = new Phrase();
            contactInfoPhrase.Add(new Chunk("Contact\n", titleFont));
            contactInfoPhrase.Add(new Chunk("Cell: 780-235-5930\n" +
                "E-Mail: k.emeronye@gmail.com\n", headerFont));

            PdfPCell contactInfoCell = new PdfPCell(contactInfoPhrase)
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                BackgroundColor = lightGrey, // Same background color as bank info for consistency
                Border = Rectangle.NO_BORDER,
                Padding = 8,
                Colspan = 2 // This cell spans over 2 columns
            };
            headerTable.AddCell(contactInfoCell);

            document.Add(headerTable);
        }


        private void AddInvoiceTable(Document document, List<PackageView> packageList, InvoiceView invoice)
        {
            Font tableHeaderFont = FontFactory.GetFont("Arial", 10, Font.BOLD);
            Font tableRowFont = FontFactory.GetFont("Arial", 10, Font.NORMAL);
            Font BoldFont = FontFactory.GetFont("Arial", 10, Font.BOLD);

            BaseColor yellowColor = new BaseColor(255, 255, 0);

            PdfPTable itemsTable = new PdfPTable(new float[] { 1, 2, 1, 1, 1, 1 });
            itemsTable.WidthPercentage = 100;
            itemsTable.SpacingBefore = 20;

            itemsTable.AddCell(new PdfPCell(new Phrase("Date", tableHeaderFont)) { BackgroundColor = yellowColor });
            itemsTable.AddCell(new PdfPCell(new Phrase("Services Rendered", tableHeaderFont)) { BackgroundColor = yellowColor });
            itemsTable.AddCell(new PdfPCell(new Phrase("PackageNo.", tableHeaderFont)) { BackgroundColor = yellowColor });
            itemsTable.AddCell(new PdfPCell(new Phrase("Hours", tableHeaderFont)) { BackgroundColor = yellowColor });
            itemsTable.AddCell(new PdfPCell(new Phrase("Reg. $/hr", tableHeaderFont)) { BackgroundColor = yellowColor });
            itemsTable.AddCell(new PdfPCell(new Phrase("Total", tableHeaderFont)) { BackgroundColor = yellowColor });

            foreach (var package in packageList)
            {
                itemsTable.AddCell(new PdfPCell(new Phrase(package.EndDate.ToString("dd MMM yyyy"), tableRowFont)));
                itemsTable.AddCell(new PdfPCell(new Phrase("Technical Review", tableRowFont))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER
                });

                itemsTable.AddCell(new PdfPCell(new Phrase(package.PackageNumber.ToString(), tableRowFont)));
                itemsTable.AddCell(new PdfPCell(new Phrase(package.SpendTime.ToString("F2"), tableRowFont)));
                itemsTable.AddCell(new PdfPCell(new Phrase(package.Rate.ToString("C2"), tableRowFont)));
                double total = package.SpendTime * package.Rate;
                itemsTable.AddCell(new PdfPCell(new Phrase(total.ToString("C2"), tableRowFont)));
            }

            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                   
                    itemsTable.AddCell(new PdfPCell(new Phrase(" ")));
                }
            }

            double subtotal = 0;
            foreach (var package in packageList)
            {
                subtotal += package.SpendTime * package.Rate;
            }
            double gst = subtotal * 0.05;
            double grandTotal = subtotal + gst;

            PdfPTable summaryTable = new PdfPTable(2);
            summaryTable.HorizontalAlignment = Element.ALIGN_RIGHT;
            summaryTable.TotalWidth = 141f;
            summaryTable.LockedWidth = true;
            summaryTable.SpacingBefore = 20;

            summaryTable.AddCell(new PdfPCell(new Phrase("Sub-total:", tableRowFont)) { BackgroundColor = yellowColor });
            summaryTable.AddCell(new PdfPCell(new Phrase(subtotal.ToString("C2"), tableRowFont)));
            summaryTable.AddCell(new PdfPCell(new Phrase("GST:", tableRowFont)) { BackgroundColor = yellowColor });
            summaryTable.AddCell(new PdfPCell(new Phrase(gst.ToString("C2"), tableRowFont)));
            summaryTable.AddCell(new PdfPCell(new Phrase("Grand Total:", BoldFont)) { BackgroundColor = yellowColor });
            summaryTable.AddCell(new PdfPCell(new Phrase(grandTotal.ToString("C2"), BoldFont)));

            PdfPCell summaryCell = new PdfPCell(summaryTable);
            summaryCell.Colspan = 6;
            summaryCell.HorizontalAlignment = Element.ALIGN_RIGHT;


            itemsTable.AddCell(summaryCell);
            document.Add(itemsTable);
        }
    }
}