using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Security.Claims;
using System.Collections.Generic;
using System.Threading.Tasks;
using TinaKingSystem.Entities;

namespace TinaKingWebApp.Authentication
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ProtectedSessionStorage _sessionStorage;

        public CustomAuthenticationStateProvider(ProtectedSessionStorage sessionStorage)
        {
            _sessionStorage = sessionStorage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var userSessionResult = await _sessionStorage.GetAsync<UserSession>("UserSession");
                if (!userSessionResult.Success || userSessionResult.Value == null)
                {
                    return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                }

                var userSession = userSessionResult.Value;
                var identity = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, userSession.UserName),
                    new Claim("FirstName", userSession.FirstName), // Custom claim for FirstName
                    new Claim(ClaimTypes.Role, userSession.Role),
                    new Claim("UserID", userSession.UserID.ToString())
                }, "CustomAuthType");

                var user = new ClaimsPrincipal(identity);
                return new AuthenticationState(user);
            }
            catch
            {
                // Log exception
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
        }

        public async Task UpdateAuthenticationState(UserSession userSession)
        {
            try
            {
                Console.WriteLine($"Updating authentication state for user: {userSession?.UserName}");
                if (userSession != null)
                {
                    await _sessionStorage.SetAsync("UserSession", userSession);

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, userSession.UserName),
                        new Claim(ClaimTypes.Role, userSession.Role),
                        // Ensure you're checking for null or empty values before adding
                        new Claim("FirstName", userSession.FirstName ?? ""),
                        new Claim("UserID", userSession.UserID.ToString())
                    };

                    var identity = new ClaimsIdentity(claims, "CustomAuthType");
                    var user = new ClaimsPrincipal(identity);
                    NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
                }
                else
                {
                    await _sessionStorage.DeleteAsync("UserSession");
                    NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()))));
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()))));
            }
        }

        public async Task<string> GetCurrentUserClientId()
        {
            var authState = await GetAuthenticationStateAsync();
            var user = authState.User;

            if (user.Identity.IsAuthenticated)
            {
                // Assuming ClientID is stored as a claim
                var clientIdClaim = user.FindFirst("UserID")?.Value;
                if (!string.IsNullOrEmpty(clientIdClaim))
                {
                    return clientIdClaim;
                }
            }

            return null; // Or handle accordingly if the user is not authenticated or ClientID is not found
        }

    }
}
