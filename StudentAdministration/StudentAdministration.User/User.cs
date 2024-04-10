using Azure.Data.Tables;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using OnlineStore.Communication.Account;
using StudentAdministration.Communication.Accounts.Models;
using StudentAdministration.Communication.Users;
using StudentAdministration.Communication.Users.Models;
using StudentAdministration.User.Entities;
using System.Fabric;
using System.Security.Cryptography;
using System.Text;

namespace StudentAdministration.User
{
    internal sealed class User : StatelessService, IUser, IAccount
    {
        private const string _connectionString = "DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;";
        private static TableServiceClient _serviceClient = new TableServiceClient(_connectionString);
        private static TableClient _tableClient = _serviceClient.GetTableClient("Users");

        public User(StatelessServiceContext context)
            : base(context)
        { }

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

        public async Task<UserUpdateResponseModel> Update(UserUpdateRequestModel? model)
        {
            var user = await _tableClient.GetEntityIfExistsAsync<TableEntity>(model?.PartitionKey, model?.Id);

            if (!user.HasValue)
            {
                return null!;
            }

            user.Value!["FirstName"] = model?.FirstName;
            user.Value!["LastName"] = model?.LastName;

            await _tableClient.UpsertEntityAsync(user.Value);

            return new UserUpdateResponseModel();
        }

        public async Task<UserGetByIdResponseModel> GetById(string? userId, string userPartitionKey)
        {
            try
            {
                var query = _tableClient.QueryAsync<UserEntity>(x => x.Id == userId && x.PartitionKey == userPartitionKey);

                var user = query!.ToBlockingEnumerable().FirstOrDefault();

                return new UserGetByIdResponseModel()
                {
                    Id = user!.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Index = user.Index,
                    EmailAddress = user.EmailAddress,
                    PartitionKey = user.PartitionKey
                };
            }
            catch
            {
                return null!;
            }
        }

        public async Task<UserGetInitialsResponseModel> GetInitials(string? userId, string userPartitionKey)
        {
            try
            {
                var query = _tableClient.QueryAsync<UserEntity>(x => x.Id == userId && x.PartitionKey == userPartitionKey);

                var user = query!.ToBlockingEnumerable().FirstOrDefault();

                return await Task.FromResult(new UserGetInitialsResponseModel()
                {
                    Id = user!.Id,
                    EmailAddress = user.EmailAddress,
                    Initials = string.Concat(user.FirstName?.First(), user.LastName?.First())
                });
            }
            catch
            {
                return null!;
            }
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
                (string hashedPassword1, string salt1) = PasswordHasher.HashPassword("Proba.123");

                await _tableClient.UpsertEntityAsync(new TableEntity("1", "a524095a-0688-487b-8b73-cc28f084cfd9")
                {
                    { "Id", "a524095a-0688-487b-8b73-cc28f084cfd9" },
                    { "FirstName", "Samantha" },
                    { "LastName", "Rodriguez" },
                    { "Index", null },
                    { "EmailAddress", "samantha@mailinator.com" },
                    { "Password", hashedPassword1 },
                    { "Salt", salt1 },
                    { "Role", "Professor" }
                });

                (string hashedPassword2, string salt2) = PasswordHasher.HashPassword("Proba.123");

                await _tableClient.UpsertEntityAsync(new TableEntity("1", "0e9c175c-e9ac-4863-9747-7cf26ca4c8de")
                {
                    { "Id", "0e9c175c-e9ac-4863-9747-7cf26ca4c8de" },
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

        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return this.CreateServiceRemotingInstanceListeners();
        }

        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            await InitializeTable();
            await PopulateProfessors();

            //long iterations = 0;

            //while (true)
            //{
            //    cancellationToken.ThrowIfCancellationRequested();

            //    ServiceEventSource.Current.ServiceMessage(this.Context, "Working-{0}", ++iterations);

            //    await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            //}
        }
    }
}
