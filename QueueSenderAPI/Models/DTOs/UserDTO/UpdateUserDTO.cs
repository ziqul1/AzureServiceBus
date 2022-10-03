namespace QueueSenderAPI.Models.DTOs.UserDTO
{
    public class UpdateUserDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public int Age { get; set; }
        public string Email { get; set; }
    }
}
