namespace EasyDoc.Application.Abstractions.Utils;

public interface IPhoneNumberService
{
    bool IsValid(string raw);
    bool TryNormalize(string raw, out string e164);
}