using FC.CodeFlix.Catalog.Application.UseCases.Genre.Common;
using FluentAssertions;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using UseCases = FC.CodeFlix.Catalog.Application.UseCases.Genre.UpdateGenre;
using DomainEntity = FC.CodeFlix.Catalog.Domain.Entity;
using FC.CodeFlix.Catalog.Application.Exceptions;
using FC.CodeFlix.Catalog.Domain.Exceptions;
using System.Collections.Generic;

namespace FC.CodeFlix.Catalog.UnitTests.Application.Genre.UpdateGenre
{
    [Collection(nameof(UpdateGenreFixture))]
    public class UpdateGenreTest
    {
        private readonly UpdateGenreFixture _fixture;

        public UpdateGenreTest(UpdateGenreFixture fixture) => this._fixture = fixture;

        [Fact(DisplayName = nameof(Update))]
        [Trait("Application", "UpdateGenre - Use Cases")]
        public async Task Update()
        {
            var genreRepositoryMock = this._fixture.GetGenreRepositoryMock();
            var unitOfWorkMock = this._fixture.GetUnitOfWorkMock();
            var examplerGenre = this._fixture.GetExampleGenre();
            var newNameExample = this._fixture.GetValidGenreName();
            var newIsActive = !examplerGenre.IsActive;
            genreRepositoryMock.Setup(p => p.Get(It.Is<Guid>(x => x.Equals(examplerGenre.Id)), It.IsAny<CancellationToken>())).ReturnsAsync(examplerGenre);
            var useCase = new UseCases.UpdateGenre(genreRepositoryMock.Object, unitOfWorkMock.Object, this._fixture.GetCategoryRepositoryMock().Object);
            var input = new UseCases.UpdateGenreInput(examplerGenre.Id, newNameExample, newIsActive);

            GenreModelOutput output = await useCase.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Id.Should().Be(examplerGenre.Id);
            output.Name.Should().Be(newNameExample);
            output.IsActive.Should().Be(newIsActive);
            output.CreatedAt.Should().BeSameDateAs(examplerGenre.CreatedAt);
            output.Categories.Should().BeEmpty();
            genreRepositoryMock.Verify(p => p.Update(It.Is<DomainEntity.Genre>(x => x.Id.Equals(examplerGenre.Id)), It.IsAny<CancellationToken>()), Times.Once);
            unitOfWorkMock.Verify(p => p.Commit(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = nameof(ThrowWhenNotFound))]
        [Trait("Application", "UpdateGenre - Use Cases")]
        public async Task ThrowWhenNotFound()
        {
            var genreRepositoryMock = this._fixture.GetGenreRepositoryMock();
            var exampleId = Guid.NewGuid();
            genreRepositoryMock.Setup(p => p.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ThrowsAsync(new NotFoundException($"Genre '{exampleId}' not found."));
            var useCase = new UseCases.UpdateGenre(genreRepositoryMock.Object, this._fixture.GetUnitOfWorkMock().Object, this._fixture.GetCategoryRepositoryMock().Object);
            var input = new UseCases.UpdateGenreInput(exampleId, this._fixture.GetValidGenreName(), true);

            var action = async () => await useCase.Handle(input, CancellationToken.None);

            await action.Should().ThrowAsync<NotFoundException>()
                .WithMessage($"Genre '{exampleId}' not found.");
        }

        [Theory(DisplayName = nameof(ThrowNameIsInvalid))]
        [Trait("Application", "UpdateGenre - Use Cases")]
        [InlineData("")]
        [InlineData("  ")]
        [InlineData(null)]
        public async Task ThrowNameIsInvalid(string name)
        {
            var genreRepositoryMock = this._fixture.GetGenreRepositoryMock();
            var unitOfWorkMock = this._fixture.GetUnitOfWorkMock();
            var examplerGenre = this._fixture.GetExampleGenre();
            var newNameExample = this._fixture.GetValidGenreName();
            var newIsActive = !examplerGenre.IsActive;
            genreRepositoryMock.Setup(p => p.Get(It.Is<Guid>(x => x.Equals(examplerGenre.Id)), It.IsAny<CancellationToken>())).ReturnsAsync(examplerGenre);
            var useCase = new UseCases.UpdateGenre(genreRepositoryMock.Object, unitOfWorkMock.Object, this._fixture.GetCategoryRepositoryMock().Object);
            var input = new UseCases.UpdateGenreInput(examplerGenre.Id, name, newIsActive);

            var action = async () => await useCase.Handle(input, CancellationToken.None);

            await action.Should().ThrowAsync<EntityValidationException>()
                .WithMessage($"Name should not be empty or null");
        }

        [Theory(DisplayName = nameof(UpdateGenreonlyName))]
        [Trait("Application", "UpdateGenre - Use Cases")]
        [InlineData(true)]
        [InlineData(false)]
        public async Task UpdateGenreonlyName(bool isActive)
        {
            var genreRepositoryMock = this._fixture.GetGenreRepositoryMock();
            var unitOfWorkMock = this._fixture.GetUnitOfWorkMock();
            var examplerGenre = this._fixture.GetExampleGenre(isActive);
            var newNameExample = this._fixture.GetValidGenreName();
            genreRepositoryMock.Setup(p => p.Get(It.Is<Guid>(x => x.Equals(examplerGenre.Id)), It.IsAny<CancellationToken>())).ReturnsAsync(examplerGenre);
            var useCase = new UseCases.UpdateGenre(genreRepositoryMock.Object, unitOfWorkMock.Object, this._fixture.GetCategoryRepositoryMock().Object);
            var input = new UseCases.UpdateGenreInput(examplerGenre.Id, newNameExample);

            GenreModelOutput output = await useCase.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Id.Should().Be(examplerGenre.Id);
            output.Name.Should().Be(newNameExample);
            output.IsActive.Should().Be(isActive);
            output.CreatedAt.Should().BeSameDateAs(examplerGenre.CreatedAt);
            output.Categories.Should().BeEmpty();
            genreRepositoryMock.Verify(p => p.Update(It.Is<DomainEntity.Genre>(x => x.Id.Equals(examplerGenre.Id)), It.IsAny<CancellationToken>()), Times.Once);
            unitOfWorkMock.Verify(p => p.Commit(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = nameof(UpdateAddingCategoriesIds))]
        [Trait("Application", "UpdateGenre - Use Cases")]
        public async Task UpdateAddingCategoriesIds()
        {
            var genreRepositoryMock = this._fixture.GetGenreRepositoryMock();
            var categoryRepositoryMock = this._fixture.GetCategoryRepositoryMock();
            var unitOfWorkMock = this._fixture.GetUnitOfWorkMock();
            var examplerGenre = this._fixture.GetExampleGenre();
            var exampleCategoriesIdsList = this._fixture.GetRandomIdsList();
            var newNameExample = this._fixture.GetValidGenreName();
            var newIsActive = !examplerGenre.IsActive;
            genreRepositoryMock.Setup(p => p.Get(It.Is<Guid>(x => x.Equals(examplerGenre.Id)), It.IsAny<CancellationToken>())).ReturnsAsync(examplerGenre);
            categoryRepositoryMock.Setup(p => p.GetIdsListByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>())).ReturnsAsync(exampleCategoriesIdsList);
            var useCase = new UseCases.UpdateGenre(genreRepositoryMock.Object, unitOfWorkMock.Object, categoryRepositoryMock.Object);
            var input = new UseCases.UpdateGenreInput(examplerGenre.Id, newNameExample, newIsActive, exampleCategoriesIdsList);

            GenreModelOutput output = await useCase.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Id.Should().Be(examplerGenre.Id);
            output.Name.Should().Be(newNameExample);
            output.IsActive.Should().Be(newIsActive);
            output.CreatedAt.Should().BeSameDateAs(examplerGenre.CreatedAt);
            output.Categories.Should().HaveCount(exampleCategoriesIdsList.Count);
            exampleCategoriesIdsList.ForEach(expectedId => 
                output.Categories.Should().Contain(relation => relation.Id.Equals(expectedId)));
            genreRepositoryMock.Verify(p => p.Update(It.Is<DomainEntity.Genre>(x => x.Id.Equals(examplerGenre.Id)), It.IsAny<CancellationToken>()), Times.Once);
            unitOfWorkMock.Verify(p => p.Commit(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = nameof(UpdateReplacingCategoriesIds))]
        [Trait("Application", "UpdateGenre - Use Cases")]
        public async Task UpdateReplacingCategoriesIds()
        {
            var genreRepositoryMock = this._fixture.GetGenreRepositoryMock();
            var categoryRepositoryMock = this._fixture.GetCategoryRepositoryMock();
            var unitOfWorkMock = this._fixture.GetUnitOfWorkMock();
            var examplerGenre = this._fixture.GetExampleGenre(categoriesIds: this._fixture.GetRandomIdsList());
            var exampleCategoriesIdsList = this._fixture.GetRandomIdsList();
            var newNameExample = this._fixture.GetValidGenreName();
            var newIsActive = !examplerGenre.IsActive;
            genreRepositoryMock.Setup(p => p.Get(It.Is<Guid>(x => x.Equals(examplerGenre.Id)), It.IsAny<CancellationToken>())).ReturnsAsync(examplerGenre);
            categoryRepositoryMock.Setup(p => p.GetIdsListByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>())).ReturnsAsync(exampleCategoriesIdsList);
            var useCase = new UseCases.UpdateGenre(genreRepositoryMock.Object, unitOfWorkMock.Object, categoryRepositoryMock.Object);
            var input = new UseCases.UpdateGenreInput(examplerGenre.Id, newNameExample, newIsActive, exampleCategoriesIdsList);

            GenreModelOutput output = await useCase.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Id.Should().Be(examplerGenre.Id);
            output.Name.Should().Be(newNameExample);
            output.IsActive.Should().Be(newIsActive);
            output.CreatedAt.Should().BeSameDateAs(examplerGenre.CreatedAt);
            output.Categories.Should().HaveCount(exampleCategoriesIdsList.Count);
            exampleCategoriesIdsList.ForEach(expectedId =>
                output.Categories.Should().Contain(relation => relation.Id.Equals(expectedId)));
            genreRepositoryMock.Verify(p => p.Update(It.Is<DomainEntity.Genre>(x => x.Id.Equals(examplerGenre.Id)), It.IsAny<CancellationToken>()), Times.Once);
            unitOfWorkMock.Verify(p => p.Commit(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = nameof(ThrowWhenCategoryNotFound))]
        [Trait("Application", "UpdateGenre - Use Cases")]
        public async Task ThrowWhenCategoryNotFound()
        {
            var genreRepositoryMock = this._fixture.GetGenreRepositoryMock();
            var unitOfWorkMock = this._fixture.GetUnitOfWorkMock();
            var categoryRepositoryMock = this._fixture.GetCategoryRepositoryMock();
            var examplerGenre = this._fixture.GetExampleGenre(categoriesIds: this._fixture.GetRandomIdsList());
            var exampleNewCategoriesIdsList = this._fixture.GetRandomIdsList(10);
            var listReturnedByCategoryRepository = exampleNewCategoriesIdsList.GetRange(0, exampleNewCategoriesIdsList.Count - 2);
            var idsNotReturnedByCategoryRepositgory = exampleNewCategoriesIdsList.GetRange(exampleNewCategoriesIdsList.Count - 2, 2);
            var newNameExample = this._fixture.GetValidGenreName();
            var newIsActive = !examplerGenre.IsActive;
            genreRepositoryMock.Setup(p => p.Get(It.Is<Guid>(x => x.Equals(examplerGenre.Id)), It.IsAny<CancellationToken>())).ReturnsAsync(examplerGenre);
            categoryRepositoryMock.Setup(p => p.GetIdsListByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>())).ReturnsAsync(listReturnedByCategoryRepository);
            var useCase = new UseCases.UpdateGenre(genreRepositoryMock.Object, unitOfWorkMock.Object, categoryRepositoryMock.Object);
            var input = new UseCases.UpdateGenreInput(examplerGenre.Id, newNameExample, newIsActive, exampleNewCategoriesIdsList);

            var action = async () => await useCase.Handle(input, CancellationToken.None);

            var notFoundIdsAsString = string.Join(", ", idsNotReturnedByCategoryRepositgory);
            await action.Should().ThrowAsync<RelatedAggregateException>()
                .WithMessage($"Related category id (or ids) not found: {notFoundIdsAsString}");
        }

        [Fact(DisplayName = nameof(UpdateWithoutCategoriesIds))]
        [Trait("Application", "UpdateGenre - Use Cases")]
        public async Task UpdateWithoutCategoriesIds()
        {
            var genreRepositoryMock = this._fixture.GetGenreRepositoryMock();
            var unitOfWorkMock = this._fixture.GetUnitOfWorkMock();
            var exampleCategoriesIdsList = this._fixture.GetRandomIdsList();
            var examplerGenre = this._fixture.GetExampleGenre(categoriesIds: exampleCategoriesIdsList);
            var newNameExample = this._fixture.GetValidGenreName();
            var newIsActive = !examplerGenre.IsActive;
            genreRepositoryMock.Setup(p => p.Get(It.Is<Guid>(x => x.Equals(examplerGenre.Id)), It.IsAny<CancellationToken>())).ReturnsAsync(examplerGenre);
            var useCase = new UseCases.UpdateGenre(genreRepositoryMock.Object, unitOfWorkMock.Object, this._fixture.GetCategoryRepositoryMock().Object);
            var input = new UseCases.UpdateGenreInput(examplerGenre.Id, newNameExample, newIsActive);

            GenreModelOutput output = await useCase.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Id.Should().Be(examplerGenre.Id);
            output.Name.Should().Be(newNameExample);
            output.IsActive.Should().Be(newIsActive);
            output.CreatedAt.Should().BeSameDateAs(examplerGenre.CreatedAt);
            output.Categories.Should().HaveCount(exampleCategoriesIdsList.Count);
            exampleCategoriesIdsList.ForEach(expectedId =>
                output.Categories.Should().Contain(relation => relation.Id.Equals(expectedId)));
            genreRepositoryMock.Verify(p => p.Update(It.Is<DomainEntity.Genre>(x => x.Id.Equals(examplerGenre.Id)), It.IsAny<CancellationToken>()), Times.Once);
            unitOfWorkMock.Verify(p => p.Commit(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = nameof(UpdateWithEmptyCategoriesIds))]
        [Trait("Application", "UpdateGenre - Use Cases")]
        public async Task UpdateWithEmptyCategoriesIds()
        {
            var genreRepositoryMock = this._fixture.GetGenreRepositoryMock();
            var unitOfWorkMock = this._fixture.GetUnitOfWorkMock();
            var exampleCategoriesIdsList = this._fixture.GetRandomIdsList();
            var examplerGenre = this._fixture.GetExampleGenre(categoriesIds: exampleCategoriesIdsList);
            var newNameExample = this._fixture.GetValidGenreName();
            var newIsActive = !examplerGenre.IsActive;
            genreRepositoryMock.Setup(p => p.Get(It.Is<Guid>(x => x.Equals(examplerGenre.Id)), It.IsAny<CancellationToken>())).ReturnsAsync(examplerGenre);
            var useCase = new UseCases.UpdateGenre(genreRepositoryMock.Object, unitOfWorkMock.Object, this._fixture.GetCategoryRepositoryMock().Object);
            var input = new UseCases.UpdateGenreInput(examplerGenre.Id, newNameExample, newIsActive, new List<Guid>());

            GenreModelOutput output = await useCase.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Id.Should().Be(examplerGenre.Id);
            output.Name.Should().Be(newNameExample);
            output.IsActive.Should().Be(newIsActive);
            output.CreatedAt.Should().BeSameDateAs(examplerGenre.CreatedAt);
            output.Categories.Should().BeEmpty();
            genreRepositoryMock.Verify(p => p.Update(It.Is<DomainEntity.Genre>(x => x.Id.Equals(examplerGenre.Id)), It.IsAny<CancellationToken>()), Times.Once);
            unitOfWorkMock.Verify(p => p.Commit(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
