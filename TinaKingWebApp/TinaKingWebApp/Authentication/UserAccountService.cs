using TinaKingSystem.DAL;
using TinaKingSystem.Entities;
using TinaKingSystem.ViewModels;

namespace TinaKingWebApp.Authentication
{
    public class UserAccountService
    {
        private readonly WFS_2590Context? _WFS_2590Context;
        public UserAccountService(WFS_2590Context WFS_2590Context)
        {
            _WFS_2590Context = WFS_2590Context;
        }

        public ClientSearchView? GetClient(string UserName, string Password)
        {
            return _WFS_2590Context.Clients
                .Where(x => x.Username == UserName && x.Password == Password)
                .Select(c => new ClientSearchView
                {
                    ClientID = c.ClientID,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    Email = c.Email,
                    Phone = c.Phone,
                    City = c.City,
                    UserName = c.Username,
                    Password = c.Password,
                    Role = c.Role
                })
                .FirstOrDefault();
        }

        public Client GetByUserName(string Username)
        {
            return _WFS_2590Context.Clients
                .FirstOrDefault(x => x.Username == Username);
        }
    }
}

