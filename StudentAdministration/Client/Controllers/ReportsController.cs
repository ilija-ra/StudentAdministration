using Client.Models.Reports;
using FastReport;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Client.Controllers
{
    public class ReportsController : Controller
    {
        private readonly HttpClient _httpClient;

        public ReportsController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("http://localhost:8645");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", UserSingleton.Instance.JwtToken);
        }

        [HttpGet]
        [Route("GenerateReport")]
        public async Task<IActionResult> GenerateReport()
        {
            var response = await _httpClient.GetAsync($"/Reports/GenerateReport/{UserSingleton.Instance.Id}");

            if (!response.IsSuccessStatusCode)
            {
                return View("Error");
            }

            var jsonResult = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ReportViewModel>(jsonResult, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (result is null)
            {
                return View("Error");
            }

            FastReport.Utils.Config.WebMode = true;
            Report report = new Report();

            report.Load(@"C:/GithubRepositoriesFaculty/StudentAdministration/StudentAdministration/Client/StudentReport.frx");
            report.SetParameterValue("StudentFullName", $"{UserSingleton.Instance.FirstName} {UserSingleton.Instance.LastName}");
            report.SetParameterValue("AverageGrade", $"{result.AverageGrade.ToString()}");
            report.RegisterData(result.Subjects, "SubjectsRef");

            if (report.Report.Prepare())
            {
                FastReport.Export.PdfSimple.PDFSimpleExport pdfExport = new FastReport.Export.PdfSimple.PDFSimpleExport();
                pdfExport.ShowProgress = false;
                pdfExport.Subject = "Subject Report";
                pdfExport.Title = "Student Report";
                MemoryStream ms = new MemoryStream();
                report.Report.Export(pdfExport, ms);
                report.Dispose();
                pdfExport.Dispose();
                ms.Position = 0;

                return File(ms, "application/pdf", "myreport.pdf");
            }
            else
            {
                return null!;
            }
        }
    }
}
