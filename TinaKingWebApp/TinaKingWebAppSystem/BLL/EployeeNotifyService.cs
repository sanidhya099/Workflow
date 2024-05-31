using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinaKingSystem.BLL
{
    public class EployeeNotifyService
    {
        public int nEmployeeID { get; set; }

        [Inject] public PackageService PackageService { get; set; }
        [Inject] public EmployeeService EmployeeService { get; set; }   

        public void RegisterNotify()
        {

        }

    }
}
