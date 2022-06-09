using FC.CodeFlix.Catalog.Application.Exceptions;
using FC.CodeFlix.Catalog.Domain.Exceptions;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using DomainEntity = FC.CodeFlix.Catalog.Domain.Entity;
using UseCases = FC.CodeFlix.Catalog.Application.UseCases.Genre.CreateGenre;

namespace FC.CodeFlix.Catalog.UnitTests.Application.Genre.CreateGenre
{
    [Collection(nameof(CreateGenreTestFixture))]
    public class CreateGenreTest
    {
        private readonly CreateGenreTestFixture _fixture;

        public CreateGenreTest(CreateGenreTestFixture fixture) => this._fixture = fixture;

        [Fact(DisplayName = nameof(Create))]
        [Trait("Application", "CreateGenre - Use Cases")]
        public async Task Create()
        {
            var genreRepositoryMock = this._fixture.GetGenreRepositoryMock();
            var unitOfWorkMock = this._fixture.GetUnitOfWorkMock();
            var categoryRepositoryMock = this._fixture.GetCategoryRepositoryMock();
            var useCase = new UseCases.CreateGenre(genreRepositoryMock.Object, unitOfWorkMock.Object, categoryRepositoryMock.Object);
            var input = this._fixture.GetExampleInput();

            var dateTimeBefore = DateTime.Now;
            var output = await useCase.Handle(input, CancellationToken.None);
            var dateTimeAfter = DateTime.Now.AddSeconds(1);

            genreRepositoryMock.Verify(p => p.Insert(
                It.IsAny<DomainEntity.Genre>(),
                It.IsAny<CancellationToken>()
                ), Times.Once
            );

            unitOfWorkMock.Verify(p => p.Commit(
                It.IsAny<CancellationToken>()
                ), Times.Once
            );

            output.Should().NotBeNull();
            output.Id.Should().NotBeEmpty();
            output.Name.Should().Be(input.Name);
            output.IsActive.Should().Be(input.IsActive);
            output.Categories.Should().BeEmpty();
            output.CreatedAt.Should().NotBeSameDateAs(default);
            (output.CreatedAt >= dateTimeBefore).Should().BeTrue();
            (output.CreatedAt <= dateTimeAfter).Should().BeTrue();
        }

        [Fact(DisplayName = nameof(CreateWithRelatedCategories))]
        [Trait("Application", "CreateGenre - Use Cases")]
        public async Task CreateWithRelatedCategories()
        {
            var input = this._fixture.GetExampleInputWithCategories();
            var genreRepositoryMock = this._fixture.GetGenreRepositoryMock();
            var unitOfWorkMock = this._fixture.GetUnitOfWorkMock();
            var categoryRepositoryMock = this._fixture.GetCategoryRepositoryMock();
            categoryRepositoryMock.Setup(p => p.GetIdsListByIds(
                It.IsAny<List<Guid>>(),
                It.IsAny<CancellationToken>())
            ).ReturnsAsync((IReadOnlyList<Guid>)input.CategoriesIds!);
            var useCase = new UseCases.CreateGenre(genreRepositoryMock.Object, unitOfWorkMock.Object, categoryRepositoryMock.Object);

            var output = await useCase.Handle(input, CancellationToken.None);

            genreRepositoryMock.Verify(p => p.Insert(
                It.IsAny<DomainEntity.Genre>(),
                It.IsAny<CancellationToken>()
                ), Times.Once
            );

            unitOfWorkMock.Verify(p => p.Commit(
                It.IsAny<CancellationToken>()
                ), Times.Once
            );

            output.Should().NotBeNull();
            output.Id.Should().NotBeEmpty();
            output.Name.Should().Be(input.Name);
            output.IsActive.Should().Be(input.IsActive);
            output.CreatedAt.Should().NotBeSameDateAs(default);
            output.Categories.Should().HaveCount(input.CategoriesIds?.Count ?? 0);
            input.CategoriesIds?.ForEach(id => output.Categories.Should().Contain(relation => relation.Id.Equals(id)));
        }

        [Fact(DisplayName = nameof(CreateThrowWhenRelatedCategoryNotFound))]
        [Trait("Application", "CreateGenre - Use Cases")]
        public async Task CreateThrowWhenRelatedCategoryNotFound()
        {
            var input = this._fixture.GetExampleInputWithCategories();
            var exampleGuid = input.CategoriesIds!.LastOrDefault();
            var genreRepositoryMock = this._fixture.GetGenreRepositoryMock();
            var categoryRepositoryMock = this._fixture.GetCategoryRepositoryMock();
            var unitOfWorkMock = this._fixture.GetUnitOfWorkMock();
            categoryRepositoryMock.Setup(p => p.GetIdsListByIds(
                It.IsAny<List<Guid>>(),
                It.IsAny<CancellationToken>())
            ).ReturnsAsync((IReadOnlyList<Guid>)input.CategoriesIds!.FindAll(p => p != exampleGuid));

            var useCase = new UseCases.CreateGenre(genreRepositoryMock.Object, unitOfWorkMock.Object, categoryRepositoryMock.Object);

            var action = async () => await useCase.Handle(input, CancellationToken.None);

            await action.Should().ThrowAsync<RelatedAggregateException>()
                .WithMessage($"Related category id (or ids) not found: {exampleGuid}");
            categoryRepositoryMock.Verify(p => p.GetIdsListByIds(
                It.IsAny<List<Guid>>(),
                It.IsAny<CancellationToken>()
                ), Times.Once
            );
        }

        [Theory(DisplayName = nameof(ThrowWhenNameIsInvalid))]
        [Trait("Application", "CreateGenre - Use Cases")]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public async Task ThrowWhenNameIsInvalid(string name)
        {
            var input = this._fixture.GetExampleInput(name);
            var genreRepositoryMock = this._fixture.GetGenreRepositoryMock();
            var categoryRepositoryMock = this._fixture.GetCategoryRepositoryMock();
            var unitOfWorkMock = this._fixture.GetUnitOfWorkMock();

            var useCase = new UseCases.CreateGenre(genreRepositoryMock.Object, unitOfWorkMock.Object, categoryRepositoryMock.Object);

            var action = async () => await useCase.Handle(input, CancellationToken.None);

            await action.Should().ThrowAsync<EntityValidationException>()
                .WithMessage("Name should not be empty or null");
        }
    }
}
