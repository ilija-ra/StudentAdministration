﻿namespace StudentAdministration.Communication.Subjects.Models
{
    public class SubjectGetAllEnrolledResponseModel
    {
        public ICollection<SubjectGetAllEnrolledItemModel>? Items { get; set; } = new List<SubjectGetAllEnrolledItemModel>();
    }

    public class SubjectGetAllEnrolledItemModel
    {
        public string? Id { get; set; }

        public SubjectItemModel? Subject { get; set; }

        public string? StudentId { get; set; }

        public string? StudentPartitionKey { get; set; }

        public string? ProfessorFullName { get; set; }
    }

    public class SubjectItemModel
    {
        public string? Id { get; set; }

        public string? Title { get; set; }

        public string? Department { get; set; }

        public int? Grade { get; set; }

        public string? PartitionKey { get; set; }
    }
}
