namespace Domain.Entities;

public class User
{
    public int Id { get; set; }
    public string ApartmentNumber { get; set; }
    public string PasswordHash { get; set; }
    public string Role { get; set; } 
    public bool IsAdmin => Role == "Admin";
}