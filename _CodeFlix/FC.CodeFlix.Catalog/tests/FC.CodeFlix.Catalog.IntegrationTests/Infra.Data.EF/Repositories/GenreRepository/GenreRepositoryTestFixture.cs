using FC.CodeFlix.Catalog.Domain.Entity;
using FC.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.CodeFlix.Catalog.IntegrationTests.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace FC.CodeFlix.Catalog.IntegrationTests.Infra.Data.EF.Repositories.GenreRepository
{
    [CollectionDefinition(nameof(GenreRepositoryTestFixture))]
    public class GenreRepositoryTestFixtureCollection : ICollectionFixture<GenreRepositoryTestFixture> { }

    public class GenreRepositoryTestFixture: BaseFixture
    {
        public string GetValidGenreName() => this.Faker.Commerce.Categories(1)[0];

        public bool GetRandomBoolean() => new Random().NextDouble() < 0.5;

        public Genre GetExampleGenre(bool? isActive = null, List<Guid>? categoriesIds = null, string name = null)
        {
            var genre = new Genre(name ?? this.GetValidGenreName(), isActive ?? this.GetRandomBoolean());
            categoriesIds?.ForEach(genre.AddCategory);

            return genre;
        }

        public List<Genre> GetExampleListGenresByNames(List<string> names) => names.Select(name => this.GetExampleGenre(name: name)).ToList();
        public List<Genre> GetExampleListGenres(int count = 10) => Enumerable.Range(1, count).Select(_ => this.GetExampleGenre()).ToList();

        public string GetValidCategoryName()
        {
            var categoryName = string.Empty;
            while (categoryName.Length < 3 || categoryName.Length > 255)
                categoryName = Faker.Commerce.Categories(1)[0];

            return Faker.Commerce.Categories(1)[0];
        }

        public string GetValidCategoryDescription()
        {
            var categoryDescription = Faker.Commerce.ProductDescription();
            if (categoryDescription.Length > 10_000)
                categoryDescription = categoryDescription[..10_000];

            return categoryDescription;
        }

        public Category GetExampleCategory() => new(GetValidCategoryName(), GetValidCategoryDescription(), GetRandomBoolean());

        public List<Category> GetExampleCategoryList(int length = 10) => Enumerable.Range(1, length).Select(_ => this.GetExampleCategory()).ToList();

        public List<Genre> CloneGenresListOrdered(List<Genre> genreList, string orderBy, SearchOrder order)
        {
            var listClone = new List<Genre>(genreList);
            var orderedEnumerable = (orderBy.ToLower(), order) switch
            {
                ("name", SearchOrder.Asc) => listClone.OrderBy(p => p.Name).ThenBy(p => p.Id),
                ("name", SearchOrder.Desc) => listClone.OrderByDescending(p => p.Name).ThenByDescending(p => p.Id),
                ("id", SearchOrder.Asc) => listClone.OrderBy(p => p.Id),
                ("id", SearchOrder.Desc) => listClone.OrderByDescending(p => p.Id),
                ("createdat", SearchOrder.Asc) => listClone.OrderBy(p => p.CreatedAt),
                ("createdat", SearchOrder.Desc) => listClone.OrderByDescending(p => p.CreatedAt),
                _ => listClone.OrderBy(p => p.Name).ThenBy(p => p.Id)
            };

            return orderedEnumerable.ThenBy(p => p.Id).ToList();
        }
    }
}
