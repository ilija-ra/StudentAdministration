using System.ComponentModel.DataAnnotations;

namespace StudentAdministration.Client.Models.Subjects
{
    public class GetAllSubjectsByProfessorViewModelResponse
    {
        public ICollection<GetAllSubjectsByProfessorViewModelItem>? Items { get; set; } = new List<GetAllSubjectsByProfessorViewModelItem>();
    }

    public class GetAllSubjectsByProfessorViewModelItem
    {
        [Display(Name = "Subject id")]
        public string? SubjectId { get; set; }

        [Display(Name = "Subject partition key")]
        public string? SubjectPartitionKey { get; set; }

        public string? Title { get; set; }

        public string? Department { get; set; }

        [Display(Name = "Professor id")]
        public string? ProfessorId { get; set; }

        [Display(Name = "Professor partition key")]
        public string? ProfessorPartitionKey { get; set; }

        [Display(Name = "Professor full name")]
        public string? ProfessorFullName { get; set; }
    }
}
