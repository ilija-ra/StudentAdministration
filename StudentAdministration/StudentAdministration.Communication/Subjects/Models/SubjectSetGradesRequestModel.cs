﻿namespace StudentAdministration.Communication.Subjects.Models
{
    public class SubjectSetGradesRequestModel
    {
        public string? SubjectId { get; set; }

        public string? SubjectPartitionKey { get; set; }

        public string? StudentId { get; set; }

        public string? StudentPartitionKey { get; set; }

        public int? Grade { get; set; }
    }
}
