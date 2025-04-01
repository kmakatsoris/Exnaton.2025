namespace Exnaton.Models;

public class AppSettings
{
    public string InternalEnvironment { get; set; }
    public string Version { get; set; }
    public ConnectionStrings ConnectionStrings { get; set; }
}

public class ConnectionStrings
{
    public string DbConnection { get; set; }
    public string AmazonS3BucketBaseUrl { get; set; }
}