using Entities.Domains;
using Entities.Repositories;
using Services.AuthorServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Services.AuthorServices.AuthorViewModels;

namespace Services.AuthorServices
{
    public class AuthorService : IAuthorService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AuthorService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<AuthorUpdateVM>> GetAllAsync(Expression<Func<Author, bool>>? filter = null)
        {
            var categories = await _unitOfWork.GetRepository<Author>().GetAllAsync(filter);
            return categories.Select(c => new AuthorUpdateVM
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                BirthDate = c.BirthDate,
                Email = c.Email
            });
        }

        public async Task<AuthorVM> GetByIdAsync(object id)
        {
            var Author = await _unitOfWork.GetRepository<Author>().GetByIdAsync(id);
            return new AuthorVM
            {
                Id = Author.Id,
                Name = Author.Name,
                Description = Author.Description,
                BirthDate = Author.BirthDate,
                Email = Author.Email,
                CreatedAt = Author.CreatedDate,
                UpdatedAt = Author.ModifiedDate
            };
        }

        public async Task AddAsync(AuthorCreateVM entity)
        {
            var Author = new Author
            {
                Name = entity.Name,
                Description = entity.Description,
                BirthDate = entity.BirthDate,
                Email = entity.Email
            };
            await _unitOfWork.GetRepository<Author>().AddAsync(Author);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateAsync(AuthorUpdateVM entity)
        {
            var Author = new Author
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                BirthDate = entity.BirthDate,
                Email = entity.Email,
                ModifiedDate = DateTime.UtcNow
            };
            await _unitOfWork.GetRepository<Author>().UpdateAsync(Author);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(AuthorUpdateVM entity)
        {
            var Author = new Author
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                BirthDate = entity.BirthDate,
                Email = entity.Email
            };
            await _unitOfWork.GetRepository<Author>().DeleteAsync(Author);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(AuthorUpdateVM entity)
        {
            var Author = new Author
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                BirthDate = entity.BirthDate,
                Email = entity.Email,
                IsDeleted = true,
                DeletedDate = DateTime.Now
            };
            await _unitOfWork.GetRepository<Author>().UpdateAsync(Author);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task ExecuteUpdateAsync(AuthorUpdateVM entity)
        {
            await _unitOfWork.GetRepository<Author>().GetDbSet()
                .Where(c => c.Id == entity.Id)
                .ExecuteUpdateAsync(c => c
                    .SetProperty(x => x.Name, entity.Name)
                    .SetProperty(x => x.Description, entity.Description)
                    .SetProperty(x => x.ModifiedDate, DateTime.UtcNow)
                    .SetProperty(x => x.BirthDate, entity.BirthDate)
                    .SetProperty(x => x.Email, entity.Email)
                );
        }

        public async Task ExecuteDeleteAsync(AuthorUpdateVM entity)
        {
            await _unitOfWork.GetRepository<Author>().GetDbSet()
                .Where(c => c.Id == entity.Id)
                .ExecuteDeleteAsync();
        }
    }
}
