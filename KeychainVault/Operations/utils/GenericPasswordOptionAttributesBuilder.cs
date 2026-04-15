using System;
using System.Collections.Generic;
using KeychainVault.Contracts;
using KeychainVault.Interop;
using KeychainVault.Operations.Options;
using KeychainVault.Validation;

namespace KeychainVault.Operations.utils;

/// <summary>
/// Provides method to build keychain attributes for generic password options.
/// </summary>
internal abstract class GenericPasswordOptionAttributesBuilder : IOptionAttributeBuilder<GenericPasswordOption>
{
    /// <summary>
    /// Builds the attribute keys and values for the generic password dictionary attribute creation.
    /// </summary>
    /// <param name="option">The generic password option containing the attributes to build.</param>
    /// <param name="toRelease">A list of IntPtr objects to which created pointer objects (during the process) are added. This list must be freed by the caller.</param>
    /// <returns>A tuple containing the list of attribute keys and the list of corresponding attribute values.</returns>
    public static (List<IntPtr> keys, List<IntPtr> values) Build(GenericPasswordOption option, List<IntPtr> toRelease)
    {
        List<IntPtr> keys = new();
        List<IntPtr> values = new();
        
        if (option.Synchronizable != null)
        {
            keys.Add(KeychainConstants.KSecAttrSynchronizable);
            values.Add(option.Synchronizable.Value ? KeychainConstants.KCFBooleanTrue : KeychainConstants.KCFBooleanFalse);
        }

        if (option.Label != null)
        {
            Validator.IsStringValid(option.Label, $"{nameof(option)}.{nameof(option.Label)}");

            var cfLabel = KeychainHelpers.CreateCFString(option.Label);
            toRelease.Add(cfLabel);

            keys.Add(KeychainConstants.KSecAttrLabel);
            values.Add(cfLabel);
        }

        if (option.Description != null)
        {
            Validator.IsStringValid(option.Description, $"{nameof(option)}.{nameof(option.Description)}");
            
            var cfDescription = KeychainHelpers.CreateCFString(option.Description);
            toRelease.Add(cfDescription);
            
            keys.Add(KeychainConstants.KSecAttrDescription);
            values.Add(cfDescription);
        }

        if (option.Comment != null)
        {
            Validator.IsStringValid(option.Comment, $"{nameof(option)}.{nameof(option.Comment)}");
            
            var cfComment = KeychainHelpers.CreateCFString(option.Comment);
            toRelease.Add(cfComment);
            
            keys.Add(KeychainConstants.KSecAttrComment);
            values.Add(cfComment);
        }

        if (option.AccessGroup != null)
        {
            Validator.IsStringValid(option.AccessGroup, $"{nameof(option)}.{nameof(option.AccessGroup)}");
            
            var cfAccessGroup = KeychainHelpers.CreateCFString(option.AccessGroup);
            toRelease.Add(cfAccessGroup);
            
            keys.Add(KeychainConstants.KSecAttrAccessGroup);
            values.Add(cfAccessGroup);
        }

        if (option.Accessible != null)
        {
            var cfAccessible = option.Accessible.Value switch
            {
                AccessibilityOption.WhenUnlocked => KeychainConstants.KSecAttrAccessibleWhenUnlocked,
                AccessibilityOption.AfterFirstUnlock => KeychainConstants.KSecAttrAccessibleAfterFirstUnlock,
                AccessibilityOption.Always => KeychainConstants.KSecAttrAccessibleAlways,
                AccessibilityOption.WhenUnlockedThisDeviceOnly => KeychainConstants.KSecAttrAccessibleWhenUnlockedThisDeviceOnly,
                AccessibilityOption.AfterFirstUnlockThisDeviceOnly => KeychainConstants.KSecAttrAccessibleAfterFirstUnlockThisDeviceOnly,
                _ => throw new ArgumentOutOfRangeException(
                    nameof(option.Accessible),
                    option.Accessible,
                    $"Option {option.Accessible} not supported."
                    )
            };
            
            keys.Add(KeychainConstants.KSecAttrAccessible);
            values.Add(cfAccessible);
        }

        if (option.IsInvisible != null)
        {
            keys.Add(KeychainConstants.KSecAttrIsInvisible);
            values.Add(option.IsInvisible.Value ? KeychainConstants.KCFBooleanTrue : KeychainConstants.KCFBooleanFalse);
        }

        if (option.IsNegative != null)
        {
            keys.Add(KeychainConstants.KSecAttrIsNegative);
            values.Add(option.IsNegative.Value ? KeychainConstants.KCFBooleanTrue : KeychainConstants.KCFBooleanFalse);
        }

        if (option.Creator != null)
        {
            var cfCreator = KeychainHelpers.CreateCFNumber(option.Creator.Value);
            toRelease.Add(cfCreator);
            
            keys.Add(KeychainConstants.KSecAttrCreator);
            values.Add(cfCreator);
        }

        if (option.Type != null)
        {
            var cfType = KeychainHelpers.CreateCFNumber(option.Type.Value);
            toRelease.Add(cfType);
            
            keys.Add(KeychainConstants.KSecAttrType);
            values.Add(cfType);
        }
        
        return (keys, values);
    }
}