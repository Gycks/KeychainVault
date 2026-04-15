#nullable enable
namespace KeychainVault.Operations.Options;

public class InternetPasswordOption : GenericPasswordOption
{
    public string? SecurityDomain { get; init; }
    public InternetProtocolOption? Protocol { get; init; }
    public int? Port { get; init; }
    public string? Path { get; init; }
    public InternetAuthenticationTypeOption? AuthenticationType { get; init; }
    public GenericPasswordOption? Generics { get; init; }
    
}