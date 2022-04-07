using FC.CodeFlix.Catalog.Application.Exceptions;
using FC.CodeFlix.Catalog.Application.UseCases.Category.Common;
using FC.CodeFlix.Catalog.Application.UseCases.Category.UpdateCategory;
using FC.CodeFlix.Catalog.Domain.Entity;
using FC.CodeFlix.Catalog.Domain.Exceptions;
using FluentAssertions;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using UseCase = FC.CodeFlix.Catalog.Application.UseCases.Category.UpdateCategory;

namespace FC.CodeFlix.Catalog.UnitTests.Application.UpdateCategory
{
    [Collection(nameof(UpdateCategoryTestFixture))]
    public class UpdateCategoryTest
    {
        private readonly UpdateCategoryTestFixture _fixture;

        public UpdateCategoryTest(UpdateCategoryTestFixture fixture) => this._fixture = fixture;

        [Theory(DisplayName = nameof(Test_UpdateCategory))]
        [Trait("Application", "UpdateCategory - Use Cases")]
        [MemberData(nameof(UpdateCategoryTestDataGenerator.GetCategoriesToUpdate), parameters: 12, MemberType = typeof(UpdateCategoryTestDataGenerator))]
        public async Task Test_UpdateCategory(Category category, UpdateCategoryInput input)
        {
            var repositoryMock = this._fixture.GetRepositoryMock();
            var unitOfWorkMock = this._fixture.GetUnitOfWorkMock();
            repositoryMock.Setup(p =>
                p.Get(category.Id, It.IsAny<CancellationToken>())
            ).ReturnsAsync(category);
            var useCase = new UseCase.UpdateCategory(repositoryMock.Object, unitOfWorkMock.Object);

            CategoryModelOutput output = await useCase.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Name.Should().Be(input.Name);
            output.Description.Should().Be(input.Description);
            output.IsActive.Should().Be((bool)input.IsActive!);
            repositoryMock.Verify(p => p.Get(category.Id, It.IsAny<CancellationToken>()), Times.Once);
            repositoryMock.Verify(p => p.Update(category, It.IsAny<CancellationToken>()), Times.Once);
            unitOfWorkMock.Verify(p => p.Commit(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory(DisplayName = nameof(Test_UpdateCategory_WithoutIsActive))]
        [Trait("Application", "UpdateCategory - Use Cases")]
        [MemberData(nameof(UpdateCategoryTestDataGenerator.GetCategoriesToUpdate), parameters: 12, MemberType = typeof(UpdateCategoryTestDataGenerator))]
        public async Task Test_UpdateCategory_WithoutIsActive(Category category, UpdateCategoryInput exampleInput)
        {
            var input = new UpdateCategoryInput(exampleInput.Id, exampleInput.Name, exampleInput.Description);
            var repositoryMock = this._fixture.GetRepositoryMock();
            var unitOfWorkMock = this._fixture.GetUnitOfWorkMock();
            repositoryMock.Setup(p =>
                p.Get(category.Id, It.IsAny<CancellationToken>())
            ).ReturnsAsync(category);
            var useCase = new UseCase.UpdateCategory(repositoryMock.Object, unitOfWorkMock.Object);

            CategoryModelOutput output = await useCase.Handle(exampleInput, CancellationToken.None);

            output.Should().NotBeNull();
            output.Name.Should().Be(input.Name);
            output.Description.Should().Be(input.Description);
            output.IsActive.Should().Be(category.IsActive);
            repositoryMock.Verify(p => p.Get(category.Id, It.IsAny<CancellationToken>()), Times.Once);
            repositoryMock.Verify(p => p.Update(category, It.IsAny<CancellationToken>()), Times.Once);
            unitOfWorkMock.Verify(p => p.Commit(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory(DisplayName = nameof(Test_UpdateCategory_OnlyName))]
        [Trait("Application", "UpdateCategory - Use Cases")]
        [MemberData(nameof(UpdateCategoryTestDataGenerator.GetCategoriesToUpdate), parameters: 12, MemberType = typeof(UpdateCategoryTestDataGenerator))]
        public async Task Test_UpdateCategory_OnlyName(Category category, UpdateCategoryInput exampleInput)
        {
            var input = new UpdateCategoryInput(exampleInput.Id, exampleInput.Name);
            var repositoryMock = this._fixture.GetRepositoryMock();
            var unitOfWorkMock = this._fixture.GetUnitOfWorkMock();
            repositoryMock.Setup(p =>
                p.Get(category.Id, It.IsAny<CancellationToken>())
            ).ReturnsAsync(category);
            var useCase = new UseCase.UpdateCategory(repositoryMock.Object, unitOfWorkMock.Object);

            CategoryModelOutput output = await useCase.Handle(exampleInput, CancellationToken.None);

            output.Should().NotBeNull();
            output.Name.Should().Be(input.Name);
            output.Description.Should().Be(category.Description);
            output.IsActive.Should().Be(category.IsActive);
            repositoryMock.Verify(p => p.Get(category.Id, It.IsAny<CancellationToken>()), Times.Once);
            repositoryMock.Verify(p => p.Update(category, It.IsAny<CancellationToken>()), Times.Once);
            unitOfWorkMock.Verify(p => p.Commit(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = nameof(Test_UpdateCategory_Throw))]
        [Trait("Application", "UpdateCategory - Use Cases")]
        public async Task Test_UpdateCategory_Throw()
        {
            var repositoryMock = this._fixture.GetRepositoryMock();
            var unitOfWorkMock = this._fixture.GetUnitOfWorkMock();
            var exampleInput = this._fixture.GetValidInput();
            repositoryMock.Setup(p =>
                p.Get(exampleInput.Id, It.IsAny<CancellationToken>())
            ).ThrowsAsync(new NotFoundException($"category '{exampleInput.Id}' not found"));

            var useCase = new UseCase.UpdateCategory(repositoryMock.Object, unitOfWorkMock.Object);
            var task = async () => await useCase.Handle(exampleInput, CancellationToken.None);

            await task.Should().ThrowAsync<NotFoundException>();

            repositoryMock.Verify(p => p.Get( exampleInput.Id, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory(DisplayName = nameof(Test_UpdateCategory_ThrowWhenCantUpdateCategory))]
        [Trait("Application", "UpdateCategory - Use Cases")]
        [MemberData(nameof(UpdateCategoryTestDataGenerator.GetInvalidInputs), parameters: 12, MemberType = typeof(UpdateCategoryTestDataGenerator))]
        public async Task Test_UpdateCategory_ThrowWhenCantUpdateCategory(UpdateCategoryInput input, string expectedExceptionMessage)
        {
            var category = this._fixture.GetExampleCategory();
            input.Id = category.Id;
            var repositoryMock = this._fixture.GetRepositoryMock();
            var unitOfWorkMock = this._fixture.GetUnitOfWorkMock();
            repositoryMock.Setup(p =>
                p.Get(category.Id, It.IsAny<CancellationToken>())
            ).ReturnsAsync(category);
            var useCase = new UseCase.UpdateCategory(repositoryMock.Object, unitOfWorkMock.Object);

            var task = async () => await useCase.Handle(input, CancellationToken.None);

            await task.Should().ThrowAsync<EntityValidationException>().WithMessage(expectedExceptionMessage);

            repositoryMock.Verify(p => p.Get(category.Id, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
