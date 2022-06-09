using FC.CodeFlix.Catalog.UnitTests.Application.Genre.Common;
using Xunit;

namespace FC.CodeFlix.Catalog.UnitTests.Application.Genre.UpdateGenre
{
    [CollectionDefinition(nameof(UpdateGenreFixture))]
    public class UpdateGenreFixtureCollection : ICollectionFixture<UpdateGenreFixture> { }

    public class UpdateGenreFixture : GenreUseCasesBaseFixture
    {

    }
}
