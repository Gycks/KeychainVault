#nullable enable
using KeychainVault.Operations.Options;

namespace KeychainVault.Contracts;

public interface IKeychain
{
    void AddGenericPasswordItem(string service, string account, byte[] secret, GenericPasswordOption? option = null);
    void AddInternetPasswordItem(string service, string account, byte[] secret, InternetPasswordOption? option=null);
    byte[]? LoadGenericPasswordItem(string service, string account, GenericPasswordOption? option = null);
    byte[]? LoadInternetPasswordItem(string server, string account, InternetPasswordOption? option = null);
    bool DeleteGenericPasswordItem(string service, string account, GenericPasswordOption? option = null);
    bool DeleteInternetPasswordItem(string server, string account, InternetPasswordOption? option = null);
}