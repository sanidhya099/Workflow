using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinaKingSystem.ViewModels
{
    public  class ForgetPwdView
    {
        public int ClientID { get; set; }
        public string UserName { get; internal set; }
        public string Email { get; internal set; }
    }
}
