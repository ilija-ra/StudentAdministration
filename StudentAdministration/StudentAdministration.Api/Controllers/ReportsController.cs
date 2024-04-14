using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using StudentAdministration.Api.Identity;
using StudentAdministration.Communication.Report;

namespace StudentAdministration.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ReportsController : ControllerBase
    {
        [Authorize(Policy = IdentityData.RequireStudentRole)]
        [HttpGet]
        [Route("GenerateReport/{studentId}")]
        public async Task<IActionResult> GenerateReport(string? studentId)
        {
            var reportProxy = ServiceProxy.Create<IReport>(new Uri("fabric:/StudentAdministration/StudentAdministration.Report"));
            var result = await reportProxy.GenerateReport(studentId);

            if (result is null)
            {
                return BadRequest();
            }

            return Ok(result);
        }
    }
}
