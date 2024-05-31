using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinaKingSystem.ViewModels
{
    public class DialogView
    {
        public string Description { get; set; }
        public string Value {  get; set; }

        public string Button1 { get; set; } = "OK";
        public string Button2 { get; set; } = "Cancel";
    }

}
