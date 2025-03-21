using back_end.Repository;
using Microsoft.AspNetCore.Mvc;

namespace back_end.Controllers
{
    public class ActivitiesController : BaseApiController
    {
        private readonly IActivitiesRepository _activityRepository;
        public ActivitiesController(IActivitiesRepository activityRepository)
        {
            _activityRepository = activityRepository;
        }

        [HttpGet]
        public async Task<ActionResult> GetActivities()
        {
            var resp = await _activityRepository.GetAll();
            return Ok(resp);
        }
    }
}
