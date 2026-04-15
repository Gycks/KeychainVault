using System;
using System.Runtime.InteropServices;

namespace KeychainVault.Interop
{
    internal static class KeychainConstants
    {
        
        private static readonly IntPtr SecurityHandle =
            NativeLibrary.Load(AppleFrameworks.SecurityFramework);

        private static readonly IntPtr CoreFoundationHandle =
            NativeLibrary.Load(AppleFrameworks.CoreFoundationFramework);

        internal const uint KCFStringEncodingUTF8 = 0x08000100;
        internal const int KCFNumberSInt32Type = 3;

        private static IntPtr ReadCFObjectConstant(IntPtr frameworkHandle, string symbolName)
        {
            IntPtr symbol = NativeLibrary.GetExport(frameworkHandle, symbolName);
            return Marshal.ReadIntPtr(symbol);
        }

        private static IntPtr ReadExportAddress(IntPtr frameworkHandle, string symbolName)
        {
            return NativeLibrary.GetExport(frameworkHandle, symbolName);
        }

        internal static IntPtr KSecClass =>
            ReadCFObjectConstant(SecurityHandle, "kSecClass");

        internal static IntPtr KSecClassGenericPassword =>
            ReadCFObjectConstant(SecurityHandle, "kSecClassGenericPassword");
        
        internal static IntPtr KSecClassInternetPassword =>
            ReadCFObjectConstant(SecurityHandle, "kSecClassInternetPassword");

        internal static IntPtr KSecAttrService =>
            ReadCFObjectConstant(SecurityHandle, "kSecAttrService");

        internal static IntPtr KSecAttrAccount =>
            ReadCFObjectConstant(SecurityHandle, "kSecAttrAccount");

        internal static IntPtr KSecValueData =>
            ReadCFObjectConstant(SecurityHandle, "kSecValueData");

        internal static IntPtr KCFBooleanTrue =>
            ReadCFObjectConstant(CoreFoundationHandle, "kCFBooleanTrue");
        
        internal static IntPtr KCFBooleanFalse =>
            ReadCFObjectConstant(CoreFoundationHandle, "kCFBooleanFalse");

        internal static IntPtr KCFTypeDictionaryKeyCallBacks =>
            ReadExportAddress(CoreFoundationHandle, "kCFTypeDictionaryKeyCallBacks");

        internal static IntPtr KCFTypeDictionaryValueCallBacks =>
            ReadExportAddress(CoreFoundationHandle, "kCFTypeDictionaryValueCallBacks");
        
        internal static IntPtr KSecAttrSynchronizable =>
            ReadCFObjectConstant(SecurityHandle, "kSecAttrSynchronizable");

        internal static IntPtr KSecAttrLabel =>
            ReadCFObjectConstant(SecurityHandle, "kSecAttrLabel");

        internal static IntPtr KSecAttrDescription =>
            ReadCFObjectConstant(SecurityHandle, "kSecAttrDescription");

        internal static IntPtr KSecAttrComment =>
            ReadCFObjectConstant(SecurityHandle, "kSecAttrComment");

        internal static IntPtr KSecAttrAccessGroup =>
            ReadCFObjectConstant(SecurityHandle, "kSecAttrAccessGroup");

        internal static IntPtr KSecAttrAccessible =>
            ReadCFObjectConstant(SecurityHandle, "kSecAttrAccessible");
        
        internal static IntPtr KSecAttrAccessibleWhenUnlocked =>
            ReadCFObjectConstant(SecurityHandle, "kSecAttrAccessibleWhenUnlocked");

        internal static IntPtr KSecAttrAccessibleAfterFirstUnlock =>
            ReadCFObjectConstant(SecurityHandle, "kSecAttrAccessibleAfterFirstUnlock");

        internal static IntPtr KSecAttrAccessibleAlways =>
            ReadCFObjectConstant(SecurityHandle, "kSecAttrAccessibleAlways");

        internal static IntPtr KSecAttrAccessibleWhenUnlockedThisDeviceOnly =>
            ReadCFObjectConstant(SecurityHandle, "kSecAttrAccessibleWhenUnlockedThisDeviceOnly");

        internal static IntPtr KSecAttrAccessibleAfterFirstUnlockThisDeviceOnly =>
            ReadCFObjectConstant(SecurityHandle, "kSecAttrAccessibleAfterFirstUnlockThisDeviceOnly");

        internal static IntPtr KSecAttrIsInvisible =>
            ReadCFObjectConstant(SecurityHandle, "kSecAttrIsInvisible");

        internal static IntPtr KSecAttrIsNegative =>
            ReadCFObjectConstant(SecurityHandle, "kSecAttrIsNegative");

        internal static IntPtr KSecAttrCreator =>
            ReadCFObjectConstant(SecurityHandle, "kSecAttrCreator");

        internal static IntPtr KSecAttrType =>
            ReadCFObjectConstant(SecurityHandle, "kSecAttrType");
        
        internal static IntPtr KSecAttrServer =>
            ReadCFObjectConstant(SecurityHandle, "kSecAttrServer");

        internal static IntPtr KSecAttrSecurityDomain =>
            ReadCFObjectConstant(SecurityHandle, "kSecAttrSecurityDomain");

        internal static IntPtr KSecAttrProtocol =>
            ReadCFObjectConstant(SecurityHandle, "kSecAttrProtocol");

        internal static IntPtr KSecAttrPort =>
            ReadCFObjectConstant(SecurityHandle, "kSecAttrPort");

        internal static IntPtr KSecAttrPath =>
            ReadCFObjectConstant(SecurityHandle, "kSecAttrPath");

        internal static IntPtr KSecAttrAuthenticationType =>
            ReadCFObjectConstant(SecurityHandle, "kSecAttrAuthenticationType");
        
        internal static IntPtr KSecAttrProtocolHTTP =>
            ReadCFObjectConstant(SecurityHandle, "kSecAttrProtocolHTTP");

        internal static IntPtr KSecAttrProtocolHTTPS =>
            ReadCFObjectConstant(SecurityHandle, "kSecAttrProtocolHTTPS");

        internal static IntPtr KSecAttrProtocolFTP =>
            ReadCFObjectConstant(SecurityHandle, "kSecAttrProtocolFTP");

        internal static IntPtr KSecAttrProtocolSSH =>
            ReadCFObjectConstant(SecurityHandle, "kSecAttrProtocolSSH");
        
        internal static IntPtr KSecAttrAuthenticationTypeDefault =>
            ReadCFObjectConstant(SecurityHandle, "kSecAttrAuthenticationTypeDefault");

        internal static IntPtr KSecAttrAuthenticationTypeHTTPBasic =>
            ReadCFObjectConstant(SecurityHandle, "kSecAttrAuthenticationTypeHTTPBasic");

        internal static IntPtr KSecAttrAuthenticationTypeHTTPDigest =>
            ReadCFObjectConstant(SecurityHandle, "kSecAttrAuthenticationTypeHTTPDigest");
        
        internal static IntPtr KSecUseDataProtectionKeychain =>
            ReadCFObjectConstant(SecurityHandle, "kSecUseDataProtectionKeychain");
        
        internal static IntPtr KSecReturnData =>
            ReadCFObjectConstant(SecurityHandle, "kSecReturnData");

        internal static IntPtr KSecMatchLimit =>
            ReadCFObjectConstant(SecurityHandle, "kSecMatchLimit");

        internal static IntPtr KSecMatchLimitOne =>
            ReadCFObjectConstant(SecurityHandle, "kSecMatchLimitOne");
        
    }
}