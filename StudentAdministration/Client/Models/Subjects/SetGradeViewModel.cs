namespace Client.Models.Subjects
{
    public class SetGradeViewModel
    {
        public string? SubjectId { get; set; }

        public string? SubjectPartitionKey { get; set; }

        public string? StudentId { get; set; }

        public string? StudentPartitionKey { get; set; }

        public int? Grade { get; set; }
    }
}
