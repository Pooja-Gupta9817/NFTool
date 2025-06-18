using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopTool.App.Core.Models
{
    //This allows reusable data access for any entity class (like User, Product, etc.)
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);

        Task AddAsync(T entity);            // ✅ new: just adds to context
        Task SaveChangesAsync();            // ✅ new: commits to DB

        void Update(T entity);
        void Delete(T entity);
    }


}
