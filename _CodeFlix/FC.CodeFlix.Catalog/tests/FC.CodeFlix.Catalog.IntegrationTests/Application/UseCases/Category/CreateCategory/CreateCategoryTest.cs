using FC.CodeFlix.Catalog.Application.UseCases.Category.CreateCategory;
using FC.CodeFlix.Catalog.Domain.Exceptions;
using FC.CodeFlix.Catalog.Infra.Data.EF;
using FC.CodeFlix.Catalog.Infra.Data.EF.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using ApplicationUseCases = FC.CodeFlix.Catalog.Application.UseCases.Category.CreateCategory;

namespace FC.CodeFlix.Catalog.IntegrationTests.Application.UseCases.Category.CreateCategory
{
    [Collection(nameof(CreateCategoryTestFixture))]
    public class CreateCategoryTest
    {
        private readonly CreateCategoryTestFixture _fixture;

        public CreateCategoryTest(CreateCategoryTestFixture fixture) => this._fixture = fixture;

        [Fact(DisplayName = nameof(CreateCategory))]
        [Trait("Integration/Application", "Category - Use Cases")]
        public async void CreateCategory()
        {
            var dbContext = this._fixture.CreateDbContext();
            var repository = new CategoryRepository(dbContext);
            var unitOfWork = new UnitOfWork(dbContext);
            var useCase = new ApplicationUseCases.CreateCategory(repository, unitOfWork);
            var input = this._fixture.GetInput();

            var output = await useCase.Handle(input, CancellationToken.None);
            var dbCategory = await this._fixture.CreateDbContext(true).Categories.FindAsync(output.Id);

            dbCategory.Should().NotBeNull();
            dbCategory!.Name.Should().Be(input.Name);
            dbCategory.Description.Should().Be(input.Description);
            dbCategory.IsActive.Should().Be(input.IsActive);
            dbCategory.CreatedAt.Should().Be(output.CreatedAt);
            output.Should().NotBeNull();
            output.Name.Should().Be(input.Name);
            output.Description.Should().Be(input.Description);
            output.IsActive.Should().Be(input.IsActive);
            output.Id.Should().NotBeEmpty();
            output.CreatedAt.Should().NotBeSameDateAs(default);
        }

        [Fact(DisplayName = nameof(CreateCategoryWithOnlyName))]
        [Trait("Integration/Application", "Category - Use Cases")]
        public async void CreateCategoryWithOnlyName()
        {
            var dbContext = this._fixture.CreateDbContext();
            var repository = new CategoryRepository(dbContext);
            var unitOfWork = new UnitOfWork(dbContext);
            var useCase = new ApplicationUseCases.CreateCategory(repository, unitOfWork);
            var input = new CreateCategoryInput(this._fixture.GetInput().Name);

            var output = await useCase.Handle(input, CancellationToken.None);
            var dbCategory = await this._fixture.CreateDbContext(true).Categories.FindAsync(output.Id);

            dbCategory.Should().NotBeNull();
            dbCategory!.Name.Should().Be(input.Name);
            dbCategory.Description.Should().Be(string.Empty);
            dbCategory.IsActive.Should().Be(true);
            dbCategory.CreatedAt.Should().Be(output.CreatedAt);
            output.Should().NotBeNull();
            output.Name.Should().Be(input.Name);
            output.Description.Should().Be(string.Empty);
            output.IsActive.Should().Be(true);
            output.Id.Should().NotBeEmpty();
            output.CreatedAt.Should().NotBeSameDateAs(default);
        }

        [Fact(DisplayName = nameof(CreateCategoryWithOnlyNameAndDescription))]
        [Trait("Integration/Application", "Category - Use Cases")]
        public async void CreateCategoryWithOnlyNameAndDescription()
        {
            var dbContext = this._fixture.CreateDbContext();
            var repository = new CategoryRepository(dbContext);
            var unitOfWork = new UnitOfWork(dbContext);
            var useCase = new ApplicationUseCases.CreateCategory(repository, unitOfWork);
            var exampleInput = this._fixture.GetInput();
            var input = new CreateCategoryInput(exampleInput.Name, exampleInput.Description);

            var output = await useCase.Handle(input, CancellationToken.None);
            var dbCategory = await this._fixture.CreateDbContext(true).Categories.FindAsync(output.Id);

            dbCategory.Should().NotBeNull();
            dbCategory!.Name.Should().Be(input.Name);
            dbCategory.Description.Should().Be(input.Description);
            dbCategory.IsActive.Should().Be(true);
            dbCategory.CreatedAt.Should().Be(output.CreatedAt);
            output.Should().NotBeNull();
            output.Name.Should().Be(input.Name);
            output.Description.Should().Be(input.Description);
            output.IsActive.Should().Be(true);
            output.Id.Should().NotBeEmpty();
            output.CreatedAt.Should().NotBeSameDateAs(default);
        }

        [Theory(DisplayName = nameof(CreateCategoryThrowWhenCantInstantiateCategory))]
        [Trait("Integration/Application", "Category - Use Cases")]
        [MemberData(
            nameof(CreateCategoryTestDataGenerator.GetInvalidInputs),
            parameters: 4,
            MemberType = typeof(CreateCategoryTestDataGenerator))]
        public async void CreateCategoryThrowWhenCantInstantiateCategory(CreateCategoryInput input, string exceptionMessage)
        {
            var dbContext = this._fixture.CreateDbContext();
            var repository = new CategoryRepository(dbContext);
            var unitOfWork = new UnitOfWork(dbContext);
            var useCase = new ApplicationUseCases.CreateCategory(repository, unitOfWork);

            var task = async () => await useCase.Handle(input, CancellationToken.None);

            await task.Should().ThrowAsync<EntityValidationException>().WithMessage(exceptionMessage);

            var dbCategoriesList = this._fixture.CreateDbContext(true).Categories.AsNoTracking().ToList();
            dbCategoriesList.Should().BeEmpty();
        }
    }
}
