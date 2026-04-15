using System;
using System.Collections.Generic;
using KeychainVault.Contracts;
using KeychainVault.Interop;
using KeychainVault.Operations.Options;
using KeychainVault.Validation;

namespace KeychainVault.Operations.utils;

/// <summary>
/// Provides method to build keychain attributes for internet password options.
/// </summary>
internal abstract class InternetPasswordOptionAttributesBuilder : IOptionAttributeBuilder<InternetPasswordOption>
{
    /// <summary>
    /// Builds the attribute keys and values for the internet password dictionary attribute creation.
    /// </summary>
    /// <param name="option">The internet password option containing the attributes to build.</param>
    /// <param name="toRelease">A list of IntPtr objects to which created pointer objects (during the process) are added. This list must be freed by the caller.</param>
    /// <returns>A tuple containing the list of attribute keys and the list of corresponding attribute values.</returns>
    public static (List<IntPtr> keys, List<IntPtr> values) Build(InternetPasswordOption option,
        List<IntPtr> toRelease)
    {
        List<IntPtr> keys = new();
        List<IntPtr> values = new();

        if (option.SecurityDomain is not null)
        {
            Validator.IsStringValid(option.SecurityDomain, $"{nameof(option)}.{nameof(option.SecurityDomain)}");
            
            var cfSecurityDomain = KeychainHelpers.CreateCFString(option.SecurityDomain);
            toRelease.Add(cfSecurityDomain);
            
            keys.Add(KeychainConstants.KSecAttrSecurityDomain);
            values.Add(cfSecurityDomain);
        }

        if (option.Protocol is not null)
        {
            var cfProtocol = option.Protocol switch
            {
                InternetProtocolOption.Http => KeychainConstants.KSecAttrProtocolHTTP,
                InternetProtocolOption.Https => KeychainConstants.KSecAttrProtocolHTTPS,
                InternetProtocolOption.Ftp => KeychainConstants.KSecAttrProtocolFTP,
                InternetProtocolOption.Ssh => KeychainConstants.KSecAttrProtocolSSH,
                _ => throw new ArgumentOutOfRangeException(
                    nameof(option.Protocol),
                    option.Protocol,
                    $"Option {option.Protocol} not supported.")
            };
            
            keys.Add(KeychainConstants.KSecAttrProtocolHTTP);
            values.Add(cfProtocol);
        }

        if (option.Port is not null)
        {
            var cfPort = KeychainHelpers.CreateCFNumber(option.Port.Value);
            toRelease.Add(cfPort);
            
            keys.Add(KeychainConstants.KSecAttrPort);
            values.Add(cfPort);
        }

        if (option.Path is not null)
        {
            Validator.IsStringValid(option.Path, $"{nameof(option)}.{nameof(option.Path)}");
            var cfPath = KeychainHelpers.CreateCFString(option.Path);
            toRelease.Add(cfPath);
            
            keys.Add(KeychainConstants.KSecAttrPath);
            values.Add(cfPath);
        }

        if (option.AuthenticationType is not null)
        {
            var cfAuthenticationType = option.AuthenticationType switch
            {
                InternetAuthenticationTypeOption.Default => KeychainConstants.KSecAttrAuthenticationTypeDefault,
                InternetAuthenticationTypeOption.HttpBasic => KeychainConstants.KSecAttrAuthenticationTypeHTTPBasic,
                InternetAuthenticationTypeOption.HttpDigest => KeychainConstants.KSecAttrAuthenticationTypeHTTPDigest,
                _ => throw new ArgumentOutOfRangeException(
                    nameof(option.AuthenticationType),
                    option.AuthenticationType,
                    $"Option {option.AuthenticationType} not supported.")
            };
            
            keys.Add(KeychainConstants.KSecAttrAuthenticationType);
            values.Add(cfAuthenticationType);
        }

        var (genericsKeys, genericsValues) = GenericPasswordOptionAttributesBuilder.Build(option.Generics, toRelease);
        
        if ((genericsKeys is null) != (genericsValues is null))
        {
            throw new ArgumentException("Option keys and values must both be provided or both be null.");
        }

        if (genericsKeys != null)
        {
            if (genericsKeys.Count != genericsValues.Count)
            {
                throw new ArgumentException("Option keys and values must have the same length.");
            }

            if (genericsKeys.Count > 0)
            {
                keys.AddRange(genericsKeys);
                values.AddRange(genericsValues);
            }
        }
        
        return (keys, values);
    }
}