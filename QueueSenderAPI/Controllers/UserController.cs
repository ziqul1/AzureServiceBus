using Microsoft.AspNetCore.Mvc;
using QueueSenderAPI.Data.Services.UserService;
using QueueSenderAPI.Models.DTOs.UserDTO;

namespace QueueSenderAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService) => _userService = userService;

        // GET: api/Users
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            return Ok(await _userService.GetAllActiveUsersAsync());
        }

        // POST: api/Users
        [HttpPost]
        public async Task<IActionResult> CreateAsync(CreateUserDTO createUserDTO)
        {
            var user = await _userService.CreateAsync(createUserDTO);

            if (user != true)
                return BadRequest();

            // return Ok(user) ???
            return Ok();
        }

        // PUT: api/Users/1
        //[HttpPut("{id}")]
        [HttpPut]
        public async Task<IActionResult> UpdateAsync(UpdateUserDTO updateUserDTO)
        {
            if (await _userService.UpdateAsync(updateUserDTO) != 1)
                return BadRequest();

            return Ok();
        }
    }
}
