using System.Text;
using System.Text.Json;
using System.Xml.Serialization;
<<<<<<< HEAD
using Microsoft.Maui.Storage;
=======
>>>>>>> bf49aa6198f381896113163ead8b83e92944c023
using CabconMAUI.Models;
using CabconMAUI.Services.Interfaces;

namespace CabconMAUI.Services;

public sealed class ReadExportService : IReadExportService
{
    public async Task<string> ExportAsync(MeterReadResult data, ExportFormat format)
    {
        var root = Path.Combine(FileSystem.AppDataDirectory, "exports");
        Directory.CreateDirectory(root);

        var ts = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        var ext = format == ExportFormat.Csv ? "csv" : "xml";
        var path = Path.Combine(root, $"meter_read_{ts}.{ext}");

        if (format == ExportFormat.Csv)
        {
<<<<<<< HEAD
            var content = GenerateCsvContent(data);
            await File.WriteAllTextAsync(path, content);
        }
        else
        {
            var content = GenerateXmlContent(data);
            await File.WriteAllTextAsync(path, content);
        }

        // Share the file if possible
        await ShareFileAsync(path, format);
        return path;
    }

    public async Task<string> ExportAndShareAsync(MeterReadResult data, ExportFormat format)
    {
        var path = await ExportAsync(data, format);
        await ShareFileAsync(path, format);
        return path;
    }

    public async Task<bool> ShareFileAsync(string filePath, ExportFormat format)
    {
        try
        {
            await Share.Default.RequestAsync(new ShareFileRequest
            {
                Title = $"Meter Read {format.ToString().ToUpper()} Export",
                File = new ShareFile(filePath)
            });
            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Share failed: {ex.Message}");
            return false;
        }
    }

    private string GenerateCsvContent(MeterReadResult data)
    {
        var sb = new StringBuilder();
        
        // Header with metadata
        sb.AppendLine("# Meter Read Export");
        sb.AppendLine($"# Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        sb.AppendLine($"# Source: {data.Source}");
        sb.AppendLine($"# Success: {data.IsSuccess}");
        sb.AppendLine($"# Message: {data.Message}");
        sb.AppendLine();

        // Group values by feature for better organization
        var groupedValues = data.Values
            .GroupBy(kv => kv.Key.Contains(":") ? kv.Key.Split(":")[0] : "General")
            .OrderBy(g => g.Key);

        foreach (var group in groupedValues)
        {
            sb.AppendLine($"# {group.Key}");
            foreach (var kv in group.OrderBy(x => x.Key))
            {
                var key = EscapeCsv(kv.Key.Contains(":") ? kv.Key.Split(":")[1] : kv.Key);
                var value = EscapeCsv(kv.Value);
                sb.AppendLine($"{key},{value}");
            }
            sb.AppendLine();
        }

        return sb.ToString();
    }

    private string GenerateXmlContent(MeterReadResult data)
    {
        var sb = new StringBuilder();
        sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        sb.AppendLine("<MeterReadExport>");
        sb.AppendLine($"  <Metadata>");
        sb.AppendLine($"    <Generated>{DateTime.Now:yyyy-MM-ddTHH:mm:ss}</Generated>");
        sb.AppendLine($"    <Source>{EscapeXml(data.Source)}</Source>");
        sb.AppendLine($"    <Success>{data.IsSuccess}</Success>");
        sb.AppendLine($"    <Message>{EscapeXml(data.Message)}</Message>");
        sb.AppendLine($"  </Metadata>");
        
        if (data.RawBuffer?.Length > 0)
        {
            sb.AppendLine($"  <RawData>");
            sb.AppendLine($"    <Hex>{Convert.ToHexString(data.RawBuffer)}</Hex>");
            sb.AppendLine($"    <Size>{data.RawBuffer.Length}</Size>");
            sb.AppendLine($"  </RawData>");
        }

        sb.AppendLine($"  <Values>");
        
        var groupedValues = data.Values
            .GroupBy(kv => kv.Key.Contains(":") ? kv.Key.Split(":")[0] : "General")
            .OrderBy(g => g.Key);

        foreach (var group in groupedValues)
        {
            sb.AppendLine($"    <Group name=\"{EscapeXml(group.Key)}\">");
            foreach (var kv in group.OrderBy(x => x.Key))
            {
                var key = kv.Key.Contains(":") ? kv.Key.Split(":")[1] : kv.Key;
                sb.AppendLine($"      <Item>");
                sb.AppendLine($"        <Key>{EscapeXml(key)}</Key>");
                sb.AppendLine($"        <Value>{EscapeXml(kv.Value)}</Value>");
                sb.AppendLine($"      </Item>");
            }
            sb.AppendLine($"    </Group>");
        }
        
        sb.AppendLine($"  </Values>");
        sb.AppendLine("</MeterReadExport>");
        
        return sb.ToString();
=======
            var sb = new StringBuilder();
            sb.AppendLine("Key,Value");
            foreach (var kv in data.Values)
            {
                var key = EscapeCsv(kv.Key);
                var value = EscapeCsv(kv.Value);
                sb.AppendLine($"{key},{value}");
            }

            await File.WriteAllTextAsync(path, sb.ToString());
            return path;
        }

        var dto = new ExportEnvelope
        {
            Source = data.Source,
            Message = data.Message,
            IsSuccess = data.IsSuccess,
            Items = data.Values.Select(kv => new ExportItem { Key = kv.Key, Value = kv.Value }).ToList(),
            RawHex = Convert.ToHexString(data.RawBuffer)
        };

        await using var stream = File.Create(path);
        var serializer = new XmlSerializer(typeof(ExportEnvelope));
        serializer.Serialize(stream, dto);
        await stream.FlushAsync();
        return path;
>>>>>>> bf49aa6198f381896113163ead8b83e92944c023
    }

    static string EscapeCsv(string text)
    {
        if (text.Contains(',') || text.Contains('"') || text.Contains('\n'))
        {
            return $"\"{text.Replace("\"", "\"\"")}\"";
        }
        return text;
    }

<<<<<<< HEAD
    static string EscapeXml(string text)
    {
        if (string.IsNullOrEmpty(text)) return text;
        
        return text.Replace("&", "&amp;")
                 .Replace("<", "&lt;")
                 .Replace(">", "&gt;")
                 .Replace("\"", "&quot;")
                 .Replace("'", "&apos;");
    }

=======
>>>>>>> bf49aa6198f381896113163ead8b83e92944c023
    public sealed class ExportEnvelope
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
        public string RawHex { get; set; } = string.Empty;
        public List<ExportItem> Items { get; set; } = new();
    }

    public sealed class ExportItem
    {
        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }
}

