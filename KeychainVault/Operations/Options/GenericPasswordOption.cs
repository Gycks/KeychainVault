#nullable enable

using System;
using System.Collections.Generic;
using KeychainVault.Contracts;
using KeychainVault.Interop;
using KeychainVault.Validation;

namespace KeychainVault.Operations.Options;

public class GenericPasswordOption : IPasswordOption
{
    public bool? Synchronizable { get; init; }
    public string? Label { get; init; }
    public string? Description { get; init; }
    public string? Comment { get; init; }
    public string? AccessGroup { get; init; }
    public AccessibilityOption? Accessible { get; init; }
    public bool? IsInvisible { get; init; }
    public bool? IsNegative { get; init; }
    public int? Creator { get; init; }
    public int? Type { get; init; }
    
    
    public virtual (List<IntPtr> keys, List<IntPtr> values) BuildForInsertion(List<IntPtr> toRelease)
    {
        List<IntPtr> keys = new();
        List<IntPtr> values = new();
        
        if (Synchronizable != null)
        {
            keys.Add(KeychainConstants.KSecAttrSynchronizable);
            values.Add(Synchronizable.Value ? KeychainConstants.KCFBooleanTrue : KeychainConstants.KCFBooleanFalse);
        }

        if (Label != null)
        {
            Validator.IsStringValid(Label, $"{nameof(GenericPasswordOption)}.{nameof(Label)}");

            var cfLabel = KeychainHelpers.CreateCFString(Label);
            toRelease.Add(cfLabel);

            keys.Add(KeychainConstants.KSecAttrLabel);
            values.Add(cfLabel);
        }

        if (Description != null)
        {
            Validator.IsStringValid(Description, $"{nameof(GenericPasswordOption)}.{nameof(Description)}");
            
            var cfDescription = KeychainHelpers.CreateCFString(Description);
            toRelease.Add(cfDescription);
            
            keys.Add(KeychainConstants.KSecAttrDescription);
            values.Add(cfDescription);
        }

        if (Comment != null)
        {
            Validator.IsStringValid(Comment, $"{nameof(GenericPasswordOption)}.{nameof(Comment)}");
            
            var cfComment = KeychainHelpers.CreateCFString(Comment);
            toRelease.Add(cfComment);
            
            keys.Add(KeychainConstants.KSecAttrComment);
            values.Add(cfComment);
        }

        if (AccessGroup != null)
        {
            Validator.IsStringValid(AccessGroup, $"{nameof(GenericPasswordOption)}.{nameof(AccessGroup)}");
            
            var cfAccessGroup = KeychainHelpers.CreateCFString(AccessGroup);
            toRelease.Add(cfAccessGroup);
            
            keys.Add(KeychainConstants.KSecAttrAccessGroup);
            values.Add(cfAccessGroup);
        }

        if (Accessible != null)
        {
            if (Synchronizable == true &&
                (Accessible == AccessibilityOption.WhenUnlockedThisDeviceOnly ||
                 Accessible == AccessibilityOption.AfterFirstUnlockThisDeviceOnly))
            {
                throw new ArgumentException($"Invalid option configuration. Option {nameof(Accessible)} must not be device restricted if {nameof(Synchronizable)} is true.)");
            }
            
            var cfAccessible = Accessible.Value switch
            {
                AccessibilityOption.WhenUnlocked => KeychainConstants.KSecAttrAccessibleWhenUnlocked,
                AccessibilityOption.AfterFirstUnlock => KeychainConstants.KSecAttrAccessibleAfterFirstUnlock,
                AccessibilityOption.Always => KeychainConstants.KSecAttrAccessibleAlways,
                AccessibilityOption.WhenUnlockedThisDeviceOnly => KeychainConstants.KSecAttrAccessibleWhenUnlockedThisDeviceOnly,
                AccessibilityOption.AfterFirstUnlockThisDeviceOnly => KeychainConstants.KSecAttrAccessibleAfterFirstUnlockThisDeviceOnly,
                _ => throw new ArgumentOutOfRangeException(
                    nameof(Accessible),
                    Accessible,
                    $"Option {Accessible} not supported."
                    )
            };
            
            keys.Add(KeychainConstants.KSecAttrAccessible);
            values.Add(cfAccessible);
        }

        if (IsInvisible != null)
        {
            keys.Add(KeychainConstants.KSecAttrIsInvisible);
            values.Add(IsInvisible.Value ? KeychainConstants.KCFBooleanTrue : KeychainConstants.KCFBooleanFalse);
        }

        if (IsNegative != null)
        {
            keys.Add(KeychainConstants.KSecAttrIsNegative);
            values.Add(IsNegative.Value ? KeychainConstants.KCFBooleanTrue : KeychainConstants.KCFBooleanFalse);
        }

        if (Creator != null)
        {
            var cfCreator = KeychainHelpers.CreateCFNumber(Creator.Value);
            toRelease.Add(cfCreator);
            
            keys.Add(KeychainConstants.KSecAttrCreator);
            values.Add(cfCreator);
        }

        if (Type != null)
        {
            var cfType = KeychainHelpers.CreateCFNumber(Type.Value);
            toRelease.Add(cfType);
            
            keys.Add(KeychainConstants.KSecAttrType);
            values.Add(cfType);
        }
        
        return (keys, values);
    }

    public virtual (List<IntPtr> keys, List<IntPtr> values) BuildForQuery(List<IntPtr> toRelease)
    {
        List<IntPtr> keys = new();
        List<IntPtr> values = new();
        
        if (Synchronizable != null)
        {
            keys.Add(KeychainConstants.KSecAttrSynchronizable);
            values.Add(Synchronizable.Value ? KeychainConstants.KCFBooleanTrue : KeychainConstants.KCFBooleanFalse);
        }
        
        if (AccessGroup != null)
        {
            Validator.IsStringValid(AccessGroup, $"{nameof(GenericPasswordOption)}.{nameof(AccessGroup)}");
            
            var cfAccessGroup = KeychainHelpers.CreateCFString(AccessGroup);
            toRelease.Add(cfAccessGroup);
            
            keys.Add(KeychainConstants.KSecAttrAccessGroup);
            values.Add(cfAccessGroup);
        }
        
        return  (keys, values);
    }
}
