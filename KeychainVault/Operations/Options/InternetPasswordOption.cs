#nullable enable
using System;
using System.Collections.Generic;
using KeychainVault.Interop;
using KeychainVault.Validation;

namespace KeychainVault.Operations.Options;

public class InternetPasswordOption : GenericPasswordOption
{
    public string? SecurityDomain { get; init; }
    public InternetProtocolOption? Protocol { get; init; }
    public int? Port { get; init; }
    public string? Path { get; init; }
    public InternetAuthenticationTypeOption? AuthenticationType { get; init; }
    

    private (List<IntPtr> keys, List<IntPtr> values) Build(List<IntPtr> toRelease)
    {
        List<IntPtr> keys = new();
        List<IntPtr> values = new();

        if (SecurityDomain is not null)
        {
            Validator.IsStringValid(SecurityDomain, $"{nameof(InternetPasswordOption)}.{nameof(SecurityDomain)}");
            
            var cfSecurityDomain = KeychainHelpers.CreateCFString(SecurityDomain);
            toRelease.Add(cfSecurityDomain);
            
            keys.Add(KeychainConstants.KSecAttrSecurityDomain);
            values.Add(cfSecurityDomain);
        }

        if (Protocol is not null)
        {
            var cfProtocol = Protocol switch
            {
                InternetProtocolOption.Http => KeychainConstants.KSecAttrProtocolHTTP,
                InternetProtocolOption.Https => KeychainConstants.KSecAttrProtocolHTTPS,
                InternetProtocolOption.Ftp => KeychainConstants.KSecAttrProtocolFTP,
                InternetProtocolOption.Ssh => KeychainConstants.KSecAttrProtocolSSH,
                _ => throw new ArgumentOutOfRangeException(
                    nameof(Protocol),
                    Protocol,
                    $"Option {Protocol} not supported.")
            };
            
            keys.Add(KeychainConstants.KSecAttrProtocol);
            values.Add(cfProtocol);
        }

        if (Port is not null)
        {
            var cfPort = KeychainHelpers.CreateCFNumber(Port.Value);
            toRelease.Add(cfPort);
            
            keys.Add(KeychainConstants.KSecAttrPort);
            values.Add(cfPort);
        }

        if (Path is not null)
        {
            Validator.IsStringValid(Path, $"{nameof(InternetPasswordOption)}.{nameof(Path)}");
            var cfPath = KeychainHelpers.CreateCFString(Path);
            toRelease.Add(cfPath);
            
            keys.Add(KeychainConstants.KSecAttrPath);
            values.Add(cfPath);
        }

        if (AuthenticationType is not null)
        {
            var cfAuthenticationType = AuthenticationType switch
            {
                InternetAuthenticationTypeOption.Default => KeychainConstants.KSecAttrAuthenticationTypeDefault,
                InternetAuthenticationTypeOption.HttpBasic => KeychainConstants.KSecAttrAuthenticationTypeHTTPBasic,
                InternetAuthenticationTypeOption.HttpDigest => KeychainConstants.KSecAttrAuthenticationTypeHTTPDigest,
                _ => throw new ArgumentOutOfRangeException(
                    nameof(AuthenticationType),
                    AuthenticationType,
                    $"Option {AuthenticationType} not supported.")
            };
            
            keys.Add(KeychainConstants.KSecAttrAuthenticationType);
            values.Add(cfAuthenticationType);
        }
        return (keys, values);
        
    }

    public override (List<IntPtr> keys, List<IntPtr> values) BuildForInsertion(List<IntPtr> toRelease)
    {
        var (keys, values) = Build(toRelease);
        var (genericsKeys, genericsValues) = base.BuildForInsertion(toRelease);
        
        if ((genericsKeys is null) != (genericsValues is null))
        {
            throw new ArgumentException("Option keys and values must both be provided or both be null.");
        }

        if (genericsKeys != null && genericsValues != null)
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
    
    public override (List<IntPtr> keys, List<IntPtr> values) BuildForQuery(List<IntPtr> toRelease)
    {
        var (keys, values) = Build(toRelease);
        var (genericsKeys, genericsValues) = base.BuildForQuery(toRelease);
        
        if ((genericsKeys is null) != (genericsValues is null))
        {
            throw new ArgumentException("Option keys and values must both be provided or both be null.");
        }

        if (genericsKeys != null && genericsValues != null)
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