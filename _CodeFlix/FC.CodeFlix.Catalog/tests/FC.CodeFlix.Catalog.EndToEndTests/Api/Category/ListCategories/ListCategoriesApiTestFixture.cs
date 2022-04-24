using FC.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.CodeFlix.Catalog.EndToEndTests.Api.Category.Common;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using DomainEntity = FC.CodeFlix.Catalog.Domain.Entity;

namespace FC.CodeFlix.Catalog.EndToEndTests.Api.Category.ListCategories
{
    [CollectionDefinition(nameof(ListCategoriesApiTestFixture))]
    public class ListCategoriesApiTestFixtureCollection : ICollectionFixture<ListCategoriesApiTestFixture> { }

    public class ListCategoriesApiTestFixture : CategoryBaseFixture
    {
        public List<DomainEntity.Category> GetExampleCategoryListWithNames(List<string> names)
            => names.Select(name =>
            {
                var category = this.GetExampleCategory();
                category.Update(name);

                return category;
            }).ToList();

        public List<DomainEntity.Category> CloneCategoryListOrdered(List<DomainEntity.Category> categoriesList, string orderBy, SearchOrder order)
        {
            var listClone = new List<DomainEntity.Category>(categoriesList);
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
