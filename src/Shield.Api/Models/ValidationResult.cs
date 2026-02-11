namespace Shield.Api.Models;

public sealed class ValidationResult
{
    public bool IsValid { get; }
    public string? ErrorMessage { get; }

    private ValidationResult(bool isValid, string? errorMessage)
    {
        IsValid = isValid;
        ErrorMessage = errorMessage;
    }

    public static ValidationResult Valid() => new(true, null);

    public static ValidationResult Invalid(string? errorMessage)
    {
        var message = string.IsNullOrWhiteSpace(errorMessage) ? "Provided is invalid." : errorMessage;
        return new ValidationResult(false, message);
    }
}
