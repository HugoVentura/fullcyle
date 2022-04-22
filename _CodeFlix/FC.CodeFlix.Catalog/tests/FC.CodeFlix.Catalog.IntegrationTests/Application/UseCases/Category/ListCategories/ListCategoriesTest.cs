using FC.CodeFlix.Catalog.Application.UseCases.Category.Common;
using FC.CodeFlix.Catalog.Application.UseCases.Category.ListCategories;
using FC.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.CodeFlix.Catalog.Infra.Data.EF;
using FC.CodeFlix.Catalog.Infra.Data.EF.Repositories;
using FluentAssertions;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using ApplicationUseCases = FC.CodeFlix.Catalog.Application.UseCases.Category.ListCategories;

namespace FC.CodeFlix.Catalog.IntegrationTests.Application.UseCases.Category.ListCategories
{
    [Collection(nameof(ListCategoriesTestFixture))]
    public class ListCategoriesTest
    {
        private readonly ListCategoriesTestFixture _fixture;

        public ListCategoriesTest(ListCategoriesTestFixture fixture) => this._fixture = fixture;

        [Fact(DisplayName = nameof(SearchReturnsListAndTotal))]
        [Trait("Integration/Application", "ListCategories - Use Cases")]
        public async Task SearchReturnsListAndTotal()
        {
            CodeFlixCatalogDbContext dbContext = this._fixture.CreateDbContext();
            var exampleCategoriesList = this._fixture.GetExampleCategoryList(15);
            await dbContext.AddRangeAsync(exampleCategoriesList);
            await dbContext.SaveChangesAsync(CancellationToken.None);
            var categoryRepository = new CategoryRepository(dbContext);
            var input = new ListCategoriesInput(1, 20);
            var useCase = new ApplicationUseCases.ListCategories(categoryRepository);

            var output = await useCase.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Items.Should().NotBeNullOrEmpty();
            output.Page.Should().Be(input.Page);
            output.PerPage.Should().Be(input.PerPage);
            output.Total.Should().Be(exampleCategoriesList.Count);
            output.Items.Should().HaveCount(exampleCategoriesList.Count);
            foreach (CategoryModelOutput outputItem in output.Items)
            {
                var exampleItem = exampleCategoriesList.Find(p => p.Id.Equals(outputItem.Id));

                exampleItem.Should().NotBeNull();
                outputItem.Id.Should().Be(exampleItem!.Id);
                outputItem.Name.Should().Be(exampleItem.Name);
                outputItem.Description.Should().Be(exampleItem.Description);
                outputItem.IsActive.Should().Be(exampleItem.IsActive);
                outputItem.CreatedAt.Should().Be(exampleItem.CreatedAt);
            }
        }

        [Fact(DisplayName = nameof(SearchReturnsEmptyWhenEmpty))]
        [Trait("Integration/Application", "ListCategories - Use Cases")]
        public async Task SearchReturnsEmptyWhenEmpty()
        {
            CodeFlixCatalogDbContext dbContext = this._fixture.CreateDbContext();
            var categoryRepository = new CategoryRepository(dbContext);
            var input = new ListCategoriesInput(1, 20);
            var useCase = new ApplicationUseCases.ListCategories(categoryRepository);

            var output = await useCase.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Items.Should().NotBeNull();
            output.Page.Should().Be(input.Page);
            output.PerPage.Should().Be(input.PerPage);
            output.Total.Should().Be(0);
            output.Items.Should().BeEmpty();
        }

        [Theory(DisplayName = nameof(SearchReturnsPaginated))]
        [Trait("Integration/Application", "ListCategories - Use Cases")]
        [InlineData(10, 1, 5, 5)]
        [InlineData(10, 2, 5, 5)]
        [InlineData(7, 2, 5, 2)]
        [InlineData(7, 3, 5, 0)]
        public async Task SearchReturnsPaginated(int quantityCategoriesToGenerate, int page, int perPage, int expectedQuantityItems)
        {
            CodeFlixCatalogDbContext dbContext = this._fixture.CreateDbContext();
            var exampleCategoriesList = this._fixture.GetExampleCategoryList(quantityCategoriesToGenerate);
            await dbContext.AddRangeAsync(exampleCategoriesList);
            await dbContext.SaveChangesAsync(CancellationToken.None);
            var categoryRepository = new CategoryRepository(dbContext);
            var input = new ListCategoriesInput(page, perPage);
            var useCase = new ApplicationUseCases.ListCategories(categoryRepository);

            var output = await useCase.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Items.Should().NotBeNull();
            output.Page.Should().Be(input.Page);
            output.PerPage.Should().Be(input.PerPage);
            output.Total.Should().Be(exampleCategoriesList.Count);
            output.Items.Should().HaveCount(expectedQuantityItems);
            foreach (CategoryModelOutput outputItem in output.Items)
            {
                var exampleItem = exampleCategoriesList.Find(p => p.Id.Equals(outputItem.Id));

                exampleItem.Should().NotBeNull();
                outputItem.Id.Should().Be(exampleItem!.Id);
                outputItem.Name.Should().Be(exampleItem.Name);
                outputItem.Description.Should().Be(exampleItem.Description);
                outputItem.IsActive.Should().Be(exampleItem.IsActive);
                outputItem.CreatedAt.Should().Be(exampleItem.CreatedAt);
            }
        }

        [Theory(DisplayName = nameof(SearchByText))]
        [Trait("Integration/Application", "ListCategories - Use Cases")]
        [InlineData("Action", 1, 5, 1, 1)]
        [InlineData("Horror", 1, 5, 3, 3)]
        [InlineData("Horror", 2, 5, 0, 3)]
        [InlineData("Sci-fi", 1, 5, 4, 4)]
        [InlineData("Sci-fi", 1, 2, 2, 4)]
        [InlineData("Sci-fi", 2, 3, 1, 4)]
        [InlineData("Sci-fi Other", 1, 3, 0, 0)]
        [InlineData("Robots", 1, 5, 2, 2)]
        public async Task SearchByText(string search, int page, int perPage, int expectedQuantityItemsReturned, int expectedQuantityTotalItems)
        {
            var categoryNamesList = new List<string>()
                { "Action", "Horror", "Horror - Robots", "Horror - Based on Reals Facts", "Drama", "Sci-fi IA", "Sci-fi Space", "Sci-fi Robots", "Sci-fi Future" };

            CodeFlixCatalogDbContext dbContext = this._fixture.CreateDbContext();
            var exampleCategoriesList = this._fixture.GetExampleCategoryListWithNames(categoryNamesList);
            await dbContext.AddRangeAsync(exampleCategoriesList);
            await dbContext.SaveChangesAsync(CancellationToken.None);
            var categoryRepository = new CategoryRepository(dbContext);
            var input = new ListCategoriesInput(page, perPage, search);
            var useCase = new ApplicationUseCases.ListCategories(categoryRepository);

            var output = await useCase.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Items.Should().NotBeNull();
            output.Page.Should().Be(input.Page);
            output.PerPage.Should().Be(input.PerPage);
            output.Total.Should().Be(expectedQuantityTotalItems);
            output.Items.Should().HaveCount(expectedQuantityItemsReturned);
            foreach (CategoryModelOutput outputItem in output.Items)
            {
                var exampleItem = exampleCategoriesList.Find(p => p.Id.Equals(outputItem.Id));

                exampleItem.Should().NotBeNull();
                outputItem.Id.Should().Be(exampleItem!.Id);
                outputItem.Name.Should().Be(exampleItem.Name);
                outputItem.Description.Should().Be(exampleItem.Description);
                outputItem.IsActive.Should().Be(exampleItem.IsActive);
                outputItem.CreatedAt.Should().Be(exampleItem.CreatedAt);
            }
        }

        [Theory(DisplayName = nameof(SearchOrdered))]
        [Trait("Integration/Application", "ListCategories - Use Cases")]
        [InlineData("name", SearchOrder.Asc)]
        [InlineData("name", SearchOrder.Desc)]
        [InlineData("id", SearchOrder.Asc)]
        [InlineData("id", SearchOrder.Desc)]
        [InlineData("createdat", SearchOrder.Asc)]
        [InlineData("createdat", SearchOrder.Desc)]
        [InlineData("", SearchOrder.Asc)]
        public async Task SearchOrdered(string orderBy, SearchOrder searchOrder)
        {
            CodeFlixCatalogDbContext dbContext = this._fixture.CreateDbContext();
            var exampleCategoriesList = this._fixture.GetExampleCategoryList(15);
            await dbContext.AddRangeAsync(exampleCategoriesList);
            await dbContext.SaveChangesAsync(CancellationToken.None);
            var categoryRepository = new CategoryRepository(dbContext);
            var input = new ListCategoriesInput(1, 20, "", orderBy, searchOrder);
            var useCase = new ApplicationUseCases.ListCategories(categoryRepository);

            var output = await useCase.Handle(input, CancellationToken.None);
            var expectedOrderedList = this._fixture.CloneCategoryListOrdered(exampleCategoriesList, input.Sort, input.Dir);

            output.Should().NotBeNull();
            output.Items.Should().NotBeNull();
            output.Page.Should().Be(input.Page);
            output.PerPage.Should().Be(input.PerPage);
            output.Total.Should().Be(exampleCategoriesList.Count);
            output.Items.Should().HaveCount(exampleCategoriesList.Count);
            for (var idx = 0; idx < expectedOrderedList.Count; idx++)
            {
                var expectedItem = expectedOrderedList[idx];
                var outputItem = output.Items[idx];

                expectedItem.Should().NotBeNull();
                outputItem.Should().NotBeNull();
                outputItem.Id.Should().Be(expectedItem.Id);
                outputItem.Name.Should().Be(expectedItem.Name);
                outputItem.Description.Should().Be(expectedItem.Description);
                outputItem.IsActive.Should().Be(expectedItem.IsActive);
                outputItem.CreatedAt.Should().Be(expectedItem.CreatedAt);
            }
        }
    }
}
