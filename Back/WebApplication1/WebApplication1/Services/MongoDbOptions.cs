namespace BackendApi.Options
{
    public class MongoDbOptions
    {
        public string ConnectionString { get; set; } = default!;
        public string Database { get; set; } = default!;
    }
}
