using Azure.Data.Tables;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using OnlineStore.Communication.Account;
using StudentAdministration.Communication.Accounts.Models;
using StudentAdministration.Communication.Users;
using StudentAdministration.Communication.Users.Models;
using System.Fabric;
using System.Security.Cryptography;
using System.Text;

namespace StudentAdministration.User
{
    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    internal sealed class User : StatelessService, IUser, IAccount
    {
        public User(StatelessServiceContext context)
            : base(context)
        { }

        private const string _connectionString = "DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;";
        private static TableServiceClient _serviceClient = new TableServiceClient(_connectionString);
        private static TableClient _tableClient = _serviceClient.GetTableClient("Users");

        #region IAccountImplementation

        public async Task<AccountLoginResponseModel> Login(AccountLoginRequestModel? model)
        {
            TableEntity foundUser = null!;
            var users = _tableClient.QueryAsync<TableEntity>();

            await foreach (var user in users)
            {
                if (user["EmailAddress"].ToString() == model?.EmailAddress)
                {
                    foundUser = user;
                }
            }

            if (foundUser is null)
            {
                return null!;
            }

            if (!PasswordHasher.VerifyPassword(foundUser["Password"].ToString()!, foundUser["Salt"].ToString()!, model?.Password!))
            {
                return null!;
            }

            return new AccountLoginResponseModel() { JwtToken = null };
        }

        public async Task<AccountRegisterResponseModel> Register(AccountRegisterRequestModel? model)
        {
            try
            {
                var users = _tableClient.QueryAsync<TableEntity>();

                await foreach (var user in users)
                {
                    if (user["EmailAddress"].ToString() == model?.EmailAddress)
                    {
                        return null!;
                    }
                }

                model!.Id = Guid.NewGuid().ToString();
                (string hashedPassword, string salt) = PasswordHasher.HashPassword(model?.Password!);

                var userEntity = new TableEntity(AssignToPartition(model!.Id).ToString(), model!.Id)
                {
                    { "Id", model?.Id },
                    { "FirstName", model?.FirstName },
                    { "LastName", model?.LastName },
                    { "Index", model?.Index },
                    { "EmailAddress", model?.EmailAddress },
                    { "Password", hashedPassword },
                    { "Salt", salt },
                    { "Role", "Student" }
                };

                await _tableClient.UpsertEntityAsync(userEntity);

                return new AccountRegisterResponseModel();
            }
            catch
            {
                return null!;
            }
        }

        #endregion

        #region IUserImplementation

        public Task<UserUpdateResponseModel> Update(UserUpdateRequestModel? model)
        {
            throw new NotImplementedException();
        }

        public Task<UserGetByIdResponseModel> GetById(string? userId)
        {
            throw new NotImplementedException();
        }

        public Task<UserGetInitialsResponseModel> GetInitials(string? userId)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region PrivateHelpers

        private async Task InitializeTable()
        {
            await _tableClient.CreateIfNotExistsAsync();
        }

        private async Task PopulateProfessors()
        {
            try
            {
                var profId1 = Guid.NewGuid().ToString();
                (string hashedPassword1, string salt1) = PasswordHasher.HashPassword("Proba.123");

                await _tableClient.UpsertEntityAsync(new TableEntity(AssignToPartition(profId1).ToString(), profId1)
                {
                    { "Id", profId1 },
                    { "FirstName", "Samantha" },
                    { "LastName", "Rodriguez" },
                    { "Index", null },
                    { "EmailAddress", "samantha@mailinator.com" },
                    { "Password", hashedPassword1 },
                    { "Salt", salt1 },
                    { "Role", "Professor" }
                });

                var profId2 = Guid.NewGuid().ToString();
                (string hashedPassword2, string salt2) = PasswordHasher.HashPassword("Proba.123");

                await _tableClient.UpsertEntityAsync(new TableEntity(AssignToPartition(profId2).ToString(), profId2)
                {
                    { "Id", profId2 },
                    { "FirstName", "Benjamin" },
                    { "LastName", "Hayes" },
                    { "Index", null },
                    { "EmailAddress", "hayes@mailinator.com" },
                    { "Password", hashedPassword2 },
                    { "Salt", salt2 },
                    { "Role", "Professor" }
                });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private static int AssignToPartition(string partitionKey)
        {
            // Use SHA256 hash function to hash the partition key
            byte[] hashedValue;
            using (SHA256 sha256 = SHA256.Create())
            {
                hashedValue = sha256.ComputeHash(Encoding.UTF8.GetBytes(partitionKey));
            }

            // Convert hashed value to integer
            int intValue = BitConverter.ToInt32(hashedValue, 0);

            // Map the hashed value to one of three partitions
            int partitionIndex = Math.Abs(intValue % 3);

            return partitionIndex;
        }

        #endregion

        /// <summary>
        /// Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return this.CreateServiceRemotingInstanceListeners();
        }

        /// <summary>
        /// This is the main entry point for your service instance.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service instance.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            await InitializeTable();
            await PopulateProfessors();

            long iterations = 0;

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                ServiceEventSource.Current.ServiceMessage(this.Context, "Working-{0}", ++iterations);

                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }
    }
}
