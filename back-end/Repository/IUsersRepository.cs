using back_end.DTOs;

namespace back_end.Repository
{
    public interface IUsersRepository
    {
        Task<IEnumerable<UserDto>> GetAll();
        Task<UserDto?> GetById(int id);  
        Task<UserDto> Create(UserDto userDto);  
        Task<bool> Update(UserDto userDto);  
        Task<bool> Delete(int id);
        Task<IEnumerable<CountryDto>> GetCountries();
    }
}
