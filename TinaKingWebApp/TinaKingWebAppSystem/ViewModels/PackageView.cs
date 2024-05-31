using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinaKingSystem.ViewModels
{
    public class PackageView
    {
        public int PackageID { get; set; }
        public int ClientID { get; set; }
        public string ClientName { get; set; }
        public string Email { get; set; }
        public string PackageNumber { get; set; }
        public string TypeOfRequest { get; set; }
        public DateTime DateSubmitted { get; set; } = DateTime.Today;
        public DateTime Deadline { get; set; }
        public DateTime EndDate { get; set; } 
        public string Status { get; set; }
        public int FollowUpPackage { get; set; }
        public int Priority { get; set; }
        public string Information { get; set; }
        public List<string> DocumentUrls { get; set; } = new List<string>(); 
        public List<string> DocumentNames { get; set; } = new List<string>();
        public string str_documents;

        public int SpendTime;
        public int Rate;
        public string Summery;
        public int Total;

        public string ApproveState;
        public bool Selected { get; set; } = true;

        public string GetSpent()
        {
            string ret = "";
            DateTime now = DateTime.Now;
            int h = (int)(now - DateSubmitted).TotalHours;
            if (h > 24)
            {
                ret = (int)(h / 24) + "day(s)";
            }
            h = h % 24;
            if (h > 0) ret += " " + h + "hour(s)";
            return ret;
        }
    }

}
