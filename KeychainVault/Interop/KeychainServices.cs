using System;
using System.Runtime.InteropServices;

namespace KeychainVault.Interop
{
    internal static class KeychainServices
    {
        
        [DllImport(AppleFrameworks.CoreFoundationFramework)]
        internal static extern IntPtr CFStringCreateWithCString(IntPtr allocator, byte[] cStr, uint encoding);
        
        [DllImport(AppleFrameworks.CoreFoundationFramework)]
        internal static extern IntPtr CFDataCreate(IntPtr allocator, byte[] bytes, nint length);
        
        [DllImport(AppleFrameworks.CoreFoundationFramework)]
        internal static extern IntPtr CFDataGetBytePtr(IntPtr cfData);
        
        [DllImport(AppleFrameworks.CoreFoundationFramework)]
        internal static extern nint CFDataGetLength(IntPtr cfData);
        
        [DllImport(AppleFrameworks.CoreFoundationFramework)]
        internal static extern IntPtr CFNumberCreate(
            IntPtr allocator,
            int theType,
            ref int value
        );
        
        [DllImport(AppleFrameworks.CoreFoundationFramework)]
        internal static extern IntPtr CFDictionaryCreate(
            IntPtr allocator,
            IntPtr[] keys,
            IntPtr[] values,
            nint count,
            IntPtr keyCallbacks,
            IntPtr valueCallbacks
        );
        
        [DllImport(AppleFrameworks.CoreFoundationFramework)]
        internal static extern void CFRelease(IntPtr cf);
        
        [DllImport(AppleFrameworks.SecurityFramework)]
        internal static extern int SecItemAdd(IntPtr attributes, IntPtr result);
        
        [DllImport(AppleFrameworks.SecurityFramework)]
        internal static extern int SecItemCopyMatching(IntPtr query, out IntPtr result);
        
        [DllImport(AppleFrameworks.SecurityFramework)]
        internal static extern int SecItemUpdate(IntPtr query, IntPtr attributesToUpdate);
        
        [DllImport(AppleFrameworks.SecurityFramework)]
        internal static extern int SecItemDelete(IntPtr query);
        
    }
}