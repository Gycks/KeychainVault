#nullable enable
using KeychainVault.Operations.Options;

namespace KeychainVault.Contracts;

/// <summary>
/// Provides methods for interacting with Keychain access to store, retrieve, and delete password items.
/// </summary>
public interface IKeychain
{
    /// <summary>
    /// Adds a generic password item to the keychain.
    /// </summary>
    /// <param name="service">The service name associated with the password.</param>
    /// <param name="account">The account name associated with the password.</param>
    /// <param name="secret">The password data as a byte array.</param>
    /// <param name="updateIfExists">If true, updates the item if it already exists; otherwise, throws an exception.</param>
    /// <param name="option">Optional additional options for the password item.</param>
    void AddGenericPasswordItem(string service, string account, byte[] secret, bool updateIfExists = true, GenericPasswordOption? option = null);

    /// <summary>
    /// Adds an internet password item to the keychain.
    /// </summary>
    /// <param name="service">The service name associated with the password.</param>
    /// <param name="account">The account name associated with the password.</param>
    /// <param name="secret">The password data as a byte array.</param>
    /// <param name="updateIfExists">If true, updates the item if it already exists; otherwise, throws an exception.</param>
    /// <param name="option">Optional additional options for the password item.</param>
    void AddInternetPasswordItem(string service, string account, byte[] secret, bool updateIfExists = true, InternetPasswordOption? option = null);

    /// <summary>
    /// Loads a generic password item from the keychain.
    /// </summary>
    /// <param name="service">The service name associated with the password.</param>
    /// <param name="option">Optional additional options for the password item.</param>
    /// <returns>The password data as a byte array, or null if not found.</returns>
    byte[]? LoadGenericPasswordItem(string service, GenericPasswordOption? option = null);

    /// <summary>
    /// Loads an internet password item from the keychain.
    /// </summary>
    /// <param name="server">The server name associated with the password.</param>
    /// <param name="option">Optional additional options for the password item.</param>
    /// <returns>The password data as a byte array, or null if not found.</returns>
    byte[]? LoadInternetPasswordItem(string server, InternetPasswordOption? option = null);

    /// <summary>
    /// Deletes a generic password item from the keychain.
    /// </summary>
    /// <param name="service">The service name associated with the password.</param>
    /// <param name="account">Optional account name associated with the password.</param>
    /// <param name="option">Optional additional options for the password item.</param>
    /// <returns>True if the item was deleted, false if it was not found.</returns>
    bool DeleteGenericPasswordItem(string service, string? account=null, GenericPasswordOption? option = null);

    /// <summary>
    /// Deletes an internet password item from the keychain.
    /// </summary>
    /// <param name="server">The server name associated with the password.</param>
    /// <param name="account">Optional account name associated with the password.</param>
    /// <param name="option">Optional additional options for the password item.</param>
    /// <returns>True if the item was deleted, false if it was not found.</returns>
    bool DeleteInternetPasswordItem(string server, string? account=null, InternetPasswordOption? option = null);
}