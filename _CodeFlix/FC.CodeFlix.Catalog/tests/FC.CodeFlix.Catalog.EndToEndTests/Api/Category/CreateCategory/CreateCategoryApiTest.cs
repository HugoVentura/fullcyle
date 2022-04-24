using FC.CodeFlix.Catalog.Api.ApiModels.Response;
using FC.CodeFlix.Catalog.Application.UseCases.Category.Common;
using FC.CodeFlix.Catalog.Application.UseCases.Category.CreateCategory;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace FC.CodeFlix.Catalog.EndToEndTests.Api.Category.CreateCategory
{
    [Collection(nameof(CreateCategoryApiTestFixture))]
    public class CreateCategoryApiTest : IDisposable
    {
        private readonly CreateCategoryApiTestFixture _fixture;

        public CreateCategoryApiTest(CreateCategoryApiTestFixture fixture) => this._fixture = fixture;

        [Fact(DisplayName = nameof(CreateCategory))]
        [Trait("EndToEnd/API", "Category/Create - Endpoints")]
        public async Task CreateCategory()
        {
            var input = this._fixture.GetExampleInput();

            var (response, output) = await this._fixture.ApiClient.Post<ApiResponse<CategoryModelOutput>>("/categories", input);
            var dbCategory = await this._fixture.Persistence.GetById(output!.Data.Id);

            response.Should().NotBeNull();
            response!.StatusCode.Should().Be(HttpStatusCode.Created);

            output.Should().NotBeNull();
            output!.Data.Should().NotBeNull();
            output.Data.Id.Should().NotBeEmpty();
            output.Data.Name.Should().Be(input.Name);
            output.Data.Description.Should().Be(input.Description);
            output.Data.IsActive.Should().Be(input.IsActive);
            output.Data.CreatedAt.Should().NotBeSameDateAs(default);

            dbCategory.Should().NotBeNull();
            dbCategory!.Id.Should().NotBeEmpty();
            dbCategory.Name.Should().Be(input.Name);
            dbCategory.Description.Should().Be(input.Description);
            dbCategory.IsActive.Should().Be(input.IsActive);
            dbCategory.CreatedAt.Should().NotBeSameDateAs(default);
        }

        [Theory(DisplayName = nameof(ErrorWhenCantInstantiateAggregate))]
        [Trait("EndToEnd/API", "Category/Create - Endpoints")]
        [MemberData(nameof(CreateCategoryApiTestDataGenerator.GetInvalidInputs), MemberType = typeof(CreateCategoryApiTestDataGenerator))]
        public async Task ErrorWhenCantInstantiateAggregate(CreateCategoryInput input, string expectedDetail)
        {
            var (response, output) = await this._fixture.ApiClient.Post<ProblemDetails>("/categories", input);

            response.Should().NotBeNull();
            response!.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
            output.Should().NotBeNull();
            output!.Title.Should().Be("One or more validation errors ocurred");
            output.Type.Should().Be("UnprocessableEntity");
            output.Status.Should().Be(StatusCodes.Status422UnprocessableEntity);
            output.Detail.Should().Be(expectedDetail);
        }

        public void Dispose() => this._fixture.CleanPersistence();
    }
}
