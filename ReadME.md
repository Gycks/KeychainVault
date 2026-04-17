# KeychainVault

[![.NET](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/)

A lightweight .NET library for securely storing, retrieving, updating, and deleting secrets using the native Apple Keychain APIs. 
It wraps the native `SecItemAdd`, `SecItemCopyMatching`, `SecItemUpdate`, and `SecItemDelete` functions via P/Invoke, providing a safe and easy-to-use interface for secret management on macOS.


## Features

- **Secure Storage**: Store generic and internet passwords securely in the macOS Keychain
- **Data Protection**: Automatic support for macOS Data Protection Keychain
- **Fine-Grained Control**: Use option objects for detailed configuration 
(labels, comments, synchronization, etc. See Official Keychain documentation)
- **Memory Safe**: Handles secrets as `byte[]` with explicit memory management

## Platform Considerations

| Platform  | Status        | Implementation                     |
|-----------|---------------|------------------------------------|
| **macOS** | Supported     | P/Invoke to Security.framework     |
| **iOS**   | Not supported | None (possibly in future releases) |

*This library is Apple Keychain access only. For cross-platform credential storage, use it behind an abstraction layer.*

## Installation

### NuGet

```csharp
dotnet add package GycksLab.KeychainVault
```

## Quick Start

```csharp
using KeychainVault;
using System.Text;

// Create a keychain instance
IKeychain keychain = new MacOSKeychain();

// Store a password
byte[] secret = Encoding.UTF8.GetBytes("my-secret-password");
keychain.AddGenericPasswordItem(
    service: "com.myapp.auth",
    account: "user@example.com",
    secret: secret
);

// Retrieve the password
var item = keychain.LoadGenericPasswordItem(
    service: "com.myapp.auth"
);

if (item != null)
{
    string password = Encoding.UTF8.GetString(item.Secret);
    Console.WriteLine($"Retrieved password: {password}");
    Console.WriteLine($"Account: {item.Account}");
}
```

## Initialization

The `MacOSKeychain` class provides two constructors for initializing the keychain instance:

### Default Constructor

```csharp
IKeychain keychain = new MacOSKeychain();
```

This constructor enforces the use of the macOS Data Protection Keychain, which provides enhanced security by requiring device authentication (passcode or biometric) to access stored items. According to Apple's official documentation, the Data Protection Keychain ensures that sensitive data is protected even if the device is lost or stolen, as items are encrypted and only accessible when the device is unlocked.

It is recommended to use this default constructor for maximum security.

### Parameterized Constructor

```csharp
IKeychain keychain = new MacOSKeychain(useDataProtectionKeychain: false);
```

This constructor allows opting out of the Data Protection Keychain, using the legacy file-based keychain instead. However, this is not recommended as it provides weaker security guarantees.

**Note**: The parameterized constructor is provided for backward compatibility and specific use cases where the Data Protection Keychain may not be suitable. However, for most applications, it is advisable to use the default constructor to leverage the enhanced security features of the Data Protection Keychain.
For local development, you can use the parameterized constructor to avoid issues with the Data Protection Keychain, which may require additional setup (e.g., provisioning profiles, entitlements and app signing). However, ensure that you switch to the default constructor for production builds to maintain strong security practices.

## Usage

### Adding Passwords

#### Generic Passwords

Store application-specific passwords or secrets:

```csharp
// Basic usage
keychain.AddGenericPasswordItem(
    service: "com.myapp.database",
    account: "dbuser",
    secret: Encoding.UTF8.GetBytes("dbpassword123")
);

// With options
var options = new GenericPasswordOption
{
    Label = "Database Password",
    Comment = "Connection string password",
    Synchronizable = false
};

keychain.AddGenericPasswordItem(
    service: "com.myapp.database",
    account: "dbuser",
    secret: Encoding.UTF8.GetBytes("dbpassword123"),
    updateIfExists: true,
    option: options
);
```

#### Internet Passwords

Store web service credentials:

```csharp
keychain.AddInternetPasswordItem(
    service: "https://api.example.com",
    account: "apiuser@example.com",
    secret: Encoding.UTF8.GetBytes("apitoken123")
);
```

### Retrieving Passwords

```csharp
// Generic password
var genericItem = keychain.LoadGenericPasswordItem(
    service: "com.myapp.database"
);

// Internet password
var internetItem = keychain.LoadInternetPasswordItem(
    server: "api.example.com"
);

if (genericItem != null)
{
    string password = Encoding.UTF8.GetString(genericItem.Secret);
    string account = genericItem.Account;
    string label = genericItem.Label;
    // Use the retrieved data...
}
```

### Deleting Passwords

```csharp
// Returns true if deleted, false if not found
bool deleted = keychain.DeleteGenericPasswordItem(
    service: "com.myapp.database",
    account: "dbuser"
);
```

## API Reference

The main interface is `IKeychain` with the following methods:

- `AddGenericPasswordItem()` - Store a generic password
- `AddInternetPasswordItem()` - Store an internet password
- `LoadGenericPasswordItem()` - Retrieve a generic password
- `LoadInternetPasswordItem()` - Retrieve an internet password
- `DeleteGenericPasswordItem()` - Delete a generic password
- `DeleteInternetPasswordItem()` - Delete an internet password

## Security Considerations

- Secrets are handled as `byte[]` and are wiped after each `AddItem` operation.
