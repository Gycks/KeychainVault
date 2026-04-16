using System;
using System.Collections.Generic;

namespace KeychainVault.Contracts;

public interface IPasswordOption
{
    /// <summary>
    /// Builds the class attributes to be used in the subsequent insertion query building.
    /// </summary>
    /// <param name="toRelease">A list of IntPtr objects to which created pointer objects (during the process) are added. This list must be freed by the caller.</param>
    /// <returns>A tuple containing the list of attribute keys and the list of corresponding attribute values.</returns>
    (List<IntPtr> keys, List<IntPtr> values) BuildForInsertion(List<IntPtr> toRelease);
    
    /// <summary>
    /// Builds the class attributes to be used in the subsequent lookup query building.
    /// </summary>
    /// <param name="toRelease">A list of IntPtr objects to which created pointer objects (during the process) are added. This list must be freed by the caller.</param>
    /// <returns>A tuple containing the list of attribute keys and the list of corresponding attribute values.</returns>
    (List<IntPtr> keys, List<IntPtr> values) BuildForQuery(List<IntPtr> toRelease);
    
    /// <summary>
    /// Builds the class attributes to be used in the subsequent update query building.
    /// </summary>
    /// <param name="toRelease">A list of IntPtr objects to which created pointer objects (during the process) are added. This list must be freed by the caller.</param>
    /// <returns>A tuple containing the list of attribute keys and the list of corresponding attribute values.</returns>
    (List<IntPtr> keys, List<IntPtr> values) BuildForUpdate(List<IntPtr> toRelease);
}