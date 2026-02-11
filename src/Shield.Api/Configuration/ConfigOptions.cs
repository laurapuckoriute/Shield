namespace Shield.Api.Configuration;

public sealed class ConfigOptions
{
    public const string SectionName = "Config";
    public const string DefaultApiKey = "testapikey";

    public string? API_KEY { get; set; } = DefaultApiKey;
}
