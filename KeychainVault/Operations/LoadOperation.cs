#nullable enable
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using KeychainVault.Errors;
using KeychainVault.Interop;
using KeychainVault.Model;
using KeychainVault.Operations.Options;
using KeychainVault.Validation;

namespace KeychainVault.Operations;

internal static class LoadOperation
{
    private static KeychainItemData? LoadItem(IntPtr secClass, 
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
                KeychainConstants.KSecReturnAttributes,
                KeychainConstants.KSecMatchLimit
            ];

            List<IntPtr> values =
            [
                secClass,
                cfPrimaryValue,
                KeychainConstants.KCFBooleanTrue,
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
            
            var dataPtr = KeychainServices.CFDataGetBytePtr(result);

            if (dataPtr == IntPtr.Zero)
            {
                return null;
            }

            var secretPtr = KeychainServices.CFDictionaryGetValue(result, KeychainConstants.KSecValueData);
            var accountPtr = KeychainServices.CFDictionaryGetValue(result, KeychainConstants.KSecAttrAccount);
            var servicePtr = KeychainServices.CFDictionaryGetValue(result, KeychainConstants.KSecAttrService);
            var labelPtr = KeychainServices.CFDictionaryGetValue(result, KeychainConstants.KSecAttrLabel);
            var commentPtr = KeychainServices.CFDictionaryGetValue(result, KeychainConstants.KSecAttrComment);
            var descriptionPtr = KeychainServices.CFDictionaryGetValue(result, KeychainConstants.KSecAttrDescription);

            return new KeychainItemData
            {
                Secret = KeychainHelpers.ReadCFData(secretPtr),
                Account = KeychainHelpers.ReadCFString(accountPtr),
                Service = KeychainHelpers.ReadCFString(servicePtr),
                Label = KeychainHelpers.ReadCFString(labelPtr),
                Comment = KeychainHelpers.ReadCFString(commentPtr),
                Description = KeychainHelpers.ReadCFString(descriptionPtr)
            };
        }
        finally
        {
            if (result != IntPtr.Zero)
            {
                KeychainServices.CFRelease(result);
            }
        }
    }
    
    internal static KeychainItemData? LoadGenericPassword(string service, bool useDataProtectionKeychain, GenericPasswordOption? option=null)
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

    internal static KeychainItemData? LoadInternetPassword(string server, bool useDataProtectionKeychain, InternetPasswordOption? option=null)
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