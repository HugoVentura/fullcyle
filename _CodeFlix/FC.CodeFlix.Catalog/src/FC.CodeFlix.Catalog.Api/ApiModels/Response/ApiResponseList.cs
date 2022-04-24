using FC.CodeFlix.Catalog.Application.Common;

namespace FC.CodeFlix.Catalog.Api.ApiModels.Response
{
    public class ApiResponseList<TItemData> : ApiResponse<IReadOnlyList<TItemData>>
    {
        public ApiResponseListMeta Meta { get; private set; }

        public ApiResponseList(int currentPage, int perPage, int total, IReadOnlyList<TItemData> data) : base(data) => this.Meta = new(currentPage, perPage, total);

        public ApiResponseList(PaginatedListOutput<TItemData> paginatedListOutput) : base(paginatedListOutput.Items) =>
            this.Meta = new(paginatedListOutput.Page, paginatedListOutput.PerPage, paginatedListOutput.Total);
    }
}
