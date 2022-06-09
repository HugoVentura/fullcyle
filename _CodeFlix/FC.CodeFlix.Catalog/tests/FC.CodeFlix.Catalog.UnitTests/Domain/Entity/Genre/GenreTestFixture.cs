using FC.CodeFlix.Catalog.UnitTests.Common;
using System;
using System.Collections.Generic;
using Xunit;
using DomainEntity = FC.CodeFlix.Catalog.Domain.Entity;

namespace FC.CodeFlix.Catalog.UnitTests.Domain.Entity.Genre
{
    [CollectionDefinition(nameof(GenreTestFixture))]
    public class GenreTestFixtureCollection: ICollectionFixture<GenreTestFixture> { }
    public class GenreTestFixture: BaseFixture
    {
        public string GetValidName() => this.Faker.Commerce.Categories(1)[0];

        public DomainEntity.Genre GetExampleGenre(bool isActive = true, List<Guid>? categoryIdList = null)
        {
            var genre = new DomainEntity.Genre(this.GetValidName(), isActive);
            if (categoryIdList is not null)
                foreach(var categoryId in categoryIdList)
                    genre.AddCategory(categoryId);

            return genre;
        }
    }
}
