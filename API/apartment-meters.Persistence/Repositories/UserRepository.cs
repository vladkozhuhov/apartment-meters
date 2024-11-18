using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;

namespace Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User> GetByApartmentNumberAsync(string apartmentNumber) =>
        await _context.Users.FirstOrDefaultAsync(u => u.ApartmentNumber == apartmentNumber);

    public async Task<User> GetByIdAsync(int id) =>
        await _context.Users.FindAsync(id);

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }
}