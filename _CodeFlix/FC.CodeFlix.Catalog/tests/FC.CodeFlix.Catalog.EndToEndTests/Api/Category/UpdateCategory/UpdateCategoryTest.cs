using FC.CodeFlix.Catalog.Application.UseCases.Category.Common;
using FC.CodeFlix.Catalog.Application.UseCases.Category.UpdateCategory;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace FC.CodeFlix.Catalog.EndToEndTests.Api.Category.UpdateCategory
{
    [Collection(nameof(UpdateCategoryTestFixture))]
    public class UpdateCategoryTest : IDisposable
    {
        private readonly UpdateCategoryTestFixture _fixture;
        private readonly string _route = "/categories";

        public UpdateCategoryTest(UpdateCategoryTestFixture fixture) => this._fixture = fixture;

        [Fact(DisplayName = nameof(UpdateCategory))]
        [Trait("EndToEnd/API", "Category/Update - Endpoints")]
        public async Task UpdateCategory()
        {
            var exampleCategoriesList = this._fixture.GetExampleCategoriesList(20);
            await this._fixture.Persistence.InsertList(exampleCategoriesList);
            var exampleCategory = exampleCategoriesList[10];
            var updateInput = this._fixture.GetExampleInput(exampleCategory.Id);

            var (response, output) = await this._fixture.ApiClient.Put<CategoryModelOutput>(this._route, updateInput);
            var dbCategory = await this._fixture.Persistence.GetById(exampleCategory.Id);

            response.Should().NotBeNull();
            response!.StatusCode.Should().Be(HttpStatusCode.OK);
            output.Should().NotBeNull();
            output!.Id.Should().Be(exampleCategory.Id);
            output.Name.Should().Be(updateInput.Name);
            output.Description.Should().Be(updateInput.Description);
            output.IsActive.Should().Be((bool)updateInput.IsActive!);
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
            var updateInput = new UpdateCategoryInput(exampleCategory.Id, this._fixture.GetValidCategoryName());

            var (response, output) = await this._fixture.ApiClient.Put<CategoryModelOutput>(this._route, updateInput);
            var dbCategory = await this._fixture.Persistence.GetById(exampleCategory.Id);

            response.Should().NotBeNull();
            response!.StatusCode.Should().Be(HttpStatusCode.OK);
            output.Should().NotBeNull();
            output!.Id.Should().Be(exampleCategory.Id);
            output.Name.Should().Be(updateInput.Name);
            output.Description.Should().Be(exampleCategory.Description);
            output.IsActive.Should().Be((bool)exampleCategory.IsActive!);
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
            var updateInput = new UpdateCategoryInput(exampleCategory.Id, this._fixture.GetValidCategoryName(), this._fixture.GetValidCategoryDescription());

            var (response, output) = await this._fixture.ApiClient.Put<CategoryModelOutput>(this._route, updateInput);
            var dbCategory = await this._fixture.Persistence.GetById(exampleCategory.Id);

            response.Should().NotBeNull();
            response!.StatusCode.Should().Be(HttpStatusCode.OK);
            output.Should().NotBeNull();
            output!.Id.Should().Be(exampleCategory.Id);
            output.Name.Should().Be(updateInput.Name);
            output.Description.Should().Be(updateInput.Description);
            output.IsActive.Should().Be((bool)exampleCategory.IsActive!);
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
            var updateInput = this._fixture.GetExampleInput(Guid.NewGuid());

            var (response, output) = await this._fixture.ApiClient.Put<ProblemDetails>(this._route, updateInput);

            response.Should().NotBeNull();
            response!.StatusCode.Should().Be(HttpStatusCode.NotFound);
            output.Should().NotBeNull();
            output!.Title.Should().Be("Not Found");
            output.Type.Should().Be("NotFound");
            output.Status.Should().Be(StatusCodes.Status404NotFound);
            output.Detail.Should().Be($"Category '{updateInput.Id}' not found.");
        }

        [Theory(DisplayName = nameof(ErrorWhenCantInstantiateAggregate))]
        [Trait("EndToEnd/API", "Category/Update - Endpoints")]
        [MemberData(nameof(UpdateCategoryApiTestDataGenerator.GetInvalidInputs), MemberType = typeof(UpdateCategoryApiTestDataGenerator))]
        public async Task ErrorWhenCantInstantiateAggregate(UpdateCategoryInput input, string expectedDetail)
        {
            var exampleCategoriesList = this._fixture.GetExampleCategoriesList(20);
            await this._fixture.Persistence.InsertList(exampleCategoriesList);
            var exampleCategory = exampleCategoriesList[10];
            input.Id = exampleCategory.Id;

            var (response, output) = await this._fixture.ApiClient.Put<ProblemDetails>(this._route, input);

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
