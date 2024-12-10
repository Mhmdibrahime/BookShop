using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Entities.Domains;
using Services.AuthorServices.AuthorViewModels;

namespace Services.AuthorServices
{
    public interface IAuthorService 
    {
         Task<IEnumerable<AuthorUpdateVM>> GetAllAsync(
            Expression<Func<Author, bool>>? filter = null);
        Task<AuthorVM> GetByIdAsync(object id);
        Task AddAsync(AuthorCreateVM entity);
        Task UpdateAsync(AuthorUpdateVM entity);
        Task DeleteAsync(AuthorUpdateVM entity);
        Task SoftDeleteAsync(AuthorUpdateVM entity);
        Task ExecuteUpdateAsync(AuthorUpdateVM entity);
        Task ExecuteDeleteAsync(AuthorUpdateVM entity);
    }
}
