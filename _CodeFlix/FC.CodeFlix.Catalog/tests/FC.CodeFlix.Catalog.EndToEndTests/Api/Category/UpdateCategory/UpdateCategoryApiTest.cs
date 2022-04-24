using FC.CodeFlix.Catalog.Api.ApiModels.Category;
using FC.CodeFlix.Catalog.Api.ApiModels.Response;
using FC.CodeFlix.Catalog.Application.UseCases.Category.Common;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace FC.CodeFlix.Catalog.EndToEndTests.Api.Category.UpdateCategory
{
    [Collection(nameof(UpdateCategoryApiTestFixture))]
    public class UpdateCategoryApiTest : IDisposable
    {
        private readonly UpdateCategoryApiTestFixture _fixture;
        private readonly string _route = "/categories";

        public UpdateCategoryApiTest(UpdateCategoryApiTestFixture fixture) => this._fixture = fixture;

        [Fact(DisplayName = nameof(UpdateCategory))]
        [Trait("EndToEnd/API", "Category/Update - Endpoints")]
        public async Task UpdateCategory()
        {
            var exampleCategoriesList = this._fixture.GetExampleCategoriesList(20);
            await this._fixture.Persistence.InsertList(exampleCategoriesList);
            var exampleCategory = exampleCategoriesList[10];
            var updateInput = this._fixture.GetExampleInput();

            var (response, output) = await this._fixture.ApiClient.Put<ApiResponse<CategoryModelOutput>>($"{this._route}/{exampleCategory.Id}", updateInput);
            var dbCategory = await this._fixture.Persistence.GetById(exampleCategory.Id);

            response.Should().NotBeNull();
            response!.StatusCode.Should().Be(HttpStatusCode.OK);
            output.Should().NotBeNull();
            output!.Data.Should().NotBeNull();
            output.Data.Id.Should().Be(exampleCategory.Id);
            output.Data.Name.Should().Be(updateInput.Name);
            output.Data.Description.Should().Be(updateInput.Description);
            output.Data.IsActive.Should().Be((bool)updateInput.IsActive!);
            dbCategory.Should().NotBeNull();
            dbCategory!.Name.Should().Be(updateInput.Name);
            dbCategory.Description.Should().Be(updateInput.Description);
            dbCategory.IsActive.Should().Be((bool)updateInput.IsActive);
        }

        [Fact(DisplayName = nameof(UpdateCategoryOnlyName))]
        [Trait("EndToEnd/API", "Category/Update - Endpoints")]
        public async Task UpdateCategoryOnlyName()
        {
            var exampleCategoriesList = this._fixture.GetExampleCategoriesList(20);
            await this._fixture.Persistence.InsertList(exampleCategoriesList);
            var exampleCategory = exampleCategoriesList[10];
            var updateInput = new UpdateCategoryApiInput(this._fixture.GetValidCategoryName());

            var (response, output) = await this._fixture.ApiClient.Put<ApiResponse<CategoryModelOutput>>($"{this._route}/{exampleCategory.Id}", updateInput);
            var dbCategory = await this._fixture.Persistence.GetById(exampleCategory.Id);

            response.Should().NotBeNull();
            response!.StatusCode.Should().Be(HttpStatusCode.OK);
            output.Should().NotBeNull();
            output!.Data.Should().NotBeNull();
            output.Data.Id.Should().Be(exampleCategory.Id);
            output.Data.Name.Should().Be(updateInput.Name);
            output.Data.Description.Should().Be(exampleCategory.Description);
            output.Data.IsActive.Should().Be((bool)exampleCategory.IsActive!);
            dbCategory.Should().NotBeNull();
            dbCategory!.Name.Should().Be(updateInput.Name);
            dbCategory.Description.Should().Be(exampleCategory.Description);
            dbCategory.IsActive.Should().Be((bool)exampleCategory.IsActive);
        }

        [Fact(DisplayName = nameof(UpdateCategoryNameAndDescription))]
        [Trait("EndToEnd/API", "Category/Update - Endpoints")]
        public async Task UpdateCategoryNameAndDescription()
        {
            var exampleCategoriesList = this._fixture.GetExampleCategoriesList(20);
            await this._fixture.Persistence.InsertList(exampleCategoriesList);
            var exampleCategory = exampleCategoriesList[10];
            var updateInput = new UpdateCategoryApiInput(this._fixture.GetValidCategoryName(), this._fixture.GetValidCategoryDescription());

            var (response, output) = await this._fixture.ApiClient.Put<ApiResponse<CategoryModelOutput>>($"{this._route}/{exampleCategory.Id}", updateInput);
            var dbCategory = await this._fixture.Persistence.GetById(exampleCategory.Id);

            response.Should().NotBeNull();
            response!.StatusCode.Should().Be(HttpStatusCode.OK);
            output.Should().NotBeNull();
            output!.Data.Should().NotBeNull();
            output.Data.Id.Should().Be(exampleCategory.Id);
            output.Data.Name.Should().Be(updateInput.Name);
            output.Data.Description.Should().Be(updateInput.Description);
            output.Data.IsActive.Should().Be((bool)exampleCategory.IsActive!);
            dbCategory.Should().NotBeNull();
            dbCategory!.Name.Should().Be(updateInput.Name);
            dbCategory.Description.Should().Be(updateInput.Description);
            dbCategory.IsActive.Should().Be((bool)exampleCategory.IsActive);
        }

        [Fact(DisplayName = nameof(ErrorWhenNotFound))]
        [Trait("EndToEnd/API", "Category/Update - Endpoints")]
        public async Task ErrorWhenNotFound()
        {
            var exampleCategoriesList = this._fixture.GetExampleCategoriesList(20);
            await this._fixture.Persistence.InsertList(exampleCategoriesList);
            var randomGuid = Guid.NewGuid();
            var updateInput = this._fixture.GetExampleInput();

            var (response, output) = await this._fixture.ApiClient.Put<ProblemDetails>($"{this._route}/{randomGuid}", updateInput);

            response.Should().NotBeNull();
            response!.StatusCode.Should().Be(HttpStatusCode.NotFound);
            output.Should().NotBeNull();
            output!.Title.Should().Be("Not Found");
            output.Type.Should().Be("NotFound");
            output.Status.Should().Be(StatusCodes.Status404NotFound);
            output.Detail.Should().Be($"Category '{randomGuid}' not found.");
        }

        [Theory(DisplayName = nameof(ErrorWhenCantInstantiateAggregate))]
        [Trait("EndToEnd/API", "Category/Update - Endpoints")]
        [MemberData(nameof(UpdateCategoryApiTestDataGenerator.GetInvalidInputs), MemberType = typeof(UpdateCategoryApiTestDataGenerator))]
        public async Task ErrorWhenCantInstantiateAggregate(UpdateCategoryApiInput updateInput, string expectedDetail)
        {
            var exampleCategoriesList = this._fixture.GetExampleCategoriesList(20);
            await this._fixture.Persistence.InsertList(exampleCategoriesList);
            var exampleCategory = exampleCategoriesList[10];

            var (response, output) = await this._fixture.ApiClient.Put<ProblemDetails>($"{this._route}/{exampleCategory.Id}", updateInput);

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
