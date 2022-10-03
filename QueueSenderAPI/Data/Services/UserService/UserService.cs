using Azure.Messaging.ServiceBus;
using Microsoft.EntityFrameworkCore;
using QueueSenderAPI.Data.DbContexts;
using QueueSenderAPI.Models;
using QueueSenderAPI.Models.DTOs.UserDTO;
using System.Text.Json;

namespace QueueSenderAPI.Data.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly UserContext _userContext;
        private readonly string _connectionString;
        private readonly string _queueName;

        public UserService(UserContext userContext)
        {
            _userContext = userContext;
            _connectionString = "";
            _queueName = "";
        }

        public async Task<List<GetSingleUserDTO>> GetAllActiveUsersAsync()
        {
            return await _userContext.Users
                .Where(x => x.IsFlagActive == true)
                .Select(x => GetSingleUserToDTO(x)).ToListAsync();
        }

        public async Task<bool> CreateAsync(CreateUserDTO createUserDTO)
        {
            var user = new User
            {
                Name = createUserDTO.Name,
                Surname = createUserDTO.Surname,
                Age = createUserDTO.Age,
                Email = createUserDTO.Email,
                IsFlagActive = false
            };

            _userContext.Users.Add(user);
            if (await _userContext.SaveChangesAsync() != 1)
                throw new Exception("Cos sie zepsulo przy zapisie do bazy chłopaku");

            var serializedUser = JsonSerializer.Serialize(user); 
            ServiceBusClient client = new ServiceBusClient(_connectionString);
            ServiceBusSender sender = client.CreateSender(_queueName);
            using ServiceBusMessageBatch messageBatch = await sender.CreateMessageBatchAsync();

            if (!messageBatch.TryAddMessage(new ServiceBusMessage(serializedUser)))
                throw new Exception($"The message is too large to fit in the batch.");

            await sender.SendMessagesAsync(messageBatch);
            await client.DisposeAsync();
            await sender.DisposeAsync();

            return true;
        }

        public async Task<long> UpdateAsync(UpdateUserDTO updateUserDTO)
        {
            var user = await _userContext.Users.Where(x => x.Id == updateUserDTO.Id).FirstOrDefaultAsync();

            if (user == null)
                throw new Exception("Panie, nie ma takiego gościa w bazie");

            user.Name = updateUserDTO.Name;
            user.Surname = updateUserDTO.Surname;
            user.Age = updateUserDTO.Age;
            user.Email = updateUserDTO.Email;

            var result = await _userContext.SaveChangesAsync();

            if (result != 1)
                throw new Exception("Cos sie zepsulo przy zapisie do bazy chłopaku");

            var serializedUser = JsonSerializer.Serialize(user);
            ServiceBusClient client = new ServiceBusClient(_connectionString);
            ServiceBusSender sender = client.CreateSender(_queueName);
            using ServiceBusMessageBatch messageBatch = await sender.CreateMessageBatchAsync();

            if (!messageBatch.TryAddMessage(new ServiceBusMessage(serializedUser)))
                throw new Exception($"The message is too large to fit in the batch.");

            await sender.SendMessagesAsync(messageBatch);
            await client.DisposeAsync();
            await sender.DisposeAsync();

            return result;
        }

        private static GetSingleUserDTO GetSingleUserToDTO(User user) =>
                    new GetSingleUserDTO
                    {
                        Id = user.Id,
                        Name = user.Name,
                        Surname = user.Surname,
                        Email = user.Email,
                        Age = user.Age,
                        IsFlagActive = user.IsFlagActive,
                    };
    }
}
