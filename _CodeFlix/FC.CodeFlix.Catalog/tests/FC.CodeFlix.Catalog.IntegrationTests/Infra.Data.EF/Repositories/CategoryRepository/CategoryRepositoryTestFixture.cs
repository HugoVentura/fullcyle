using FC.CodeFlix.Catalog.Domain.Entity;
using FC.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.CodeFlix.Catalog.IntegrationTests.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace FC.CodeFlix.Catalog.IntegrationTests.Infra.Data.EF.Repositories.CategoryRepository
{
    [CollectionDefinition(nameof(CategoryRepositoryTestFixture))]
    public class CategoryRepositoryTestFixtureCollection : ICollectionFixture<CategoryRepositoryTestFixture>
    {
    }

    public class CategoryRepositoryTestFixture : BaseFixture
    {
        public string GetValidCategoryName()
        {
            var categoryName = string.Empty;
            while (categoryName.Length < 3 || categoryName.Length > 255)
                categoryName = Faker.Commerce.Categories(1)[0];

            return Faker.Commerce.Categories(1)[0];
        }

        public string GetValidCategoryDescription()
        {
            var categoryDescription = Faker.Commerce.ProductDescription();
            if (categoryDescription.Length > 10_000)
                categoryDescription = categoryDescription[..10_000];

            return categoryDescription;
        }

        public bool GetRandomBoolean() => new Random().NextDouble() < 0.5;

        public Category GetExampleCategory() => new(GetValidCategoryName(), GetValidCategoryDescription(), GetRandomBoolean());

        public List<Category> GetExampleCategoryList(int length = 10) => Enumerable.Range(1, length).Select(_ => this.GetExampleCategory()).ToList();

        public List<Category> GetExampleCategoryListWithNames(List<string> names)
            => names.Select(name =>
            {
                var category = this.GetExampleCategory();
                category.Update(name);
                
                return category;
            }).ToList();

        public List<Category> CloneCategoryListOrdered(List<Category> categoriesList, string orderBy, SearchOrder order)
        {
            var listClone = new List<Category>(categoriesList);
            var orderedEnumerable = (orderBy.ToLower(), order) switch
            {
                ("name", SearchOrder.Asc) => listClone.OrderBy(p => p.Name).ThenBy(p => p.Id),
                ("name", SearchOrder.Desc) => listClone.OrderByDescending(p => p.Name).ThenByDescending(p => p.Id),
                ("id", SearchOrder.Asc) => listClone.OrderBy(p => p.Id),
                ("id", SearchOrder.Desc) => listClone.OrderByDescending(p => p.Id),
                ("createdat", SearchOrder.Asc) => listClone.OrderBy(p => p.CreatedAt),
                ("createdat", SearchOrder.Desc) => listClone.OrderByDescending(p => p.CreatedAt),
                _ => listClone.OrderBy(p => p.Name).ThenBy(p => p.Id)
            };

            return orderedEnumerable.ThenBy(p => p.Id).ToList();
        }
    }
}
