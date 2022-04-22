using FC.CodeFlix.Catalog.Application.Common;
using FC.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;
using MediatR;

namespace FC.CodeFlix.Catalog.Application.UseCases.Category.ListCategories
{
    public class ListCategoriesInput : PaginatedListInput, IRequest<ListCategoriesOutput>
    {
        public ListCategoriesInput(int page = 1, int perPage = 15, string search = "", string sort = "", SearchOrder dir = SearchOrder.Asc) : 
            base(page, perPage, search, sort, dir)
        {
        }

        public void SetValues(int? page, int? perPage, string? search, string? sort, SearchOrder? dir)
        {
            if (page is not null)
                this.Page = page.Value;

            if (perPage is not null)
                this.PerPage = perPage.Value;

            if (!string.IsNullOrWhiteSpace(search))
                this.Search = search;

            if (!string.IsNullOrWhiteSpace(sort))
                this.Sort = sort;

            if ((dir is not null) && Enum.IsDefined(dir.Value))
                this.Dir = dir.Value;
        }
    }
}
