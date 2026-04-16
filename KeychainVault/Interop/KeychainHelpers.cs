#nullable enable
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
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
    
    internal static void SafeRelease(List<IntPtr> toRelease)
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
    
    internal static string? ReadCFString(IntPtr cfString)
    {
        if (cfString == IntPtr.Zero)
        {
            return null;
        }

        byte[] buffer = new byte[1024];
        bool ok = KeychainServices.CFStringGetCString(
            cfString,
            buffer,
            buffer.Length,
            KeychainConstants.KCFStringEncodingUTF8);

        if (!ok)
        {
            throw new InvalidOperationException("Failed to read CFString.");
        }

        int nullIndex = Array.IndexOf(buffer, (byte)0);
        int length = nullIndex >= 0 ? nullIndex : buffer.Length;
        return Encoding.UTF8.GetString(buffer, 0, length);
    }
    
    internal static byte[] ReadCFData(IntPtr cfData)
    {
        if (cfData == IntPtr.Zero)
        {
            return Array.Empty<byte>();
        }

        var length = KeychainServices.CFDataGetLength(cfData);
        if (length <= 0)
        {
            return Array.Empty<byte>();
        }

        var dataPtr = KeychainServices.CFDataGetBytePtr(cfData);
        if (dataPtr == IntPtr.Zero)
        {
            throw new InvalidOperationException("Failed to read CFData bytes.");
        }

        var bytes = new byte[(int)length];
        Marshal.Copy(dataPtr, bytes, 0, (int)length);
        return bytes;
    }
}