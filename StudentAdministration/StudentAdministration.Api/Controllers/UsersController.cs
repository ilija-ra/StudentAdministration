using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using StudentAdministration.Communication.Users;
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
            //var userManagementProxy = ServiceProxy.Create<IUserManagement>(new Uri("fabric:/StudentAdministration/StudentAdministration.User"));
            //var result = await userManagementProxy.UserGetById(userId);

            //return Ok(result);
            return Ok();
        }

        [HttpPut]
        [Route("Update")]
        public async Task<IActionResult> Update([FromBody] UserUpdateRequestModel model)
        {
            var userProxy = ServiceProxy.Create<IUser>(new Uri("fabric:/StudentAdministration/StudentAdministration.User"));
            var result = await userProxy.Update(model);

            if (result is null)
            {
                return BadRequest("Error occured during this action!");
            }

            return Ok(result);
        }

        [HttpGet]
        [Route("GetInitials/{userId}")]
        public async Task<IResult> GetInitials(string userId)
        {
            return Results.Ok();
        }
    }
}
