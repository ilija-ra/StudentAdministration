﻿namespace StudentAdministration.Communication.Users.Models
{
    public class UserGetByIdResponseModel
    {
        public string? Id { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Index { get; set; }

        public string? EmailAddress { get; set; }

        public string? PartitionKey { get; set; }
    }
}
