using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinaKingSystem.ViewModels
{
    public class EmployeeView
    {
        public string FirstName { get; set; }

        // Removed [Required] attribute
        public string LastName { get; set; }

        // Removed [Required] attribute
        public string UserName { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        // Removed [Required] attribute
        public string province { get; set; }

        // Removed [Required] attribute
        public string city { get; set; }

        public int EmployeeID { get; set; }

        // Removed [Required] and [EmailAddress] attributes
        public string Email { get; set; }

        // Removed [Required] attribute
        public string Password { get; set; }

        // Removed [Required] attribute
        public string PostalCode { get; set; }

        public int RoleID { get; set; }

        // Removed [Required] attribute
        public string Role { get; set; }

        // Removed [Required] and [RegularExpression] attributes
        public string Phone { get; set; }

        public string Status { get; set; }
    }
}
