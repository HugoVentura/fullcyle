using FC.CodeFlix.Catalog.Application.Exceptions;
using FC.CodeFlix.Catalog.Application.UseCases.Category.DeleteCategory;
using FC.CodeFlix.Catalog.Infra.Data.EF;
using FC.CodeFlix.Catalog.Infra.Data.EF.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using ApplicationUseCase = FC.CodeFlix.Catalog.Application.UseCases.Category.DeleteCategory;

namespace FC.CodeFlix.Catalog.IntegrationTests.Application.UseCases.Category.DeleteCategory
{
    [Collection(nameof(DeleteCategoryTestFixture))]
    public class DeleteCategoryTest
    {
        private readonly DeleteCategoryTestFixture _fixture;

        public DeleteCategoryTest(DeleteCategoryTestFixture fixture) => this._fixture = fixture;

        [Fact(DisplayName = nameof(DeleteCategory))]
        [Trait("Integration/Application", "DeleteCategory - Use Cases")]
        public async Task DeleteCategory()
        {
            var dbContext = this._fixture.CreateDbContext();
            var categoryExample = _fixture.GetExampleCategory();
            var exampleCategoriesList = this._fixture.GetExampleCategoryList(10);
            await dbContext.AddRangeAsync(exampleCategoriesList);
            var tracking = await dbContext.AddAsync(categoryExample);
            await dbContext.SaveChangesAsync();
            tracking.State = EntityState.Detached;
            var repository = new CategoryRepository(dbContext);
            var unitOfWork = new UnitOfWork(dbContext);
            var useCase = new ApplicationUseCase.DeleteCategory(repository, unitOfWork);

            var input = new DeleteCategoryInput(categoryExample.Id);
            await useCase.Handle(input, CancellationToken.None);

            var assertDbContext = this._fixture.CreateDbContext(true);
            var dbCategoryDeleted = await assertDbContext.Categories.FindAsync(categoryExample.Id);
            var dbCategories = await assertDbContext.Categories.ToListAsync();

            dbCategoryDeleted.Should().BeNull();
            dbCategories.Should().HaveCount(exampleCategoriesList.Count);
        }

        [Fact(DisplayName = nameof(DeleteCategoryThrowWhenCategoryNotFound))]
        [Trait("Integration/Application", "DeleteCategory - Use Cases")]
        public async Task DeleteCategoryThrowWhenCategoryNotFound()
        {
            var dbContext = this._fixture.CreateDbContext();
            var exampleCategoriesList = this._fixture.GetExampleCategoryList(10);
            await dbContext.AddRangeAsync(exampleCategoriesList);
            await dbContext.SaveChangesAsync();
            var repository = new CategoryRepository(dbContext);
            var unitOfWork = new UnitOfWork(dbContext);
            var useCase = new ApplicationUseCase.DeleteCategory(repository, unitOfWork);
            var input = new DeleteCategoryInput(Guid.NewGuid());

            var task = async () => await useCase.Handle(input, CancellationToken.None);

            await task.Should().ThrowAsync<NotFoundException>()
                .WithMessage($"Category '{input.Id}' not found.");
        }
    }
}
