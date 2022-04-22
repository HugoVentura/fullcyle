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

            var (response, output) = await this._fixture.ApiClient.Post<CategoryModelOutput>("/categories", input);
            var dbCategory = await this._fixture.Persistence.GetById(output!.Id);

            response.Should().NotBeNull();
            response!.StatusCode.Should().Be(HttpStatusCode.Created);

            output.Should().NotBeNull();
            output.Id.Should().NotBeEmpty();
            output.Name.Should().Be(input.Name);
            output.Description.Should().Be(input.Description);
            output.IsActive.Should().Be(input.IsActive);
            output.CreatedAt.Should().NotBeSameDateAs(default);

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
