#nullable enable
using KeychainVault.Operations.Options;

namespace KeychainVault.Contracts;

public interface IKeychain
{
    void AddGenericPasswordItem(string service, string account, byte[] secret, GenericPasswordOption? option = null);
    void AddInternetPasswordItem(string service, string account, byte[] secret, InternetPasswordOption? option=null);
}