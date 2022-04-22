using Bogus;
using FC.CodeFlix.Catalog.Infra.Data.EF;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;

namespace FC.CodeFlix.Catalog.EndToEndTests.Base
{
    public class BaseFixture
    {
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
        }

        public CodeFlixCatalogDbContext CreateDbContext() => new CodeFlixCatalogDbContext(new DbContextOptionsBuilder<CodeFlixCatalogDbContext>()
            .UseInMemoryDatabase($"end2end-tests-db").Options);

        public void CleanPersistence()
        {
            var context = this.CreateDbContext();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }
    }
}
