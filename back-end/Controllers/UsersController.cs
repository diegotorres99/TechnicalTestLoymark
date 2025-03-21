using back_end.DTOs;
using back_end.Repository;
using Microsoft.AspNetCore.Mvc;

namespace back_end.Controllers
{
    public class UsersController : BaseApiController
    {
        private readonly IUsersRepository _usersRepository;
        public UsersController(IUsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
        }

        [HttpGet]
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

        [HttpPost]
        public async Task<ActionResult> CreateUser([FromBody] UserDto userDto)
        {
            if (userDto == null)
                return BadRequest("Invalid user data.");

            var createdUser = await _usersRepository.Create(userDto);
            return CreatedAtAction(nameof(GetUser), new { id = createdUser.Id }, createdUser);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateUser(int id, [FromBody] UserDto userDto)
        {
            if (userDto == null || id != userDto.Id)
                return BadRequest("User data is invalid or ID mismatch.");

            var existingUser = await _usersRepository.GetById(id);
            if (existingUser == null)
                return NotFound($"User with ID {id} not found.");

            await _usersRepository.Update(userDto);
            return NoContent(); 
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            var existingUser = await _usersRepository.GetById(id);
            if (existingUser == null)
                return NotFound($"User with ID {id} not found.");

            await _usersRepository.Delete(id);
            return NoContent(); 
        }
    }
}
