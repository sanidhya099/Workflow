using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinaKingSystem.BLL;
using TinaKingSystem.DAL;
using TinaKingSystem.Services;

namespace TinaKingSystem
{
    public static class TinaKingExtension
    {
        public static void AddTinaKingDependencies(this IServiceCollection services,
            Action<DbContextOptionsBuilder> options)
        {


            services.AddDbContext<WFS_2590Context>(options);

            services.AddTransient<EmployeeService>((ServiceProvider) =>
            {
                var context = ServiceProvider.GetService<WFS_2590Context>();
                return new EmployeeService(context);
            });
            services.AddTransient<ClientDataService>((ServiceProvider) =>
            {
                var context = ServiceProvider.GetService<WFS_2590Context>();
                return new ClientDataService(context);
            });


            services.AddTransient<ReviewFormService>((ServiceProvider) =>
            {
                var context = ServiceProvider.GetService<WFS_2590Context>();
                return new ReviewFormService(context);
            });

            services.AddTransient<PackageService>((ServiceProvider) =>
            {
                var context = ServiceProvider.GetService<WFS_2590Context>();
                return new PackageService(context);
            });
            services.AddTransient<DrawingRequestService>((ServiceProvider) =>
            {
                var context = ServiceProvider.GetService<WFS_2590Context>();
                return new DrawingRequestService(context);
            });
            services.AddTransient<ClientInputService>((ServiceProvider) =>
                {
                    var context = ServiceProvider.GetService<WFS_2590Context>();
                    return new ClientInputService(context);
                });

            services.AddTransient<PackageService>((ServiceProvider) =>
            {
                var context = ServiceProvider.GetService<WFS_2590Context>();
                return new PackageService(context);
            });

            services.AddTransient<EventService>((ServiceProvider) => {
                var context = ServiceProvider.GetService<WFS_2590Context>();
                return new EventService(context);
            });

            services.AddTransient<InvoiceService>((ServiceProvider) => {
                var context = ServiceProvider.GetService<WFS_2590Context>();
                return new InvoiceService(context);
            });
            services.AddTransient<DrawingService>((ServiceProvider) => {
                var context = ServiceProvider.GetService<WFS_2590Context>();
                return new DrawingService(context);
            });
        }
    }
}
