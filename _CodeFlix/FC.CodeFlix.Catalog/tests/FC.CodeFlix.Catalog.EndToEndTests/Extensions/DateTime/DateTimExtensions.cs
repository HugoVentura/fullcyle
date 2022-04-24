namespace FC.CodeFlix.Catalog.EndToEndTests.Extensions.DateTime
{
    internal static class DateTimExtensions
    {
        public static System.DateTime TrimMilliSeconds(this System.DateTime value) => new System.DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute,
            value.Second, 0, value.Kind);
    }
}
