using DesktopTool.App.Core;
using DesktopTool.App.UI.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopTool.App.Data
{
    public class UserRepository : Core.IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UI.Model.User>> GetAllAsync() => await _context.Users.ToListAsync();

        public async Task<UI.Model.User> GetByIdAsync(int id) => await _context.Users.FindAsync(id);

        public async Task AddAsync(UI.Model.User entity) => await _context.Users.AddAsync(entity);

        public void Update(UI.Model.User entity) => _context.Users.Update(entity);

        public void Delete(UI.Model.User entity) => _context.Users.Remove(entity);

        public async Task<UI.Model.User> GetByEmailAsync(string email) =>
            await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

}
