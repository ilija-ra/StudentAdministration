namespace Client
{
    public sealed class UserSingleton
    {
        public string? Id { get; private set; }

        public string? FirstName { get; private set; }

        public string? LastName { get; private set; }

        public string? Index { get; private set; }

        public string? EmailAddress { get; private set; }

        public string? Role { get; private set; }

        public string? PartitionKey { get; private set; }

        public string? JwtToken { get; private set; }

        public bool? IsInstantiated { get; private set; } = false;

        private static UserSingleton? _instance;
        public static UserSingleton Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new UserSingleton();
                }

                return _instance;
            }
        }

        public void InitializeValues(string id, string firstName, string lastName, string index, string emailAddress, string role, string partitionKey, string jwtToken, bool isInstantiated)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Index = index;
            EmailAddress = emailAddress;
            Role = role;
            PartitionKey = partitionKey;
            JwtToken = jwtToken;
            IsInstantiated = isInstantiated;
        }
    }
}
