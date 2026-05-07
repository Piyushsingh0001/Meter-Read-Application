using System.Xml;
using CabconMAUI.Models;
using CabconMAUI.Services.Interfaces;

namespace CabconMAUI.Services;

public class CommandRepositoryService : ICommandRepositoryService
{
    private readonly List<CommandDefinition> _commands = new();
    private readonly Dictionary<string, CommandDefinition> _commandByTag = new();
    private readonly Dictionary<MeterReadFeature, List<CommandDefinition>> _commandsByFeature = new();

    public async Task<List<CommandDefinition>> LoadCommandsAsync()
    {
        if (_commands.Any())
        {
            return _commands.ToList();
        }

        await Task.Run(() => LoadFromXml());
        return _commands.ToList();
    }

    public CommandDefinition? GetCommandByTag(string tag)
    {
        _commandByTag.TryGetValue(tag, out var command);
        return command;
    }

    public List<CommandDefinition> GetCommandsByFeature(MeterReadFeature feature)
    {
        _commandsByFeature.TryGetValue(feature, out var commands);
        return commands?.ToList() ?? new List<CommandDefinition>();
    }

    private void LoadFromXml()
    {
        try
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var resourceName = "CabconMAUI.Resources.Raw.Configuration.1PCommandRepository.xml";
            
            using var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null)
            {
                throw new FileNotFoundException($"Resource not found: {resourceName}");
            }

            var doc = new XmlDocument();
            doc.Load(stream);

            var commandNodes = doc.SelectNodes("//COMMAND");
            if (commandNodes == null) return;

            foreach (XmlNode node in commandNodes)
            {
                var command = ParseCommandNode(node);
                if (command != null)
                {
                    _commands.Add(command);
                    _commandByTag[command.TagNo] = command;

                    // Map commands to features
                    var feature = MapTagToFeature(command.TagNo);
                    if (feature.HasValue)
                    {
                        command.AssociatedFeature = feature.Value;
                        if (!_commandsByFeature.ContainsKey(feature.Value))
                        {
                            _commandsByFeature[feature.Value] = new List<CommandDefinition>();
                        }
                        _commandsByFeature[feature.Value].Add(command);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to load command repository: {ex.Message}", ex);
        }
    }

    private CommandDefinition? ParseCommandNode(XmlNode node)
    {
        try
        {
            var tagNode = node.SelectSingleNode("TAGNO");
            var commandBytesNode = node.SelectSingleNode("CommandDataBytes");
            var stopByteNode = node.SelectSingleNode("ResponseStopByte");
            var stopByte2Node = node.SelectSingleNode("ResponseStopByte_2");

            if (tagNode?.InnerText == null || commandBytesNode?.InnerText == null || stopByteNode?.InnerText == null)
            {
                return null;
            }

            var command = new CommandDefinition
            {
                TagNo = tagNode.InnerText.Trim(),
                CommandDataBytes = ParseHexString(commandBytesNode.InnerText.Trim()),
                ResponseStopByte = byte.Parse(stopByteNode.InnerText.Trim(), System.Globalization.NumberStyles.HexNumber)
            };

            if (stopByte2Node?.InnerText != null && !string.IsNullOrWhiteSpace(stopByte2Node.InnerText))
            {
                command.ResponseStopByte2 = byte.Parse(stopByte2Node.InnerText.Trim(), System.Globalization.NumberStyles.HexNumber);
            }

            return command;
        }
        catch
        {
            return null;
        }
    }

    private byte[] ParseHexString(string hex)
    {
        hex = hex.Replace(".", "");
        return Enumerable.Range(0, hex.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                     .ToArray();
    }

    private MeterReadFeature? MapTagToFeature(string tag)
    {
        return tag switch
        {
            "IECReadoutAssociation" => MeterReadFeature.Instantaneous,
            "TamperCommand" => MeterReadFeature.Tamper,
            "DailyProfileCommand" => MeterReadFeature.DailyProfile,
            "LoadProfileCommand" => MeterReadFeature.LoadSurvey,
            "MeterSignon" => MeterReadFeature.Nameplate,
            "AccessAssociation" => MeterReadFeature.Billing,
            _ => null
        };
    }
}
