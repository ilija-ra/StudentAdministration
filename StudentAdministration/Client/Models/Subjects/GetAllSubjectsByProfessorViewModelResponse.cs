namespace StudentAdministration.Client.Models.Subjects
{
    public class GetAllSubjectsByProfessorViewModelResponse
    {
        public ICollection<GetAllSubjectsByProfessorViewModelItem>? Items { get; set; } = new List<GetAllSubjectsByProfessorViewModelItem>();
    }

    public class GetAllSubjectsByProfessorViewModelItem
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
