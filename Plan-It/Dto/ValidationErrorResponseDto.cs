public class ValidationErrorResponse
{
    public string? Status { get; set; }
    public string? Message { get; set; }
    public Dictionary<string, string[]>? Errors { get; set; }
}
