using Bogus;
using FC.CodeFlix.Catalog.Infra.Data.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;

namespace FC.CodeFlix.Catalog.EndToEndTests.Base
{
    public class BaseFixture
    {
        private readonly string _dbConntectionString;

        protected Faker Faker { get; set; }

        public CustomWebApplicationFactory<Program> WebAppFactory { get; set; }
        public HttpClient HttpClient { get; set; }
        public ApiClient ApiClient { get; set; }

        public BaseFixture()
        {
            Faker = new Faker("pt_BR");
            WebAppFactory = new CustomWebApplicationFactory<Program>();
            HttpClient = WebAppFactory.CreateClient();
            ApiClient = new ApiClient(HttpClient);
            var configuration = WebAppFactory.Services.GetService(typeof(IConfiguration));
            ArgumentNullException.ThrowIfNull(configuration);
            this._dbConntectionString = ((IConfiguration)configuration).GetConnectionString("CatalogDb");
        }

        public CodeFlixCatalogDbContext CreateDbContext() => new(new DbContextOptionsBuilder<CodeFlixCatalogDbContext>()
            .UseMySql(this._dbConntectionString, ServerVersion.AutoDetect(this._dbConntectionString)).Options);

        public void CleanPersistence()
        {
            var context = this.CreateDbContext();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }
    }
}
