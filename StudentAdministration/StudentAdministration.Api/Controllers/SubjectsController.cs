using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using StudentAdministration.Communication.Subjects;
using StudentAdministration.Communication.Subjects.Models;
using System.Fabric;

namespace StudentAdministration.Api.Controllers
{
    public class SubjectsController : ControllerBase
    {
        [HttpPost]
        [Route("Enroll")]
        public async Task<IActionResult> Enroll([FromBody] SubjectEnrollRequestModel model)
        {
            var subjectProxy = ServiceProxy.Create<ISubject>(new Uri("fabric:/StudentAdministration/StudentAdministration.Subject"), await getAvailablePartitionKey());
            var result = await subjectProxy.Enroll(model);

            if (result is null)
            {
                return BadRequest();
            }

            return Ok(result);
        }

        [HttpGet]
        [Route("GetAllEnrolled/{studentId}")]
        public async Task<IActionResult> GetAllEnrolled(string? studentId)
        {
            var subjectProxy = ServiceProxy.Create<ISubject>(new Uri("fabric:/StudentAdministration/StudentAdministration.Subject"), await getAvailablePartitionKey());
            var result = await subjectProxy.GetAllEnrolled(studentId);

            if (result is null)
            {
                return BadRequest();
            }

            return Ok(result);
        }

        [HttpPost]
        [Route("DropOut")]
        public async Task<IActionResult> DropOut(string? subjectId, string? studentId)
        {
            var subjectProxy = ServiceProxy.Create<ISubject>(new Uri("fabric:/StudentAdministration/StudentAdministration.Subject"), await getAvailablePartitionKey());
            var result = await subjectProxy.DropOut(subjectId, studentId);

            if (result is null)
            {
                return BadRequest();
            }

            return Ok(result);
        }

        [HttpGet]
        [Route("GetStudentsBySubject/{subjectId}")]
        public async Task<IActionResult> GetStudentsBySubject(string? subjectId)
        {
            var subjectProxy = ServiceProxy.Create<ISubject>(new Uri("fabric:/StudentAdministration/StudentAdministration.Subject"), await getAvailablePartitionKey());
            var result = await subjectProxy.GetStudentsBySubject(subjectId);

            if (result is null)
            {
                return BadRequest();
            }

            return Ok(result);
        }

        [HttpPut]
        [Route("SetGrades")]
        public async Task<IActionResult> SetGrades([FromBody] SubjectSetGradesRequestModel model)
        {
            var subjectProxy = ServiceProxy.Create<ISubject>(new Uri("fabric:/StudentAdministration/StudentAdministration.Subject"), await getAvailablePartitionKey());
            var result = await subjectProxy.SetGrades(model);

            if (result is null)
            {
                return BadRequest();
            }

            return Ok(result);
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var subjectProxy = ServiceProxy.Create<ISubject>(new Uri("fabric:/StudentAdministration/StudentAdministration.Subject"), await getAvailablePartitionKey());
            var result = await subjectProxy.GetAll();

            if (result is null)
            {
                return BadRequest();
            }

            return Ok(result);
        }

        [HttpPut]
        [Route("ConfirmSubjects")]
        public async Task<IActionResult> ConfirmSubjects()
        {
            var subjectProxy = ServiceProxy.Create<ISubject>(new Uri("fabric:/StudentAdministration/StudentAdministration.Subject"), await getAvailablePartitionKey());
            var result = await subjectProxy.ConfirmSubjects();

            if (result is null)
            {
                return BadRequest();
            }

            return Ok(result);
        }

        private async Task<ServicePartitionKey> getAvailablePartitionKey()
        {
            var partitionKey = new ServicePartitionKey();
            var fabricClient = new FabricClient();
            var availablePartitions = await fabricClient.QueryManager.GetPartitionListAsync(new Uri("fabric:/StudentAdministration/StudentAdministration.Subject"));

            foreach (var partition in availablePartitions)
            {
                var key = partition.PartitionInformation as Int64RangePartitionInformation;

                if (key == null)
                {
                    continue;
                }

                partitionKey = new ServicePartitionKey(key.LowKey);
            }

            return partitionKey;
        }
    }
}
