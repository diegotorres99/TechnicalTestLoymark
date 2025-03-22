using back_end.DTOs;
using back_end.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text;

namespace front_end.Controllers
{
    public class UserController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl;

        public UserController(HttpClient httpClient, IOptions<ApiSettings> apiSettings)
        {
            _httpClient = httpClient;
            _apiUrl = apiSettings.Value.BaseUrl;  
        }

        public IActionResult User()
        {
            return View();
        }

        public async Task<IActionResult> getCountries()
        {
            var countries = await _httpClient.GetFromJsonAsync<IEnumerable<CountryDto>>(_apiUrl + "user/countries");

            if (countries == null)
            {
                return View("Error");
            }

            return StatusCode(StatusCodes.Status200OK, countries);
        }

        public async Task<IActionResult> getUsers()
        {
            var users = await _httpClient.GetFromJsonAsync<IEnumerable<UserDto>>(_apiUrl + "user/users");

            if (users == null)
            {
                return View("Error");
            }

            return StatusCode(StatusCodes.Status200OK, users);
        }

        public async Task<IActionResult> DeleteUser(int id)
        {
            var response = await _httpClient.DeleteAsync(_apiUrl + "user/delete/" + id);  

            if (response.IsSuccessStatusCode)
            {
                return Ok();
            }
            else
            {
                return View("Error");
            }
        }
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserDto userDto)
        {
            var jsonContent = new StringContent(JsonSerializer.Serialize(userDto), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_apiUrl + "user/CreateUser", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                return Ok();
            }
            else
            {
                return View("Error");
            }
        }

    }
}

