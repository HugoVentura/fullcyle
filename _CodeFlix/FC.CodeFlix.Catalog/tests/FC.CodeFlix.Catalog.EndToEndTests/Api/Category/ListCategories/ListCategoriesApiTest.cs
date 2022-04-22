using FC.CodeFlix.Catalog.Application.UseCases.Category.Common;
using FC.CodeFlix.Catalog.Application.UseCases.Category.ListCategories;
using FC.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace FC.CodeFlix.Catalog.EndToEndTests.Api.Category.ListCategories
{
    [Collection(nameof(ListCategoriesApiTestFixture))]
    public class ListCategoriesApiTest : IDisposable
    {
        private readonly ListCategoriesApiTestFixture _fixture;
        private readonly string _route = "/categories";

        public ListCategoriesApiTest(ListCategoriesApiTestFixture fixture) => this._fixture = fixture;

        [Fact(DisplayName = nameof(ListCategoriesAndTotalByDefault))]
        [Trait("EndToEnd/API", "Category/List - Endpoints")]
        public async Task ListCategoriesAndTotalByDefault()
        {
            var defaultPerPage = 15;
            var exampleCategoriesList = this._fixture.GetExampleCategoriesList(20);
            await this._fixture.Persistence.InsertList(exampleCategoriesList);

            var (response, output) = await this._fixture.ApiClient.Get<ListCategoriesOutput>(this._route);

            response.Should().NotBeNull();
            response!.StatusCode.Should().Be(HttpStatusCode.OK);
            output.Should().NotBeNull();
            output!.Total.Should().Be(exampleCategoriesList.Count);
            output.Page.Should().Be(1);
            output.PerPage.Should().Be(defaultPerPage);
            output.Items.Should().HaveCount(defaultPerPage);
            foreach (CategoryModelOutput outputItem in output.Items)
            {
                var exampleItem = exampleCategoriesList.FirstOrDefault(p => p.Id.Equals(outputItem.Id));

                exampleItem.Should().NotBeNull();
                outputItem!.Name.Should().Be(exampleItem!.Name);
                outputItem.Description.Should().Be(exampleItem.Description);
                outputItem.IsActive.Should().Be(exampleItem.IsActive);
                outputItem.CreatedAt.Should().Be(exampleItem.CreatedAt);
            }
        }

        [Fact(DisplayName = nameof(ItemsEmptyWhenPersistenceEmpty))]
        [Trait("EndToEnd/API", "Category/List - Endpoints")]
        public async Task ItemsEmptyWhenPersistenceEmpty()
        {
            var (response, output) = await this._fixture.ApiClient.Get<ListCategoriesOutput>(this._route);

            response.Should().NotBeNull();
            response!.StatusCode.Should().Be(HttpStatusCode.OK);
            output.Should().NotBeNull();
            output!.Total.Should().Be(0);
            output.Items.Should().BeEmpty();
        }

        [Fact(DisplayName = nameof(ListCategoriesAndTotal))]
        [Trait("EndToEnd/API", "Category/List - Endpoints")]
        public async Task ListCategoriesAndTotal()
        {
            var exampleCategoriesList = this._fixture.GetExampleCategoriesList(20);
            await this._fixture.Persistence.InsertList(exampleCategoriesList);
            var input = new ListCategoriesInput(1, 5);

            var (response, output) = await this._fixture.ApiClient.Get<ListCategoriesOutput>(this._route, input);

            response.Should().NotBeNull();
            response!.StatusCode.Should().Be(HttpStatusCode.OK);
            output.Should().NotBeNull();
            output!.Total.Should().Be(exampleCategoriesList.Count);
            output.Page.Should().Be(input.Page);
            output.PerPage.Should().Be(input.PerPage);
            output.Items.Should().HaveCount(input.PerPage);
            foreach (CategoryModelOutput outputItem in output.Items)
            {
                var exampleItem = exampleCategoriesList.FirstOrDefault(p => p.Id.Equals(outputItem.Id));

                exampleItem.Should().NotBeNull();
                outputItem!.Name.Should().Be(exampleItem!.Name);
                outputItem.Description.Should().Be(exampleItem.Description);
                outputItem.IsActive.Should().Be(exampleItem.IsActive);
                outputItem.CreatedAt.Should().Be(exampleItem.CreatedAt);
            }
        }

        [Theory(DisplayName = nameof(SearchPaginated))]
        [Trait("EndToEnd/API", "Category/List - Endpoints")]
        [InlineData(10, 1, 5, 5)]
        [InlineData(10, 2, 5, 5)]
        [InlineData(7, 2, 5, 2)]
        [InlineData(7, 3, 5, 0)]
        public async Task SearchPaginated(int quantityCategoriesToGenerate, int page, int perPage, int expectedQuantityItems)
        {
            var exampleCategoriesList = this._fixture.GetExampleCategoriesList(quantityCategoriesToGenerate);
            await this._fixture.Persistence.InsertList(exampleCategoriesList);
            var input = new ListCategoriesInput(page, perPage);

            var (response, output) = await this._fixture.ApiClient.Get<ListCategoriesOutput>(this._route, input);

            response.Should().NotBeNull();
            response!.StatusCode.Should().Be(HttpStatusCode.OK);
            output.Should().NotBeNull();
            output!.Total.Should().Be(exampleCategoriesList.Count);
            output.Page.Should().Be(input.Page);
            output.PerPage.Should().Be(input.PerPage);
            output.Items.Should().HaveCount(expectedQuantityItems);
            foreach (CategoryModelOutput outputItem in output.Items)
            {
                var exampleItem = exampleCategoriesList.FirstOrDefault(p => p.Id.Equals(outputItem.Id));

                exampleItem.Should().NotBeNull();
                outputItem!.Name.Should().Be(exampleItem!.Name);
                outputItem.Description.Should().Be(exampleItem.Description);
                outputItem.IsActive.Should().Be(exampleItem.IsActive);
                outputItem.CreatedAt.Should().Be(exampleItem.CreatedAt);
            }
        }

        [Theory(DisplayName = nameof(SearchByText))]
        [Trait("EndToEnd/API", "Category/List - Endpoints")]
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

            var exampleCategoriesList = this._fixture.GetExampleCategoryListWithNames(categoryNamesList);
            await this._fixture.Persistence.InsertList(exampleCategoriesList);
            var input = new ListCategoriesInput(page, perPage, search);

            var (response, output) = await this._fixture.ApiClient.Get<ListCategoriesOutput>(this._route, input);

            response.Should().NotBeNull();
            response!.StatusCode.Should().Be(HttpStatusCode.OK);
            output.Should().NotBeNull();
            output!.Total.Should().Be(expectedQuantityTotalItems);
            output.Page.Should().Be(input.Page);
            output.PerPage.Should().Be(input.PerPage);
            output.Items.Should().HaveCount(expectedQuantityItemsReturned);
            foreach (CategoryModelOutput outputItem in output.Items)
            {
                var exampleItem = exampleCategoriesList.FirstOrDefault(p => p.Id.Equals(outputItem.Id));

                exampleItem.Should().NotBeNull();
                outputItem!.Name.Should().Be(exampleItem!.Name);
                outputItem.Description.Should().Be(exampleItem.Description);
                outputItem.IsActive.Should().Be(exampleItem.IsActive);
                outputItem.CreatedAt.Should().Be(exampleItem.CreatedAt);
            }
        }

        [Theory(DisplayName = nameof(SearchOrdered))]
        [Trait("EndToEnd/API", "Category/List - Endpoints")]
        [InlineData("name", SearchOrder.Desc)]
        [InlineData("name", SearchOrder.Asc)]
        [InlineData("id", SearchOrder.Desc)]
        [InlineData("id", SearchOrder.Asc)]
        [InlineData("createdat", SearchOrder.Desc)]
        [InlineData("createdat", SearchOrder.Asc)]
        [InlineData("", SearchOrder.Asc)]
        public async Task SearchOrdered(string orderBy, SearchOrder searchOrder)
        {
            var exampleCategoriesList = this._fixture.GetExampleCategoriesList(10);
            await this._fixture.Persistence.InsertList(exampleCategoriesList);
            var input = new ListCategoriesInput(1, 20, "", orderBy, searchOrder);

            var (response, output) = await this._fixture.ApiClient.Get<ListCategoriesOutput>(this._route, input);
            var expectedOrderedList = this._fixture.CloneCategoryListOrdered(exampleCategoriesList, input.Sort, input.Dir);

            response.Should().NotBeNull();
            response!.StatusCode.Should().Be(HttpStatusCode.OK);
            output.Should().NotBeNull();
            output!.Page.Should().Be(input.Page);
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

        public void Dispose() => this._fixture.CleanPersistence();
    }
}
