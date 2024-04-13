using Microsoft.AspNetCore.Mvc;
using StudentAdministration.Client.Models.Users;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Client.Controllers
{
    public class UsersController : Controller
    {
        private readonly HttpClient _httpClient;

        public UsersController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("http://localhost:8645");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", UserSingleton.Instance.JwtToken);
        }

        [HttpGet]
        [Route("UserProfile")]
        public async Task<IActionResult> UserProfile()
        {
            var response = await _httpClient.GetAsync($"/Users/GetById/{UserSingleton.Instance.Id}/{UserSingleton.Instance.PartitionKey}");

            if (!response.IsSuccessStatusCode)
            {
                return View("Error");
            }

            var jsonResult = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<UserGetByIdViewModelResponse>(jsonResult, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (result is null)
            {
                return View("Error");
            }

            var user = new UserViewModel()
            {
                Id = result.Id,
                FirstName = result.FirstName,
                LastName = result.LastName,
                Index = result.Index,
                EmailAddress = result.EmailAddress,
                PartitionKey = result.PartitionKey
            };

            return View(user);
        }

        [HttpPost]
        [Route("Update")]
        public async Task<IActionResult> Update(UserViewModel model)
        {
            var jsonModel = JsonSerializer.Serialize(new
            {
                Id = UserSingleton.Instance.Id,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PartitionKey = UserSingleton.Instance.PartitionKey
            });

            var content = new StringContent(jsonModel, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"/Users/Update", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessfulMessage"] = "Updated info successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Error occured during update process!";
            }

            return RedirectToAction("UserProfile");
        }
    }
}
