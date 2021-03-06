using FC.CodeFlix.Catalog.Application.Exceptions;
using FC.CodeFlix.Catalog.Application.UseCases.Category.Common;
using FC.CodeFlix.Catalog.Application.UseCases.Category.UpdateCategory;
using FC.CodeFlix.Catalog.Domain.Exceptions;
using FluentAssertions;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using UseCase = FC.CodeFlix.Catalog.Application.UseCases.Category.UpdateCategory;
using DomainEntity = FC.CodeFlix.Catalog.Domain.Entity;

namespace FC.CodeFlix.Catalog.UnitTests.Application.Category.UpdateCategory
{
    [Collection(nameof(UpdateCategoryTestFixture))]
    public class UpdateCategoryTest
    {
        private readonly UpdateCategoryTestFixture _fixture;

        public UpdateCategoryTest(UpdateCategoryTestFixture fixture) => _fixture = fixture;

        [Theory(DisplayName = nameof(UpdateCategory))]
        [Trait("Application", "UpdateCategory - Use Cases")]
        [MemberData(nameof(UpdateCategoryTestDataGenerator.GetCategoriesToUpdate), parameters: 10, MemberType = typeof(UpdateCategoryTestDataGenerator))]
        public async Task UpdateCategory(DomainEntity.Category category, UpdateCategoryInput input)
        {
            var repositoryMock = _fixture.GetRepositoryMock();
            var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
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

        [Theory(DisplayName = nameof(UpdateCategory_WithoutIsActive))]
        [Trait("Application", "UpdateCategory - Use Cases")]
        [MemberData(nameof(UpdateCategoryTestDataGenerator.GetCategoriesToUpdate), parameters: 10, MemberType = typeof(UpdateCategoryTestDataGenerator))]
        public async Task UpdateCategory_WithoutIsActive(DomainEntity.Category category, UpdateCategoryInput exampleInput)
        {
            var input = new UpdateCategoryInput(exampleInput.Id, exampleInput.Name, exampleInput.Description);
            var repositoryMock = _fixture.GetRepositoryMock();
            var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
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

        [Theory(DisplayName = nameof(UpdateCategory_OnlyName))]
        [Trait("Application", "UpdateCategory - Use Cases")]
        [MemberData(nameof(UpdateCategoryTestDataGenerator.GetCategoriesToUpdate), parameters: 10, MemberType = typeof(UpdateCategoryTestDataGenerator))]
        public async Task UpdateCategory_OnlyName(DomainEntity.Category category, UpdateCategoryInput exampleInput)
        {
            var input = new UpdateCategoryInput(exampleInput.Id, exampleInput.Name);
            var repositoryMock = _fixture.GetRepositoryMock();
            var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
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

        [Fact(DisplayName = nameof(UpdateCategory_ThrowWhenCategoryNotFound))]
        [Trait("Application", "UpdateCategory - Use Cases")]
        public async Task UpdateCategory_ThrowWhenCategoryNotFound()
        {
            var repositoryMock = _fixture.GetRepositoryMock();
            var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
            var exampleInput = _fixture.GetValidInput();
            repositoryMock.Setup(p =>
                p.Get(exampleInput.Id, It.IsAny<CancellationToken>())
            ).ThrowsAsync(new NotFoundException($"category '{exampleInput.Id}' not found"));

            var useCase = new UseCase.UpdateCategory(repositoryMock.Object, unitOfWorkMock.Object);
            var task = async () => await useCase.Handle(exampleInput, CancellationToken.None);

            await task.Should().ThrowAsync<NotFoundException>();

            repositoryMock.Verify(p => p.Get(exampleInput.Id, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory(DisplayName = nameof(UpdateCategory_ThrowWhenCantUpdateCategory))]
        [Trait("Application", "UpdateCategory - Use Cases")]
        [MemberData(nameof(UpdateCategoryTestDataGenerator.GetInvalidInputs), parameters: 12, MemberType = typeof(UpdateCategoryTestDataGenerator))]
        public async Task UpdateCategory_ThrowWhenCantUpdateCategory(UpdateCategoryInput input, string expectedExceptionMessage)
        {
            var category = _fixture.GetExampleCategory();
            input.Id = category.Id;
            var repositoryMock = _fixture.GetRepositoryMock();
            var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
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
