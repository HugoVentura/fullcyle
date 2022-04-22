using FC.CodeFlix.Catalog.UnitTests.Application.Category.Common;
using Xunit;

namespace FC.CodeFlix.Catalog.UnitTests.Application.Category.DeleteCategory
{
    [CollectionDefinition(nameof(DeletecategoryTestFixture))]
    public class DeletecategoryTestFixtureCollection : ICollectionFixture<DeletecategoryTestFixture>
    { }

    public class DeletecategoryTestFixture : CategoryUseCasesBaseFixture
    {
    }
}
