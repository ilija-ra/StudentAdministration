namespace StudentAdministration.Communication.Subjects.Models
{
    public class SubjectSetGradesRequestModel
    {
        public string? Id { get; set; }

        public string? SubjectId { get; set; }

        public string? SubjectPartitionKey { get; set; }

        public List<SubjectSetGradesItemModel> StudentGrades { get; set; } = new List<SubjectSetGradesItemModel>();
    }

    public class SubjectSetGradesItemModel
    {
        public string? Id { get; set; }

        public string? StudentId { get; set; }

        public string? StudentPartitionKey { get; set; }

        public int? Grade { get; set; }
    }
}
