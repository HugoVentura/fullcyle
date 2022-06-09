using FC.CodeFlix.Catalog.Application.Exceptions;
using FC.CodeFlix.Catalog.Application.UseCases.Genre.UpdateGenre;
using FC.CodeFlix.Catalog.Infra.Data.EF;
using FC.CodeFlix.Catalog.Infra.Data.EF.Models;
using FC.CodeFlix.Catalog.Infra.Data.EF.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using UseCase = FC.CodeFlix.Catalog.Application.UseCases.Genre.UpdateGenre;

namespace FC.CodeFlix.Catalog.IntegrationTests.Application.UseCases.Genre.UpdateGenre
{
    [Collection(nameof(UpdateGenreTestFixture))]
    public class UpdateGenreTest
    {
        private readonly UpdateGenreTestFixture _fixture;

        public UpdateGenreTest(UpdateGenreTestFixture fixture) => this._fixture = fixture;

        [Fact(DisplayName = nameof(UpdateGenre))]
        [Trait("Integration/Application", "UpdateGenre - Use Cases")]
        public async Task UpdateGenre()
        {
            var exampleGenres = this._fixture.GetExampleListGenres(10);
            var targetGenre = exampleGenres[5];
            var arrangeDbContext = this._fixture.CreateDbContext();
            await arrangeDbContext.AddRangeAsync(exampleGenres);
            await arrangeDbContext.SaveChangesAsync();
            var actDbContext = this._fixture.CreateDbContext(true);
            var updateGenre = new UseCase.UpdateGenre(new GenreRepository(actDbContext), new UnitOfWork(actDbContext), new CategoryRepository(actDbContext));
            var input = new UpdateGenreInput(targetGenre.Id, this._fixture.GetValidGenreName(), !targetGenre.IsActive);

            var output = await updateGenre.Handle(input, CancellationToken.None);

            var assertDbContext = this._fixture.CreateDbContext(true);
            var assertGenreFromDb = await assertDbContext.Genres.FindAsync(targetGenre.Id);
            output.Should().NotBeNull();
            output.Id.Should().Be(targetGenre.Id);
            output.Name.Should().Be(input.Name);
            output.IsActive.Should().Be(input.IsActive!.Value);
            assertGenreFromDb.Should().NotBeNull();
            assertGenreFromDb!.Id.Should().Be(targetGenre.Id);
            assertGenreFromDb.Name.Should().Be(input.Name);
            assertGenreFromDb.IsActive.Should().Be(input.IsActive!.Value);
        }
        
        [Fact(DisplayName = nameof(UpdateGenreWithCategoriesRelations))]
        [Trait("Integration/Application", "UpdateGenre - Use Cases")]
        public async Task UpdateGenreWithCategoriesRelations()
        {
            var exampleCategories = this._fixture.GetExampleCategoryList(10);
            var exampleGenres = this._fixture.GetExampleListGenres(10);
            var targetGenre = exampleGenres[5];
            var relatedCategories = exampleCategories.GetRange(0, 5);
            var newRelatedCategories = exampleCategories.GetRange(5, 3);
            relatedCategories.ForEach(category => targetGenre.AddCategory(category.Id));
            var relations = targetGenre.Categories.Select(categoryId => new GenresCategories(targetGenre.Id, categoryId)).ToList();
            var arrangeDbContext = this._fixture.CreateDbContext();
            await arrangeDbContext.AddRangeAsync(exampleGenres);
            await arrangeDbContext.AddRangeAsync(exampleCategories);
            await arrangeDbContext.AddRangeAsync(relations);
            await arrangeDbContext.SaveChangesAsync();
            var actDbContext = this._fixture.CreateDbContext(true);
            var updateGenre = new UseCase.UpdateGenre(new GenreRepository(actDbContext), new UnitOfWork(actDbContext), new CategoryRepository(actDbContext));
            var input = new UpdateGenreInput(targetGenre.Id, this._fixture.GetValidGenreName(), !targetGenre.IsActive, 
                newRelatedCategories.Select(category => category.Id).ToList());

            var output = await updateGenre.Handle(input, CancellationToken.None);

            var assertDbContext = this._fixture.CreateDbContext(true);
            var assertGenreFromDb = await assertDbContext.Genres.FindAsync(targetGenre.Id);
            var relatedCategoriesIdsfromOutput = output.Categories.Select(relatedCategory => relatedCategory.Id).ToList();
            var relatedCategoriesIdsfromDb = await assertDbContext.GenresCategories.AsNoTracking().Where(relation => relation.GenreId.Equals(input.Id))
                .Select(relation => relation.CategoryId).ToListAsync();
            output.Should().NotBeNull();
            output.Id.Should().Be(targetGenre.Id);
            output.Name.Should().Be(input.Name);
            output.IsActive.Should().Be(input.IsActive!.Value);
            output.Categories.Should().HaveCount(newRelatedCategories.Count);
            relatedCategoriesIdsfromOutput.Should().BeEquivalentTo(input.CategoriesIds);
            assertGenreFromDb.Should().NotBeNull();
            assertGenreFromDb!.Id.Should().Be(targetGenre.Id);
            assertGenreFromDb.Name.Should().Be(input.Name);
            assertGenreFromDb.IsActive.Should().Be(input.IsActive!.Value);
            relatedCategoriesIdsfromDb.Should().BeEquivalentTo(input.CategoriesIds);
        }

        [Fact(DisplayName = nameof(UpdateGenreThrowsWhenCategoriesDoesntExists))]
        [Trait("Integration/Application", "UpdateGenre - Use Cases")]
        public async Task UpdateGenreThrowsWhenCategoriesDoesntExists()
        {
            var exampleCategories = this._fixture.GetExampleCategoryList(10);
            var exampleGenres = this._fixture.GetExampleListGenres(10);
            var targetGenre = exampleGenres[5];
            var relatedCategories = exampleCategories.GetRange(0, 5);
            var newRelatedCategories = exampleCategories.GetRange(5, 3);
            relatedCategories.ForEach(category => targetGenre.AddCategory(category.Id));
            var relations = targetGenre.Categories.Select(categoryId => new GenresCategories(targetGenre.Id, categoryId)).ToList();
            var arrangeDbContext = this._fixture.CreateDbContext();
            await arrangeDbContext.AddRangeAsync(exampleGenres);
            await arrangeDbContext.AddRangeAsync(exampleCategories);
            await arrangeDbContext.AddRangeAsync(relations);
            await arrangeDbContext.SaveChangesAsync();
            var actDbContext = this._fixture.CreateDbContext(true);
            var updateGenre = new UseCase.UpdateGenre(new GenreRepository(actDbContext), new UnitOfWork(actDbContext), new CategoryRepository(actDbContext));
            var categoryIdsToRelate = newRelatedCategories.Select(category => category.Id).ToList();
            var invalidCategoryId = Guid.NewGuid();
            categoryIdsToRelate.Add(invalidCategoryId);
            var input = new UpdateGenreInput(targetGenre.Id, this._fixture.GetValidGenreName(), !targetGenre.IsActive, categoryIdsToRelate);

            var action = async () => await updateGenre.Handle(input, CancellationToken.None);

            await action.Should().ThrowAsync<RelatedAggregateException>().WithMessage($"Related category id (or ids) not found: {invalidCategoryId}");
        }

        [Fact(DisplayName = nameof(UpdateGenreTrhowsWhenNotFound))]
        [Trait("Integration/Application", "UpdateGenre - Use Cases")]
        public async Task UpdateGenreTrhowsWhenNotFound()
        {
            var exampleGenres = this._fixture.GetExampleListGenres(10);
            var arrangeDbContext = this._fixture.CreateDbContext();
            await arrangeDbContext.AddRangeAsync(exampleGenres);
            await arrangeDbContext.SaveChangesAsync();
            var actDbContext = this._fixture.CreateDbContext(true);
            var updateGenre = new UseCase.UpdateGenre(new GenreRepository(actDbContext), new UnitOfWork(actDbContext), new CategoryRepository(actDbContext));
            var randomGuid = Guid.NewGuid();
            var input = new UpdateGenreInput(randomGuid, this._fixture.GetValidGenreName(), true);

            var action = async () => await updateGenre.Handle(input, CancellationToken.None);

            await action.Should().ThrowAsync<NotFoundException>().WithMessage($"Genre '{randomGuid}' not found.");
        }

        [Fact(DisplayName = nameof(UpdateGenreWithoutNewCategoriesRelations))]
        [Trait("Integration/Application", "UpdateGenre - Use Cases")]
        public async Task UpdateGenreWithoutNewCategoriesRelations()
        {
            var exampleCategories = this._fixture.GetExampleCategoryList(10);
            var exampleGenres = this._fixture.GetExampleListGenres(10);
            var targetGenre = exampleGenres[5];
            var relatedCategories = exampleCategories.GetRange(0, 5);
            relatedCategories.ForEach(category => targetGenre.AddCategory(category.Id));
            var relations = targetGenre.Categories.Select(categoryId => new GenresCategories(targetGenre.Id, categoryId)).ToList();
            var arrangeDbContext = this._fixture.CreateDbContext();
            await arrangeDbContext.AddRangeAsync(exampleGenres);
            await arrangeDbContext.AddRangeAsync(exampleCategories);
            await arrangeDbContext.AddRangeAsync(relations);
            await arrangeDbContext.SaveChangesAsync();
            var actDbContext = this._fixture.CreateDbContext(true);
            var updateGenre = new UseCase.UpdateGenre(new GenreRepository(actDbContext), new UnitOfWork(actDbContext), new CategoryRepository(actDbContext));
            var input = new UpdateGenreInput(targetGenre.Id, this._fixture.GetValidGenreName(), !targetGenre.IsActive);

            var output = await updateGenre.Handle(input, CancellationToken.None);

            var assertDbContext = this._fixture.CreateDbContext(true);
            var assertGenreFromDb = await assertDbContext.Genres.FindAsync(targetGenre.Id);
            var relatedCategoriesIdsfromOutput = output.Categories.Select(relatedCategory => relatedCategory.Id).ToList();
            var relatedCategoriesIdsfromDb = await assertDbContext.GenresCategories.AsNoTracking().Where(relation => relation.GenreId.Equals(input.Id))
                .Select(relation => relation.CategoryId).ToListAsync();
            var expectedRelatedCategory = relatedCategories.Select(category => category.Id).ToList();
            output.Should().NotBeNull();
            output.Id.Should().Be(targetGenre.Id);
            output.Name.Should().Be(input.Name);
            output.IsActive.Should().Be(input.IsActive!.Value);
            output.Categories.Should().HaveCount(relatedCategories.Count);
            relatedCategoriesIdsfromOutput.Should().BeEquivalentTo(expectedRelatedCategory);
            assertGenreFromDb.Should().NotBeNull();
            assertGenreFromDb!.Id.Should().Be(targetGenre.Id);
            assertGenreFromDb.Name.Should().Be(input.Name);
            assertGenreFromDb.IsActive.Should().Be(input.IsActive!.Value);
            relatedCategoriesIdsfromDb.Should().BeEquivalentTo(expectedRelatedCategory);
        }

        [Fact(DisplayName = nameof(UpdateGenreWithEmptyCategoryIdsCleanRelations))]
        [Trait("Integration/Application", "UpdateGenre - Use Cases")]
        public async Task UpdateGenreWithEmptyCategoryIdsCleanRelations()
        {
            var exampleCategories = this._fixture.GetExampleCategoryList(10);
            var exampleGenres = this._fixture.GetExampleListGenres(10);
            var targetGenre = exampleGenres[5];
            var relatedCategories = exampleCategories.GetRange(0, 5);
            relatedCategories.ForEach(category => targetGenre.AddCategory(category.Id));
            var relations = targetGenre.Categories.Select(categoryId => new GenresCategories(targetGenre.Id, categoryId)).ToList();
            var arrangeDbContext = this._fixture.CreateDbContext();
            await arrangeDbContext.AddRangeAsync(exampleGenres);
            await arrangeDbContext.AddRangeAsync(exampleCategories);
            await arrangeDbContext.AddRangeAsync(relations);
            await arrangeDbContext.SaveChangesAsync();
            var actDbContext = this._fixture.CreateDbContext(true);
            var updateGenre = new UseCase.UpdateGenre(new GenreRepository(actDbContext), new UnitOfWork(actDbContext), new CategoryRepository(actDbContext));
            var input = new UpdateGenreInput(targetGenre.Id, this._fixture.GetValidGenreName(), !targetGenre.IsActive, new List<Guid>());

            var output = await updateGenre.Handle(input, CancellationToken.None);

            var assertDbContext = this._fixture.CreateDbContext(true);
            var assertGenreFromDb = await assertDbContext.Genres.FindAsync(targetGenre.Id);
            var relatedCategoriesIdsfromOutput = output.Categories.Select(relatedCategory => relatedCategory.Id).ToList();
            var relatedCategoriesIdsfromDb = await assertDbContext.GenresCategories.AsNoTracking().Where(relation => relation.GenreId.Equals(input.Id))
                .Select(relation => relation.CategoryId).ToListAsync();
            output.Should().NotBeNull();
            output.Id.Should().Be(targetGenre.Id);
            output.Name.Should().Be(input.Name);
            output.IsActive.Should().Be(input.IsActive!.Value);
            output.Categories.Should().HaveCount(0);
            relatedCategoriesIdsfromOutput.Should().BeEquivalentTo(new List<Guid>());
            assertGenreFromDb.Should().NotBeNull();
            assertGenreFromDb!.Id.Should().Be(targetGenre.Id);
            assertGenreFromDb.Name.Should().Be(input.Name);
            assertGenreFromDb.IsActive.Should().Be(input.IsActive!.Value);
            relatedCategoriesIdsfromDb.Should().BeEquivalentTo(new List<Guid>());
        }   
    }
}
