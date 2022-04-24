namespace FC.CodeFlix.Catalog.Api.ApiModels.Response
{
    public class ApiResponseListMeta
    {
        public int CurrentPage { get; private set; }
        public int PerPage { get; private set; }
        public int Total { get; private set; }

        public ApiResponseListMeta(int currentPage, int perPage, int total)
        {
            this.CurrentPage = currentPage;
            this.PerPage = perPage;
            this.Total = total;
        }
    }
}
