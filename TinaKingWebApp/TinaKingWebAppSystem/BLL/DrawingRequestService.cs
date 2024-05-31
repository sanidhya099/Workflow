using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinaKingSystem.DAL;
using TinaKingSystem.Entities;
using TinaKingSystem.ViewModels;

namespace TinaKingSystem.Services
{
    public class DrawingRequestService
    {
        private readonly WFS_2590Context _WFS_2590Context;

        public DrawingRequestService(WFS_2590Context WFS_2590Context)
        {
            _WFS_2590Context = WFS_2590Context;
        }

        // Generate a new ID for drawing request
        private int GenerateNewRequestId()
        {
            // Get the maximum ID from existing drawing requests and increment by 1
            int maxId = _WFS_2590Context.DrawingRequests.Max(r => (int?)r.DrawingRequestID) ?? 0;
            int newId = maxId + 1;

            return newId;
        }

        // Save drawing request asynchronously
        public async Task SaveDrawingRequest(DrawingRequestView drawingRequest)
        {
            // Generate a new ID for the drawing request
            int newRequestId = GenerateNewRequestId();

            // Create a new DrawingRequest entity from drawingRequest view model
            var entity = new DrawingRequest
            {
                DrawingRequestID = newRequestId,
                IssuedForConstruction = drawingRequest.IssuedForConstruction,
                WeldMapComplete = drawingRequest.WeldMapComplete,
                ThicknessRangeQualifiedAcceptable = drawingRequest.ThicknessRangeQualifiedAcceptable,
                CorrectWPS = drawingRequest.CorrectWPS,
                //AdditionalComments = drawingRequest.AdditionalComments
            };

            // Add the new drawing request entity to the DbSet and save changes to the database
            _WFS_2590Context.DrawingRequests.Add(entity);
            await _WFS_2590Context.SaveChangesAsync();
        }
    }
}
