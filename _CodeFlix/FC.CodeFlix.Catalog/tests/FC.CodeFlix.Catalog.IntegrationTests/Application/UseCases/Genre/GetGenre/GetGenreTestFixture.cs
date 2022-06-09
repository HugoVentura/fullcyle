﻿using FC.CodeFlix.Catalog.IntegrationTests.Application.UseCases.Genre.Common;
using Xunit;

namespace FC.CodeFlix.Catalog.IntegrationTests.Application.UseCases.Genre.GetGenre
{
    [CollectionDefinition(nameof(GetGenreTestFixture))]
    public class GetGenreTestFixtureCollection : ICollectionFixture<GetGenreTestFixture> { }

    public class GetGenreTestFixture : GenreUseCasesBaseFixture
    {

    }
}
