#nullable enable
using System;

namespace KeychainVault.Model;

public class KeychainItemData
{
    public byte[] Secret { get; init; } = Array.Empty<byte>();
    public string? Account { get; init; }
    public string? Service { get; init; }
    public string? Label { get; init; }
    public string? Comment { get; init; }
    public string? Description { get; init; }
}