using AgroGestor360.Client.Models;
using AgroGestor360.Client.Services;
using System.Diagnostics;

IApiService apiService = new ApiService();
apiService.ConnectToHttpClient();
apiService.SetClientDevicePlatform(Environment.OSVersion.ToString());
apiService.SetClientAccessToken("38D941C88617485496B07AF837C5E64E");

//IQuotesService quotesService = new QuotesService();
//IInvoicesService invoicesService = new InvoicesService();
IReportsService reportsService = new ReportsService();
string serverURL = "http://localhost:5010";

//var quotations = await quotesService.GetAllAsync(serverURL);
//var SelectedQuotation = quotations.FirstOrDefault();
//var code = SelectedQuotation?.Code;

Dictionary<int, string> resportState = new()
{
    {1,"Todos"},
    {2,"Pagadas"},
    {3,"Pendientes"},
    {4,"Canceladas"},
};
SaleReportParameters code = new(resportState[1], "NONE", new DateTime(2024,06,01), DateTime.Now, null, null);

//if (SelectedQuotation is null)
//{
//    Console.WriteLine("No hay cotizaciones disponibles");
//    return;
//}

//string title = $"{SelectedQuotation.Date:yyyyMMdd} - Cotización de {SelectedQuotation.TotalAmount:F2} para {SelectedQuotation.CustomerName}";
string title = "REPORTE DE VENTAS";
Console.WriteLine($"El titulo del fichero sera este: {title}");

string cacheDirectory = Path.GetTempPath();
string filePath = Path.Combine(cacheDirectory, title + ".pdf");
Console.WriteLine($"El fichero se guardara en: {filePath}");

//bool result = await reportsService.GeneratePDFCustomerQuoteReportAsync(serverURL, code!, filePath);
string result = await reportsService.GeneratePDFSaleReportAsync(serverURL, code, filePath);
Console.WriteLine(string.IsNullOrEmpty(result) ? "El fichero no se genero" : "El fichero se ha generado correctamente");

//if (result)
//{
//    Console.WriteLine("Abriendo el fichero...");
//    //filePath = Path.Combine(cacheDirectory, $"{} - {title}.pdf");
//    Process.Start(new ProcessStartInfo("cmd", $"/c start \"\" \"{filePath}\"") { CreateNoWindow = true });
//}

Console.WriteLine("Pulse una tecla para salir...");
Console.ReadKey();
