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

            return View(result);
        }

        [HttpPost]
        [Route("Update")]
        public async Task<IActionResult> Update(UserGetByIdViewModelResponse model)
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
                //var jsonResult = await response.Content.ReadAsStringAsync();

                //var result = JsonSerializer.Deserialize<UserUpdateViewModelResponse>(jsonResult, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return RedirectToAction("UserProfile");
            }
            else
            {
                return View("Error");
            }
        }
    }
}
