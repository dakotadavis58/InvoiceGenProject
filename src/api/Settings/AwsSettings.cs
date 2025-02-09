namespace InvoiceGeneratorAPI.Settings;

public class AwsSettings
{
    public string AccessKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public string BucketName { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public SesSettings SES { get; set; } = new();
}

public class SesSettings
{
    public string FromEmail { get; set; } = string.Empty;
}