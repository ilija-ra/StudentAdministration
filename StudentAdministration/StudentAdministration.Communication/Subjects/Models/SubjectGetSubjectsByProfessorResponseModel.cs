namespace StudentAdministration.Communication.Subjects.Models
{
    public class SubjectGetSubjectsByProfessorResponseModel
    {
        public ICollection<SubjectGetSubjectsByProfessorItemModel>? Items { get; set; } = new List<SubjectGetSubjectsByProfessorItemModel>();
    }

    public class SubjectGetSubjectsByProfessorItemModel
    {
        public string? SubjectId { get; set; }

        public string? SubjectPartitionKey { get; set; }

        public string? Title { get; set; }

        public string? Department { get; set; }

        public string? ProfessorId { get; set; }

        public string? ProfessorPartitionKey { get; set; }

        public string? ProfessorFullName { get; set; }
    }
}
