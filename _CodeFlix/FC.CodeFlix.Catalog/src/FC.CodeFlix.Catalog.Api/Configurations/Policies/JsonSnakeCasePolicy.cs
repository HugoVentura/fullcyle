using FC.CodeFlix.Catalog.Api.Extensions.String;
using System.Text.Json;

namespace FC.CodeFlix.Catalog.Api.Configurations.Policies
{
    public class JsonSnakeCasePolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name) => name.ToSnakeCase();
    }
}
