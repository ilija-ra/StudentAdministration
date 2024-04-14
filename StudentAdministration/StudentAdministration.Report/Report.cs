using Azure.Data.Tables;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using StudentAdministration.Communication.Report;
using StudentAdministration.Communication.Report.Models;
using StudentAdministration.Communication.Subjects.Entities;
using System.Fabric;

namespace StudentAdministration.Report
{
    internal sealed class Report : StatelessService, IReport
    {
        private const string _gradeName = "Grades";
        private const string _subjectName = "Subjects";
        private const string _connectionString = "DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;";
        private static TableServiceClient _serviceClient = new TableServiceClient(_connectionString);
        private static TableClient _subjectTableClient = _serviceClient.GetTableClient(_subjectName);
        private static TableClient _gradeTableClient = _serviceClient.GetTableClient(_gradeName);

        public Report(StatelessServiceContext context)
            : base(context)
        { }

        #region IReportImplementation

        public async Task<ReportGenerateReportResponseModel> GenerateReport(string? studentId)
        {
            try
            {
                var subjects = new List<ReportGenerateReportItemModel>();
                var gradeEntities = _gradeTableClient.QueryAsync<GradeEntity>(x => x.StudentId!.Equals(studentId)).ToBlockingEnumerable().Where(x => x.Grade > 5);
                var subjectEntities = _subjectTableClient.QueryAsync<SubjectEntity>().ToBlockingEnumerable();

                if (gradeEntities.Count() == 0)
                {
                    return new ReportGenerateReportResponseModel()
                    {
                        AverageGrade = 0,
                        Subjects = subjects
                    };
                }

                foreach (var entity in gradeEntities)
                {
                    var subject = subjectEntities.FirstOrDefault(x => x.Id == entity.SubjectId && x.PartitionKey == entity.SubjectPartitionKey);

                    subjects.Add(new ReportGenerateReportItemModel()
                    {
                        Id = entity.Id,
                        SubjectId = entity.SubjectId,
                        SubjectTitle = subject?.Title,
                        SubjectDepartment = subject?.Department,
                        SubjectGrade = entity?.Grade,
                        ProfessorFullName = entity?.ProfessorFullName
                    });
                }

                return new ReportGenerateReportResponseModel()
                {
                    AverageGrade = Math.Round((double)gradeEntities.Sum(x => x.Grade)! / gradeEntities.Count(), 2),
                    Subjects = subjects
                };
            }
            catch
            {
                return null!;
            }
        }

        #endregion

        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return this.CreateServiceRemotingInstanceListeners();
        }

        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            long iterations = 0;

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                ServiceEventSource.Current.ServiceMessage(this.Context, "Working-{0}", ++iterations);

                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }
    }
}
