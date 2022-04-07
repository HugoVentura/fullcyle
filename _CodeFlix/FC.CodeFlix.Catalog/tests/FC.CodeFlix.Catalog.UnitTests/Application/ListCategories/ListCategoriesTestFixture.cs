using FC.CodeFlix.Catalog.UnitTests.Common;
using Xunit;

namespace FC.CodeFlix.Catalog.UnitTests.Application.ListCategories
{
    [CollectionDefinition(nameof(ListCategoriesTestFixture))]
    public class ListCategoriesTestFixtureCollection : ICollectionFixture<ListCategoriesTestFixture> { }

    public class ListCategoriesTestFixture : BaseFixture
    {
    }
}
