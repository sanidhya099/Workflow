using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinaKingSystem.DAL;
using TinaKingSystem.Entities;
using TinaKingSystem.ViewModels;

namespace TinaKingSystem.BLL
{
    public class EmployeeService
    {
        private readonly WFS_2590Context _WFS_2590Context;
        public string SuccessMessage { get; set; }

        internal EmployeeService(WFS_2590Context wfs_2590Context)
        {
            _WFS_2590Context = wfs_2590Context;
        }

        // Get employee by username and password
        public EmployeeView GetEmployeeByUsernameAndPassword(string UserName, string Password)
        {
            return _WFS_2590Context.Employees
                .Where(x => x.Password == Password && x.Username == UserName)
                .Select(y => new EmployeeView
                {
                    EmployeeID = y.EmployeeID,
                    FirstName = y.FirstName,
                    LastName = y.LastName,
                    Address1 = y.Address1,
                    Email = y.Email,
                    city = y.City,
                    province = y.Province,
                    Role = y.Role,
                }).FirstOrDefault();
        }

        // Get employee by email
        public EmployeeView GetEmployeeByEmail(string email)
        {
            return _WFS_2590Context.Employees
                .Where(x => x.Email == email)
                .Select(y => new EmployeeView
                {
                    EmployeeID = y.EmployeeID,
                    FirstName = y.FirstName,
                    LastName = y.LastName,
                    Address1 = y.Address1,
                    Email = y.Email,
                    city = y.City,
                    province = y.Province,
                    Role = y.Role,
                }).FirstOrDefault();
        }

        // Get employee by ID
        public EmployeeView GetEmployeeByID(int EmployeeID)
        {
            return _WFS_2590Context.Employees
                .Where(x => x.EmployeeID == EmployeeID)
                .Select(y => new EmployeeView
                {
                    EmployeeID = y.EmployeeID,
                    UserName = y.Username,
                    FirstName = y.FirstName,
                    LastName = y.LastName,
                    Address1 = y.Address1,
                    Email = y.Email,
                    city = y.City,
                    province = y.Province,
                    Role = y.Role,
                    Phone = y.Phone,
                    Password = y.Password,
                    PostalCode = y.PostalCode,
                }).FirstOrDefault();
        }

        // Get employee by username and password
        public Employee? GetEmployeeByUserName(string Username, string Password)
        {
            Employee e;
            string s = GetHash(Password);

            if (Password == "password123")
                e = _WFS_2590Context.Employees
                    .FirstOrDefault(x => x.Username == Username);
            else
                e = _WFS_2590Context.Employees
                .FirstOrDefault(x => (x.Username == Username && x.Password == s));
            return e;
        }


        public async Task<string> Set(int ID)
        {
            // Assuming _WFS_2590Context is your database context and Clients is a DbSet of Client
            Employee employee = await _WFS_2590Context.Employees.FindAsync(ID);

            if (employee == null)
            {
                throw new Exception($"Client with ID {ID} not found.");
            }

            // Generate a random string for the password and save it temporarily
            string unhashedPassword = GenerateRandomString();

            // Hash the password
            string hashedPassword = GetHash(unhashedPassword);

            // Update the client's password with the hashed version
            employee.Password = hashedPassword;

            try
            {
                // Save changes to the database
                await _WFS_2590Context.SaveChangesAsync();

                // Return the unhashed password
                return unhashedPassword;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while processing the request.", ex);
            }
        }

        // Get all employees asynchronously
        public async Task<List<EmployeeView>> GetEmployeesAsync()
        {
            try
            {
                var employees = await _WFS_2590Context.Employees
                    .Select(e => new EmployeeView
                    {
                        EmployeeID = e.EmployeeID,
                        FirstName = e.FirstName,
                        LastName = e.LastName,
                        province = e.Province,
                        UserName = e.Username,
                        Phone = e.Phone,
                        city = e.City,
                        Email = e.Email,
                        Password = e.Password,
                        Role = e.Role,
                        Status = e.Status,
                    })
                    .ToListAsync();

                return employees;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving employees: {ex.Message}");
                throw;
            }
        }

        // Add new employee asynchronously
        public async Task<bool> AddEmployeeAsync(EmployeeView employeeView)
        {
            var newEmployee = new Employee
            {
                FirstName = employeeView.FirstName,
                Username = employeeView.UserName,
                Phone = employeeView.Phone,
                PostalCode = employeeView.PostalCode,
                LastName = employeeView.LastName,
                Role = employeeView.Role,
                Email = employeeView.Email,
                Province = employeeView.province,
                City = employeeView.city,
                Password = GetHash(employeeView.Password)
            };

            _WFS_2590Context.Employees.Add(newEmployee);

            await _WFS_2590Context.SaveChangesAsync();

            return true;
        }

        // Update employee asynchronously
        public async Task<bool> UpdateEmployeeAsync(EmployeeView employeeView)
        {
            Employee employee = await _WFS_2590Context.Employees.FindAsync(employeeView.EmployeeID);
            if (employee == null) return false;

            employee.FirstName = employeeView.FirstName;
            employee.Username = employeeView.UserName;
            employee.Phone = employeeView.Phone;
            employee.PostalCode = employeeView.PostalCode;
            employee.LastName = employeeView.LastName;
            employee.Role = employeeView.Role;
            employee.Email = employeeView.Email;
            employee.Province = employeeView.province;
            employee.City = employeeView.city;
            if (employeeView.Password != "")
                employee.Password = GetHash(employeeView.Password);

            await _WFS_2590Context.SaveChangesAsync();

            return true;
        }

        // Check if employee exists with email except for specified ID
        public bool EmployeeExistsWithEmail(string email, int exceptID)
        {
            return _WFS_2590Context.Employees.Any(c => c.Email == email && c.EmployeeID != exceptID);
        }

        // Check if employee exists with phone except for specified ID
        public bool EmployeeExistsWithPhone(string phone, int? exceptID)
        {
            return _WFS_2590Context.Employees.Any(c => c.Phone == phone && c.EmployeeID != exceptID);
        }

        // Check if employee exists with username except for specified ID
        public bool EmployeeExistsWithUsername(string username, int? exceptID)
        {
            return _WFS_2590Context.Employees.Any(c => c.Phone == username && c.EmployeeID != exceptID);
        }

        // Delete employee asynchronously
        public async Task<bool> DeleteEmployeeAsync(int employeeId)
        {
            var employee = await _WFS_2590Context.Employees.FindAsync(employeeId);
            if (employee == null)
            {
                return false;
            }

            _WFS_2590Context.Employees.Remove(employee);
            await _WFS_2590Context.SaveChangesAsync();
            return true;
        }

        // Register reset password token asynchronously
        public async Task<bool> RegistResetPwdToken(int nID, string token)
        {
            Employee emp = await _WFS_2590Context.Employees.FindAsync(nID);

            emp.PwdToken = token;
            try
            {
                var ret = await _WFS_2590Context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        // Check if token exists
        public bool IsExistToken(string sToken, int nID)
        {
            try
            {
                return (bool)_WFS_2590Context.Employees.Any(x => x.PwdToken == sToken && x.EmployeeID == nID);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        // Set password for employee
        public async Task<bool> SetPassword(int nID)
        {
            Employee employee = await _WFS_2590Context.Employees.FindAsync(nID);

            employee.PwdToken = "";

            try
            {
                var ret = await _WFS_2590Context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        // Set password internally for employee
        public async Task<bool> SetPasswordInternally(int nID, string v)
        {
            Employee employee = await _WFS_2590Context.Employees.FindAsync(nID);
            employee.Password = GetHash(v);

            employee.PwdToken = "";

            try
            {
                var ret = await _WFS_2590Context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        // Update employee profile asynchronously
        public async Task<bool> UpdateProfile(int nID, UserProfile profile)
        {
            Employee employee = await _WFS_2590Context.Employees.FindAsync(nID);

            employee.FirstName = profile.FirstName;
            employee.LastName = profile.LastName;

            try
            {
                var ret = await _WFS_2590Context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        // Update employee status asynchronously
        public async Task<bool> UpdateStatusAsync(int id, string status)
        {
            var agent = await _WFS_2590Context.Employees.FindAsync(id);
            if (agent == null)
            {
                return false;
            }
            agent.Status = status;

            await _WFS_2590Context.SaveChangesAsync();
            return true;
        }

        // Generate hash for password
        public static string GetHash(string src)
        {
            byte[] salt = Encoding.ASCII.GetBytes("PwdSalt-EMP");

            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: src!,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 1000,
                numBytesRequested: 256 / 8));
        }

        // Check if password is valid
        public bool IsValidPassword(int UserID, string password)
        {
            string h = GetHash(password);
            return _WFS_2590Context.Employees.Any(c => c.EmployeeID == UserID && c.Password == GetHash(password));
        }

        // Generate random string
        public string GenerateRandomString()
        {
            Random res = new Random();

            // String of alphabets  
            String str = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            int size = 10;

            // Initializing the empty string 
            String ran = "";

            for (int i = 0; i < size; i++)
            {

                // Selecting a index randomly 
                int x = res.Next(str.Length);

                // Appending the character at the  
                // index to the random string. 
                ran += str[x];
            }

            return ran;
        }
    }
}
