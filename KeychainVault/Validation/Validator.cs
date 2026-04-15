using System;
using KeychainVault.Errors;

namespace KeychainVault.Validation;

internal class Validator
{
    internal static void IsPlatformSupported()
    {
        if (!OperatingSystem.IsMacOS())
        {
            throw new PlatformNotSupportedException("Only Mac OS X platforms are supported.");
        }
    }

    internal static void IsStringValid(string value, string paramName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, paramName);

        if (value.Contains('\0'))
        {
            throw new ArgumentException($"{nameof(paramName)} must not contain embedded null characters.");
        }
    }
    
    internal static void IsSecretValid(byte[] value, string paramName)
    {
        ArgumentNullException.ThrowIfNull(value, paramName);

        if (value.Length == 0)
            throw new ArgumentException("Secret must not be empty.", paramName);
    }
    
    internal static void IsResponseCodeSuccess(int status, string operation)
    {
        var reason = status switch
        {
            Error.ErrSecDuplicateItem => "Item already exists.",
            Error.ErrSecItemNotFound => "Item does not exist or could not be found.",
            Error.ErrSecAuthFailed => "Keychain authentication failed.",
            _ => $"Unknown error. Status: {status}"
        };

        if (status != Error.ErrSecSuccess)
        {
            throw new InvalidOperationException($"Keychain {operation} failed — {reason}");
        }
    }
}