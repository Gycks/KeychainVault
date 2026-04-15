#nullable enable
using System;
using System.Collections.Generic;
using KeychainVault.Interop;
using KeychainVault.Operations.Options;
using KeychainVault.Validation;

namespace KeychainVault.Operations;

internal static class SaveOperation
{

    private static int SaveItem(IntPtr secClass, IntPtr primaryKey, string primaryValue, string primaryValueName,
        string account, byte[] secret, List<IntPtr> toRelease, bool useDataProtectionKeychain,
        List<IntPtr>? optionKeys=null, List<IntPtr>? optionValues=null)
    {
        Validator.IsStringValid(primaryValue, primaryValueName);
        Validator.IsStringValid(account, nameof(account));
        Validator.IsSecretValid(secret, nameof(secret));
        
        var cfPrimaryValue = KeychainHelpers.CreateCFString(primaryValue);
        toRelease.Add(cfPrimaryValue);
        
        var cfAccount = KeychainHelpers.CreateCFString(account);
        toRelease.Add(cfAccount);
        
        var cfSecretData = KeychainHelpers.CreateCFData(secret);
        toRelease.Add(cfSecretData);
        
        List<IntPtr> keys =
        [
            KeychainConstants.KSecClass,
            primaryKey,
            KeychainConstants.KSecAttrAccount,
            KeychainConstants.KSecValueData
        ];

        List<IntPtr> values =
        [
            secClass,
            cfPrimaryValue,
            cfAccount,
            cfSecretData
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

        if (optionKeys != null && optionValues != null)
        {
            if (optionKeys.Count != optionValues.Count)
            {
                throw new ArgumentException("Option keys and values must have the same length.");
            }

            if (optionKeys.Count > 0)
            {
                keys.AddRange(optionKeys);
                values.AddRange(optionValues);
            }
        }
        
        var attributes = KeychainHelpers.CreateCFDictionary(keys.ToArray(), values.ToArray());
        toRelease.Add(attributes);
            
        return KeychainServices.SecItemAdd(attributes, IntPtr.Zero);
    }
    
    internal static void SaveGenericPassword(string service, string account, byte[] secret, bool useDataProtectionKeychain=false, GenericPasswordOption? option=null)
    {
        var toRelease = new List<IntPtr>();
        
        try
        {
            int status;
            if (option != null)
            {
                var (oKeys, oValues) = option.BuildForInsertion(toRelease);
                status = SaveItem(KeychainConstants.KSecClassGenericPassword, KeychainConstants.KSecAttrService,
                    service, nameof(service), account, secret, toRelease, useDataProtectionKeychain, oKeys, oValues);
            }
            else
            {
                status = SaveItem(KeychainConstants.KSecClassGenericPassword, KeychainConstants.KSecAttrService,
                    service, nameof(service), account, secret, toRelease, useDataProtectionKeychain);
            }
            Validator.IsResponseCodeSuccess(status, nameof(SaveGenericPassword));
        }
        finally
        {
            Array.Clear(secret, 0, secret.Length);
            KeychainHelpers.SafeRelease(toRelease);
        }
    }

    internal static void SaveInternetPassword(string server, string account, byte[] secret, bool useDataProtectionKeychain=false, InternetPasswordOption? option=null)
    {
        var toRelease = new List<IntPtr>();

        try
        {
            int status;
            if (option != null)
            {
                var (oKeys, oValues) = option.BuildForInsertion(toRelease);
                status = SaveItem(KeychainConstants.KSecClassInternetPassword, KeychainConstants.KSecAttrServer,
                    server, nameof(server), account, secret, toRelease, useDataProtectionKeychain, oKeys, oValues);
            }
            else
            {
                status = SaveItem(KeychainConstants.KSecClassInternetPassword, KeychainConstants.KSecAttrServer,
                    server, nameof(server), account, secret, toRelease, useDataProtectionKeychain);
            }
            
            Validator.IsResponseCodeSuccess(status, nameof(SaveInternetPassword));
        }
        finally
        {
            Array.Clear(secret, 0, secret.Length);
            KeychainHelpers.SafeRelease(toRelease);
        }
    }

}