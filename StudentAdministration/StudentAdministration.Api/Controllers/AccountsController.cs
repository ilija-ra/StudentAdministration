using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using OnlineStore.Communication.Account;
using StudentAdministration.Communication.Accounts.Models;

namespace StudentAdministration.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountsController : ControllerBase
    {
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] AccountLoginRequestModel model)
        {
            var userProxy = ServiceProxy.Create<IAccount>(new Uri("fabric:/StudentAdministration/StudentAdministration.User"));
            var result = await userProxy.Login(model);

            if (result is null)
            {
                return BadRequest("Something went wrong! Check your credentials.");
            }

            return Ok(result);
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] AccountRegisterRequestModel model)
        {
            var userProxy = ServiceProxy.Create<IAccount>(new Uri("fabric:/StudentAdministration/StudentAdministration.User"));
            var result = await userProxy.Register(model);

            if (result is null)
            {
                return BadRequest("Something went wrong during registration process!");
            }

            return Ok(result);
        }
    }
}
