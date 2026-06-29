using System.Reflection;
using System.Xml;
using CabconMAUI.Models;

namespace CabconMAUI.Services;

public class MeterCommandRepository
{
    private readonly Dictionary<string, List<MeterCommand>> _commands = new();

    public async Task<List<MeterCommand>> GetInstantaneousCommandsAsync()
    {
        var cacheKey = "Instantaneous";
        if (_commands.ContainsKey(cacheKey))
            return _commands[cacheKey];

        var commands = await LoadCommandsFromXml("Instantaneous_c.xml");
        _commands[cacheKey] = commands;
        return commands;
    }

    public async Task<List<MeterCommand>> GetBillingCommandsAsync()
    {
        var cacheKey = "Billing";
        if (_commands.ContainsKey(cacheKey))
            return _commands[cacheKey];

        var commands = await LoadCommandsFromXml("Billing.xml");
        _commands[cacheKey] = commands;
        return commands;
    }

    public async Task<List<MeterCommand>> GetTamperCommandsAsync()
    {
        var cacheKey = "Tamper";
        if (_commands.ContainsKey(cacheKey))
            return _commands[cacheKey];

        var commands = await LoadCommandsFromXml("Tamper.xml");
        _commands[cacheKey] = commands;
        return commands;
    }

    public async Task<List<MeterCommand>> GetLoadSurveyCommandsAsync()
    {
        var cacheKey = "LoadSurvey";
        if (_commands.ContainsKey(cacheKey))
            return _commands[cacheKey];

        var commands = await LoadCommandsFromXml("LoadSurvey.xml");
        _commands[cacheKey] = commands;
        return commands;
    }

    public async Task<List<MeterCommand>> GetDailyProfileCommandsAsync()
    {
        var cacheKey = "DailyProfile";
        if (_commands.ContainsKey(cacheKey))
            return _commands[cacheKey];

        var commands = await LoadCommandsFromXml("DailyProfile.xml");
        _commands[cacheKey] = commands;
        return commands;
    }

    private async Task<List<MeterCommand>> LoadCommandsFromXml(string fileName)
    {
        var commands = new List<MeterCommand>();

        try
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"CabconMAUI.Resources.Raw.Configuration.{fileName}";

            using var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null)
            {
                System.Diagnostics.Debug.WriteLine($"[Repository] XML file not found: {resourceName}");
                return commands;
            }

            using var reader = new StreamReader(stream);
            var xmlContent = await reader.ReadToEndAsync();

            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlContent);

            var nodes = xmlDoc.SelectNodes("//DLMS");
            if (nodes != null)
            {
                foreach (XmlNode node in nodes)
                {
                    var command = ParseDlmsNode(node);
                    if (command != null)
                    {
                        commands.Add(command);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Repository] Error loading {fileName}: {ex.Message}");
        }

        return commands;
    }

    private MeterCommand? ParseDlmsNode(XmlNode node)
    {
        try
        {
            var classNode = node.SelectSingleNode("Class");
            var obisNode = node.SelectSingleNode("ObisCode");
            var attributeNode = node.SelectSingleNode("Attribute");
            var scaleNode = node.SelectSingleNode("Scale");
            var unitNode = node.SelectSingleNode("Unit");

            if (classNode?.InnerText == null || obisNode?.InnerText == null || attributeNode?.InnerText == null)
                return null;

            var command = new MeterCommand
            {
                Name = GenerateCommandName(obisNode.InnerText, unitNode?.InnerText),
                ObisCode = ParseObisCode(obisNode.InnerText),
                Class = byte.Parse(classNode.InnerText),
                Attribute = byte.Parse(attributeNode.InnerText),
                Scale = double.TryParse(scaleNode?.InnerText, out var scale) ? scale : 1.0,
                Unit = unitNode?.InnerText ?? string.Empty
            };

            return command;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Repository] Error parsing DLMS node: {ex.Message}");
            return null;
        }
    }

    private string GenerateCommandName(string obisCode, string? unit)
    {
        var obisStr = $"{obisCode[0]}-{obisCode[1]}:{obisCode[2]}.{obisCode[3]}.{obisCode[4]}.{obisCode[5]}";
        
        return obisStr switch
        {
            "0-0:96.1.0.255" => "Meter Number",
            "1-0:1.8.0.255" => "Energy Import",
            "1-0:2.8.0.255" => "Energy Export",
            "1-0:1.7.0.255" => "Active Power Total",
            "1-0:2.7.0.255" => "Reactive Power Total",
            "1-0:32.7.0.255" => "Voltage L1",
            "1-0:52.7.0.255" => "Voltage L2",
            "1-0:72.7.0.255" => "Voltage L3",
            "1-0:31.7.0.255" => "Current L1",
            "1-0:51.7.0.255" => "Current L2",
            "1-0:71.7.0.255" => "Current L3",
            "1-0:14.7.0.255" => "Power Factor",
            "1-0:12.7.0.255" => "Frequency",
            "0-0:43.0.0.255" => "Clock",
            _ => $"Parameter {obisStr}"
        };
    }

    private byte[] ParseObisCode(string obisCode)
    {
        var bytes = new byte[6];
        for (int i = 0; i < 6 && i < obisCode.Length; i += 2)
        {
            if (i + 1 < obisCode.Length)
            {
                bytes[i / 2] = Convert.ToByte(obisCode.Substring(i, 2), 16);
            }
        }
        return bytes;
    }
}

public class MeterCommand
{
    public string Name { get; set; } = string.Empty;
    public byte[] ObisCode { get; set; } = Array.Empty<byte>();
    public byte Class { get; set; }
    public byte Attribute { get; set; }
    public double Scale { get; set; } = 1.0;
    public string Unit { get; set; } = string.Empty;
}
