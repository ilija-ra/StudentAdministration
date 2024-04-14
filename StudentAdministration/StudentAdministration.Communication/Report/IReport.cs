using Microsoft.ServiceFabric.Services.Remoting;
using StudentAdministration.Communication.Report.Models;

namespace StudentAdministration.Communication.Report
{
    public interface IReport : IService
    {
        Task<ReportGenerateReportResponseModel> GenerateReport(string? studentId);
    }
}
