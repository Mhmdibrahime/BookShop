using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Entities.Domains;
using Services.CategoryServices.CategoryViewModels;

namespace Services.CategoryServices
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryUpdateVM>> GetAllAsync(
            Expression<Func<Category, bool>>? filter = null);
        Task<CategoryVM> GetByIdAsync(object id);
        Task AddAsync(CategoryCreateVM entity);
        Task UpdateAsync(CategoryUpdateVM entity);
        Task DeleteAsync(CategoryUpdateVM entity);
        Task SoftDeleteAsync(CategoryUpdateVM entity);
        Task ExecuteUpdateAsync(CategoryUpdateVM entity);
        Task ExecuteDeleteAsync(CategoryUpdateVM entity);
    }
}
