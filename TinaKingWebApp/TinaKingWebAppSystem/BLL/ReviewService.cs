using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinaKingSystem.DAL;
using TinaKingSystem.ViewModels;

namespace TinaKingSystem.BLL
{
    public class ReviewService
    {
        private readonly WFS_2590Context _WFS_2590Context;

        public ReviewService(WFS_2590Context context)
        {
            _WFS_2590Context = context;
        }
        public async Task SaveReviewAsync(ReviewModel reviewModel)
        {
            _WFS_2590Context.Add(reviewModel);
            await _WFS_2590Context.SaveChangesAsync();
        }
    }

}
