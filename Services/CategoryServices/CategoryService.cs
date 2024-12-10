using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Entities.Domains;
using Entities.Repositories;
using Microsoft.EntityFrameworkCore;
using Services.CategoryServices.CategoryViewModels;

namespace Services.CategoryServices
{
    public class CategoryService(IUnitOfWork unitOfWork) : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<IEnumerable<CategoryUpdateVM>> GetAllAsync(Expression<Func<Category, bool>>? filter = null)
        {
            var categories = await _unitOfWork.GetRepository<Category>().GetAllAsync(filter);
            return categories.Select(c => new CategoryUpdateVM()
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
            });
        }

        public async Task<CategoryVM> GetByIdAsync(object id)
        {
            var category = await _unitOfWork.GetRepository<Category>().GetByIdAsync(id);
            return new CategoryVM
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                CreatedAt = category.CreatedDate,
                UpdatedAt = category.ModifiedDate,
            };
        }

        public async Task AddAsync(CategoryCreateVM entity)
        {
            var category = new Category
            {
                Name = entity.Name,
                Description = entity.Description,
            };
            await _unitOfWork.GetRepository<Category>().AddAsync(category);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateAsync(CategoryUpdateVM entity)
        {
            var category = new Category
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                ModifiedDate = DateTime.UtcNow
            };
            await _unitOfWork.GetRepository<Category>().UpdateAsync(category);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(CategoryUpdateVM entity)
        {
            var category = new Category
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description
            };
            await _unitOfWork.GetRepository<Category>().DeleteAsync(category);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(CategoryUpdateVM entity)
        {
            var category = new Category
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                IsDeleted = true,
                DeletedDate = DateTime.Now
            };
            await _unitOfWork.GetRepository<Category>().UpdateAsync(category);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task ExecuteUpdateAsync(CategoryUpdateVM entity)
        {
            await _unitOfWork.GetRepository<Category>().GetDbSet()
                .Where(c => c.Id == entity.Id)
                .ExecuteUpdateAsync(c => c
                    .SetProperty(x => x.Name, entity.Name)
                    .SetProperty(x => x.Description, entity.Description)
                    .SetProperty(x=>x.ModifiedDate, DateTime.UtcNow)
                );
        }

        public async Task ExecuteDeleteAsync(CategoryUpdateVM entity)
        {
            await _unitOfWork.GetRepository<Category>().GetDbSet()
                .Where(c => c.Id == entity.Id)
                .ExecuteDeleteAsync();
        }
    }
}
