using FC.CodeFlix.Catalog.Application.Exceptions;
using FC.CodeFlix.Catalog.Application.UseCases.Genre.Common;
using FluentAssertions;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using UseCases = FC.CodeFlix.Catalog.Application.UseCases.Genre.GetGenre;

namespace FC.CodeFlix.Catalog.UnitTests.Application.Genre.GetGenre
{
    [Collection(nameof(GetGenreTestFixture))]
    public class GetGenreTest
    {
        private readonly GetGenreTestFixture _fixture;

        public GetGenreTest(GetGenreTestFixture fixture) => this._fixture = fixture;

        [Fact(DisplayName = nameof(Get))]
        [Trait("Application", "GetGenre - Use Cases")]
        public async Task Get()
        {
            var genreRepositoryMock = this._fixture.GetGenreRepositoryMock();
            var examplerGenre = this._fixture.GetExampleGenre(categoriesIds: this._fixture.GetRandomIdsList());
            genreRepositoryMock.Setup(p => p.Get(It.Is<Guid>(x => x.Equals(examplerGenre.Id)), It.IsAny<CancellationToken>())).ReturnsAsync(examplerGenre);
            var useCase = new UseCases.GetGenre(genreRepositoryMock.Object);
            var input = new UseCases.GetGenreInput(examplerGenre.Id);

            GenreModelOutput output = await useCase.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Id.Should().Be(examplerGenre.Id);
            output.Name.Should().Be(examplerGenre.Name);
            output.IsActive.Should().Be(examplerGenre.IsActive);
            output.CreatedAt.Should().BeSameDateAs(examplerGenre.CreatedAt);
            output.Categories.Should().HaveCount(examplerGenre.Categories.Count);
            foreach (var expectedId in examplerGenre.Categories)
                output.Categories.Should().Contain(relation => relation.Id.Equals(expectedId));
            genreRepositoryMock.Verify(p => p.Get(It.Is<Guid>(x => x.Equals(examplerGenre.Id)), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = nameof(ThrowWhenNotFound))]
        [Trait("Application", "GetGenre - Use Cases")]
        public async Task ThrowWhenNotFound()
        {
            var genreRepositoryMock = this._fixture.GetGenreRepositoryMock();
            var exampleId = Guid.NewGuid();
            genreRepositoryMock.Setup(p => p.Get(It.Is<Guid>(p => p.Equals(exampleId)), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new NotFoundException($"Genre '{exampleId}' not found"));
            var useCase = new UseCases.GetGenre(genreRepositoryMock.Object);
            var input = new UseCases.GetGenreInput(exampleId);

            var action = async () => await useCase.Handle(input, CancellationToken.None);

            await action.Should().ThrowAsync<NotFoundException>()
                .WithMessage($"Genre '{exampleId}' not found");
            genreRepositoryMock.Verify(p => p.Get(It.Is<Guid>(x => x.Equals(exampleId)), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
