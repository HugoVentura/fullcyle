using System.Collections.Generic;

namespace FC.CodeFlix.Catalog.EndToEndTests.Models
{
    public class TestApiResponseList<TOutputItem>: TestApiResponse<List<TOutputItem>>
    {
        public TestApiResponseListMeta? Meta { get; set; }

        public TestApiResponseList(List<TOutputItem> data) : base(data) { }

        public TestApiResponseList() { }

        public TestApiResponseList(List<TOutputItem> data, TestApiResponseListMeta meta ) : base(data) => this.Meta = meta;
    }

    public class TestApiResponseListMeta
    {
        public int CurrentPage { get; set; }
        public int PerPage { get; set; }
        public int Total { get; set; }

        public TestApiResponseListMeta() { }

        public TestApiResponseListMeta(int currentPage, int perPage, int total)
        {
            this.CurrentPage = currentPage;
            this.PerPage = perPage;
            this.Total = total;
        }
    }

    public class TestApiResponse<TOutput>
    {
        public TOutput? Data { get; set; }

        public TestApiResponse() { }

        public TestApiResponse(TOutput data) => this.Data = data;
    }
}
