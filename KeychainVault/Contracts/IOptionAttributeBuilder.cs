using System;
using System.Collections.Generic;

namespace KeychainVault.Contracts;

/// <summary>
/// Provides method to build keychain attributes for (T) password options.
/// </summary>
internal interface IOptionAttributeBuilder<T>
{
    /// <summary>
    /// Builds the attribute keys and values for the (T) password dictionary attribute creation.
    /// </summary>
    /// <param name="option">The (T) password option containing the attributes to build.</param>
    /// <param name="toRelease">A list of IntPtr objects to which created pointer objects (during the process) are added. This list must be freed by the caller.</param>
    /// <returns>A tuple containing the list of attribute keys and the list of corresponding attribute values.</returns>
    internal static abstract(List<IntPtr> keys, List<IntPtr> values) Build(T option, List<IntPtr> toRelease);
}