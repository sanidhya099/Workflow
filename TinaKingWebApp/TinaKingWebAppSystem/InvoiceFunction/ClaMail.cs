using EmailService;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TinaKingSystem.ViewModels;

namespace TinaKingSystem.InvoiceFunction
{
    public class ClaMail
    {
        private readonly EmailSender emailSender;
        private const double GstRate = 0.05; 

        private const string MainTemplate = @"
<html>
<head>
    <title>TinaKing Invoice</title>
    <style>
        body { font-family: Arial, sans-serif; }
        table { width: 80%; border-collapse: collapse; }
        th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }
        th { background-color: yellow; }
        .footer { background-color:yellow; font-weight:bold; }
    </style>
</head>
<body>
    <h2>TinaKing Consulting Services Inc.</h2>
    <h5> Invoice Number : {InvoiceNo} <h5>
    <div class='content-container' style='display: flex; justify-content: space-around;'>
        <div class='header-section'>
            <p>Invoice Date: {DATE}<br>240 Southridge NW,<br>Edmonton, AB<br>Canada<br>T6H 4M9<br><br>Cell: 780-235-5930<br>E-Mail: k.emeronye@gmail.com</p>
        </div>
        <div class='invoice-details'>
            <p>Bank: Royal Bank Of Canada<br>Bank Swift Code: 92020<br>Bank Acc No.: 234-4321233<br>Business Number: {BUSINESSNO}<br>GST #: {GSTNO}</p>
        </div>
    </div>
    <table class='invoice-table'>
        <tr>
            <th>Date</th>
            <th>Services Rendered</th>
            <th>Package No.</th>
            <th>Hours Worked</th>
            <th>Reg. $/hr</th>
            <th>Total</th>
        </tr>
        {PACKAGE}
        <tr >
            <td colspan='3'><td>
            <td class='footer'>Sub-total</td>
            <td class='footer'>{TOTAL}</td>
        </tr>
        <tr >
            <td colspan='3'><td>
            <td class='footer'>GST</td>
            <td class='footer'>{GST}</td>
        </tr>
        <tr >
            <td colspan='3'><td>
            <td class='footer'>Grand Total(CAD)</td>
            <td class='footer'>{GTOTAL}</td>
        </tr>
    </table>
</body>
</html>";

        private const string PackageTemplate = @"
<tr>
    <td>{END}</td>
    <td>{SERVICERENDERED}</td>
    <td>{NO}</td>
    <td>{SPEND}</td>
    <td>{RATE}</td>
    <td>{TOTAL}</td>
</tr>";

        public ClaMail(EmailSender emailSender)
        {
            this.emailSender = emailSender;
        }

        public async Task<bool> SendAsync(string email, InvoiceView invoiceView, List<PackageView> packages)
        {
            string body = GenerateBody(invoiceView, packages);
            try
            {
                await emailSender.Send(email, "TinaKing Invoice", body);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private string GenerateBody(InvoiceView invoiceView, List<PackageView> packages)
        {
            StringBuilder packageDetails = new StringBuilder();
            double total = 0;
            foreach (var package in packages)
            {
                double packageTotal = package.SpendTime * package.Rate;
                packageDetails.AppendLine(PackageTemplate
                    .Replace("{END}", package.EndDate.ToString("dd MMM yyyy"))
                    .Replace("{SERVICERENDERED}", "Technical Review")
                    .Replace("{NO}", package.PackageNumber)
                    .Replace("{SPEND}", package.SpendTime.ToString("F2"))
                    .Replace("{RATE}", package.Rate.ToString("C2"))
                    .Replace("{TOTAL}", packageTotal.ToString("C2")));
                total += packageTotal;
            }

            double gst = total * GstRate;
            double grandTotal = total + gst;

            return MainTemplate
                .Replace("{DATE}", invoiceView.Regist.ToString("dd MMM yyyy"))
                .Replace("{InvoiceNo}", invoiceView.InvoiceNo)
                .Replace("{BUSINESSNO}", invoiceView.Business)
                .Replace("{GSTNO}", "524879644")
                .Replace("{PACKAGE}", packageDetails.ToString())
                .Replace("{TOTAL}", total.ToString("C2"))
                .Replace("{GST}", gst.ToString("C2"))
                .Replace("{GTOTAL}", grandTotal.ToString("C2"));
        }
    }
}
