using Microsoft.ServiceFabric.Services.Remoting;
using StudentAdministration.Communication.Accounts.Models;

namespace OnlineStore.Communication.Account
{
    public interface IAccount : IService
    {
        Task<AccountLoginResponseModel> Login(AccountLoginRequestModel? model);

        Task<AccountRegisterResponseModel> Register(AccountRegisterRequestModel? model);
    }
}
