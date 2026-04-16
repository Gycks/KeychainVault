#nullable enable
using System;
using System.Collections.Generic;
using KeychainVault.Errors;
using KeychainVault.Interop;
using KeychainVault.Operations.Options;
using KeychainVault.Validation;

namespace KeychainVault.Operations;

public class DeleteOperation
{
    private static int DeleteItem(IntPtr secClass, IntPtr primaryKey, string primaryValue, string primaryValueName,
        string? account, List<IntPtr> toRelease, bool useDataProtectionKeychain,
        List<IntPtr>? optionKeys=null, List<IntPtr>? optionValues=null)
    {
        Validator.IsStringValid(primaryValue, primaryValueName);
        
        var cfPrimaryValue = KeychainHelpers.CreateCFString(primaryValue);
        toRelease.Add(cfPrimaryValue);

        var cfAccount = IntPtr.Zero;
        if (!string.IsNullOrWhiteSpace(account))
        {
            Validator.IsStringValid(account, nameof(account));
            cfAccount = KeychainHelpers.CreateCFString(account);
            toRelease.Add(cfAccount);
        }
        
        
        List<IntPtr> keys =
        [
            KeychainConstants.KSecClass,
            primaryKey,
        ];

        List<IntPtr> values =
        [
            secClass,
            cfPrimaryValue,
        ];

        if (cfAccount != IntPtr.Zero)
        {
            keys.Add(KeychainConstants.KSecAttrAccount);
            values.Add(cfAccount);
        }
        
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
            
        return KeychainServices.SecItemDelete(query);
    }
    
    internal static bool DeleteGenericPassword(string service, string? account, bool useDataProtectionKeychain=false, GenericPasswordOption? option=null)
    {
        var toRelease = new List<IntPtr>();
        
        try
        {
            int status;
            if (option != null)
            {
                var (oKeys, oValues) = option.BuildForQuery(toRelease);
                status = DeleteItem(KeychainConstants.KSecClassGenericPassword, KeychainConstants.KSecAttrService,
                    service, nameof(service), account, toRelease, useDataProtectionKeychain, oKeys, oValues);
            }
            else
            {
                status = DeleteItem(KeychainConstants.KSecClassGenericPassword, KeychainConstants.KSecAttrService,
                    service, nameof(service), account, toRelease, useDataProtectionKeychain);
            }

            if (status == Error.ErrSecItemNotFound)
            {
                return false;
            }

            Validator.IsResponseCodeSuccess(status, nameof(DeleteGenericPassword));
            return true;
        }
        finally
        {
            KeychainHelpers.SafeRelease(toRelease);
        }
    }

    internal static bool DeleteInternetPassword(string server, string? account, bool useDataProtectionKeychain=false, InternetPasswordOption? option=null)
    {
        var toRelease = new List<IntPtr>();

        try
        {
            int status;
            if (option != null)
            {
                var (oKeys, oValues) = option.BuildForQuery(toRelease);
                status = DeleteItem(KeychainConstants.KSecClassInternetPassword, KeychainConstants.KSecAttrServer,
                    server, nameof(server), account, toRelease, useDataProtectionKeychain, oKeys, oValues);
            }
            else
            {
                status = DeleteItem(KeychainConstants.KSecClassInternetPassword, KeychainConstants.KSecAttrServer,
                    server, nameof(server), account, toRelease, useDataProtectionKeychain);
            }
            
            if (status == Error.ErrSecItemNotFound)
            {
                return false;
            }

            Validator.IsResponseCodeSuccess(status, nameof(DeleteGenericPassword));
            return true;
        }
        finally
        {
            KeychainHelpers.SafeRelease(toRelease);
        }
    }
}