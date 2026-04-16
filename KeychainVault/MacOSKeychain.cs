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
        private readonly bool _useDataProtectionKeychain;

        public MacOSKeychain() : this(true) { }

        internal MacOSKeychain(bool useDataProtectionKeychain)
        {
            _useDataProtectionKeychain = useDataProtectionKeychain;
        }
        
        private void AssertPlatformValid()
        {
            if (!OperatingSystem.IsMacOS())
            {
                throw new PlatformNotSupportedException("Only Mac OS X platforms are supported.");
            }
        }
        
        public void AddGenericPasswordItem(string service, string account, byte[] secret, 
            bool updateIfExists=true, GenericPasswordOption? option=null)
        {
            AssertPlatformValid();
            SaveOperation.SaveGenericPassword(service, account, secret, updateIfExists, 
                useDataProtectionKeychain: _useDataProtectionKeychain, option: option);
        }


        public void AddInternetPasswordItem(string server, string account, byte[] secret, 
            bool updateIfExists=true, InternetPasswordOption? option=null)
        {
            AssertPlatformValid();
            SaveOperation.SaveInternetPassword(server, account, secret, updateIfExists, 
                useDataProtectionKeychain: _useDataProtectionKeychain, option: option);
        }
        
        public byte[]? LoadGenericPasswordItem(string service, string account, GenericPasswordOption? option = null)
        {
            AssertPlatformValid();
            return LoadOperation.LoadGenericPassword(service, account, useDataProtectionKeychain: _useDataProtectionKeychain, option);
        }

        public byte[]? LoadInternetPasswordItem(string server, string account, InternetPasswordOption? option = null)
        {
            AssertPlatformValid();
            return LoadOperation.LoadInternetPassword(server, account, useDataProtectionKeychain: _useDataProtectionKeychain, option);
        }

        public bool DeleteGenericPasswordItem(string service, string account, GenericPasswordOption? option = null)
        {
            AssertPlatformValid();
            return DeleteOperation.DeleteGenericPassword(service, account, useDataProtectionKeychain: _useDataProtectionKeychain, option);
        }

        public bool DeleteInternetPasswordItem(string server, string account, InternetPasswordOption? option = null)
        {
            AssertPlatformValid();
            return DeleteOperation.DeleteInternetPassword(server, account, useDataProtectionKeychain: _useDataProtectionKeychain, option);
        }
    }
}