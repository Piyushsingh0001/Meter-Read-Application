using CabconMAUI.Models;

namespace CabconMAUI.Services.Interfaces;

public interface ICommandRepositoryService
{
    Task<List<CommandDefinition>> LoadCommandsAsync();
    CommandDefinition? GetCommandByTag(string tag);
    List<CommandDefinition> GetCommandsByFeature(MeterReadFeature feature);
}

public class CommandDefinition
{
    public string TagNo { get; set; } = string.Empty;
    public byte[] CommandDataBytes { get; set; } = Array.Empty<byte>();
    public byte ResponseStopByte { get; set; }
    public byte? ResponseStopByte2 { get; set; }
    public MeterReadFeature? AssociatedFeature { get; set; }
}
