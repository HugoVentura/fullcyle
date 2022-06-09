using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using UseCases = FC.CodeFlix.Catalog.Application.UseCases.Genre.DeleteGenre;
using DomainEntity = FC.CodeFlix.Catalog.Domain.Entity;
using FC.CodeFlix.Catalog.Application.Exceptions;
using FluentAssertions;

namespace FC.CodeFlix.Catalog.UnitTests.Application.Genre.DeleteGenre
{
    [Collection(nameof(DeleteGenreTestFixture))]
    public class DeleteGenreTest
    {
        private DeleteGenreTestFixture _fixture;

        public DeleteGenreTest(DeleteGenreTestFixture fixture) => this._fixture = fixture;

        [Fact(DisplayName = nameof(Delete))]
        [Trait("Application", "DeleteGenre - Use Cases")]
        public async Task Delete()
        {
            var genreRepositoryMock = this._fixture.GetGenreRepositoryMock();
            var unitOfWorkMock = this._fixture.GetUnitOfWorkMock();
            var examplerGenre = this._fixture.GetExampleGenre();
            genreRepositoryMock.Setup(p => p.Get(It.Is<Guid>(x => x.Equals(examplerGenre.Id)), It.IsAny<CancellationToken>())).ReturnsAsync(examplerGenre);
            var useCase = new UseCases.DeleteGenre(genreRepositoryMock.Object, unitOfWorkMock.Object);
            var input = new UseCases.DeleteGenreInput(examplerGenre.Id);

            await useCase.Handle(input, CancellationToken.None);

            genreRepositoryMock.Verify(p => p.Get(It.Is<Guid>(x => x.Equals(examplerGenre.Id)), It.IsAny<CancellationToken>()), Times.Once);
            genreRepositoryMock.Verify(p => p.Delete(It.Is<DomainEntity.Genre>(x => x.Id.Equals(examplerGenre.Id)), It.IsAny<CancellationToken>()), Times.Once);
            unitOfWorkMock.Verify(p => p.Commit(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = nameof(ThrowWhenNotFound))]
        [Trait("Application", "DeleteGenre - Use Cases")]
        public async Task ThrowWhenNotFound()
        {
            var genreRepositoryMock = this._fixture.GetGenreRepositoryMock();
            var unitOfWorkMock = this._fixture.GetUnitOfWorkMock();

            var exampleId = Guid.NewGuid();
            genreRepositoryMock.Setup(p => p.Get(It.Is<Guid>(p => p.Equals(exampleId)), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new NotFoundException($"Genre '{exampleId}' not found"));
            var useCase = new UseCases.DeleteGenre(genreRepositoryMock.Object, unitOfWorkMock.Object);
            var input = new UseCases.DeleteGenreInput(exampleId);

            var action = async () => await useCase.Handle(input, CancellationToken.None);

            await action.Should().ThrowAsync<NotFoundException>()
                .WithMessage($"Genre '{exampleId}' not found");
            genreRepositoryMock.Verify(p => p.Get(It.Is<Guid>(x => x.Equals(exampleId)), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
