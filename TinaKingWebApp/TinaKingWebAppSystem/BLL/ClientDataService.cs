using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core.Tokenizer;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using TinaKingSystem.DAL;
using TinaKingSystem.Entities;
using TinaKingSystem.ViewModels;

namespace TinaKingSystem.BLL
{
    public class UserProfile
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class ClientDataService
    {
        private readonly WFS_2590Context? _WFS_2590Context;

        internal ClientDataService(WFS_2590Context WFS_2590Context)
        {
            _WFS_2590Context = WFS_2590Context;
        }

        #region Public Methods

        // Get client by username and password
        public ClientSearchView GetClient(string UserName, string Password)
        {
            return _WFS_2590Context.Clients
                .Where(x => x.Username == UserName && x.Password == GetHash(Password))
                .Select(c => new ClientSearchView
                {
                    ClientID = c.ClientID,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    Email = c.Email,
                    Phone = c.Phone,
                    City = c.City
                })
                .FirstOrDefault();
        }

        // Get client by ID
        public Client? GetByID(int ID)
        {
            return _WFS_2590Context.Clients.FirstOrDefault(x => x.ClientID == ID);
        }

        // Get client by username and password
        public Client? GetByUserName(string Username, string Password)
        {
            return _WFS_2590Context.Clients.FirstOrDefault(x => x.Username == Username && x.Password == GetHash(Password));
        }

        // Get client by email
        public Client? GetByEmail(string Email)
        {
            DbSet<Client> cList = _WFS_2590Context.Clients;
            try
            {
                return cList.FirstOrDefault(x => x.Email == Email);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        // Get list of clients
        public async Task<List<ClientSearchView>> GetClients()
        {
            return await _WFS_2590Context.Clients
                .Select(c => new ClientSearchView
                {
                    ClientID = c.ClientID,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    UserName = c.Username,
                    Password = "",
                    Company = c.Company,
                    Email = c.Email,
                    Phone = c.Phone,
                    City = c.City,
                    Status = c.Status,
                })
                .ToListAsync();
        }

        // Get client edit view
        public ClientEditView GetClientEdit(int ClientID)
        {
            if (ClientID == 0)
            {
                throw new ArgumentNullException("Please provide a Client");
            }

            return _WFS_2590Context.Clients
                .Where(x => (x.ClientID == ClientID))
                .Select(x => new ClientEditView
                {
                    ClientID = x.ClientID,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Province = x.Province,
                    City = x.City,
                    Company = x.Company,
                    Phone = x.Phone,
                    Email = x.Email,
                    Password = "",
                    UserName = x.Username,
                    Address1 = x.Address1
                }).FirstOrDefault();
        }

        // Get clients by last name or phone number
        public List<ClientSearchView> GetClients(string lastName, string phone)
        {
            if (string.IsNullOrWhiteSpace(lastName) && string.IsNullOrWhiteSpace(phone))
            {
                throw new ArgumentNullException("Please provide either a last name and/or phone number");
            }

            if (string.IsNullOrWhiteSpace(lastName))
            {
                lastName = Guid.NewGuid().ToString();
            }

            if (string.IsNullOrWhiteSpace(phone))
            {
                phone = Guid.NewGuid().ToString();
            }

            return _WFS_2590Context.Clients
                .Where(x => (x.LastName.Contains(lastName) || x.Phone.Contains(phone)))
                .Select(c => new ClientSearchView
                {
                    ClientID = c.ClientID,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    Email = c.Email,
                    Phone = c.Phone,
                    City = c.City,
                })
                .ToList();
        }

        // Add or edit client
        public void AddEditClient(ClientEditView editClient)
        {
            // Business Logic and Parameter Exceptions
            List<Exception> errorList = new List<Exception>();

            if (editClient == null)
            {
                throw new ArgumentNullException("No client was supplied");
            }

            // Checking required fields
            if (string.IsNullOrWhiteSpace(editClient.FirstName))
            {
                errorList.Add(new Exception("First name is required"));
            }

            if (string.IsNullOrWhiteSpace(editClient.LastName))
            {
                errorList.Add(new Exception("Last name is required"));
            }

            if (string.IsNullOrWhiteSpace(editClient.Email))
            {
                errorList.Add(new Exception("Email is required"));
            }

            if (string.IsNullOrWhiteSpace(editClient.Phone))
            {
                errorList.Add(new Exception("Phone is required"));
            }

            Client client = new Client();

            // Set client properties
            client.Username = editClient.UserName;
            client.Role = editClient.Role = "Client";
            client.FirstName = editClient.FirstName;
            client.LastName = editClient.LastName;
            client.City = editClient.City;
            client.Province = editClient.Province;
            client.Address1 = editClient.Address1;
            client.Company = editClient.Company;
            client.Email = editClient.Email;
            client.Phone = editClient.Phone;
            client.Password = GetHash(editClient.Password);
            client.Status = "active";

            // Add client to context and save changes
            _WFS_2590Context.Clients.Add(client);
            _WFS_2590Context.SaveChanges();
            if (errorList.Count > 0)
            {
                throw new AggregateException("Unable to add or edit client. Please check error message(s)", errorList);
            }
        }

        // Check if client with given email exists
        public bool ClientExistsWithEmail(string email, int currentClientId)
        {
            return _WFS_2590Context.Clients.Any(c => c.Email == email && c.ClientID != currentClientId);
        }

        // Check if client with given phone number exists
        public bool ClientExistsWithPhone(string phone, int currentClientId)
        {
            return _WFS_2590Context.Clients.Any(c => c.Phone == phone && c.ClientID != currentClientId);
        }

        // Delete client asynchronously
        public async Task<bool> DeleteClientAsync(int clientId)
        {
            var client = await _WFS_2590Context.Clients.FindAsync(clientId);
            if (client == null)
            {
                return false;
            }

            _WFS_2590Context.Clients.Remove(client);
            await _WFS_2590Context.SaveChangesAsync();
            return true;
        }

        // Update client status asynchronously
        public async Task<bool> UpdateClientStatusAsync(int clientId, string status)
        {
            var client = await _WFS_2590Context.Clients.FindAsync(clientId);
            if (client == null)
            {
                return false;
            }
            client.Status = status;

            await _WFS_2590Context.SaveChangesAsync();
            return true;
        }

        // Register client password reset token
        public async Task<bool> RegistClientResetPwdToken(int ClientID, string token)
        {
            Client client = await _WFS_2590Context.Clients.FindAsync(ClientID);

            client.PwdToken = token;

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

        // Check if token exists for a client
        public bool IsExistToken(string sToken, int nID)
        {
            try
            {
                return (bool)_WFS_2590Context.Clients.Any(x => x.PwdToken == sToken && x.ClientID == nID);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        // Set password for a client
        public async Task<string> Set(int ID)
        {
            Client client = await _WFS_2590Context.Clients.FindAsync(ID);

            if (client == null)
            {
                throw new Exception($"Client with ID {ID} not found.");
            }

            string unhashedPassword = GenerateRandomString();

            string hashedPassword = GetHash(unhashedPassword);

            client.Password = hashedPassword;

            try
            {
                await _WFS_2590Context.SaveChangesAsync();

                return unhashedPassword;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while processing the request.", ex);
            }
        }

        // Set password for a client
        public async Task<bool> SetPassword(int nID)
        {
            Client client = await _WFS_2590Context.Clients.FindAsync(nID);

            client.PwdToken = "";

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

        // Set password internally for a client
        public async Task<bool> SetPasswordInternally(int nID, string v)
        {
            Client client = await _WFS_2590Context.Clients.FindAsync(nID);
            client.Password = GetHash(v);

            client.PwdToken = "";

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

        // Update client profile
        public async Task<bool> UpdateProfile(int nID, UserProfile profile)
        {
            Client client = await _WFS_2590Context.Clients.FindAsync(nID);

            client.FirstName = profile.FirstName;
            client.LastName = profile.LastName;

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

        // Update client information
        public async Task UpdateClientInfo(ClientEditView editClient)
        {
            if (editClient == null)
            {
                throw new ArgumentNullException(nameof(editClient), "No client was supplied");
            }
            List<Exception> errorList = new List<Exception>();

            if (string.IsNullOrWhiteSpace(editClient.FirstName))
            {
                errorList.Add(new ArgumentException("First name is required"));
            }

            if (string.IsNullOrWhiteSpace(editClient.LastName))
            {
                errorList.Add(new ArgumentException("Last name is required"));
            }

            if (string.IsNullOrWhiteSpace(editClient.Email))
            {
                errorList.Add(new ArgumentException("Email is required"));
            }

            if (string.IsNullOrWhiteSpace(editClient.Phone))
            {
                errorList.Add(new ArgumentException("Phone is required"));
            }

            if (editClient.ClientID == 0)
            {
                bool clientExists = _WFS_2590Context.Clients.Any(x => x.FirstName == editClient.FirstName && x.LastName == editClient.LastName && x.Phone == editClient.Phone);
                if (clientExists)
                {
                    errorList.Add(new InvalidOperationException("Client already exists in the database and cannot be added again"));
                }
            }
            if (errorList.Any())
            {
                throw new AggregateException("Unable to add or edit client. Please check error message(s)", errorList);
            }

            try
            {
                Client client = await _WFS_2590Context.Clients.FindAsync(editClient.ClientID);
                if (client == null)
                {
                    throw new KeyNotFoundException($"Client with ID {editClient.ClientID} not found.");
                }
                client.Username = editClient.UserName;
                client.Role = "Client";
                client.FirstName = editClient.FirstName;
                client.LastName = editClient.LastName;
                client.City = editClient.City;
                client.Province = editClient.Province;
                client.Address1 = editClient.Address1;
                client.Company = editClient.Company;
                client.Email = editClient.Email;
                client.Phone = editClient.Phone;


                if (!string.IsNullOrWhiteSpace(editClient.Password))
                {
                    client.Password = GetHash(editClient.Password);
                }

                await _WFS_2590Context.SaveChangesAsync();

            }
            catch (Exception e)
            {
                errorList.Add(e);
                throw new AggregateException("An error occurred while updating the client information.", errorList);
            }
        }

        // Check if password is valid for a client
        public bool IsValidPassword(int UserID, string password)
        {
            return _WFS_2590Context.Clients.Any(c => c.ClientID == UserID && c.Password == GetHash(password));
        }

        // Generate random string
        public string GenerateRandomString()
        {
            Random res = new Random();

            String str = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            int size = 10;

            String ran = "";

            for (int i = 0; i < size; i++)
            {
                int x = res.Next(str.Length);
                ran += str[x];
            }

            return ran;
        }

        #endregion

        #region Private Methods

        // Get hash for a given string
        private static string GetHash(string src)
        {
            byte[] salt = Encoding.ASCII.GetBytes("PwdSalt-CLI");

            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: src!,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 1000,
                numBytesRequested: 256 / 8));
        }

        #endregion
    }
}
