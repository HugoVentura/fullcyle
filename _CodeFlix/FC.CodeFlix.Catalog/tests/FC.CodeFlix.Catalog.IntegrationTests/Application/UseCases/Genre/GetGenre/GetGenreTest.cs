using FC.CodeFlix.Catalog.Application.Exceptions;
using FC.CodeFlix.Catalog.Infra.Data.EF.Models;
using FC.CodeFlix.Catalog.Infra.Data.EF.Repositories;
using FluentAssertions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using UseCase = FC.CodeFlix.Catalog.Application.UseCases.Genre.GetGenre;

namespace FC.CodeFlix.Catalog.IntegrationTests.Application.UseCases.Genre.GetGenre
{
    [Collection(nameof(GetGenreTestFixture))]
    public class GetGenreTest
    {
        private readonly GetGenreTestFixture _fixture;

        public GetGenreTest(GetGenreTestFixture fixture) => this._fixture = fixture;

        [Fact(DisplayName = nameof(GetGenre))]
        [Trait("Integration/Application", "GetGenre - Use Cases")]
        public async Task GetGenre()
        {
            var genresExampleList = this._fixture.GetExampleListGenres(10);
            var expectedGenre = genresExampleList[5];
            var arrangeDbContext = this._fixture.CreateDbContext();
            await arrangeDbContext.Genres.AddRangeAsync(genresExampleList);
            await arrangeDbContext.SaveChangesAsync();
            var genreRepository = new GenreRepository(this._fixture.CreateDbContext(true));
            var useCase = new UseCase.GetGenre(genreRepository);
            var input = new UseCase.GetGenreInput(expectedGenre.Id);

            var output = await useCase.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Id.Should().Be(expectedGenre.Id);
            output.Name.Should().Be(expectedGenre.Name);
            output.IsActive.Should().Be(expectedGenre.IsActive);
            output.CreatedAt.Should().Be(expectedGenre.CreatedAt);
        }

        [Fact(DisplayName = nameof(GetGenreThrowsWhenNotFound))]
        [Trait("Integration/Application", "GetGenre - Use Cases")]
        public async Task GetGenreThrowsWhenNotFound()
        {
            var genresExampleList = this._fixture.GetExampleListGenres(10);
            var randomGuid = Guid.NewGuid();
            var arrangeDbContext = this._fixture.CreateDbContext();
            await arrangeDbContext.Genres.AddRangeAsync(genresExampleList);
            await arrangeDbContext.SaveChangesAsync();
            var genreRepository = new GenreRepository(this._fixture.CreateDbContext(true));
            var useCase = new UseCase.GetGenre(genreRepository);
            var input = new UseCase.GetGenreInput(randomGuid);

            var action = async () => await useCase.Handle(input, CancellationToken.None);

            await action.Should().ThrowAsync<NotFoundException>().WithMessage($"Genre '{randomGuid}' not found.");
        }

        [Fact(DisplayName = nameof(GetGenreWithCategoryRelations))]
        [Trait("Integration/Application", "GetGenre - Use Cases")]
        public async Task GetGenreWithCategoryRelations()
        {
            var genresExampleList = this._fixture.GetExampleListGenres(10);
            var categoriesExampleList = this._fixture.GetExampleCategoryList(5);
            var expectedGenre = genresExampleList[5];
            categoriesExampleList.ForEach(category => expectedGenre.AddCategory(category.Id));
            var arrangeDbContext = this._fixture.CreateDbContext();
            await arrangeDbContext.Categories.AddRangeAsync(categoriesExampleList);
            await arrangeDbContext.Genres.AddRangeAsync(genresExampleList);
            await arrangeDbContext.GenresCategories.AddRangeAsync(expectedGenre.Categories.Select(categoryId => new GenresCategories(expectedGenre.Id, categoryId)));
            await arrangeDbContext.SaveChangesAsync();
            var genreRepository = new GenreRepository(this._fixture.CreateDbContext(true));
            var useCase = new UseCase.GetGenre(genreRepository);
            var input = new UseCase.GetGenreInput(expectedGenre.Id);

            var output = await useCase.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Id.Should().Be(expectedGenre.Id);
            output.Name.Should().Be(expectedGenre.Name);
            output.IsActive.Should().Be(expectedGenre.IsActive);
            output.CreatedAt.Should().Be(expectedGenre.CreatedAt);
            output.Categories.Should().HaveCount(expectedGenre.Categories.Count);
            output.Categories.ToList().ForEach(relationModel =>
            {
                expectedGenre.Categories.Should().Contain(relationModel.Id);
                relationModel.Name.Should().BeNull();
            });
        }
    }
}
