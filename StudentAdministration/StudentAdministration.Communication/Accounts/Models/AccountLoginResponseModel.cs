namespace StudentAdministration.Communication.Accounts.Models
{
    public class AccountLoginResponseModel
    {
        public string? Id { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Index { get; set; }

        public string? EmailAddress { get; set; }

        public string? Role { get; set; }

        public string? PartitionKey { get; set; }

        public string? JwtToken { get; set; }
    }
}
