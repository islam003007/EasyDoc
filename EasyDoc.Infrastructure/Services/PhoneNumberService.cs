using EasyDoc.Application.Abstractions.Utils;
using EasyDoc.Application.Options;
using Microsoft.Extensions.Options;
using PhoneNumbers;

namespace EasyDoc.Infrastructure.Services;

internal class PhoneNumberService : IPhoneNumberService
{
    private readonly PhoneNumberUtil _utl;
    private readonly string _countryIso;

    public PhoneNumberService(IOptions<ApplicationOptions> applicationOptions)
    {
        _utl = PhoneNumberUtil.GetInstance();
        _countryIso = applicationOptions.Value.CountryCode;
    }

    public bool IsValid(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return false;


        try
        {
            var phoneNumber = _utl.Parse(raw, _countryIso);
            return _utl.IsValidNumber(phoneNumber);
        }
        catch (NumberParseException)
        {
            return false;
        }
    }
    public bool TryNormalize(string raw, out string e164)
    {
        e164 = null!;

        if (string.IsNullOrEmpty(raw))
            return false;

        PhoneNumber phoneNumber;

        if (!TryParse(raw, out phoneNumber))
            return false;

        if (!_utl.IsValidNumber(phoneNumber))
            return false;

        e164 = _utl.Format(phoneNumber, PhoneNumberFormat.E164);
        return true;
    }

    private bool TryParse(string raw, out PhoneNumber phoneNumber)
    {
        phoneNumber = null!;

        try
        {
            phoneNumber = _utl.Parse(raw, _countryIso);

            return true;
        }
        catch (NumberParseException)
        {
            return false;
        }
    }
}
