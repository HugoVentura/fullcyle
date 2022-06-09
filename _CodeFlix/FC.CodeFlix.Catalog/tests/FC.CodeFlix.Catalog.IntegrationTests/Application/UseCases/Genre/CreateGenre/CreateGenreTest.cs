using FC.CodeFlix.Catalog.Application.Exceptions;
using FC.CodeFlix.Catalog.Application.UseCases.Genre.Common;
using FC.CodeFlix.Catalog.Infra.Data.EF;
using FC.CodeFlix.Catalog.Infra.Data.EF.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using UseCase = FC.CodeFlix.Catalog.Application.UseCases.Genre.CreateGenre;

namespace FC.CodeFlix.Catalog.IntegrationTests.Application.UseCases.Genre.CreateGenre
{
    [Collection(nameof(CreateGenreTestFixture))]
    public class CreateGenreTest
    {
        private readonly CreateGenreTestFixture _fixture;

        public CreateGenreTest(CreateGenreTestFixture fixture) => this._fixture = fixture;

        [Fact(DisplayName = nameof(CreateGenre))]
        [Trait("Integration/Application", "CreateGenre - Use Cases")]
        public async Task CreateGenre()
        {
            var input = this._fixture.GetExampleInput();
            var dbContext = this._fixture.CreateDbContext();
            var createGenre = new UseCase.CreateGenre(new GenreRepository(dbContext), new UnitOfWork(dbContext), new CategoryRepository(dbContext));

            var output = await createGenre.Handle(input, CancellationToken.None);

            var assertDbContext = this._fixture.CreateDbContext(true);
            var assertGenreFromDb = await assertDbContext.Genres.FindAsync(output.Id);
            output.Id.Should().NotBeEmpty();
            output.Name.Should().Be(input.Name);
            output.IsActive.Should().Be(input.IsActive);
            output.CreatedAt.Should().NotBe(default);
            output.Categories.Should().BeEmpty();
            assertGenreFromDb.Should().NotBeNull();
            assertGenreFromDb!.Name.Should().Be(input.Name);
            assertGenreFromDb!.IsActive.Should().Be(input.IsActive);
        }

        [Fact(DisplayName = nameof(CreateGenreWithCategoriesRelations))]
        [Trait("Integration/Application", "CreateGenre - Use Cases")]
        public async Task CreateGenreWithCategoriesRelations()
        {
            var exampleCategories = this._fixture.GetExampleCategoryList(5);
            var arrangeDbContext = this._fixture.CreateDbContext();
            await arrangeDbContext.Categories.AddRangeAsync(exampleCategories);
            await arrangeDbContext.SaveChangesAsync();
            var input = this._fixture.GetExampleInput();
            input.CategoriesIds = exampleCategories.Select(category => category.Id).ToList();
            var actDbContext = this._fixture.CreateDbContext(true);
            var createGenre = new UseCase.CreateGenre(new GenreRepository(actDbContext), new UnitOfWork(actDbContext), new CategoryRepository(actDbContext));

            var output = await createGenre.Handle(input, CancellationToken.None);

            var assertDbContext = this._fixture.CreateDbContext(true);
            var assertGenreFromDb = await assertDbContext.Genres.FindAsync(output.Id);
            var assertCategoriesRelationsFromDb = await assertDbContext.GenresCategories.AsNoTracking().Where(p => p.GenreId.Equals(output.Id)).ToListAsync();
            var assertCategoryIdsRelatedFromDb = assertCategoriesRelationsFromDb.Select(relation => relation.CategoryId).ToList();
            var assertCategoryIdsRelatedFromOutput = output.Categories.Select(p => p.Id).ToList();
            output.Id.Should().NotBeEmpty();
            output.Name.Should().Be(input.Name);
            output.IsActive.Should().Be(input.IsActive);
            output.CreatedAt.Should().NotBe(default);
            output.Categories.Should().HaveCount(input.CategoriesIds.Count);
            assertCategoryIdsRelatedFromOutput.Should().BeEquivalentTo(input.CategoriesIds);
            assertGenreFromDb.Should().NotBeNull();
            assertGenreFromDb!.Name.Should().Be(input.Name);
            assertGenreFromDb!.IsActive.Should().Be(input.IsActive);
            assertCategoriesRelationsFromDb.Should().HaveCount(input.CategoriesIds.Count);
            assertCategoryIdsRelatedFromDb.Should().BeEquivalentTo(input.CategoriesIds);
        }

        [Fact(DisplayName = nameof(CreateGenreThrowsWhenCategoriesDoesntExists))]
        [Trait("Integration/Application", "CreateGenre - Use Cases")]
        public async Task CreateGenreThrowsWhenCategoriesDoesntExists()
        {
            var exampleCategories = this._fixture.GetExampleCategoryList(5);
            var arrangeDbContext = this._fixture.CreateDbContext();
            await arrangeDbContext.Categories.AddRangeAsync(exampleCategories);
            await arrangeDbContext.SaveChangesAsync();
            var input = this._fixture.GetExampleInput();
            input.CategoriesIds = exampleCategories.Select(category => category.Id).ToList();
            var randomGuid = Guid.NewGuid();
            input.CategoriesIds.Add(randomGuid);
            var actDbContext = this._fixture.CreateDbContext(true);
            var createGenre = new UseCase.CreateGenre(new GenreRepository(actDbContext), new UnitOfWork(actDbContext), new CategoryRepository(actDbContext));

            Func<Task<GenreModelOutput>> action = async () => await createGenre.Handle(input, CancellationToken.None);

            await action.Should().ThrowAsync<RelatedAggregateException>().WithMessage($"Related category id (or ids) not found: {randomGuid}");
        }
    }
}