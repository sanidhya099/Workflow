using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinaKingSystem.ViewModels
{
    public class DrawingRequestView
    {
        public int DrawingRequestID { get; set; }   
        public int PackageID { get; set; }
        public Boolean IssuedForConstruction { get; set; }
        public Boolean WeldMapComplete { get; set; }
        public Boolean ThicknessRangeQualifiedAcceptable { get; set; }
        public Boolean CorrectWPS {  get; set; }
        public int EmployeeID {  get; set; }
        public string AdditionalComments { get; set; }
    }
}
