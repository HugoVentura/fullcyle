using FC.CodeFlix.Catalog.EndToEndTests.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using DomainEntity = FC.CodeFlix.Catalog.Domain.Entity;

namespace FC.CodeFlix.Catalog.EndToEndTests.Api.Category.Common
{
    public class CategoryBaseFixture : BaseFixture
    {
        public CategoryPersistence Persistence;

        public CategoryBaseFixture() : base() => Persistence = new CategoryPersistence(this.CreateDbContext());

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

        public bool GetRandomBoolean() => new Random().NextDouble() < 0.5;

        public string GetInvalidNameTooShort() => this.Faker.Commerce.ProductName()[..2];

        public string GetInvalidNameTooLong()
        {
            var tooLongNameForCategory = Faker.Commerce.ProductName();
            while (tooLongNameForCategory.Length <= 255)
                tooLongNameForCategory = $"{tooLongNameForCategory} {Faker.Commerce.ProductName()}";

            return tooLongNameForCategory;
        }

        public string GetInvalidDescriptionTooLong()
        {
            var tooLongDescriptionForCategory = Faker.Commerce.ProductDescription();
            while (tooLongDescriptionForCategory.Length <= 10_000)
                tooLongDescriptionForCategory = $"{tooLongDescriptionForCategory} {Faker.Commerce.ProductDescription()}";

            return tooLongDescriptionForCategory;
        }

        public DomainEntity.Category GetExampleCategory() => new(this.GetValidCategoryName(), this.GetValidCategoryDescription(), this.GetRandomBoolean());

        public List<DomainEntity.Category> GetExampleCategoriesList(int listLength = 15) => Enumerable.Range(1, listLength).Select(_ =>
            new DomainEntity.Category(this.GetValidCategoryName(), this.GetValidCategoryDescription(), this.GetRandomBoolean())).ToList();
    }
}
