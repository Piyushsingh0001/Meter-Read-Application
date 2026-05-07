using System.Text;
using System.Text.Json;
using System.Xml.Serialization;
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
    }

    static string EscapeCsv(string text)
    {
        if (text.Contains(',') || text.Contains('"') || text.Contains('\n'))
        {
            return $"\"{text.Replace("\"", "\"\"")}\"";
        }
        return text;
    }

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

