using FluentAssertions;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using UseCase = FC.CodeFlix.Catalog.Application.UseCases.Category.GetCategory;
using FC.CodeFlix.Catalog.Application.Exceptions;

namespace FC.CodeFlix.Catalog.UnitTests.Application.GetCategory
{
    [Collection(nameof(GetCategoryTestFixture))]
    public class GetCategoryTest
    {
        private readonly GetCategoryTestFixture _fixture;

        public GetCategoryTest(GetCategoryTestFixture fixture) => this._fixture = fixture;

        [Fact(DisplayName = nameof(GetCategory))]
        [Trait("Application", "GetCategory - Use Cases")]
        public async Task Test_GetCategory()
        {
            var repositoryMock = this._fixture.GetRepositoryMock();
            var exampleCategory = this._fixture.GetValidCategory();
            repositoryMock.Setup(x => x.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(exampleCategory);
            var input = new UseCase.GetCategoryInput(exampleCategory.Id);
            var useCase = new UseCase.GetCategory(repositoryMock.Object);

            var output = await useCase.Handle(input, CancellationToken.None);

            repositoryMock.Verify(x => x.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);

            output.Should().NotBeNull();
            output.Name.Should().Be(exampleCategory.Name);
            output.Description.Should().Be(exampleCategory.Description);
            output.IsActive.Should().Be(exampleCategory.IsActive);
            output.Id.Should().Be(exampleCategory.Id);
            output.CreatedAt.Should().Be(exampleCategory.CreatedAt);
        }

        [Fact(DisplayName = nameof(Test_GetCategory_NotFoundExceptionWhenCategoryDoesnExists))]
        [Trait("Application", "GetCategory - Use Cases")]
        public async Task Test_GetCategory_NotFoundExceptionWhenCategoryDoesnExists()
        {
            var repositoryMock = this._fixture.GetRepositoryMock();
            var exampleGuid = Guid.NewGuid();
            repositoryMock.Setup(x => x.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ThrowsAsync(new NotFoundException($"Category '{exampleGuid}' not found."));
            var input = new UseCase.GetCategoryInput(exampleGuid);
            var useCase = new UseCase.GetCategory(repositoryMock.Object);

            var task = async () => await useCase.Handle(input, CancellationToken.None);

            await task.Should().ThrowAsync<NotFoundException>();
            repositoryMock.Verify(x => x.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
