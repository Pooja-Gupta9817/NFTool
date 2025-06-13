using DesktopTool.App.Core;
using DesktopTool.App.Data;
using DesktopTool.App.UI.Model;
using Microsoft.EntityFrameworkCore;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _db;

    public AuthService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<bool> LoginAsync(string email, string password)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email && u.Password == password);
        return user != null;
    }

    public async Task<bool> RegisterAsync(string name, string email, string password)
    {
        var exists = await _db.Users.AnyAsync(u => u.Email == email);
        if (exists) return false;

        _db.Users.Add(new User { Name = name, Email = email, Password = password });
        await _db.SaveChangesAsync();
        return true;
    }
}
