using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PetInsurance.Exceptions;
using PetInsurance.Services.Interfaces;

namespace PetInsurance.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ExcelController : ControllerBase
{
    private readonly IExcelService _excelService;

    public ExcelController(IExcelService excelService)
    {
        _excelService = excelService;
    }

    [HttpPost("quotes/import")]
    public async Task<IActionResult> ImportQuotes(IFormFile file)
    {
        if (file is null || file.Length == 0)
        {
            throw new BusinessRuleException("Please provide a valid Excel file.");
        }

        if (!file.FileName.EndsWith(".xlsx", System.StringComparison.OrdinalIgnoreCase))
        {
            throw new BusinessRuleException("Only .xlsx Excel files are supported.");
        }

        using var stream = file.OpenReadStream();
        var (importedQuotes, importedCustomers, importedPets) = await _excelService.ImportQuotesFromExcelAsync(stream);

        return Ok(new
        {
            Message = "Excel data imported successfully.",
            ImportedQuotes = importedQuotes,
            ImportedCustomers = importedCustomers,
            ImportedPets = importedPets
        });
    }

    [HttpGet("quotes/export")]
    public async Task<IActionResult> ExportQuotes()
    {
        var fileBytes = await _excelService.ExportQuotesToExcelAsync();
        return File(
            fileBytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"PetInsurance_Quotes_{System.DateTime.UtcNow:yyyyMMddHHmmss}.xlsx");
    }
}
