﻿using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using StudentAdministration.Api.Identity;
using StudentAdministration.Api.Validators;
using StudentAdministration.Communication.Subjects;
using StudentAdministration.Communication.Subjects.Models;
using System.Fabric;

namespace StudentAdministration.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class SubjectsController : ControllerBase
    {
        [Authorize(Policy = IdentityData.RequireStudentRole)]
        [HttpPost]
        [Route("Enroll")]
        public async Task<IActionResult> Enroll([FromBody] SubjectEnrollRequestModel model)
        {
            var validator = new SubjectEnrollRequestModelValidator();
            ValidationResult valResult = validator.Validate(model);

            if (!valResult.IsValid)
            {
                return BadRequest(valResult.Errors.Select(x => x.ErrorMessage).ToList());
            }

            var subjectProxy = ServiceProxy.Create<ISubject>(new Uri("fabric:/StudentAdministration/StudentAdministration.Subject"), await getAvailablePartitionKey());
            var result = await subjectProxy.Enroll(model);

            if (result is null)
            {
                return BadRequest();
            }

            return Ok(result);
        }

        [Authorize(Policy = IdentityData.RequireStudentRole)]
        [HttpGet]
        [Route("GetAllEnrolled/{studentId}/{dropOut}")]
        public async Task<IActionResult> GetAllEnrolled(string studentId, bool? dropOut = false)
        {
            var subjectProxy = ServiceProxy.Create<ISubject>(new Uri("fabric:/StudentAdministration/StudentAdministration.Subject"), await getAvailablePartitionKey());
            var result = await subjectProxy.GetAllEnrolled(studentId, dropOut);

            if (result is null)
            {
                return BadRequest();
            }

            return Ok(result);
        }

        [Authorize(Policy = IdentityData.RequireStudentRole)]
        [HttpPost]
        [Route("DropOut")]
        public async Task<IActionResult> DropOut(SubjectDropOutRequestModel model)
        {
            var validator = new SubjectDropOutRequestModelValidator();
            ValidationResult valResult = validator.Validate(model);

            if (!valResult.IsValid)
            {
                return BadRequest(valResult.Errors.Select(x => x.ErrorMessage).ToList());
            }

            var subjectProxy = ServiceProxy.Create<ISubject>(new Uri("fabric:/StudentAdministration/StudentAdministration.Subject"), await getAvailablePartitionKey());
            var result = await subjectProxy.DropOut(model);

            if (result is null)
            {
                return BadRequest();
            }

            return Ok(result);
        }

        [Authorize(Policy = IdentityData.RequireProfessorRole)]
        [HttpGet]
        [Route("GetSubjectsByProfessor/{professorId}")]
        public async Task<IActionResult> GetSubjectsByProfessor(string professorId)
        {
            var subjectProxy = ServiceProxy.Create<ISubject>(new Uri("fabric:/StudentAdministration/StudentAdministration.Subject"), await getAvailablePartitionKey());
            var result = await subjectProxy.GetSubjectsByProfessor(professorId);

            if (result is null)
            {
                return BadRequest();
            }

            return Ok(result);
        }

        [Authorize(Policy = IdentityData.RequireProfessorRole)]
        [HttpGet]
        [Route("GetStudentsBySubject/{subjectId}")]
        public async Task<IActionResult> GetStudentsBySubject(string subjectId)
        {
            var subjectProxy = ServiceProxy.Create<ISubject>(new Uri("fabric:/StudentAdministration/StudentAdministration.Subject"), await getAvailablePartitionKey());
            var result = await subjectProxy.GetStudentsBySubject(subjectId);

            if (result is null)
            {
                return BadRequest();
            }

            return Ok(result);
        }

        [Authorize(Policy = IdentityData.RequireProfessorRole)]
        [HttpPut]
        [Route("SetGrade")]
        public async Task<IActionResult> SetGrade([FromBody] SubjectSetGradesRequestModel model)
        {
            var validator = new SubjectSetGradesRequestModelValidator();
            ValidationResult valResult = validator.Validate(model);

            if (!valResult.IsValid)
            {
                return BadRequest(valResult.Errors.Select(x => x.ErrorMessage).ToList());
            }

            var subjectProxy = ServiceProxy.Create<ISubject>(new Uri("fabric:/StudentAdministration/StudentAdministration.Subject"), await getAvailablePartitionKey());
            var result = await subjectProxy.SetGrade(model);

            if (result is null)
            {
                return BadRequest();
            }

            return Ok(result);
        }

        [Authorize(Policy = IdentityData.RequireStudentRole)]
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

        [Authorize(Policy = IdentityData.RequireStudentRole)]
        [HttpGet]
        [Route("ConfirmSubjects/{studentId}")]
        public async Task<IActionResult> ConfirmSubjects(string studentId)
        {
            var subjectProxy = ServiceProxy.Create<ISubject>(new Uri("fabric:/StudentAdministration/StudentAdministration.Subject"), await getAvailablePartitionKey());
            var result = await subjectProxy.ConfirmSubjects(studentId);

            if (result is null)
            {
                return BadRequest();
            }

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("ClearDictionaries")]
        public async Task<IActionResult> ClearDictionary()
        {
            var subjectProxy = ServiceProxy.Create<ISubject>(new Uri("fabric:/StudentAdministration/StudentAdministration.Subject"), await getAvailablePartitionKey());
            await subjectProxy.ClearDictionaries();

            return Ok();
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
