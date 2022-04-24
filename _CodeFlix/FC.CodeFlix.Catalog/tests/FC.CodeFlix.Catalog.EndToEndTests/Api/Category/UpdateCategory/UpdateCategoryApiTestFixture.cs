using FC.CodeFlix.Catalog.Api.ApiModels.Category;
using FC.CodeFlix.Catalog.EndToEndTests.Api.Category.Common;
using Xunit;

namespace FC.CodeFlix.Catalog.EndToEndTests.Api.Category.UpdateCategory
{
    [CollectionDefinition(nameof(UpdateCategoryApiTestFixture))]
    public class UpdateCategoryTestFixtureCollection : ICollectionFixture<UpdateCategoryApiTestFixture> { }

    public class UpdateCategoryApiTestFixture : CategoryBaseFixture
    {
        public UpdateCategoryApiInput GetExampleInput() 
            => new(this.GetValidCategoryName(), this.GetValidCategoryDescription(), this.GetRandomBoolean());
    }
}
