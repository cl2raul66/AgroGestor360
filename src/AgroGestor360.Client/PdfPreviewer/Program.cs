using AgroGestor360.Client.Services;
using System.Diagnostics;

IApiService apiService = new ApiService();
apiService.ConnectToHttpClient();
apiService.SetClientAccessToken("38D941C88617485496B07AF837C5E64E");

IQuotesService quotesService = new QuotesService();
IReportsService reportsService = new ReportsService();
string serverURL = "http://localhost:5010";

var quotations = await quotesService.GetAllAsync(serverURL);
var SelectedQuotation = quotations.FirstOrDefault();
var code = SelectedQuotation?.Code;

if (SelectedQuotation is null)
{
    Console.WriteLine("No hay cotizaciones disponibles");
    return;
}

string title = $"{SelectedQuotation.QuotationDate:yyyyMMdd} - Cotización de {SelectedQuotation.TotalAmount:F2} para {SelectedQuotation.CustomerName}";
Console.WriteLine($"El titulo del fichero sera este: {title}");

string cacheDirectory = Path.GetTempPath();
string filePath = Path.Combine(cacheDirectory, title + ".pdf");
Console.WriteLine($"El fichero se guardara en: {filePath}");

bool result = await reportsService.GeneratePDFCustomerQuoteReportAsync(serverURL, code!, filePath);
Console.WriteLine(result ? "El fichero se ha generado correctamente" : "El fichero no se genero");

if (result)
{
    Console.WriteLine("Abriendo el fichero...");
    Process.Start(new ProcessStartInfo("cmd", $"/c start \"\" \"{filePath}\"") { CreateNoWindow = true });
}

Console.WriteLine("Pulse una tecla para salir...");
Console.ReadKey();
