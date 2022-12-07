namespace BlazorComponentHeap.Core.Providers;

internal class SecurityProvider
{
    private EncryptProvider _encryptProvider = null!;

    public SecurityProvider(EncryptProvider encryptProvider)
    {
        _encryptProvider = encryptProvider;
    }

    internal async Task<string> EncodeExpirationDateAsync(DateTime expiration)
    {
        return await _encryptProvider.EncryptAsync("secure-date-str", expiration);
    }

    internal async Task<DateTime> DecodeExpirationDateAsync(string expirationEncodedStr)
    {
        var dateTimeStr = await _encryptProvider.DecryptTextAsync("secure-date-str", expirationEncodedStr);
        var expiration = DateTime.Parse(dateTimeStr);

        return expiration;
    }

    internal async Task<DateTime?> TryDecodeExpirationDateAsync(string expirationEncodedStr)
    {
        if (string.IsNullOrEmpty(expirationEncodedStr))
        {
            return null!;
        }

        var dateTimeStr = await _encryptProvider.DecryptTextAsync("secure-date-str", expirationEncodedStr);

        try
        {
            return DateTime.Parse(dateTimeStr);
        }
        catch (Exception ex)
        {
            return null!;
        }
    }

    internal async Task<bool?> TryParseAPIResponseAsync(string response)
    {
        await Task.Yield();
        return true;
    }
}
