using FC.CodeFlix.Catalog.Infra.Data.EF;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace FC.CodeFlix.Catalog.EndToEndTests.Base
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup>
        where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("EndToEndTest");
            builder.ConfigureServices(services =>
            {
                var serviceProvider = services.BuildServiceProvider();
                using var scope = serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetService<CodeFlixCatalogDbContext>();
                ArgumentNullException.ThrowIfNull(dbContext);

                dbContext.Database.EnsureDeleted();
                dbContext.Database.EnsureCreated();
            });            
            
            base.ConfigureWebHost(builder);
        }
    }
}
