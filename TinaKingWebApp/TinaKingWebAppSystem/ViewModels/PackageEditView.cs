using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinaKingSystem.ViewModels
{
    public class PackageEditView
    {
        public int PackageID { get; set; }
        public int ClientID { get; set; }
        public DateTime Deadline { get; set; } = DateTime.Now;
        public int Priority { get; set; }

    }
}
