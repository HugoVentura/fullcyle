using FC.CodeFlix.Catalog.Application.Interfaces;
using FC.CodeFlix.Catalog.Domain.Entity;
using FC.CodeFlix.Catalog.Domain.Repository;
using FC.CodeFlix.Catalog.UnitTests.Common;
using Moq;
using Xunit;

namespace FC.CodeFlix.Catalog.UnitTests.Application.DeleteCategory
{
    [CollectionDefinition(nameof(DeletecategoryTestFixture))]
    public class DeletecategoryTestFixtureCollection : ICollectionFixture<DeletecategoryTestFixture>
    { }

    public class DeletecategoryTestFixture : BaseFixture
    {
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

        public Category GetValidCategory() => new(this.GetValidCategoryName(), this.GetValidCategoryDescription());

        public Mock<ICategoryRepository> GetRepositoryMock() => new();

        public Mock<IUnitOfWork> GetUnitOfWorkMock() => new();
    }
}
