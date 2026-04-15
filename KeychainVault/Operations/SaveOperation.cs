#nullable enable
using System;
using System.Collections.Generic;
using KeychainVault.Interop;
using KeychainVault.Operations.Options;
using KeychainVault.Operations.utils;
using KeychainVault.Validation;

namespace KeychainVault.Operations;

public static class SaveOperation
{
    private static void ReleaseObjects(List<IntPtr> toRelease)
    {
        for (var i = toRelease.Count - 1; i >= 0; i--)
        {
            if (toRelease[i] != IntPtr.Zero)
            {
                KeychainServices.CFRelease(toRelease[i]);
            }
        }
        toRelease.Clear();
    }

    private static int SaveItem(IntPtr secClass, IntPtr primaryKey, string primaryValue, string primaryValueName,
        string account, byte[] secret, List<IntPtr> toRelease, 
        List<IntPtr>? optionKeys=null, List<IntPtr>? optionValues=null)
    {
        Validator.IsStringValid(primaryValue, nameof(primaryValue));
        Validator.IsStringValid(account, nameof(account));
        Validator.IsSecretValid(secret, nameof(secret));
        
        var cfPrimaryValue = KeychainHelpers.CreateCFString(primaryValueName);
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
    
    internal static void SaveGenericPassword(string service, string account, byte[] secret, GenericPasswordOption? option=null)
    {
        var toRelease = new List<IntPtr>();
        
        try
        {
            int status;
            if (option != null)
            {
                var (oKeys, oValues) = GenericPasswordOptionAttributesBuilder.Build(option, toRelease);
                status = SaveItem(KeychainConstants.KSecClassGenericPassword, KeychainConstants.KSecAttrService,
                    service, nameof(service), account, secret, toRelease, oKeys, oValues);
            }
            else
            {
                status = SaveItem(KeychainConstants.KSecClassGenericPassword, KeychainConstants.KSecAttrService,
                    service, nameof(service), account, secret, toRelease);
            }
            Validator.IsResponseCodeSuccess(status, nameof(SaveGenericPassword));
        }
        finally
        {
            Array.Clear(secret, 0, secret.Length);
            ReleaseObjects(toRelease);
        }
    }

    internal static void SaveInternetPassword(string server, string account, byte[] secret, InternetPasswordOption? option=null)
    {
        var toRelease = new List<IntPtr>();

        try
        {
            int status;
            if (option != null)
            {
                var (oKeys, oValues) = InternetPasswordOptionAttributesBuilder.Build(option, toRelease);
               status = SaveItem(KeychainConstants.KSecClassInternetPassword, KeychainConstants.KSecAttrServer,
                    server, nameof(server), account, secret, toRelease, oKeys, oValues);
            }
            else
            {
                status = SaveItem(KeychainConstants.KSecClassInternetPassword, KeychainConstants.KSecAttrServer,
                    server, nameof(server), account, secret, toRelease);
            }
            Validator.IsResponseCodeSuccess(status, nameof(SaveInternetPassword));
        }
        finally
        {
            Array.Clear(secret, 0, secret.Length);
            ReleaseObjects(toRelease);
        }
    }

}