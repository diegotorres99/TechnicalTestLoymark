using back_end.DTOs;
using back_end.Repository;
using Microsoft.AspNetCore.Mvc;

namespace back_end.Controllers
{
    public class UserController : BaseApiController
    {
        private readonly IUsersRepository _usersRepository;
        public UserController(IUsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
        }

        [HttpGet("users")]
        public async Task<ActionResult> GetUsers()
        {
            var resp = await _usersRepository.GetAll();
            return Ok(resp);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetUser(int id)
        {
            var user = await _usersRepository.GetById(id);
            if (user == null)
                return NotFound($"User with ID {id} not found.");

            return Ok(user);
        }

        [HttpPost("CreateUser")]
        public async Task<ActionResult> CreateUser([FromBody] UserDto userDto)
        {
            if (userDto == null)
                return BadRequest("Invalid user data.");

            var createdUser = await _usersRepository.Create(userDto);
            return CreatedAtAction(nameof(GetUser), new { id = createdUser.Id }, createdUser);
        }


        [HttpPut("UpdateUser")]
        public async Task<ActionResult> UpdateUser([FromBody] UserDto userDto)
        {
            if (userDto == null || userDto.Id <= 0)
                return BadRequest("User data is invalid.");

            var existingUser = await _usersRepository.GetById(userDto.Id);
            if (existingUser == null)
                return NotFound($"User with ID {userDto.Id} not found.");

            await _usersRepository.Update(userDto);
            return Ok();
        }

        [HttpDelete("delete/{id}")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            var existingUser = await _usersRepository.GetById(id);
            if (existingUser == null)
                return NotFound($"User with ID {id} not found.");

            await _usersRepository.Delete(id);
            return NoContent(); 
        }

        [HttpGet("countries")]
        public async Task<ActionResult<IEnumerable<CountryDto>>> GetCountries()
        {
            var countries = await _usersRepository.GetCountries();
            if (countries == null || !countries.Any())
                return NotFound("No countries found.");

            return Ok(countries);
        }
    }
}
