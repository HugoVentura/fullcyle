using FC.CodeFlix.Catalog.Application.Exceptions;
using FC.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.CodeFlix.Catalog.Infra.Data.EF;
using FC.CodeFlix.Catalog.Infra.Data.EF.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Repository = FC.CodeFlix.Catalog.Infra.Data.EF.Repositories;

namespace FC.CodeFlix.Catalog.IntegrationTests.Infra.Data.EF.Repositories.GenreRepository
{
    [Collection(nameof(GenreRepositoryTestFixture))]
    public class GenreRepositoryTest
    {
        private readonly GenreRepositoryTestFixture _fixture;

        public GenreRepositoryTest(GenreRepositoryTestFixture fixture) => this._fixture = fixture;

        [Fact(DisplayName = nameof(Insert))]
        [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
        public async Task Insert()
        {
            CodeFlixCatalogDbContext dbContext = this._fixture.CreateDbContext();
            var exampleGenre = this._fixture.GetExampleGenre();
            var categoriesListExample = this._fixture.GetExampleCategoryList(3);
            categoriesListExample.ForEach(category => exampleGenre.AddCategory(category.Id));
            await dbContext.Categories.AddRangeAsync(categoriesListExample);
            var genreRepository = new Repository.GenreRepository(dbContext);
            await dbContext.SaveChangesAsync(CancellationToken.None);

            await genreRepository.Insert(exampleGenre, CancellationToken.None);
            await dbContext.SaveChangesAsync(CancellationToken.None);

            var assertsDbContext = this._fixture.CreateDbContext(true);
            var dbGenre = await assertsDbContext.Genres.FindAsync(exampleGenre.Id);
            dbGenre.Should().NotBeNull();
            dbGenre!.Name.Should().Be(exampleGenre.Name);
            dbGenre.IsActive.Should().Be(exampleGenre.IsActive);
            dbGenre.CreatedAt.Should().Be(exampleGenre.CreatedAt);
            var genreCategoriesRelations = await assertsDbContext.GenresCategories.Where(p => p.GenreId.Equals(exampleGenre.Id)).ToListAsync();
            genreCategoriesRelations.Should().HaveCount(categoriesListExample.Count);
            genreCategoriesRelations.ForEach(relation =>
            {
                var expectedCategory = categoriesListExample.FirstOrDefault(p => p.Id.Equals(relation.CategoryId));

                expectedCategory.Should().NotBeNull();
            });
        }

        [Fact(DisplayName = nameof(Get))]
        [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
        public async Task Get()
        {
            CodeFlixCatalogDbContext dbContext = this._fixture.CreateDbContext();
            var exampleGenre = this._fixture.GetExampleGenre();
            var categoriesListExample = this._fixture.GetExampleCategoryList(3);
            categoriesListExample.ForEach(category => exampleGenre.AddCategory(category.Id));
            await dbContext.Categories.AddRangeAsync(categoriesListExample);
            await dbContext.Genres.AddAsync(exampleGenre);
            foreach (var categoryId in exampleGenre.Categories)
            {
                var relation = new GenresCategories(exampleGenre.Id, categoryId);
                await dbContext.GenresCategories.AddAsync(relation);
            }
            await dbContext.SaveChangesAsync(CancellationToken.None);
            var genreRepository = new Repository.GenreRepository(dbContext);

            var genreFromRepository = await genreRepository.Get(exampleGenre.Id, CancellationToken.None);

            genreFromRepository.Should().NotBeNull();
            genreFromRepository!.Name.Should().Be(exampleGenre.Name);
            genreFromRepository.IsActive.Should().Be(exampleGenre.IsActive);
            genreFromRepository.CreatedAt.Should().Be(exampleGenre.CreatedAt);
            genreFromRepository.Categories.Should().HaveCount(categoriesListExample.Count);
            foreach (var categoryId in genreFromRepository.Categories)
            {
                var expectedCategory = categoriesListExample.FirstOrDefault(p => p.Id.Equals(categoryId));

                expectedCategory.Should().NotBeNull();
            };
        }

        [Fact(DisplayName = nameof(GetThrowWhenNotFound))]
        [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
        public async Task GetThrowWhenNotFound()
        {
            var exampleNotFoundGuid = Guid.NewGuid();
            CodeFlixCatalogDbContext dbContext = this._fixture.CreateDbContext();
            var exampleGenre = this._fixture.GetExampleGenre();
            var categoriesListExample = this._fixture.GetExampleCategoryList(3);
            categoriesListExample.ForEach(category => exampleGenre.AddCategory(category.Id));
            await dbContext.Categories.AddRangeAsync(categoriesListExample);
            await dbContext.Genres.AddAsync(exampleGenre);
            foreach (var categoryId in exampleGenre.Categories)
            {
                var relation = new GenresCategories(exampleGenre.Id, categoryId);
                await dbContext.GenresCategories.AddAsync(relation);
            }
            await dbContext.SaveChangesAsync(CancellationToken.None);
            var genreRepository = new Repository.GenreRepository(dbContext);

            var action = async() => await genreRepository.Get(exampleNotFoundGuid, CancellationToken.None);

            await action.Should().ThrowAsync<NotFoundException>()
                .WithMessage($"Genre '{exampleNotFoundGuid}' not found.");
        }

        [Fact(DisplayName = nameof(Delete))]
        [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
        public async Task Delete()
        {
            CodeFlixCatalogDbContext dbContext = this._fixture.CreateDbContext();
            var exampleGenre = this._fixture.GetExampleGenre();
            var categoriesListExample = this._fixture.GetExampleCategoryList(3);
            categoriesListExample.ForEach(category => exampleGenre.AddCategory(category.Id));
            await dbContext.Categories.AddRangeAsync(categoriesListExample);
            await dbContext.Genres.AddAsync(exampleGenre);
            foreach (var categoryId in exampleGenre.Categories)
            {
                var relation = new GenresCategories(exampleGenre.Id, categoryId);
                await dbContext.GenresCategories.AddAsync(relation);
            }
            await dbContext.SaveChangesAsync(CancellationToken.None);
            var repositoryDbContext = this._fixture.CreateDbContext(true);
            var genreRepository = new Repository.GenreRepository(repositoryDbContext);

            await genreRepository.Delete(exampleGenre, CancellationToken.None);
            await repositoryDbContext.SaveChangesAsync();

            var assertsDbContext = this._fixture.CreateDbContext(true);
            var dbGenre = await assertsDbContext.Genres.AsNoTracking().FirstOrDefaultAsync(p => p.Id.Equals(exampleGenre.Id));
            var categoriesList = await assertsDbContext.GenresCategories
                .AsNoTracking()
                .Where(p => p.GenreId.Equals(exampleGenre.Id))
                .Select(p => p.CategoryId)
                .ToListAsync();

            dbGenre.Should().BeNull();
            categoriesList.Should().BeEmpty();
        }

        [Fact(DisplayName = nameof(Update))]
        [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
        public async Task Update()
        {
            CodeFlixCatalogDbContext dbContext = this._fixture.CreateDbContext();
            var exampleGenre = this._fixture.GetExampleGenre();
            var categoriesListExample = this._fixture.GetExampleCategoryList(3);
            categoriesListExample.ForEach(category => exampleGenre.AddCategory(category.Id));
            await dbContext.Categories.AddRangeAsync(categoriesListExample);
            await dbContext.Genres.AddAsync(exampleGenre);
            foreach (var categoryId in exampleGenre.Categories)
            {
                var relation = new GenresCategories(exampleGenre.Id, categoryId);
                await dbContext.GenresCategories.AddAsync(relation);
            }
            await dbContext.SaveChangesAsync(CancellationToken.None);

            var actDbContext = this._fixture.CreateDbContext(true);
            var genreRepository = new Repository.GenreRepository(actDbContext);
            exampleGenre.Update(this._fixture.GetValidGenreName());
            if (exampleGenre.IsActive)
                exampleGenre.Deactivate();
            else
                exampleGenre.Activate();
            await genreRepository.Update(exampleGenre, CancellationToken.None);
            await actDbContext.SaveChangesAsync();

            var assertsDbContext = this._fixture.CreateDbContext(true);
            var dbGenre = await assertsDbContext.Genres.FindAsync(exampleGenre.Id);
            dbGenre.Should().NotBeNull();
            dbGenre!.Name.Should().Be(exampleGenre.Name);
            dbGenre.IsActive.Should().Be(exampleGenre.IsActive);
            dbGenre.CreatedAt.Should().Be(exampleGenre.CreatedAt);
            var genreCategoriesRelations = await assertsDbContext.GenresCategories.Where(p => p.GenreId.Equals(exampleGenre.Id)).ToListAsync();
            genreCategoriesRelations.Should().HaveCount(categoriesListExample.Count);
            genreCategoriesRelations.ForEach(relation =>
            {
                var expectedCategory = categoriesListExample.FirstOrDefault(p => p.Id.Equals(relation.CategoryId));

                expectedCategory.Should().NotBeNull();
            });
        }

        [Fact(DisplayName = nameof(UpdateRemovingRelations))]
        [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
        public async Task UpdateRemovingRelations()
        {
            CodeFlixCatalogDbContext dbContext = this._fixture.CreateDbContext();
            var exampleGenre = this._fixture.GetExampleGenre();
            var categoriesListExample = this._fixture.GetExampleCategoryList(3);
            categoriesListExample.ForEach(category => exampleGenre.AddCategory(category.Id));
            await dbContext.Categories.AddRangeAsync(categoriesListExample);
            await dbContext.Genres.AddAsync(exampleGenre);
            foreach (var categoryId in exampleGenre.Categories)
            {
                var relation = new GenresCategories(exampleGenre.Id, categoryId);
                await dbContext.GenresCategories.AddAsync(relation);
            }
            await dbContext.SaveChangesAsync(CancellationToken.None);

            var actDbContext = this._fixture.CreateDbContext(true);
            var genreRepository = new Repository.GenreRepository(actDbContext);
            exampleGenre.Update(this._fixture.GetValidGenreName());
            if (exampleGenre.IsActive)
                exampleGenre.Deactivate();
            else
                exampleGenre.Activate();
            exampleGenre.RemoveAllCategory();
            await genreRepository.Update(exampleGenre, CancellationToken.None);
            await actDbContext.SaveChangesAsync();

            var assertsDbContext = this._fixture.CreateDbContext(true);
            var dbGenre = await assertsDbContext.Genres.FindAsync(exampleGenre.Id);
            dbGenre.Should().NotBeNull();
            dbGenre!.Name.Should().Be(exampleGenre.Name);
            dbGenre.IsActive.Should().Be(exampleGenre.IsActive);
            dbGenre.CreatedAt.Should().Be(exampleGenre.CreatedAt);
            var genreCategoriesRelations = await assertsDbContext.GenresCategories.Where(p => p.GenreId.Equals(exampleGenre.Id)).ToListAsync();
            genreCategoriesRelations.Should().BeEmpty();
        }

        [Fact(DisplayName = nameof(UpdateReplacingRelations))]
        [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
        public async Task UpdateReplacingRelations()
        {
            CodeFlixCatalogDbContext dbContext = this._fixture.CreateDbContext();
            var exampleGenre = this._fixture.GetExampleGenre();
            var categoriesListExample = this._fixture.GetExampleCategoryList(3);
            categoriesListExample.ForEach(category => exampleGenre.AddCategory(category.Id));
            var updateCategoriesListExample = this._fixture.GetExampleCategoryList(2);
            await dbContext.Categories.AddRangeAsync(categoriesListExample);
            await dbContext.Categories.AddRangeAsync(updateCategoriesListExample);
            await dbContext.Genres.AddAsync(exampleGenre);
            foreach (var categoryId in exampleGenre.Categories)
            {
                var relation = new GenresCategories(exampleGenre.Id, categoryId);
                await dbContext.GenresCategories.AddAsync(relation);
            }
            await dbContext.SaveChangesAsync(CancellationToken.None);

            var actDbContext = this._fixture.CreateDbContext(true);
            var genreRepository = new Repository.GenreRepository(actDbContext);
            exampleGenre.Update(this._fixture.GetValidGenreName());
            if (exampleGenre.IsActive)
                exampleGenre.Deactivate();
            else
                exampleGenre.Activate();
            exampleGenre.RemoveAllCategory();
            updateCategoriesListExample.ForEach(category => exampleGenre.AddCategory(category.Id));
            await genreRepository.Update(exampleGenre, CancellationToken.None);
            await actDbContext.SaveChangesAsync();

            var assertsDbContext = this._fixture.CreateDbContext(true);
            var dbGenre = await assertsDbContext.Genres.FindAsync(exampleGenre.Id);
            dbGenre.Should().NotBeNull();
            dbGenre!.Name.Should().Be(exampleGenre.Name);
            dbGenre.IsActive.Should().Be(exampleGenre.IsActive);
            dbGenre.CreatedAt.Should().Be(exampleGenre.CreatedAt);
            var genreCategoriesRelations = await assertsDbContext.GenresCategories.Where(p => p.GenreId.Equals(exampleGenre.Id)).ToListAsync();
            genreCategoriesRelations.Should().HaveCount(updateCategoriesListExample.Count);
            genreCategoriesRelations.ForEach(relation =>
            {
                var expectedCategory = updateCategoriesListExample.FirstOrDefault(p => p.Id.Equals(relation.CategoryId));

                expectedCategory.Should().NotBeNull();
            });
        }

        [Fact(DisplayName = nameof(SearchReturnsItemsAndTotal))]
        [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
        public async Task SearchReturnsItemsAndTotal()
        {
            CodeFlixCatalogDbContext dbContext = this._fixture.CreateDbContext();
            var exampleGenresList = this._fixture.GetExampleListGenres(10);
            await dbContext.Genres.AddRangeAsync(exampleGenresList);
            await dbContext.SaveChangesAsync(CancellationToken.None);
            var actDbContext = this._fixture.CreateDbContext(true);
            var genreRepository = new Repository.GenreRepository(actDbContext);
            var searchInput = new SearchInput(1, 20, string.Empty, string.Empty, SearchOrder.Asc);

            var searchResult = await genreRepository.Search(searchInput, CancellationToken.None);

            searchResult.Should().NotBeNull();
            searchResult.CurrentPage.Should().Be(searchInput.Page);
            searchResult.PerPage.Should().Be(searchInput.PerPage);
            searchResult.Total.Should().Be(exampleGenresList.Count);
            searchResult.Items.Should().HaveCount(exampleGenresList.Count);
            foreach (var resultItem in searchResult.Items)
            {
                var exampleGenre = exampleGenresList.FirstOrDefault(p => p.Id.Equals(resultItem.Id));

                exampleGenre.Should().NotBeNull();
                resultItem.Name.Should().Be(exampleGenre!.Name);
                resultItem.IsActive.Should().Be(exampleGenre.IsActive);
                resultItem.CreatedAt.Should().Be(exampleGenre.CreatedAt);
            }
        }

        [Fact(DisplayName = nameof(SearchReturnsRelations))]
        [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
        public async Task SearchReturnsRelations()
        {
            CodeFlixCatalogDbContext dbContext = this._fixture.CreateDbContext();
            var exampleGenresList = this._fixture.GetExampleListGenres(10);
            await dbContext.Genres.AddRangeAsync(exampleGenresList);
            var random = new Random();
            exampleGenresList.ForEach(exampleGenre =>
            {
                var categoriesListToRelation = this._fixture.GetExampleCategoryList(random.Next(0, 4));
                if (categoriesListToRelation.Count > 0)
                {
                    categoriesListToRelation.ForEach(category => exampleGenre.AddCategory(category.Id));
                    dbContext.Categories.AddRange(categoriesListToRelation);
                    var relationsToAdd = categoriesListToRelation.Select(category => new GenresCategories(exampleGenre.Id, category.Id)).ToList();
                    dbContext.GenresCategories.AddRange(relationsToAdd);
                }
            });
            await dbContext.SaveChangesAsync(CancellationToken.None);
            var actDbContext = this._fixture.CreateDbContext(true);
            var genreRepository = new Repository.GenreRepository(actDbContext);
            var searchInput = new SearchInput(1, 20, string.Empty, string.Empty, SearchOrder.Asc);

            var searchResult = await genreRepository.Search(searchInput, CancellationToken.None);

            searchResult.Should().NotBeNull();
            searchResult.CurrentPage.Should().Be(searchInput.Page);
            searchResult.PerPage.Should().Be(searchInput.PerPage);
            searchResult.Total.Should().Be(exampleGenresList.Count);
            searchResult.Items.Should().HaveCount(exampleGenresList.Count);
            foreach (var resultItem in searchResult.Items)
            {
                var exampleGenre = exampleGenresList.FirstOrDefault(p => p.Id.Equals(resultItem.Id));

                exampleGenre.Should().NotBeNull();
                resultItem.Name.Should().Be(exampleGenre!.Name);
                resultItem.IsActive.Should().Be(exampleGenre.IsActive);
                resultItem.CreatedAt.Should().Be(exampleGenre.CreatedAt);
                resultItem.Categories.Should().HaveCount(exampleGenre.Categories.Count);
                resultItem.Categories.Should().BeEquivalentTo(exampleGenre.Categories);
            }
        }

        [Fact(DisplayName = nameof(SearchReturnsEmptyWhenPersistenceIsEmpty))]
        [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
        public async Task SearchReturnsEmptyWhenPersistenceIsEmpty()
        {
            var actDbContext = this._fixture.CreateDbContext();
            var genreRepository = new Repository.GenreRepository(actDbContext);
            var searchInput = new SearchInput(1, 20, string.Empty, string.Empty, SearchOrder.Asc);

            var searchResult = await genreRepository.Search(searchInput, CancellationToken.None);

            searchResult.Should().NotBeNull();
            searchResult.CurrentPage.Should().Be(searchInput.Page);
            searchResult.PerPage.Should().Be(searchInput.PerPage);
            searchResult.Total.Should().Be(0);
            searchResult.Items.Should().BeEmpty();
        }

        [Theory(DisplayName = nameof(SearchReturnsPaginated))]
        [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
        [InlineData(10, 1, 5, 5)]
        [InlineData(10, 2, 5, 5)]
        [InlineData(7, 2, 5, 2)]
        [InlineData(7, 3, 5, 0)]
        public async Task SearchReturnsPaginated(int quantityToGenerate, int page, int perPage, int expectedQuantityItems)
        {
            CodeFlixCatalogDbContext dbContext = this._fixture.CreateDbContext();
            var exampleGenresList = this._fixture.GetExampleListGenres(quantityToGenerate);
            await dbContext.Genres.AddRangeAsync(exampleGenresList);
            var random = new Random();
            exampleGenresList.ForEach(exampleGenre =>
            {
                var categoriesListToRelation = this._fixture.GetExampleCategoryList(random.Next(0, 4));
                if (categoriesListToRelation.Count > 0)
                {
                    categoriesListToRelation.ForEach(category => exampleGenre.AddCategory(category.Id));
                    dbContext.Categories.AddRange(categoriesListToRelation);
                    var relationsToAdd = categoriesListToRelation.Select(category => new GenresCategories(exampleGenre.Id, category.Id)).ToList();
                    dbContext.GenresCategories.AddRange(relationsToAdd);
                }
            });
            await dbContext.SaveChangesAsync(CancellationToken.None);
            var actDbContext = this._fixture.CreateDbContext(true);
            var genreRepository = new Repository.GenreRepository(actDbContext);
            var searchInput = new SearchInput(page, perPage, string.Empty, string.Empty, SearchOrder.Asc);

            var searchResult = await genreRepository.Search(searchInput, CancellationToken.None);

            searchResult.Should().NotBeNull();
            searchResult.CurrentPage.Should().Be(searchInput.Page);
            searchResult.PerPage.Should().Be(searchInput.PerPage);
            searchResult.Total.Should().Be(exampleGenresList.Count);
            searchResult.Items.Should().HaveCount(expectedQuantityItems);
            foreach (var resultItem in searchResult.Items)
            {
                var exampleGenre = exampleGenresList.FirstOrDefault(p => p.Id.Equals(resultItem.Id));

                exampleGenre.Should().NotBeNull();
                resultItem.Name.Should().Be(exampleGenre!.Name);
                resultItem.IsActive.Should().Be(exampleGenre.IsActive);
                resultItem.CreatedAt.Should().Be(exampleGenre.CreatedAt);
                resultItem.Categories.Should().HaveCount(exampleGenre.Categories.Count);
                resultItem.Categories.Should().BeEquivalentTo(exampleGenre.Categories);
            }
        }

        [Theory(DisplayName = nameof(SearchByText))]
        [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
        [InlineData("Action", 1, 5, 1, 1)]
        [InlineData("Horror", 1, 5, 3, 3)]
        [InlineData("Horror", 2, 5, 0, 3)]
        [InlineData("Sci-fi", 1, 5, 4, 4)]
        [InlineData("Sci-fi", 1, 2, 2, 4)]
        [InlineData("Sci-fi", 2, 3, 1, 4)]
        [InlineData("Sci-fi Other", 1, 3, 0, 0)]
        [InlineData("Robots", 1, 5, 2, 2)]
        public async Task SearchByText(string search, int page, int perPage, int expectedQuantityItemsReturned, int expectedQuantityTotalItems)
        {
            CodeFlixCatalogDbContext dbContext = this._fixture.CreateDbContext();
            var exampleGenresList = this._fixture.GetExampleListGenresByNames(new List<string>()
                { "Action", "Horror", "Horror - Robots", "Horror - Based on Reals Facts", "Drama", "Sci-fi IA", "Sci-fi Space", "Sci-fi Robots", "Sci-fi Future" });
            await dbContext.Genres.AddRangeAsync(exampleGenresList);
            var random = new Random();
            exampleGenresList.ForEach(exampleGenre =>
            {
                var categoriesListToRelation = this._fixture.GetExampleCategoryList(random.Next(0, 4));
                if (categoriesListToRelation.Count > 0)
                {
                    categoriesListToRelation.ForEach(category => exampleGenre.AddCategory(category.Id));
                    dbContext.Categories.AddRange(categoriesListToRelation);
                    var relationsToAdd = categoriesListToRelation.Select(category => new GenresCategories(exampleGenre.Id, category.Id)).ToList();
                    dbContext.GenresCategories.AddRange(relationsToAdd);
                }
            });
            await dbContext.SaveChangesAsync(CancellationToken.None);
            var actDbContext = this._fixture.CreateDbContext(true);
            var genreRepository = new Repository.GenreRepository(actDbContext);
            var searchInput = new SearchInput(page, perPage, search, string.Empty, SearchOrder.Asc);

            var searchResult = await genreRepository.Search(searchInput, CancellationToken.None);

            searchResult.Should().NotBeNull();
            searchResult.CurrentPage.Should().Be(searchInput.Page);
            searchResult.PerPage.Should().Be(searchInput.PerPage);
            searchResult.Total.Should().Be(expectedQuantityTotalItems);
            searchResult.Items.Should().HaveCount(expectedQuantityItemsReturned);
            foreach (var resultItem in searchResult.Items)
            {
                var exampleGenre = exampleGenresList.FirstOrDefault(p => p.Id.Equals(resultItem.Id));

                exampleGenre.Should().NotBeNull();
                resultItem.Name.Should().Be(exampleGenre!.Name);
                resultItem.IsActive.Should().Be(exampleGenre.IsActive);
                resultItem.CreatedAt.Should().Be(exampleGenre.CreatedAt);
                resultItem.Categories.Should().HaveCount(exampleGenre.Categories.Count);
                resultItem.Categories.Should().BeEquivalentTo(exampleGenre.Categories);
            }
        }

        [Theory(DisplayName = nameof(SearchOrdered))]
        [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
        [InlineData("name", SearchOrder.Asc)]
        [InlineData("name", SearchOrder.Desc)]
        [InlineData("id", SearchOrder.Asc)]
        [InlineData("id", SearchOrder.Desc)]
        [InlineData("createdat", SearchOrder.Asc)]
        [InlineData("createdat", SearchOrder.Desc)]
        [InlineData("", SearchOrder.Asc)]
        public async Task SearchOrdered(string orderBy, SearchOrder searchOrder)
        {
            CodeFlixCatalogDbContext dbContext = this._fixture.CreateDbContext();
            var exampleGenresList = this._fixture.GetExampleListGenres(10);
            await dbContext.AddRangeAsync(exampleGenresList);
            await dbContext.SaveChangesAsync(CancellationToken.None);
            var genreRepository = new Repository.GenreRepository(dbContext);
            var searchInput = new SearchInput(1, 20, "", orderBy, searchOrder);

            var output = await genreRepository.Search(searchInput, CancellationToken.None);

            var expectedOrderedList = this._fixture.CloneGenresListOrdered(exampleGenresList, orderBy, searchOrder);
            output.Should().NotBeNull();
            output.Items.Should().NotBeNull();
            output.CurrentPage.Should().Be(searchInput.Page);
            output.PerPage.Should().Be(searchInput.PerPage);
            output.Total.Should().Be(exampleGenresList.Count);
            output.Items.Should().HaveCount(exampleGenresList.Count);
            for (var idx = 0; idx < expectedOrderedList.Count; idx++)
            {
                var expectedItem = expectedOrderedList[idx];
                var outputItem = output.Items[idx];

                expectedItem.Should().NotBeNull();
                outputItem.Should().NotBeNull();
                outputItem.Id.Should().Be(expectedItem.Id);
                outputItem.Name.Should().Be(expectedItem.Name);
                outputItem.IsActive.Should().Be(expectedItem.IsActive);
                outputItem.CreatedAt.Should().Be(expectedItem.CreatedAt);
            }
        }
    }
}
