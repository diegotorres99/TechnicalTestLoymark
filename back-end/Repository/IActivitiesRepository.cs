using back_end.DTOs;

namespace back_end.Repository
{
    public interface IActivitiesRepository
    {
        Task<IEnumerable<ActivityDto>> GetAll();
    }
}
