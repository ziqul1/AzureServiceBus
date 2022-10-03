using QueueSenderAPI.Models.DTOs.UserDTO;

namespace QueueSenderAPI.Data.Services.UserService
{
    public interface IUserService
    {
        public Task<List<GetSingleUserDTO>> GetAllActiveUsersAsync();
        public Task<bool> CreateAsync(CreateUserDTO createUserDTO);
        public Task<long> UpdateAsync(UpdateUserDTO updateUserDTO);
    }
}
