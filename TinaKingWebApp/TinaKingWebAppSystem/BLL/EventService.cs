using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TinaKingSystem.DAL;
using TinaKingSystem.Entities;
using TinaKingSystem.ViewModels;

namespace TinaKingSystem.BLL
{
    public class EventService
    {
        private readonly WFS_2590Context? _WFS_2590Context;

        internal EventService(WFS_2590Context WFS_2590Context)
        {
            _WFS_2590Context = WFS_2590Context;
        }

        // Uploads a new event to the database
        public async Task<bool> UploadEvent(
            int UserID,
            string Role,
            string Type,
            int Target,
            string Username,
            string PackageNo,
            string WPSNo
            )
        {
            EventItem newItem = new EventItem
            {
                ClientID = UserID,
                Role = Role,
                Type = Type,
                TargetID = Target,
                Regist = DateTime.Now,
                Username = Username,
                PackageNo = PackageNo,
                WPSNo = WPSNo
            };

            _WFS_2590Context.Events.Add(newItem);

            try
            {
                await _WFS_2590Context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Gets the count of new events for a user with the specified role and type
        public async Task<int> GetNewEventCount(int UserID, string Role, string Type)
        {
            DateTime dtStart;
            var LastEvent = _WFS_2590Context.Events
                    .Where(x => x.ClientID == UserID && x.Role == Role && x.Type == "Clear")
                    .OrderBy(x => x.Regist)
                    .LastOrDefault();

            // Set start date for counting new events
            if (LastEvent == null)
            {
                dtStart = new DateTime();
            }
            else
            {
                dtStart = LastEvent.Regist;
            }

            // Count new events based on type
            if (Type == "")
            {
                return await _WFS_2590Context.Events
                    .Where(x => !(x.ClientID == UserID && x.Role == Role) && x.Regist > dtStart && x.Type != "Clear")
                    .CountAsync();
            }
            else
            {
                return await _WFS_2590Context.Events
                    .Where(x => !(x.ClientID == UserID && x.Role == Role) && x.Regist > dtStart && x.Type == Type)
                    .CountAsync();
            }
        }

        // Gets the list of events asynchronously for a user with the specified role and maximum event ID
        public async Task<List<EventView>> GetEventsAsync(int UserID, string Role, int maxID)
        {
            try
            {
                return await _WFS_2590Context.Events
                            .Where(x => !(x.ClientID == UserID && x.Role == Role) && x.ID > maxID && x.Type != "Clear")
                            .Select(x => new EventView
                            {
                                ID = x.ID,
                                ClientID = x.ClientID,
                                TargetID = x.TargetID,
                                Role = x.Role,
                                Username = x.Username,
                                PackageNo = x.PackageNo,
                                WPSNo = x.WPSNo,
                                Regist = x.Regist,
                                Type = x.Type,
                            }).ToListAsync();
            }
            catch (Exception ex)
            {
                // Return an empty list if an error occurs
                return new List<EventView>();
            }
        }
    }
}
