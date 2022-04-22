using FC.CodeFlix.Catalog.Application.UseCases.Category.UpdateCategory;
using FC.CodeFlix.Catalog.EndToEndTests.Api.Category.Common;
using System;
using Xunit;

namespace FC.CodeFlix.Catalog.EndToEndTests.Api.Category.UpdateCategory
{
    [CollectionDefinition(nameof(UpdateCategoryTestFixture))]
    public class UpdateCategoryTestFixtureCollection : ICollectionFixture<UpdateCategoryTestFixture> { }

    public class UpdateCategoryTestFixture : CategoryBaseFixture
    {
        public UpdateCategoryInput GetExampleInput(Guid? id = null) 
            => new(id ?? Guid.NewGuid(), this.GetValidCategoryName(), this.GetValidCategoryDescription(), this.GetRandomBoolean());
    }
}
