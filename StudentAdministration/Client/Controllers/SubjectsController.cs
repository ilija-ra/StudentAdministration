using Client.Models.Subjects;
using Microsoft.AspNetCore.Mvc;
using StudentAdministration.Client.Models.Subjects;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Client.Controllers
{
    public class SubjectsController : Controller
    {
        private readonly HttpClient _httpClient;

        public SubjectsController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("http://localhost:8645");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", UserSingleton.Instance.JwtToken);
        }

        [HttpGet]
        [Route("Enroll")]
        public async Task<IActionResult> Enroll(string? subjectId, string? subjectPartitionKey, string? professorFullName)
        {
            var model = new EnrollViewModel()
            {
                Id = null,
                SubjectId = subjectId,
                SubjectPartitionKey = subjectPartitionKey,
                StudentId = UserSingleton.Instance.Id,
                StudentPartitionKey = UserSingleton.Instance.PartitionKey,
                StudentFullName = $"{UserSingleton.Instance.FirstName} {UserSingleton.Instance.LastName}",
                ProfessorFullName = professorFullName,
                Grade = 0
            };

            var jsonModel = JsonSerializer.Serialize(model);

            var content = new StringContent(jsonModel, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"/Subjects/Enroll", content);

            if (!response.IsSuccessStatusCode)
            {
                return View("Error");
            }

            //var jsonResult = await response.Content.ReadAsStringAsync();

            //var result = JsonSerializer.Deserialize<EnrollViewModelResponse>(jsonResult, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return RedirectToAction("GetAll");
        }

        [HttpGet]
        [Route("GetAllEnrolled")]
        public async Task<IActionResult> GetAllEnrolled()
        {
            var response = await _httpClient.GetAsync($"/Subjects/GetAllEnrolled/{UserSingleton.Instance.Id}");

            if (!response.IsSuccessStatusCode)
            {
                return View("Error");
            }

            var jsonResult = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<GetAllEnrolledViewModelResponse>(jsonResult, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (result is null)
            {
                return View("Error");
            }

            return View(result.Items);
        }

        [HttpGet]
        [Route("DropOut")]
        public async Task<IActionResult> DropOut(string? subjectId, string? studentId)
        {
            var model = new DropOutViewModel()
            {
                SubjectId = subjectId,
                StudentId = studentId,
            };

            var jsonModel = JsonSerializer.Serialize(model);

            var content = new StringContent(jsonModel, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"/Subjects/DropOut", content);

            if (!response.IsSuccessStatusCode)
            {
                return View("Error");
            }

            return RedirectToAction("GetAllEnrolled");
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var response = await _httpClient.GetAsync($"/Subjects/GetAll");

            if (!response.IsSuccessStatusCode)
            {
                return View("Error");
            }

            var jsonResult = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<GetAllViewModel>(jsonResult, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (result is null)
            {
                return View("Error");
            }

            return View(result.Items);
        }

        [HttpGet]
        [Route("ConfirmSubjects")]
        public async Task<IActionResult> ConfirmSubjects()
        {
            var response = await _httpClient.GetAsync($"/Subjects/ConfirmSubjects");

            if (!response.IsSuccessStatusCode)
            {
                return View("Error");
            }

            return RedirectToAction("GetAllEnrolled");
        }

        #region Professor

        [HttpGet]
        [Route("GetSubjectsByProfessor")]
        public async Task<IActionResult> GetSubjectsByProfessor()
        {
            var response = await _httpClient.GetAsync($"/Subjects/GetSubjectsByProfessor/{UserSingleton.Instance.Id}");

            if (!response.IsSuccessStatusCode)
            {
                return View("Error");
            }

            var jsonResult = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<GetAllSubjectsByProfessorViewModelResponse>(jsonResult, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (result is null)
            {
                return View("Error");
            }

            return View(result.Items);
        }

        [HttpGet]
        [Route("GetStudentsBySubject")]
        public async Task<IActionResult> GetStudentsBySubject(string? subjectId)
        {
            var response = await _httpClient.GetAsync($"/Subjects/GetStudentsBySubject/{subjectId}");

            if (!response.IsSuccessStatusCode)
            {
                return View("Error");
            }

            var jsonResult = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<GetStudentsBySubjectViewModelResponse>(jsonResult, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (result is null)
            {
                return View("Error");
            }

            return View(result.Items);
        }

        [HttpGet]
        [Route("SetGrade")]
        public async Task<IActionResult> SetGrade(string? subjectId, string? subjectPartitionKey, string? studentId, string? studentPartitionKey, int? grade)
        {
            var model = new SetGradeViewModel()
            {
                SubjectId = subjectId,
                SubjectPartitionKey = subjectPartitionKey,
                StudentId = studentId,
                StudentPartitionKey = studentPartitionKey,
                Grade = grade
            };

            var jsonModel = JsonSerializer.Serialize(model);

            var content = new StringContent(jsonModel, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"/Subjects/SetGrade", content);

            if (!response.IsSuccessStatusCode)
            {
                return View("Error");
            }

            return RedirectToAction("GetStudentsBySubject", new { subjectId = subjectId });
        }

        #endregion
    }
}
