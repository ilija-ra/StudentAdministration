using Microsoft.AspNetCore.Mvc;
using StudentAdministration.Communication.Users.Models;

namespace StudentAdministration.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        [HttpGet]
        [Route("GetById/{userId}")]
        public async Task<IActionResult> GetById(string userId)
        {
            //var userManagementProxy = ServiceProxy.Create<IUserManagement>(new Uri("fabric:/OnlineStore/OnlineStore.UserManagement"));
            //var result = await userManagementProxy.UserGetById(userId);

            //return Ok(result);
            return Ok();
        }

        [HttpPut]
        [Route("Update")]
        public async Task<IActionResult> Update([FromBody] UserUpdateRequestModel model)
        {
            //var userManagementProxy = ServiceProxy.Create<IUserManagement>(new Uri("fabric:/OnlineStore/OnlineStore.UserManagement"));
            //var result = await userManagementProxy.UserUpdate(model);

            //return Ok(result);
            return Ok();
        }

        [HttpGet]
        [Route("GetInitials/{userId}")]
        public async Task<IResult> GetInitials(string userId)
        {
            return Results.Ok();
        }
    }
}
