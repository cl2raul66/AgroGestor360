using AgroGestor360.Client.Models;
using AgroGestor360.Client.Tools.ReportsTemplate;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
//using QuestPDF.Previewer;
using System.Net;
using System.Text;
using System.Text.Json;

namespace AgroGestor360.Client.Services;

public interface IReportsService
{
    Task<bool> GeneratePDFCustomerQuoteReportAsync(string serverURL, string code, string path = "");
    Task<string> GeneratePDFSaleReportAsync(string serverURL, SaleReport saleReport, string path);
    Task<SaleReport?> GetSaleReportReportAsync(string serverURL, SaleReportParameters dTO);
    //Task<CustomerQuoteReport?> GetCustomerQuoteReportAsync(string serverURL, string code);
}

public class ReportsService : IReportsService
{
    public ReportsService()
    {
        QuestPDF.Settings.EnableCaching = true;
        QuestPDF.Settings.EnableDebugging = true;
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public async Task<bool> GeneratePDFCustomerQuoteReportAsync(string serverURL, string code, string path = "")
    {
        if (!string.IsNullOrWhiteSpace(serverURL) && !string.IsNullOrWhiteSpace(code))
        {
            var customerQuoteReport = await GetCustomerQuoteReport(serverURL, code);
            IDocument document = new QuoteDocument(customerQuoteReport!);

            document.GeneratePdf(path);
            //document.ShowInPreviewer();
            //document.ShowInPreviewer(12345);
            return true;
        }

        return false;
    }

    public async Task<string> GeneratePDFSaleReportAsync(string serverURL, SaleReport saleReport, string path)
    {
        if (!string.IsNullOrWhiteSpace(serverURL) && !string.IsNullOrEmpty(path))
        {
            IDocument document = new SaleDocument(saleReport!);

            string directory = Path.GetDirectoryName(path)!;
            string fileName = Path.GetFileName(path);

            string newPath = Path.Combine(directory, $"{saleReport!.IssueDate:yyyyMMdd} - {fileName}");
            document.GeneratePdf(newPath);
            await Task.CompletedTask;
            return newPath;
        }

        return string.Empty;
    }

    public async Task<SaleReport?> GetSaleReportReportAsync(string serverURL, SaleReportParameters dTO)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var json = JsonSerializer.Serialize(dTO, ApiServiceBase.ProviderJSONOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await ApiServiceBase.ProviderHttpClient!.PostAsync($"{serverURL}/reports/SaleReport", content);

            if (response.StatusCode == HttpStatusCode.NotFound || response.StatusCode == HttpStatusCode.BadRequest)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<SaleReport>(responseContent, ApiServiceBase.ProviderJSONOptions);
        }
        return null;
    }

    //public async Task<CustomerQuoteReport?> GetCustomerQuoteReportAsync(string serverURL, string code) => await GetCustomerQuoteReport(serverURL, code);

    #region EXTRA
    async Task<CustomerQuoteReport?> GetCustomerQuoteReport(string serverURL, string code)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.GetAsync($"{serverURL}/reports/customerquotereport/{code}");

            switch (response.StatusCode)
            {
                case HttpStatusCode.NotFound:
                case HttpStatusCode.BadRequest:
                    return null;
                default:
                    response.EnsureSuccessStatusCode();
                    break;
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<CustomerQuoteReport>(content, ApiServiceBase.ProviderJSONOptions);
        }
        return null;
    }
    #endregion
}
