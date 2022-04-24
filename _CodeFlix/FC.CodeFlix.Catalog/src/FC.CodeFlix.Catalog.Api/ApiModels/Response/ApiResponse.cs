namespace FC.CodeFlix.Catalog.Api.ApiModels.Response
{
    public class ApiResponse<TData>
    {
        public TData Data { get; set; }

        public ApiResponse(TData data) => this.Data = data;
    }
}
