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
byte[]? retrievedSecret = keychain.LoadGenericPasswordItem(
    service: "com.myapp.auth",
    account: "user@example.com"
);

if (retrievedSecret != null)
{
    string password = Encoding.UTF8.GetString(retrievedSecret);
    Console.WriteLine($"Retrieved password: {password}");
}
```

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
byte[]? genericSecret = keychain.LoadGenericPasswordItem(
    service: "com.myapp.database",
    account: "dbuser"
);

// Internet password
byte[]? internetSecret = keychain.LoadInternetPasswordItem(
    server: "api.example.com",
    account: "apiuser@example.com"
);

if (genericSecret != null)
{
    string password = Encoding.UTF8.GetString(genericSecret);
    // Use the password...
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

