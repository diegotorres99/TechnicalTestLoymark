using back_end.DTOs;
using back_end.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace front_end.Controllers
{
    public class ActivityController : Controller
{
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl;

        public ActivityController(HttpClient httpClient, IOptions<ApiSettings> apiSettings)
        {
            _httpClient = httpClient;
            _apiUrl = apiSettings.Value.BaseUrl;
        }
        public IActionResult Activity()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> getActivities()
        {
            var list = await _httpClient.GetFromJsonAsync<IEnumerable<ActivityDto>>(_apiUrl + "activities");

            if (list == null)
            {
                return View("Error"); 
            }

            return StatusCode(StatusCodes.Status200OK, list);
        }
    }
}
