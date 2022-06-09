using FC.CodeFlix.Catalog.Application.Exceptions;
using FC.CodeFlix.Catalog.Infra.Data.EF;
using FC.CodeFlix.Catalog.Infra.Data.EF.Models;
using FC.CodeFlix.Catalog.Infra.Data.EF.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using UseCase = FC.CodeFlix.Catalog.Application.UseCases.Genre.DeleteGenre;

namespace FC.CodeFlix.Catalog.IntegrationTests.Application.UseCases.Genre.DeleteGenre
{
    [Collection(nameof(DeleteGenreTestFixture))]
    public class DeleteGenreTest
    {
        private readonly DeleteGenreTestFixture _fixture;

        public DeleteGenreTest(DeleteGenreTestFixture fixture) => this._fixture = fixture;

        [Fact(DisplayName = nameof(DeleteGenre))]
        [Trait("Integration/Application", "DeleteGenre - Use Cases")]
        public async Task DeleteGenre()
        {
            var genresExampleList = this._fixture.GetExampleListGenres(10);
            var targetGenre = genresExampleList[5];
            var arrangeDbContext = this._fixture.CreateDbContext();
            await arrangeDbContext.Genres.AddRangeAsync(genresExampleList);
            await arrangeDbContext.SaveChangesAsync();
            var actDbContext = this._fixture.CreateDbContext(true);
            var useCase = new UseCase.DeleteGenre(new GenreRepository(actDbContext), new UnitOfWork(actDbContext));
            var input = new UseCase.DeleteGenreInput(targetGenre.Id);

            await useCase.Handle(input, CancellationToken.None);

            var assertDbContext = this._fixture.CreateDbContext(true);
            var assertGenreFromDb = await assertDbContext.Genres.FindAsync(targetGenre.Id);
            assertGenreFromDb.Should().BeNull();
        }

        [Fact(DisplayName = nameof(DeleteGenreThrowsWhenNotFound))]
        [Trait("Integration/Application", "DeleteGenre - Use Cases")]
        public async Task DeleteGenreThrowsWhenNotFound()
        {
            var genresExampleList = this._fixture.GetExampleListGenres(10);
            var arrangeDbContext = this._fixture.CreateDbContext();
            await arrangeDbContext.Genres.AddRangeAsync(genresExampleList);
            await arrangeDbContext.SaveChangesAsync();
            var actDbContext = this._fixture.CreateDbContext(true);
            var useCase = new UseCase.DeleteGenre(new GenreRepository(actDbContext), new UnitOfWork(actDbContext));
            var randomGuid = Guid.NewGuid();
            var input = new UseCase.DeleteGenreInput(randomGuid);

            var action = async () => await useCase.Handle(input, CancellationToken.None);

            await action.Should().ThrowAsync<NotFoundException>()
                .WithMessage($"Genre '{randomGuid}' not found.");
        }

        [Fact(DisplayName = nameof(DeleteGenreWithRelations))]
        [Trait("Integration/Application", "DeleteGenre - Use Cases")]
        public async Task DeleteGenreWithRelations()
        {
            var genresExampleList = this._fixture.GetExampleListGenres(10);
            var targetGenre = genresExampleList[5];
            var exampleCategories = this._fixture.GetExampleCategoryList(5);
            var arrangeDbContext = this._fixture.CreateDbContext();
            await arrangeDbContext.Genres.AddRangeAsync(genresExampleList);
            await arrangeDbContext.Categories.AddRangeAsync(exampleCategories);
            await arrangeDbContext.GenresCategories.AddRangeAsync(exampleCategories.Select(category => new GenresCategories(targetGenre.Id, category.Id)));
            await arrangeDbContext.SaveChangesAsync();
            var actDbContext = this._fixture.CreateDbContext(true);
            var useCase = new UseCase.DeleteGenre(new GenreRepository(actDbContext), new UnitOfWork(actDbContext));
            var input = new UseCase.DeleteGenreInput(targetGenre.Id);

            await useCase.Handle(input, CancellationToken.None);

            var assertDbContext = this._fixture.CreateDbContext(true);
            var assertGenreFromDb = await assertDbContext.Genres.FindAsync(targetGenre.Id);
            var assertRelations = await assertDbContext.GenresCategories.AsNoTracking().Where(relation => relation.GenreId.Equals(targetGenre.Id)).ToListAsync();
            assertGenreFromDb.Should().BeNull();
            assertRelations.Should().BeEmpty();
        }
    }
}
