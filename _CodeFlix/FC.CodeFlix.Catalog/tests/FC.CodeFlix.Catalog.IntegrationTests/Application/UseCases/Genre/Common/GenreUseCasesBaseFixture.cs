using FC.CodeFlix.Catalog.IntegrationTests.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using DomainEntity = FC.CodeFlix.Catalog.Domain.Entity;

namespace FC.CodeFlix.Catalog.IntegrationTests.Application.UseCases.Genre.Common
{
    public class GenreUseCasesBaseFixture : BaseFixture
    {
        public string GetValidGenreName() => this.Faker.Commerce.Categories(1)[0];

        public bool GetRandomBoolean() => new Random().NextDouble() < 0.5;

        public DomainEntity.Genre GetExampleGenre(bool? isActive = null, List<Guid>? categoriesIds = null, string name = null)
        {
            var genre = new DomainEntity.Genre(name ?? this.GetValidGenreName(), isActive ?? this.GetRandomBoolean());
            categoriesIds?.ForEach(genre.AddCategory);

            return genre;
        }

        public List<DomainEntity.   Genre> GetExampleListGenres(int count = 10) => Enumerable.Range(1, count).Select(_ => this.GetExampleGenre()).ToList();

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

        public DomainEntity.Category GetExampleCategory() => new(GetValidCategoryName(), GetValidCategoryDescription(), GetRandomBoolean());

        public List<DomainEntity.Category> GetExampleCategoryList(int length = 10) => Enumerable.Range(1, length).Select(_ => this.GetExampleCategory()).ToList();
    }
}
