using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinaKingSystem.ViewModels
{
    public  class ClientPackageView
    {
        public int ClientID { get; set; }
        public List<PackageView> PackageDetails { get; set; }
        public List<ClientSearchView> ClientDetails { get; set; }
        public string FirstName { get; internal set; }
        public string Email { get; internal set; }
    }
}
