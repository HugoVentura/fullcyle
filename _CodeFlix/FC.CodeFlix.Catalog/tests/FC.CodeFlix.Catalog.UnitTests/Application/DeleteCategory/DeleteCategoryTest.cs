using FC.CodeFlix.Catalog.Application.Exceptions;
using FC.CodeFlix.Catalog.Application.UseCases.Category.DeleteCategory;
using FluentAssertions;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using UseCase = FC.CodeFlix.Catalog.Application.UseCases.Category.DeleteCategory;

namespace FC.CodeFlix.Catalog.UnitTests.Application.DeleteCategory
{
    [Collection(nameof(DeletecategoryTestFixture))]
    public class DeleteCategoryTest
    {
        private readonly DeletecategoryTestFixture _fixture;

        public DeleteCategoryTest(DeletecategoryTestFixture fixture)
        {
            this._fixture = fixture;
        }

        [Fact(DisplayName = nameof(Test_DeleteCategory))]
        [Trait("Application", "Delete Category - Use Cases")]
        public async Task Test_DeleteCategory()
        {
            var repositoryMock = this._fixture.GetRepositoryMock();
            var unitOfWorkMock = this._fixture.GetUnitOfWorkMock();
            var categoryExample = this._fixture.GetValidCategory();
            repositoryMock.Setup(
                p => p.Get(categoryExample.Id, It.IsAny<CancellationToken>())
            ).ReturnsAsync(categoryExample);
            var input = new DeleteCategoryInput(categoryExample.Id);
            var useCase = new UseCase.DeleteCategory(repositoryMock.Object, unitOfWorkMock.Object);


            await useCase.Handle(input, CancellationToken.None);

            repositoryMock.Verify(
                p => p.Get(categoryExample.Id, It.IsAny<CancellationToken>()), Times.Once
            );
            repositoryMock.Verify(
                p => p.Delete(categoryExample, It.IsAny<CancellationToken>()), Times.Once
            );
            unitOfWorkMock.Verify(
                p => p.Commit(It.IsAny<CancellationToken>()), Times.Once
            );
        }

        [Fact(DisplayName = nameof(Test_DeleteCategory_ThrowWhenCategoryNotFound))]
        [Trait("Application", "Delete Category - Use Cases")]
        public async Task Test_DeleteCategory_ThrowWhenCategoryNotFound()
        {
            var repositoryMock = this._fixture.GetRepositoryMock();
            var unitOfWorkMock = this._fixture.GetUnitOfWorkMock();
            var exampleGuid = Guid.NewGuid();
            repositoryMock.Setup(
                p => p.Get(exampleGuid, It.IsAny<CancellationToken>())
            ).ThrowsAsync(new NotFoundException($"Category '{exampleGuid}'"));
            var input = new DeleteCategoryInput(exampleGuid);
            var useCase = new UseCase.DeleteCategory(repositoryMock.Object, unitOfWorkMock.Object);

            var task = async () => await useCase.Handle(input, CancellationToken.None);

            await task.Should().ThrowAsync<NotFoundException>();

            repositoryMock.Verify(
                p => p.Get(exampleGuid, It.IsAny<CancellationToken>()), Times.Once
            );
        }
    }
}
