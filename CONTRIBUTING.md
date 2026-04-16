# Contributing to KeychainVault

Thank you for your interest in contributing to this project!

## Code of Conduct

This project adheres to the [Contributor Covenant Code of Conduct](CODE_OF_CONDUCT.md). By participating in this project, you agree to abide by its terms. Please report unacceptable behavior to [gycks.wolf555@slmail.me](mailto:gycks.wolf555@slmail.me).

## How Can I Contribute?

- Fork this repo
- Work on your branch
- Create a pull request to this repo main branch [See Pull Requests](#Pull Requests)

### Reporting Bugs

Before creating bug reports, please check the issue list as you might find out that you don't need to create one. When you are creating a bug report, please include as many details as possible:

- **Use a clear and descriptive title**
- **Describe the exact steps which reproduce the problem**
- **Provide specific examples to demonstrate the steps**
- **Describe the behavior you observed after following the steps**
- **Explain which behavior you expected to see instead and why**
- **Include your .NET version and and platform specification (e.g macOS version)**
- **If possible, include code samples or error messages**

### Suggesting Enhancements

Enhancement suggestions are tracked as GitHub issues. When creating an enhancement suggestion, please include:

- **Use a clear and descriptive title**
- **Provide a step-by-step description of the suggested enhancement**
- **Provide specific examples to demonstrate the steps**
- **Describe the current behavior and expected behavior**
- **Explain why this enhancement would be useful**

### Pull Requests

- Follow template
- Document new code with XML documentation comments where applicable
- Write or update tests as needed
- Ensure all tests pass locally before submitting

## Development Setup

### Prerequisites

- **.NET 8.0** or later

### Building the Project

```bash
# Clone the repository
git clone https://github.com/Gycks/KeychainVault.git
cd KeychainVault

# Restore dependencies
dotnet restore

# Build the project
dotnet build

# Run tests
dotnet test
```

## Testing

### Running Tests

```bash
# Run all tests
dotnet test

# Run with verbose output
dotnet test --verbosity detailed

# Run specific test project
dotnet test KeychainVault.Test/
```

### Writing Tests

- Use **xUnit** for test framework
- Test files should be in `KeychainVault.Test/`
- Test class names should end with `Tests` (e.g., `MacOSKeychainTests`)
- Test method names should clearly describe what they test
- Use the Arrange-Act-Assert pattern

Example:
```csharp
[Fact]
public void AddGenericPasswordItem_WithValidParameters_StoresPassword()
{
    // Arrange
    var keychain = new MacOSKeychain();
    var secret = Encoding.UTF8.GetBytes("test-password");

    // Act
    keychain.AddGenericPasswordItem("com.test.service", "testuser", secret);

    // Assert
    var retrieved = keychain.LoadGenericPasswordItem("com.test.service", "testuser");
    Assert.NotNull(retrieved);
}
```

