using FC.CodeFlix.Catalog.Application.UseCases.Genre.Common;
using FC.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.CodeFlix.Catalog.UnitTests.Application.Genre.Common;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using DomainEntity = FC.CodeFlix.Catalog.Domain.Entity;
using UseCases = FC.CodeFlix.Catalog.Application.UseCases.Genre.ListGenres;

namespace FC.CodeFlix.Catalog.UnitTests.Application.Genre.ListGenres
{
    [Collection(nameof(ListGenresTestFixture))]
    public class ListGenresTest : GenreUseCasesBaseFixture
    {
        private readonly ListGenresTestFixture _fixture;

        public ListGenresTest(ListGenresTestFixture fixture) => this._fixture = fixture;

        [Fact(DisplayName = nameof(List))]
        [Trait("Application", "ListGenres - Use Cases")]
        public async Task List()
        {
            var genreRepositoryMock = this._fixture.GetGenreRepositoryMock();
            var genresListExample = this._fixture.GetExampleGenresList();
            var input = this._fixture.GetExampleInput();
            var outputRepositorySearch = new SearchOutput<DomainEntity.Genre>(
                currentPage: input.Page,
                perPage: input.PerPage,
                items: genresListExample,
                total: new Random().Next(50, 200));

            genreRepositoryMock.Setup(p => p.Search(It.IsAny<SearchInput>(), It.IsAny<CancellationToken>())).ReturnsAsync(outputRepositorySearch);
            var useCase = new UseCases.ListGenres(genreRepositoryMock.Object);

            UseCases.ListGenresOutput output = await useCase.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Page.Should().Be(outputRepositorySearch.CurrentPage);
            output.PerPage.Should().Be(outputRepositorySearch.PerPage);
            output.Total.Should().Be(outputRepositorySearch.Total);
            output.Items.Should().HaveCount(outputRepositorySearch.Items.Count);
            ((List<GenreModelOutput>)output.Items).ForEach(outputItem =>
            {
                var repositoryGenre = outputRepositorySearch.Items.FirstOrDefault(p => p.Id.Equals(outputItem.Id));
                outputItem.Should().NotBeNull();
                repositoryGenre.Should().NotBeNull();
                outputItem.Name.Should().Be(repositoryGenre!.Name);
                outputItem.IsActive.Should().Be(repositoryGenre.IsActive);
                outputItem.CreatedAt.Should().Be(repositoryGenre.CreatedAt);
                outputItem.Categories.Should().HaveCount(repositoryGenre.Categories.Count);
                foreach (var expectedId in repositoryGenre.Categories)
                    outputItem.Categories.Should().Contain(relation => relation.Id.Equals(expectedId));
            });

            genreRepositoryMock.Verify(p => p.Search(
                It.Is<SearchInput>(searchInput =>
                    searchInput.Page.Equals(input.Page) &&
                    searchInput.PerPage.Equals(input.PerPage) &&
                    searchInput.Search.Equals(input.Search) &&
                    searchInput.OrderBy.Equals(input.Sort) &&
                    searchInput.Order.Equals(input.Dir)
                ),
                It.IsAny<CancellationToken>()),
                Times.Once
            );
        }

        [Fact(DisplayName = nameof(ListEmpty))]
        [Trait("Application", "ListGenres - Use Cases")]
        public async Task ListEmpty()
        {
            var genreRepositoryMock = this._fixture.GetGenreRepositoryMock();
            var input = this._fixture.GetExampleInput();
            var outputRepositorySearch = new SearchOutput<DomainEntity.Genre>(
                currentPage: input.Page,
                perPage: input.PerPage,
                items: new List<DomainEntity.Genre>(),
                total: new Random().Next(50, 200));

            genreRepositoryMock.Setup(p => p.Search(It.IsAny<SearchInput>(), It.IsAny<CancellationToken>())).ReturnsAsync(outputRepositorySearch);
            var useCase = new UseCases.ListGenres(genreRepositoryMock.Object);

            UseCases.ListGenresOutput output = await useCase.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Page.Should().Be(outputRepositorySearch.CurrentPage);
            output.PerPage.Should().Be(outputRepositorySearch.PerPage);
            output.Total.Should().Be(outputRepositorySearch.Total);
            output.Items.Should().HaveCount(outputRepositorySearch.Items.Count);

            genreRepositoryMock.Verify(p => p.Search(
                It.Is<SearchInput>(searchInput =>
                    searchInput.Page.Equals(input.Page) &&
                    searchInput.PerPage.Equals(input.PerPage) &&
                    searchInput.Search.Equals(input.Search) &&
                    searchInput.OrderBy.Equals(input.Sort) &&
                    searchInput.Order.Equals(input.Dir)
                ),
                It.IsAny<CancellationToken>()),
                Times.Once
            );
        }

        [Fact(DisplayName = nameof(ListUsingDefaultInputValues))]
        [Trait("Application", "ListGenres - Use Cases")]
        public async Task ListUsingDefaultInputValues()
        {
            var genreRepositoryMock = this._fixture.GetGenreRepositoryMock();
            var outputRepositorySearch = new SearchOutput<DomainEntity.Genre>(
                currentPage: 1,
                perPage: 15,
                items: new List<DomainEntity.Genre>(),
                total: 0);

            genreRepositoryMock.Setup(p => p.Search(It.IsAny<SearchInput>(), It.IsAny<CancellationToken>())).ReturnsAsync(outputRepositorySearch);
            var useCase = new UseCases.ListGenres(genreRepositoryMock.Object);

            UseCases.ListGenresOutput output = await useCase.Handle(new UseCases.ListGenresInput(), CancellationToken.None);

            genreRepositoryMock.Verify(p => p.Search(
                It.Is<SearchInput>(searchInput =>
                    searchInput.Page.Equals(1) &&
                    searchInput.PerPage.Equals(15) &&
                    searchInput.Search.Equals(string.Empty) &&
                    searchInput.OrderBy.Equals(string.Empty) &&
                    searchInput.Order.Equals(SearchOrder.Asc)
                ),
                It.IsAny<CancellationToken>()),
                Times.Once
            );
        }
    }
}
