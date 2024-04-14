namespace Client.Models.Reports
{
    public class ReportViewModel
    {
        public double? AverageGrade { get; set; }

        public ICollection<SubjectViewModelItem>? Subjects { get; set; } = new List<SubjectViewModelItem>();
    }

    public class SubjectViewModelItem
    {
        public string? Id { get; set; }

        public string? SubjectId { get; set; }

        public string? SubjectTitle { get; set; }

        public string? SubjectDepartment { get; set; }

        public int? SubjectGrade { get; set; }

        public string? ProfessorFullName { get; set; }
    }
}
