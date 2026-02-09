namespace EasyDoc.Infrastructure.Options;

internal class MailTrapOptions
{
    public string From { get; init; } = "";
    public int Id { get; init; }
    public string ApiToken { get; init; } = "";
}
