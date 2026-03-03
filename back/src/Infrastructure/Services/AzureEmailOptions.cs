namespace Infrastructure.Services;

public class AzureEmailOptions
{
    public const string SectionName = "AzureEmail";

    public string ConnectionString { get; set; } = string.Empty;
    public string SenderAddress { get; set; } = string.Empty;
    public string RecipientAddress { get; set; } = "developers@inspand.com.br";
}
