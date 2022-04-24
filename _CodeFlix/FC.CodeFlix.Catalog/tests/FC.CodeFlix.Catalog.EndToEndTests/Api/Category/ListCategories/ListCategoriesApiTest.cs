using FC.CodeFlix.Catalog.Application.UseCases.Category.Common;
using FC.CodeFlix.Catalog.Application.UseCases.Category.ListCategories;
using FC.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.CodeFlix.Catalog.EndToEndTests.Extensions.DateTime;
using FC.CodeFlix.Catalog.EndToEndTests.Models;
using FluentAssertions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace FC.CodeFlix.Catalog.EndToEndTests.Api.Category.ListCategories
{
    [Collection(nameof(ListCategoriesApiTestFixture))]
    public class ListCategoriesApiTest : IDisposable
    {
        private readonly ListCategoriesApiTestFixture _fixture;
        private readonly string _route = "/categories";
        private readonly ITestOutputHelper _output;

        public ListCategoriesApiTest(ListCategoriesApiTestFixture fixture, ITestOutputHelper output) => (this._fixture, this._output) = (fixture, output);

        [Fact(DisplayName = nameof(ListCategoriesAndTotalByDefault))]
        [Trait("EndToEnd/API", "Category/List - Endpoints")]
        public async Task ListCategoriesAndTotalByDefault()
        {
            var defaultPerPage = 15;
            var exampleCategoriesList = this._fixture.GetExampleCategoriesList(20);
            await this._fixture.Persistence.InsertList(exampleCategoriesList);

            var (response, output) = await this._fixture.ApiClient.Get<TestApiResponseList<CategoryModelOutput>>(this._route);

            response.Should().NotBeNull();
            response!.StatusCode.Should().Be(HttpStatusCode.OK);
            output.Should().NotBeNull();
            output!.Data.Should().NotBeNull();
            output.Meta.Should().NotBeNull();
            output.Meta!.Total.Should().Be(exampleCategoriesList.Count);
            output.Meta.CurrentPage.Should().Be(1);
            output.Meta.PerPage.Should().Be(defaultPerPage);
            output.Data.Should().HaveCount(defaultPerPage);
            foreach (CategoryModelOutput outputItem in output.Data!)
            {
                var exampleItem = exampleCategoriesList.FirstOrDefault(p => p.Id.Equals(outputItem.Id));

                exampleItem.Should().NotBeNull();
                outputItem!.Name.Should().Be(exampleItem!.Name);
                outputItem.Description.Should().Be(exampleItem.Description);
                outputItem.IsActive.Should().Be(exampleItem.IsActive);
                outputItem.CreatedAt.TrimMilliSeconds().Should().Be(exampleItem.CreatedAt.TrimMilliSeconds());
            }
        }

        [Fact(DisplayName = nameof(ItemsEmptyWhenPersistenceEmpty))]
        [Trait("EndToEnd/API", "Category/List - Endpoints")]
        public async Task ItemsEmptyWhenPersistenceEmpty()
        {
            var (response, output) = await this._fixture.ApiClient.Get<TestApiResponseList<CategoryModelOutput>>(this._route);

            response.Should().NotBeNull();
            response!.StatusCode.Should().Be(HttpStatusCode.OK);
            output.Should().NotBeNull();
            output!.Meta.Should().NotBeNull();
            output.Data.Should().NotBeNull();
            output.Meta!.Total.Should().Be(0);
            output.Data.Should().BeEmpty();
        }

        [Fact(DisplayName = nameof(ListCategoriesAndTotal))]
        [Trait("EndToEnd/API", "Category/List - Endpoints")]
        public async Task ListCategoriesAndTotal()
        {
            var exampleCategoriesList = this._fixture.GetExampleCategoriesList(20);
            await this._fixture.Persistence.InsertList(exampleCategoriesList);
            var input = new ListCategoriesInput(1, 5);

            var (response, output) = await this._fixture.ApiClient.Get<TestApiResponseList<CategoryModelOutput>>(this._route, input);

            response.Should().NotBeNull();
            response!.StatusCode.Should().Be(HttpStatusCode.OK);
            output.Should().NotBeNull();
            output!.Meta.Should().NotBeNull();
            output.Data.Should().NotBeNull();
            output.Meta!.Total.Should().Be(exampleCategoriesList.Count);
            output.Meta.CurrentPage.Should().Be(input.Page);
            output.Meta.PerPage.Should().Be(input.PerPage);
            output.Data.Should().HaveCount(input.PerPage);
            foreach (CategoryModelOutput outputItem in output.Data!)
            {
                var exampleItem = exampleCategoriesList.FirstOrDefault(p => p.Id.Equals(outputItem.Id));

                exampleItem.Should().NotBeNull();
                outputItem!.Name.Should().Be(exampleItem!.Name);
                outputItem.Description.Should().Be(exampleItem.Description);
                outputItem.IsActive.Should().Be(exampleItem.IsActive);
                outputItem.CreatedAt.TrimMilliSeconds().Should().Be(exampleItem.CreatedAt.TrimMilliSeconds());
            }
        }

        [Theory(DisplayName = nameof(ListPaginated))]
        [Trait("EndToEnd/API", "Category/List - Endpoints")]
        [InlineData(10, 1, 5, 5)]
        [InlineData(10, 2, 5, 5)]
        [InlineData(7, 2, 5, 2)]
        [InlineData(7, 3, 5, 0)]
        public async Task ListPaginated(int quantityCategoriesToGenerate, int page, int perPage, int expectedQuantityItems)
        {
            var exampleCategoriesList = this._fixture.GetExampleCategoriesList(quantityCategoriesToGenerate);
            await this._fixture.Persistence.InsertList(exampleCategoriesList);
            var input = new ListCategoriesInput(page, perPage);

            var (response, output) = await this._fixture.ApiClient.Get<TestApiResponseList<CategoryModelOutput>>(this._route, input);

            response.Should().NotBeNull();
            response!.StatusCode.Should().Be(HttpStatusCode.OK);
            output.Should().NotBeNull();
            output!.Meta.Should().NotBeNull();
            output.Data.Should().NotBeNull();
            output.Meta!.Total.Should().Be(exampleCategoriesList.Count);
            output.Meta.CurrentPage.Should().Be(input.Page);
            output.Meta.PerPage.Should().Be(input.PerPage);
            output.Data.Should().HaveCount(expectedQuantityItems);
            foreach (CategoryModelOutput outputItem in output.Data!)
            {
                var exampleItem = exampleCategoriesList.FirstOrDefault(p => p.Id.Equals(outputItem.Id));

                exampleItem.Should().NotBeNull();
                outputItem!.Name.Should().Be(exampleItem!.Name);
                outputItem.Description.Should().Be(exampleItem.Description);
                outputItem.IsActive.Should().Be(exampleItem.IsActive);
                outputItem.CreatedAt.TrimMilliSeconds().Should().Be(exampleItem.CreatedAt.TrimMilliSeconds());
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

            var (response, output) = await this._fixture.ApiClient.Get<TestApiResponseList<CategoryModelOutput>>(this._route, input);

            response.Should().NotBeNull();
            response!.StatusCode.Should().Be(HttpStatusCode.OK);
            output.Should().NotBeNull();
            output!.Meta.Should().NotBeNull();
            output.Data.Should().NotBeNull();
            output.Meta!.Total.Should().Be(expectedQuantityTotalItems);
            output.Meta.CurrentPage.Should().Be(input.Page);
            output.Meta.PerPage.Should().Be(input.PerPage);
            output.Data.Should().HaveCount(expectedQuantityItemsReturned);
            foreach (CategoryModelOutput outputItem in output.Data!)
            {
                var exampleItem = exampleCategoriesList.FirstOrDefault(p => p.Id.Equals(outputItem.Id));

                exampleItem.Should().NotBeNull();
                outputItem!.Name.Should().Be(exampleItem!.Name);
                outputItem.Description.Should().Be(exampleItem.Description);
                outputItem.IsActive.Should().Be(exampleItem.IsActive);
                outputItem.CreatedAt.TrimMilliSeconds().Should().Be(exampleItem.CreatedAt.TrimMilliSeconds());
            }
        }

        [Theory(DisplayName = nameof(ListOrdered))]
        [Trait("EndToEnd/API", "Category/List - Endpoints")]
        [InlineData("name", SearchOrder.Desc)]
        [InlineData("name", SearchOrder.Asc)]
        [InlineData("id", SearchOrder.Desc)]
        [InlineData("id", SearchOrder.Asc)]
        [InlineData("", SearchOrder.Asc)]
        public async Task ListOrdered(string orderBy, SearchOrder searchOrder)
        {
            var exampleCategoriesList = this._fixture.GetExampleCategoriesList(10);
            await this._fixture.Persistence.InsertList(exampleCategoriesList);
            var input = new ListCategoriesInput(1, 20, "", orderBy, searchOrder);

            var (response, output) = await this._fixture.ApiClient.Get<TestApiResponseList<CategoryModelOutput>>(this._route, input);
            var expectedOrderedList = this._fixture.CloneCategoryListOrdered(exampleCategoriesList, input.Sort, input.Dir);

            response.Should().NotBeNull();
            response!.StatusCode.Should().Be(HttpStatusCode.OK);
            output.Should().NotBeNull();
            output!.Meta.Should().NotBeNull();
            output.Data.Should().NotBeNull();
            output.Meta!.CurrentPage.Should().Be(input.Page);
            output.Meta.PerPage.Should().Be(input.PerPage);
            output.Meta.Total.Should().Be(exampleCategoriesList.Count);
            output.Data.Should().HaveCount(exampleCategoriesList.Count);
            for (var idx = 0; idx < expectedOrderedList.Count; idx++)
            {
                var expectedItem = expectedOrderedList[idx];
                var outputItem = output.Data![idx];

                expectedItem.Should().NotBeNull();
                outputItem.Should().NotBeNull();
                outputItem.Id.Should().Be(expectedItem.Id);
                outputItem.Name.Should().Be(expectedItem.Name);
                outputItem.Description.Should().Be(expectedItem.Description);
                outputItem.IsActive.Should().Be(expectedItem.IsActive);
                outputItem.CreatedAt.TrimMilliSeconds().Should().Be(expectedItem.CreatedAt.TrimMilliSeconds());
            }

            var count = 0;
            var expectedArray = expectedOrderedList.Select(p => $"{++count} {p.Name} {p.CreatedAt} {JsonConvert.SerializeObject(p)}");
            count = 0;
            var outputArray = output.Data!.Select(p => $"{++count} {p.Name} {p.CreatedAt} {JsonConvert.SerializeObject(p)}");
            
            this._output.WriteLine("Expecteds...");
            this._output.WriteLine(String.Join('\n', expectedArray));

            this._output.WriteLine("Outputs...");
            this._output.WriteLine(String.Join('\n', outputArray));
        }

        [Theory(DisplayName = nameof(ListOrderedDates))]
        [Trait("EndToEnd/API", "Category/List - Endpoints")]
        [InlineData("createdat", SearchOrder.Desc)]
        [InlineData("createdat", SearchOrder.Asc)]
        public async Task ListOrderedDates(string orderBy, SearchOrder searchOrder)
        {
            var exampleCategoriesList = this._fixture.GetExampleCategoriesList(10);
            await this._fixture.Persistence.InsertList(exampleCategoriesList);
            var input = new ListCategoriesInput(1, 20, "", orderBy, searchOrder);

            var (response, output) = await this._fixture.ApiClient.Get<TestApiResponseList<CategoryModelOutput>>(this._route, input);

            response.Should().NotBeNull();
            response!.StatusCode.Should().Be(HttpStatusCode.OK);
            output.Should().NotBeNull();
            output!.Meta.Should().NotBeNull();
            output.Data.Should().NotBeNull();
            output.Meta!.CurrentPage.Should().Be(input.Page);
            output.Meta.PerPage.Should().Be(input.PerPage);
            output.Meta.Total.Should().Be(exampleCategoriesList.Count);
            output.Data.Should().HaveCount(exampleCategoriesList.Count);
            DateTime? lastItemDate = null;
            foreach (CategoryModelOutput outputItem in output.Data!)
            {
                var exampleItem = exampleCategoriesList.FirstOrDefault(p => p.Id.Equals(outputItem.Id));

                exampleItem.Should().NotBeNull();
                outputItem!.Name.Should().Be(exampleItem!.Name);
                outputItem.Description.Should().Be(exampleItem.Description);
                outputItem.IsActive.Should().Be(exampleItem.IsActive);
                outputItem.CreatedAt.TrimMilliSeconds().Should().Be(exampleItem.CreatedAt.TrimMilliSeconds());
                if (lastItemDate != null)
                {
                    if (searchOrder == SearchOrder.Asc)
                        Assert.True(outputItem.CreatedAt >= lastItemDate);
                    else
                        Assert.True(outputItem.CreatedAt <= lastItemDate);
                }
                lastItemDate =  outputItem.CreatedAt;
            }
        }

        public void Dispose() => this._fixture.CleanPersistence();
    }
}
