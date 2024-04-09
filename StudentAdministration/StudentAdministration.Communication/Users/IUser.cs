using Microsoft.ServiceFabric.Services.Remoting;
using StudentAdministration.Communication.Users.Models;

namespace StudentAdministration.Communication.Users
{
    public interface IUser : IService
    {
        Task<UserGetByIdResponseModel> GetById(string? userId);

        Task<UserUpdateResponseModel> Update(UserUpdateRequestModel? model);

        Task<UserGetInitialsResponseModel> GetInitials(string? userId);
    }
}
