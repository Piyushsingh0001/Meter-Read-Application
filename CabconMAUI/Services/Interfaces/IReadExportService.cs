using CabconMAUI.Models;

namespace CabconMAUI.Services.Interfaces;

public interface IReadExportService
{
    Task<string> ExportAsync(MeterReadResult data, ExportFormat format);
}

