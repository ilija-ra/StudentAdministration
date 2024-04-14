using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using OnlineStore.Communication.Account;
using StudentAdministration.Api.Identity;
using StudentAdministration.Api.Validators;
using StudentAdministration.Communication.Accounts.Models;

namespace StudentAdministration.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly JwtManager _jwtManager;

        public AccountsController(JwtManager jwtManager)
        {
            _jwtManager = jwtManager;
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] AccountLoginRequestModel model)
        {
            var validator = new AccountLoginRequestModelValidator();
            ValidationResult valResult = validator.Validate(model);

            if (!valResult.IsValid)
            {
                return BadRequest(valResult.Errors.Select(x => x.ErrorMessage).ToList());
            }

            var userProxy = ServiceProxy.Create<IAccount>(new Uri("fabric:/StudentAdministration/StudentAdministration.User"));
            var result = await userProxy.Login(model);

            if (result is null)
            {
                return BadRequest("Something went wrong! Check your credentials.");
            }

            result.JwtToken = await _jwtManager.GenerateToken(result);

            return Ok(result);
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] AccountRegisterRequestModel model)
        {
            var validator = new AccountRegisterRequestModelValidator();
            ValidationResult valResult = validator.Validate(model);

            if (!valResult.IsValid)
            {
                return BadRequest(valResult.Errors.Select(x => x.ErrorMessage).ToList());
            }

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
