#nullable enable
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using KeychainVault.Errors;
using KeychainVault.Interop;
using KeychainVault.Operations.Options;
using KeychainVault.Validation;

namespace KeychainVault.Operations;

internal static class LoadOperation
{
    private static byte[]? LoadItem(IntPtr secClass, 
        IntPtr primaryKey, string primaryValue, string primaryValueName, List<IntPtr> toRelease, 
        bool useDataProtectionKeychain, List<IntPtr>? optionKeys=null, List<IntPtr>? optionValues=null)
    {
        Validator.IsStringValid(primaryValue, primaryValueName);
        
        IntPtr result = IntPtr.Zero;

        try
        {
            var cfPrimaryValue = KeychainHelpers.CreateCFString(primaryValue);
            toRelease.Add(cfPrimaryValue);

            List<IntPtr> keys =
            [
                KeychainConstants.KSecClass,
                primaryKey,
                KeychainConstants.KSecReturnData,
                KeychainConstants.KSecMatchLimit
            ];

            List<IntPtr> values =
            [
                secClass,
                cfPrimaryValue,
                KeychainConstants.KCFBooleanTrue,
                KeychainConstants.KSecMatchLimitOne
            ];

            if (useDataProtectionKeychain)
            {
                keys.Add(KeychainConstants.KSecUseDataProtectionKeychain);
                values.Add(KeychainConstants.KCFBooleanTrue);
            }

            if ((optionKeys is null) != (optionValues is null))
            {
                throw new ArgumentException("Option keys and values must both be provided or both be null.");
            }

            if (optionKeys is not null && optionValues is not null)
            {
                if (optionKeys.Count != optionValues.Count)
                {
                    throw new ArgumentException("Option keys and values must have the same length.");
                }

                keys.AddRange(optionKeys);
                values.AddRange(optionValues);
            }

            var query = KeychainHelpers.CreateCFDictionary(keys.ToArray(), values.ToArray());
            toRelease.Add(query);

            var status = KeychainServices.SecItemCopyMatching(query, out result);

            if (status == Error.ErrSecItemNotFound)
            {
                return null;
            }

            Validator.IsResponseCodeSuccess(status, nameof(LoadItem));

            if (result == IntPtr.Zero)
            {
                throw new InvalidOperationException("Keychain returned success but no result data.");
            }

            var length = KeychainServices.CFDataGetLength(result);
            if (length <= 0)
            {
                return Array.Empty<byte>();
            }

            var bytes = new byte[(int)length];
            var dataPtr = KeychainServices.CFDataGetBytePtr(result);

            if (dataPtr == IntPtr.Zero)
            {
                throw new InvalidOperationException("Failed to read keychain result bytes.");
            }

            Marshal.Copy(dataPtr, bytes, 0, (int)length);
            return bytes;
        }
        finally
        {
            if (result != IntPtr.Zero)
            {
                KeychainServices.CFRelease(result);
            }
        }
    }
    
    internal static byte[]? LoadGenericPassword(string service, bool useDataProtectionKeychain, GenericPasswordOption? option=null)
    {
        var toRelease = new List<IntPtr>();

        try
        {
            if (option is not null)
            {
                var (oKeys, oValues) = option.BuildForQuery(toRelease);

                return LoadItem(
                    KeychainConstants.KSecClassGenericPassword,
                    KeychainConstants.KSecAttrService,
                    service,
                    nameof(service),
                    toRelease,
                    useDataProtectionKeychain,
                    oKeys,
                    oValues);
            }
            else
                return LoadItem(
                    KeychainConstants.KSecClassGenericPassword,
                    KeychainConstants.KSecAttrService,
                    service,
                    nameof(service),
                    toRelease,
                    useDataProtectionKeychain);
        }
        finally
        {
            KeychainHelpers.SafeRelease(toRelease);
        }
        
    }

    internal static byte[]? LoadInternetPassword(string server, bool useDataProtectionKeychain, InternetPasswordOption? option=null)
    {
        var toRelease = new List<IntPtr>();

        try
        {
            if (option is not null)
            {
                var (oKeys, oValues) = option.BuildForQuery(toRelease);

                return LoadItem(
                    KeychainConstants.KSecClassInternetPassword,
                    KeychainConstants.KSecAttrServer,
                    server,
                    nameof(server),
                    toRelease,
                    useDataProtectionKeychain,
                    oKeys,
                    oValues);
            }
            return LoadItem(
                KeychainConstants.KSecClassInternetPassword,
                KeychainConstants.KSecAttrServer,
                server,
                nameof(server),
                toRelease,
                useDataProtectionKeychain);
        }
        finally
        {
            KeychainHelpers.SafeRelease(toRelease);
        }
        
    }
}