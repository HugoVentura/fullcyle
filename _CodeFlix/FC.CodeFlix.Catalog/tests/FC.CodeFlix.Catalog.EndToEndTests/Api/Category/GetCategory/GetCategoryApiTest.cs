using FC.CodeFlix.Catalog.Application.UseCases.Category.Common;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace FC.CodeFlix.Catalog.EndToEndTests.Api.Category.GetCategory
{
    [Collection(nameof(GetCategoryApiTestFixture))]
    public class GetCategoryApiTest : IDisposable
    {
        private GetCategoryApiTestFixture _fixture;

        public GetCategoryApiTest(GetCategoryApiTestFixture fixture) => this._fixture = fixture;

        [Fact(DisplayName = nameof(GetCategory))]
        [Trait("EndToEnd/API", "Category/Get - Endpoints")]
        public async Task GetCategory()
        {
            var exampleCategoriesList = this._fixture.GetExampleCategoriesList(20);
            await this._fixture.Persistence.InsertList(exampleCategoriesList);
            var exampleCategory = exampleCategoriesList[10];

            var (response, output) = await this._fixture.ApiClient.Get<CategoryModelOutput>($"/categories/{exampleCategory.Id}");

            response.Should().NotBeNull();
            response!.StatusCode.Should().Be(HttpStatusCode.OK);
            output.Should().NotBeNull();
            output!.Id.Should().Be(exampleCategory.Id);
            output.Name.Should().Be(exampleCategory.Name);
            output.Description.Should().Be(exampleCategory.Description);
            output.IsActive.Should().Be(exampleCategory.IsActive);
            output.CreatedAt.Should().Be(exampleCategory.CreatedAt);
        }

        [Fact(DisplayName = nameof(ErrorWhenNotFound))]
        [Trait("EndToEnd/API", "Category/Get - Endpoints")]
        public async Task ErrorWhenNotFound()
        {
            var exampleCategoriesList = this._fixture.GetExampleCategoriesList(20);
            await this._fixture.Persistence.InsertList(exampleCategoriesList);
            var randomGuid = Guid.NewGuid();

            var (response, output) = await this._fixture.ApiClient.Get<ProblemDetails>($"/categories/{randomGuid}");

            response.Should().NotBeNull();
            response!.StatusCode.Should().Be(HttpStatusCode.NotFound);
            output.Should().NotBeNull();
            output!.Status.Should().Be(StatusCodes.Status404NotFound);
            output.Title.Should().Be("Not Found");
            output.Detail.Should().Be($"Category '{randomGuid}' not found.");
            output.Type.Should().Be("NotFound");
        }

        public void Dispose() => this._fixture.CleanPersistence();
    }
}
