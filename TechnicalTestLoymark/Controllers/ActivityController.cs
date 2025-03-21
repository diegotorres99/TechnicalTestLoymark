using back_end.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace front_end.Controllers
{
    public class ActivityController : Controller
{
        private readonly HttpClient _httpClient;

        public ActivityController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IActionResult> Activity()
        {
            var list = await _httpClient.GetFromJsonAsync<IEnumerable<ActivityDto>>("https://localhost:7177/api/activities");

            if (list == null)
            {
                return View("Error"); // Handle error if needed
            }

            return StatusCode(StatusCodes.Status200OK, list);
        }
    }
}
