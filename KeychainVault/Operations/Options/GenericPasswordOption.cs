#nullable enable

namespace KeychainVault.Operations.Options;

public class GenericPasswordOption
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
}
