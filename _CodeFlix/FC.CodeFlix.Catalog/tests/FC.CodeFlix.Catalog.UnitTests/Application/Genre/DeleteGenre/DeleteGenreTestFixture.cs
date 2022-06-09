using FC.CodeFlix.Catalog.UnitTests.Application.Genre.Common;
using Xunit;

namespace FC.CodeFlix.Catalog.UnitTests.Application.Genre.DeleteGenre
{
    [CollectionDefinition(nameof(DeleteGenreTestFixture))]
    public class DeleteGenreFixtureCollection : ICollectionFixture<DeleteGenreTestFixture> { }

    public class DeleteGenreTestFixture : GenreUseCasesBaseFixture
    {

    }
}
