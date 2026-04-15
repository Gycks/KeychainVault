#nullable enable
using KeychainVault.Operations;
using KeychainVault.Operations.Options;
using KeychainVault.Validation;

namespace KeychainVault
{
    public sealed class Keychain
    {
        public static void AddItem(string service, string account, byte[] secret, GenericPasswordOption? option = null)
        {
            Validator.IsPlatformSupported();
            SaveOperation.SaveGenericPassword(service, account, secret, option);
        }


        public static void AddItem(string service, string account, byte[] secret, InternetPasswordOption option)
        {
            Validator.IsPlatformSupported();
            SaveOperation.SaveInternetPassword(service, account, secret, option);
        }
            
    }
}