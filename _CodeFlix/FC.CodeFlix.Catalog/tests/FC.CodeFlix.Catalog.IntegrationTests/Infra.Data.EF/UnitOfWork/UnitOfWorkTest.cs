using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using UnitOfWorkInfra = FC.CodeFlix.Catalog.Infra.Data.EF;

namespace FC.CodeFlix.Catalog.IntegrationTests.Infra.Data.EF.UnitOfWork
{
    [Collection(nameof(UnitOfWorkTestFixture))]
    public class UnitOfWorkTest
    {
        private readonly UnitOfWorkTestFixture _fixture;

        public UnitOfWorkTest(UnitOfWorkTestFixture fixture) => this._fixture = fixture;

        [Fact(DisplayName = nameof(Commit))]
        [Trait("Integration/Infra.Data", "UnitOfWork - Persistence")]
        public async Task Commit()
        {
            var dbContext = this._fixture.CreateDbContext();
            var exampleCategoryList = this._fixture.GetExampleCategoryList();
            await dbContext.AddRangeAsync(exampleCategoryList);
            var unitOfWork = new UnitOfWorkInfra.UnitOfWork(dbContext);
            
            await unitOfWork.Commit(CancellationToken.None);

            var assertDbContext = this._fixture.CreateDbContext(true);
            var savedCategories = assertDbContext.Categories.AsNoTracking().ToList();
            savedCategories.Should().HaveCount(exampleCategoryList.Count);
        }

        [Fact(DisplayName = nameof(Rollback))]
        [Trait("Integration/Infra.Data", "UnitOfWork - Persistence")]
        public async Task Rollback()
        {
            var dbContext = this._fixture.CreateDbContext();
            var unitOfWork = new UnitOfWorkInfra.UnitOfWork(dbContext);

            var task = async () => await unitOfWork.Rollback(CancellationToken.None);

            await task.Should().NotThrowAsync();
        }
    }
}
