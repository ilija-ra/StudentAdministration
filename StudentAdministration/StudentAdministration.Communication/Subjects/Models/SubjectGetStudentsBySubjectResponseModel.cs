namespace StudentAdministration.Communication.Subjects.Models
{
    public class SubjectGetStudentsBySubjectResponseModel
    {
        public ICollection<SubjectGetStudentsBySubjectItemModel>? Items { get; set; } = new List<SubjectGetStudentsBySubjectItemModel>();
    }

    public class SubjectGetStudentsBySubjectItemModel
    {
        public string? Id { get; set; }

        public string? SubjectId { get; set; }

        public string? SubjectPartitionKey { get; set; }

        public string? StudentId { get; set; }

        public string? StudentPartitionKey { get; set; }

        public string? StudentFullName { get; set; }

        public int? Grade { get; set; }
    }
}
