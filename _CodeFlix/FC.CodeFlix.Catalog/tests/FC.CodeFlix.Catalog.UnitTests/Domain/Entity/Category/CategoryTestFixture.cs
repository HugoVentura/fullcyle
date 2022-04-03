using FC.CodeFlix.Catalog.UnitTests.Common;
using Xunit;
using DomainEntity = FC.CodeFlix.Catalog.Domain.Entity;

namespace FC.CodeFlix.Catalog.UnitTests.Domain.Entity.Category
{
    public class CategoryTestFixture : BaseFixture
    {
        public CategoryTestFixture() : base() { }

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

        public DomainEntity.Category GetValidCategory() => new(this.GetValidCategoryName(), this.GetValidCategoryDescription());
    }

    [CollectionDefinition(nameof(CategoryTestFixture))]
    public class CategoryTestFixtureCollection : ICollectionFixture<CategoryTestFixture> { }
}
