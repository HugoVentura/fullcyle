using FC.CodeFlix.Catalog.Application.Exceptions;
using FC.CodeFlix.Catalog.Infra.Data.EF.Repositories;
using FluentAssertions;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using UseCase = FC.CodeFlix.Catalog.Application.UseCases.Category.GetCategory;

namespace FC.CodeFlix.Catalog.IntegrationTests.Application.UseCases.Category.GetCategory
{
    [Collection(nameof(GetCategoryTestFixture))]
    public class GetCategoryTest
    {
        private readonly GetCategoryTestFixture _fixture;

        public GetCategoryTest(GetCategoryTestFixture fixture) => this._fixture = fixture;

        [Fact(DisplayName = nameof(GetCategory))]
        [Trait("Integration/Application", "GetCategory - Use Cases")]
        public async Task GetCategory()
        {
            var dbContex = this._fixture.CreateDbContext();
            var exampleCategory = _fixture.GetExampleCategory();
            dbContex.Categories.Add(exampleCategory);
            dbContex.SaveChanges();
            var repository = new CategoryRepository(dbContex);
            var input = new UseCase.GetCategoryInput(exampleCategory.Id);
            var useCase = new UseCase.GetCategory(repository);

            var output = await useCase.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Name.Should().Be(exampleCategory.Name);
            output.Description.Should().Be(exampleCategory.Description);
            output.IsActive.Should().Be(exampleCategory.IsActive);
            output.Id.Should().Be(exampleCategory.Id);
            output.CreatedAt.Should().Be(exampleCategory.CreatedAt);
        }

        [Fact(DisplayName = nameof(GetCategory_NotFoundExceptionWhenCategoryDoesnExists))]
        [Trait("Integration/Application", "GetCategory - Use Cases")]
        public async Task GetCategory_NotFoundExceptionWhenCategoryDoesnExists()
        {
            var dbContex = this._fixture.CreateDbContext();
            var exampleCategory = _fixture.GetExampleCategory();
            dbContex.Categories.Add(exampleCategory);
            dbContex.SaveChanges();
            var repository = new CategoryRepository(dbContex);
            var input = new UseCase.GetCategoryInput(Guid.NewGuid());
            var useCase = new UseCase.GetCategory(repository);

            var task = async () => await useCase.Handle(input, CancellationToken.None);

            await task.Should()
                .ThrowAsync<NotFoundException>()
                .WithMessage($"Category '{input.Id}' not found.");
        }
    }
}
