using AgroGestor360.Client.Models;
using AgroGestor360.Client.Tools.ReportsTemplate;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
//using QuestPDF.Previewer;
using System.Net;
using System.Text.Json;

namespace AgroGestor360.Client.Services;

public interface IReportsService
{
    Task<bool> GeneratePDFCustomerQuoteReportAsync(string serverURL, string code, string path = "");
    //Task<CustomerQuoteReport?> GetCustomerQuoteReportAsync(string serverURL, string code);
}

public class ReportsService : IReportsService
{
    public ReportsService()
    {
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
