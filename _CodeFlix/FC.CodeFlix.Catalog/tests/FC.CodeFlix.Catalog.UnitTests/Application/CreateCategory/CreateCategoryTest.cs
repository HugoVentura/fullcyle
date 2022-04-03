using FC.CodeFlix.Catalog.Application.UseCases.Category.CreateCategory;
using FC.CodeFlix.Catalog.Domain.Entity;
using FC.CodeFlix.Catalog.Domain.Exceptions;
using FluentAssertions;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using UseCases = FC.CodeFlix.Catalog.Application.UseCases.Category.CreateCategory;

namespace FC.CodeFlix.Catalog.UnitTests.Application.CreateCategory
{
    [Collection(nameof(CreateCategoryTestFixture))]
    public class CreateCategoryTest
    {
        private readonly CreateCategoryTestFixture _fixture;

        public CreateCategoryTest(CreateCategoryTestFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact(DisplayName = nameof(Test_CreateCategory))]
        [Trait("Application", "Category - Use Cases")]
        public async void Test_CreateCategory()
        {
            var repositoryMock = this._fixture.GetRepositoryMock();
            var unitOfWorkMock = this._fixture.GetUnitOfWorkMock();
            var useCase = new UseCases.CreateCategory(repositoryMock.Object, unitOfWorkMock.Object);
            var input = this._fixture.GetInput();

            var output = await useCase.Handle(input, CancellationToken.None);

            repositoryMock.Verify(repository => 
                repository.Insert(
                    It.IsAny<Category>(),
                    It.IsAny<CancellationToken>()
                ),
                Times.Once
            );

            unitOfWorkMock.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Once);

            output.Should().NotBeNull();
            output.Name.Should().Be(input.Name);
            output.Description.Should().Be(input.Description);
            output.IsActive.Should().Be(input.IsActive);
            output.Id.Should().NotBeEmpty();
            output.CreatedAt.Should().NotBeSameDateAs(default);
        }

        [Theory(DisplayName = nameof(Test_CreateCategory_ThrowWhenCantInstantiateCategory))]
        [Trait("Application", "Category - Use Cases")]
        [MemberData(
            nameof(CreateCategoryTestDataGenerator.GetInvalidInputs), 
            parameters: 24,
            MemberType = typeof(CreateCategoryTestDataGenerator))]
        public async void Test_CreateCategory_ThrowWhenCantInstantiateCategory(CreateCategoryInput input, string exceptionMessage)
        {
            var useCase = new UseCases.CreateCategory(this._fixture.GetRepositoryMock().Object, this._fixture.GetUnitOfWorkMock().Object);

            Func<Task> task = async () => await useCase.Handle(input, CancellationToken.None);

            await task.Should()
                .ThrowAsync<EntityValidationException>()
                .WithMessage(exceptionMessage);
        }

        [Fact(DisplayName = nameof(Test_CreateCategory_WithOnlyName))]
        [Trait("Application", "Category - Use Cases")]
        public async void Test_CreateCategory_WithOnlyName()
        {
            var repositoryMock = this._fixture.GetRepositoryMock();
            var unitOfWorkMock = this._fixture.GetUnitOfWorkMock();
            var useCase = new UseCases.CreateCategory(repositoryMock.Object, unitOfWorkMock.Object);
            var input = new CreateCategoryInput(this._fixture.GetValidCategoryName());

            var output = await useCase.Handle(input, CancellationToken.None);

            repositoryMock.Verify(repository =>
                repository.Insert(
                    It.IsAny<Category>(),
                    It.IsAny<CancellationToken>()
                ),
                Times.Once
            );

            unitOfWorkMock.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Once);

            output.Should().NotBeNull();
            output.Name.Should().Be(input.Name);
            output.Description.Should().Be(string.Empty);
            output.IsActive.Should().BeTrue();
            output.Id.Should().NotBeEmpty();
            output.CreatedAt.Should().NotBeSameDateAs(default);
        }

        [Fact(DisplayName = nameof(Test_CreateCategory_WithOnlyNameAndDescription))]
        [Trait("Application", "Category - Use Cases")]
        public async void Test_CreateCategory_WithOnlyNameAndDescription()
        {
            var repositoryMock = this._fixture.GetRepositoryMock();
            var unitOfWorkMock = this._fixture.GetUnitOfWorkMock();
            var useCase = new UseCases.CreateCategory(repositoryMock.Object, unitOfWorkMock.Object);
            var input = new CreateCategoryInput(this._fixture.GetValidCategoryName(), this._fixture.GetValidCategoryDescription());

            var output = await useCase.Handle(input, CancellationToken.None);

            repositoryMock.Verify(repository =>
                repository.Insert(
                    It.IsAny<Category>(),
                    It.IsAny<CancellationToken>()
                ),
                Times.Once
            );

            unitOfWorkMock.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Once);

            output.Should().NotBeNull();
            output.Name.Should().Be(input.Name);
            output.Description.Should().Be(input.Description);
            output.IsActive.Should().BeTrue();
            output.Id.Should().NotBeEmpty();
            output.CreatedAt.Should().NotBeSameDateAs(default);
        }
    }
}