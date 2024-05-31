using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TinaKingSystem.ViewModels
{
    public class EventView
    {
        public int ID { get; set; }

        public int ClientID { get; set; }
        public string Type { get; set; }
        public int TargetID { get; set; }
        public string Role { get; set; }
        public string Username { get; set; }
        public string PackageNo { get; set; }
        public string WPSNo { get; set; }
        public DateTime Regist { get; set; }
    }
}
