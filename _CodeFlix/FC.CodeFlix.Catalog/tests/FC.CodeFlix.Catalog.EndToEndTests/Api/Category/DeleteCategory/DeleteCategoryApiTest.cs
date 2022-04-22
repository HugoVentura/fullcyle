using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace FC.CodeFlix.Catalog.EndToEndTests.Api.Category.DeleteCategory
{
    [Collection(nameof(DeleteCategoryApiTestFixture))]
    public class DeleteCategoryApiTest : IDisposable
    {
        private readonly DeleteCategoryApiTestFixture _fixture;

        public DeleteCategoryApiTest(DeleteCategoryApiTestFixture fixture) => this._fixture = fixture;

        [Fact(DisplayName = nameof(DeleteCategory))]
        [Trait("EndToEnd/API", "Category/Delete - Endpoints")]
        public async Task DeleteCategory()
        {
            var exampleCategoriesList = this._fixture.GetExampleCategoriesList(20);
            await this._fixture.Persistence.InsertList(exampleCategoriesList);
            var exampleCategory = exampleCategoriesList[10];

            var (response, output) = await this._fixture.ApiClient.Delete<object>($"/categories/{exampleCategory.Id}");
            var persistenceCategory = await this._fixture.Persistence.GetById(exampleCategory.Id);

            response.Should().NotBeNull();
            response!.StatusCode.Should().Be(HttpStatusCode.NoContent);
            output.Should().BeNull();
            persistenceCategory.Should().BeNull();
        }

        [Fact(DisplayName = nameof(ErrorEhnNotFound))]
        [Trait("EndToEnd/API", "Category/Delete - Endpoints")]
        public async Task ErrorEhnNotFound()
        {
            var exampleCategoriesList = this._fixture.GetExampleCategoriesList(20);
            await this._fixture.Persistence.InsertList(exampleCategoriesList);
            var randomGuid = Guid.NewGuid();

            var (response, output) = await this._fixture.ApiClient.Delete<ProblemDetails>($"/categories/{randomGuid}");

            response.Should().NotBeNull();
            response!.StatusCode.Should().Be(HttpStatusCode.NotFound);
            output.Should().NotBeNull();
            output!.Title.Should().Be("Not Found");
            output.Type.Should().Be("NotFound");
            output.Status.Should().Be(StatusCodes.Status404NotFound);
            output.Detail.Should().Be($"Category '{randomGuid}' not found.");
        }

        public void Dispose() => this._fixture.CleanPersistence();
    }
}
