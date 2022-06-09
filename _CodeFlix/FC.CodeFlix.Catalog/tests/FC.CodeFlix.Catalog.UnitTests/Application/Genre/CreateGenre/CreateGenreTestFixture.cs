using FC.CodeFlix.Catalog.Application.UseCases.Genre.CreateGenre;
using FC.CodeFlix.Catalog.UnitTests.Application.Genre.Common;
using System;
using System.Linq;
using Xunit;

namespace FC.CodeFlix.Catalog.UnitTests.Application.Genre.CreateGenre
{

    [CollectionDefinition(nameof(CreateGenreTestFixture))]
    public class CreateGenreTestFixtureCollection : ICollectionFixture<CreateGenreTestFixture> { }
    public class CreateGenreTestFixture: GenreUseCasesBaseFixture
    {
        public CreateGenreInput GetExampleInput() => new(this.GetValidGenreName(), this.GetRandomBoolean());
        public CreateGenreInput GetExampleInput(string? name) => new(name!, this.GetRandomBoolean());

        public CreateGenreInput GetExampleInputWithCategories()
        {
            var numberOfCategoriesIds = new Random().Next(1, 10);
            var categoriesIds = Enumerable.Range(1, numberOfCategoriesIds)
                .Select(_ => Guid.NewGuid()).ToList();


            return new(this.GetValidGenreName(), this.GetRandomBoolean(), categoriesIds);
        }
    }
}
