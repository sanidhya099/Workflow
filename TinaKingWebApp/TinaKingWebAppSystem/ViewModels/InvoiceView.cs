using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinaKingSystem.ViewModels
{
    public class InvoiceView
    {
        public int ID { get; set; }
        public string InvoiceNo { get; set; } 
        public int UserID { get; set; }
        public string Address { get; set; } = "240 SouthRidge NW";
        public string City { get; set; } = "Edmonton, AB";
        public string Country { get; set; } = "Canada";
        public string PostalCode { get; set; } = "T6H 4M9";
        public string Business { get; set; } = "";
        public string Username { get; set; } = "";
        public string Email {  get; set; } = "k.emeronye@gmail.com";
        public int PackageCount { get; set; }
        public double Total { get; set; }
        public string Status { get; set; }
        public DateTime Regist { get; set; } 
        public double GST { get; set; }
        public double GrandTotal { get; set; }
    }

}
