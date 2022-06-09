using FC.CodeFlix.Catalog.Application.UseCases.Genre.ListGenres;
using FC.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.CodeFlix.Catalog.UnitTests.Application.Genre.Common;
using System;
using Xunit;

namespace FC.CodeFlix.Catalog.UnitTests.Application.Genre.ListGenres
{
    [CollectionDefinition(nameof(ListGenresTestFixture))]
    public class ListGenresTestFixtureCollection : ICollectionFixture<ListGenresTestFixture> { }

    public class ListGenresTestFixture : GenreUseCasesBaseFixture
    {
        public ListGenresInput GetExampleInput()
        {
            var random = new Random();
            return new ListGenresInput(
                page: random.Next(1, 10),
                perPage: random.Next(1, 100),
                search: Faker.Commerce.ProductName(),
                sort: Faker.Commerce.ProductName(),
                dir: random.Next(0, 10) > 5 ? SearchOrder.Asc : SearchOrder.Desc
            );
        }
    }
}
