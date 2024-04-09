namespace StudentAdministration.Communication.Users.Models
{
    public class UserUpdateRequestModel
    {
        public string? Id { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? PartitionKey { get; set; }
    }
}
