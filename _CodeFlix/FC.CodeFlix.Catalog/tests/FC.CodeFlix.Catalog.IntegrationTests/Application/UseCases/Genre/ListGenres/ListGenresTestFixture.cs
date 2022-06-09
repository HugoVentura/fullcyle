using FC.CodeFlix.Catalog.IntegrationTests.Application.UseCases.Genre.Common;
using Xunit;

namespace FC.CodeFlix.Catalog.IntegrationTests.Application.UseCases.Genre.ListGenres
{
    [CollectionDefinition(nameof(ListGenresTestFixture))]
    public class ListGenresTestFixtureCollection : ICollectionFixture<ListGenresTestFixture> { }

    public class ListGenresTestFixture : GenreUseCasesBaseFixture
    {
    }
}
