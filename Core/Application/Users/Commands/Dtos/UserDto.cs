namespace CleanArchitecture.Application.Users.Commands.Dtos
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? ThirdName { get; set; }
        public string? FamilyName { get; set; }
    }
}
