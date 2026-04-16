#nullable enable
using System;
using System.Collections.Generic;
using KeychainVault.Errors;
using KeychainVault.Interop;
using KeychainVault.Operations.Options;
using KeychainVault.Validation;

namespace KeychainVault.Operations;

internal static class SaveOperation
{

    private static int SaveItem(IntPtr secClass, IntPtr primaryKey, string primaryValue, string primaryValueName,
        string account, byte[]? secret, List<IntPtr> toRelease, bool useDataProtectionKeychain,
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
        
        var query = KeychainHelpers.CreateCFDictionary(keys.ToArray(), values.ToArray());
        toRelease.Add(query);
            
        return KeychainServices.SecItemAdd(query, IntPtr.Zero);
    }

    private static int UpdateItem(IntPtr secClass, IntPtr primaryKey, string primaryValue, string primaryValueName,
        string account, byte[]? secret, List<IntPtr> toRelease, bool useDataProtectionKeychain,
        List<IntPtr>? queryOptionKeys = null, List<IntPtr>? queryOptionValues = null, 
        List<IntPtr>? updateKeys = null, List<IntPtr>? updateValues = null)
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

        List<IntPtr> queryKeys =
        [
            KeychainConstants.KSecClass,
            primaryKey,
            KeychainConstants.KSecAttrAccount
        ];

        List<IntPtr> queryValues =
        [
            secClass,
            cfPrimaryValue,
            cfAccount
        ];

        if (useDataProtectionKeychain)
        {
            queryKeys.Add(KeychainConstants.KSecUseDataProtectionKeychain);
            queryValues.Add(KeychainConstants.KCFBooleanTrue);
        }

        if ((queryOptionKeys is null) != (queryOptionValues is null))
        {
            throw new ArgumentException("Query option keys and values must both be provided or both be null.");
        }

        if (queryOptionKeys is not null && queryOptionValues is not null)
        {
            if (queryOptionKeys.Count != queryOptionValues.Count)
            {
                throw new ArgumentException("Query option keys and values must have the same length.");
            }

            queryKeys.AddRange(queryOptionKeys);
            queryValues.AddRange(queryOptionValues);
        }

        var queryDict = KeychainHelpers.CreateCFDictionary(queryKeys.ToArray(), queryValues.ToArray());
        toRelease.Add(queryDict);

        List<IntPtr> attrsToUpdateKeys =
        [
            KeychainConstants.KSecValueData
        ];

        List<IntPtr> attrsToUpdateValues =
        [
            cfSecretData
        ];

        if ((updateKeys is null) != (updateValues is null))
        {
            throw new ArgumentException("Update keys and values must both be provided or both be null.");
        }

        if (updateKeys is not null && updateValues is not null)
        {
            if (updateKeys.Count != updateValues.Count)
            {
                throw new ArgumentException("Update keys and values must have the same length.");
            }

            attrsToUpdateKeys.AddRange(updateKeys);
            attrsToUpdateValues.AddRange(updateValues);
        }

        var updateDict = KeychainHelpers.CreateCFDictionary(
            attrsToUpdateKeys.ToArray(),
            attrsToUpdateValues.ToArray());

        toRelease.Add(updateDict);

        return KeychainServices.SecItemUpdate(queryDict, updateDict);
    }
    
    internal static void SaveGenericPassword(string service, string account, byte[]? secret, bool updateIfExists, 
        bool useDataProtectionKeychain=false, GenericPasswordOption? option=null)
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

            if (status == Error.ErrSecDuplicateItem && updateIfExists)
            {
                if (option is not null)
                {
                    var (queryKeys, queryValues) = option.BuildForQuery(toRelease);
                    var (updateKeys, updateValues) = option.BuildForUpdate(toRelease);
                    status = UpdateItem(KeychainConstants.KSecClassGenericPassword, KeychainConstants.KSecAttrService,
                        service, nameof(service), account, secret, toRelease, useDataProtectionKeychain, 
                        queryOptionKeys: queryKeys, queryOptionValues: queryValues, updateKeys: updateKeys, updateValues: updateValues);
                }
                else
                {
                    status = UpdateItem(KeychainConstants.KSecClassGenericPassword, KeychainConstants.KSecAttrService,
                        service, nameof(service), account, secret, toRelease, useDataProtectionKeychain);
                }
            }
            
            Validator.IsResponseCodeSuccess(status, nameof(SaveGenericPassword));
        }
        finally
        {
            if (secret != null)
            {
                Array.Clear(secret, 0, secret.Length);
            }
            KeychainHelpers.SafeRelease(toRelease);
        }
    }

    internal static void SaveInternetPassword(string server, string account, byte[]? secret, bool updateIfExists=true,
        bool useDataProtectionKeychain=false, InternetPasswordOption? option=null)
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
            
            if (status == Error.ErrSecDuplicateItem && updateIfExists)
            {
                if (option is not null)
                {
                    var (queryKeys, queryValues) = option.BuildForQuery(toRelease);
                    var (updateKeys, updateValues) = option.BuildForUpdate(toRelease);
                    status = UpdateItem(KeychainConstants.KSecClassInternetPassword, KeychainConstants.KSecAttrServer,
                        server, nameof(server), account, secret, toRelease, useDataProtectionKeychain, 
                        queryOptionKeys: queryKeys, queryOptionValues: queryValues, updateKeys: updateKeys, updateValues: updateValues);
                }
                else
                {
                    status = UpdateItem(KeychainConstants.KSecClassInternetPassword, KeychainConstants.KSecAttrServer,
                        server, nameof(server), account, secret, toRelease, useDataProtectionKeychain);
                }
            }
            
            Validator.IsResponseCodeSuccess(status, nameof(SaveInternetPassword));
        }
        finally
        {
            if (secret != null)
            {
                Array.Clear(secret, 0, secret.Length);
            }
            KeychainHelpers.SafeRelease(toRelease);
        }
    }

}