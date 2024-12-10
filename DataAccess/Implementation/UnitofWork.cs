using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Data;
using Entities.Repositories;

namespace DataAccess.Implementation
{
    public class UnitofWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public UnitofWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public IGenericRepository<T> GetRepository<T>() where T : class
        {
            return new GenericRepository<T>(_context);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
