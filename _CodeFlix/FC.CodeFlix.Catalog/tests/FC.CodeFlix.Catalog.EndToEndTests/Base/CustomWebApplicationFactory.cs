using FC.CodeFlix.Catalog.Infra.Data.EF;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace FC.CodeFlix.Catalog.EndToEndTests.Base
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup>
        where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var dbOptions = services.FirstOrDefault(p => p.ServiceType.Equals(typeof(DbContextOptions<CodeFlixCatalogDbContext>)));
                if (dbOptions is not null)
                    services.Remove(dbOptions);

                services.AddDbContext<CodeFlixCatalogDbContext>(options => { options.UseInMemoryDatabase("end2end-tests-db"); });
            });            
            
            base.ConfigureWebHost(builder);
        }
    }
}
