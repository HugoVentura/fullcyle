using FC.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;

namespace FC.CodeFlix.Catalog.Application.Common
{
    public abstract class PaginatedListInput
    {
        public int Page { get; set; }
        public int PerPage { get; set; }
        public string Search { get; set; }
        public string Sort { get; set; }
        public SearchOrder Dir { get; set; }

        protected PaginatedListInput(int page, int perPage, string search, string sort, SearchOrder dir)
        {
            Page = page;
            PerPage = perPage;
            Search = search;
            Sort = sort;
            Dir = dir;
        }

        public SearchInput ToSearchInput() => new(this.Page, this.PerPage, this.Search, this.Sort, this.Dir);
    }
}
