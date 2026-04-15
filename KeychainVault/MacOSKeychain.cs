#nullable enable
using System;
using System.Diagnostics.CodeAnalysis;
using KeychainVault.Contracts;
using KeychainVault.Operations;
using KeychainVault.Operations.Options;

namespace KeychainVault
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public sealed class MacOSKeychain : IKeychain
    {
        private void AssertPlatformValid()
        {
            if (!OperatingSystem.IsMacOS())
            {
                throw new PlatformNotSupportedException("Only Mac OS X platforms are supported.");
            }
        }
        
        public void AddGenericPasswordItem(string service, string account, byte[] secret, GenericPasswordOption? option=null)
        {
            AssertPlatformValid();
            SaveOperation.SaveGenericPassword(service, account, secret, useDataProtectionKeychain: true, option);
        }


        public void AddInternetPasswordItem(string server, string account, byte[] secret, InternetPasswordOption? option=null)
        {
            AssertPlatformValid();
            SaveOperation.SaveInternetPassword(server, account, secret, useDataProtectionKeychain: true, option);
        }
        
        public byte[]? LoadGenericPasswordItem(string service, string account, GenericPasswordOption? option = null)
        {
            AssertPlatformValid();
            return LoadOperation.LoadGenericPassword(service, account, useDataProtectionKeychain: true, option);
        }

        public byte[]? LoadInternetPasswordItem(string server, string account, InternetPasswordOption? option = null)
        {
            AssertPlatformValid();
            return LoadOperation.LoadInternetPassword(server, account, useDataProtectionKeychain: true, option);
        }

        public bool DeleteGenericPasswordItem(string service, string account, GenericPasswordOption? option = null)
        {
            AssertPlatformValid();
            return DeleteOperation.DeleteGenericPassword(service, account, useDataProtectionKeychain: true, option);
        }

        public bool DeleteInternetPasswordItem(string server, string account, InternetPasswordOption? option = null)
        {
            AssertPlatformValid();
            return DeleteOperation.DeleteInternetPassword(server, account, useDataProtectionKeychain: true, option);
        }
    }
}