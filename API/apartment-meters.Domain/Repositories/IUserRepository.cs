using Domain.Entities;

namespace Domain.Repositories;

public interface IUserRepository
{
    Task<User> GetByApartmentNumberAsync(string apartmentNumber);
    Task<User> GetByIdAsync(int id);
    Task AddAsync(User user);
}