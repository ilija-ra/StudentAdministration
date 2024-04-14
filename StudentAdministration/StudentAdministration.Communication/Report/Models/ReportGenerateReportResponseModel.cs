namespace StudentAdministration.Communication.Report.Models
{
    public class ReportGenerateReportResponseModel
    {
        public double? AverageGrade { get; set; }

        public ICollection<ReportGenerateReportItemModel>? Subjects { get; set; } = new List<ReportGenerateReportItemModel>();
    }

    public class ReportGenerateReportItemModel
    {
        public string? Id { get; set; }

        public string? SubjectId { get; set; }

        public string? SubjectTitle { get; set; }

        public string? SubjectDepartment { get; set; }

        public int? SubjectGrade { get; set; }

        public string? ProfessorFullName { get; set; }
    }
}
