using System.IO;
using System.Threading.Tasks;

namespace PetInsurance.Services.Interfaces;

public interface IExcelService
{
    Task<byte[]> ExportQuotesToExcelAsync();
    Task<(int importedQuotes, int importedCustomers, int importedPets)> ImportQuotesFromExcelAsync(Stream fileStream);
}
