namespace KeychainVault.Errors
{
    public static class Error
    {
        public const int ErrSecSuccess       =  0;
        public const int ErrSecItemNotFound  = -25300;
        public const int ErrSecDuplicateItem = -25299;
        public const int ErrSecAuthFailed    = -25293;
        public const int ErrSecMissingEntitlement = -34018;
    }
}