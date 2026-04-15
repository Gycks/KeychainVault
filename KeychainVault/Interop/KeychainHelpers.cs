using System;
using System.Text;

namespace KeychainVault.Interop;

internal static class KeychainHelpers
{
    internal static IntPtr CreateCFString(string value)
    {
        var bytes = Encoding.UTF8.GetBytes(value + "\0");
        var result = KeychainServices.CFStringCreateWithCString(
            IntPtr.Zero, bytes, KeychainConstants.KCFStringEncodingUTF8
            );
        
        if (result == IntPtr.Zero)
        {
            throw new InvalidOperationException("Failed to create CFString (CFStringCreateWithCString returned null).");
        }
        
        return result;
    }

    internal static IntPtr CreateCFData(byte[] value)
    {
        var result = KeychainServices.CFDataCreate(IntPtr.Zero, value, value.Length);
        
        if (result == IntPtr.Zero)
        {
            throw new InvalidOperationException("Failed to create CFData (CFDataCreate returned null).");
        }
        
        return result;
    }
    
    internal static IntPtr CreateCFNumber(int value)
    {
        var tmp = value;

        var result = KeychainServices.CFNumberCreate(
            IntPtr.Zero,
            KeychainConstants.KCFNumberSInt32Type,
            ref tmp
        );

        if (result == IntPtr.Zero)
        {
            throw new InvalidOperationException(
                "Failed to create CFNumber (CFNumberCreate returned null).");
        }

        return result;
    }

    internal static IntPtr CreateCFDictionary(IntPtr[] keys, IntPtr[] values)
    {
        if (keys.Length == 0 || values.Length == 0)
        {
            throw new ArgumentException($"{nameof(keys)} and {nameof(values)} must not be empty.");
        }

        if (keys.Length != values.Length)
        {
            throw new ArgumentException($"{nameof(keys)} and {nameof(values)} must have the same length.");
        }

        var result = KeychainServices.CFDictionaryCreate(
            IntPtr.Zero,
            keys,
            values,
            keys.Length,
            KeychainConstants.KCFTypeDictionaryKeyCallBacks,
            KeychainConstants.KCFTypeDictionaryValueCallBacks
        );
        
        if (result == IntPtr.Zero)
        {
            throw new InvalidOperationException("Failed to create CFDictionary (CFDictionaryCreate returned null).");
        }
        
        return result;
    }
    
    internal static void SafeRelease(ref IntPtr handle)
    {
        if (handle != IntPtr.Zero)
        {
            KeychainServices.CFRelease(handle);
            handle = IntPtr.Zero;
        }
    }
}