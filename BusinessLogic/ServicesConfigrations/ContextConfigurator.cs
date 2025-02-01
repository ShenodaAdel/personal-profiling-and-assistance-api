using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.ServicesConfigrations
{
    public static class ContextConfigurator
    {
        public static void ApplicationContextConfigurator(this IServiceCollection services, string? connectionString)
        {
            services.AddDbContext<MyDbContext>(options => options.UseSqlServer(connectionString));
        }
    }
}
