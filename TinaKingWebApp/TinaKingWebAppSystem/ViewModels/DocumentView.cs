using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinaKingSystem.ViewModels
{
    public class DocumentView
    {
        public int DocumentID { get; set; }
        public int ClientID { get; set; }
        public string DocumentName { get; set; }
        public string DocumentNumer { get; set; }
        public double DocumentSize { get; set; }
    }
}
