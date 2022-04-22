using FC.CodeFlix.Catalog.EndToEndTests.Api.Category.Common;
using Xunit;

namespace FC.CodeFlix.Catalog.EndToEndTests.Api.Category.GetCategory
{
    [CollectionDefinition(nameof(GetCategoryApiTestFixture))]
    public class GetCategoryApiTestFixtureCollection : ICollectionFixture<GetCategoryApiTestFixture> { }

    public class GetCategoryApiTestFixture : CategoryBaseFixture
    {
    }
}
