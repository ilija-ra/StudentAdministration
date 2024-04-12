using Client.Models.Accounts;
using Microsoft.AspNetCore.Mvc;
using StudentAdministration.Client.Models.Accounts;
using System.Text;
using System.Text.Json;

namespace Client.Controllers
{
    public class AccountsController : Controller
    {
        private readonly HttpClient _httpClient;

        public AccountsController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("http://localhost:8645");
            //_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", UserSingleton.JwtToken);
        }

        [HttpGet]
        [Route("Login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            var jsonModel = JsonSerializer.Serialize(model);

            var content = new StringContent(jsonModel, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"/Accounts/Login", content);

            if (response.IsSuccessStatusCode)
            {
                var jsonResult = await response.Content.ReadAsStringAsync();

                var result = JsonSerializer.Deserialize<LoginViewModelResponse>(jsonResult, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                SetSingletonUser(result);

                if (result != null && result.Role == "Student")
                {
                    return RedirectToAction("GetAllEnrolled", "Subjects");
                }
                else
                {
                    return RedirectToAction("GetSubjectsByProfessor", "Subjects");
                }
            }
            else
            {
                return View("Error");
            }
        }

        [HttpGet]
        [Route("Register")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            var jsonModel = JsonSerializer.Serialize(model);

            var content = new StringContent(jsonModel, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"/Accounts/Register", content);

            if (response.IsSuccessStatusCode)
            {
                //var jsonResult = await response.Content.ReadAsStringAsync();

                //var result = JsonSerializer.Deserialize<RegisterViewModelResponse>(jsonResult, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return RedirectToAction("Login", "Accounts");
            }
            else
            {
                return View("Error");
            }
        }

        [HttpGet]
        [Route("Logout")]
        public async Task<IActionResult> Logout()
        {
            UserSingleton.Instance.InitializeValues(
                null!,
                null!,
                null!,
                null!,
                null!,
                null!,
                null!,
                null!,
                false);

            return RedirectToAction("Index", "Home");
        }

        private void SetSingletonUser(LoginViewModelResponse? model)
        {
            UserSingleton.Instance.InitializeValues(
                model?.Id!,
                model?.FirstName!,
                model?.LastName!,
                model?.Index!,
                model?.EmailAddress!,
                model?.Role!,
                model?.PartitionKey!,
                model?.JwtToken!,
                true);
        }
    }
}
