using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using StudentAdministration.Api.Identity;
using StudentAdministration.Communication.Users;
using StudentAdministration.Communication.Users.Models;

namespace StudentAdministration.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        [HttpGet]
        [Route("GetById")]
        public async Task<IActionResult> GetById(string userId, string userPartitionKey)
        {
            var userProxy = ServiceProxy.Create<IUser>(new Uri("fabric:/StudentAdministration/StudentAdministration.User"));
            var result = await userProxy.GetById(userId, userPartitionKey);

            if (result is null)
            {
                return BadRequest("Error occured during this action!");
            }

            return Ok(result);
        }

        [Authorize(Policy = IdentityData.RequireStudentRole)]
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
        [Route("GetInitials")]
        public async Task<IActionResult> GetInitials(string userId, string userPartitionKey)
        {
            var userProxy = ServiceProxy.Create<IUser>(new Uri("fabric:/StudentAdministration/StudentAdministration.User"));
            var result = await userProxy.GetInitials(userId, userPartitionKey);

            if (result is null)
            {
                return BadRequest("Error occured during this action!");
            }

            return Ok(result);
        }
    }
}
