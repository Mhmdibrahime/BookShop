using Entities.Domains;
using Entities.Repositories;
using Services.PublisherServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Services.PublisherServices.PublisherViewModels;

namespace Services.PublisherServices
{
    public class PublisherService : IPublisherService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PublisherService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<PublisherUpdateVM>> GetAllAsync(Expression<Func<Publisher, bool>>? filter = null)
        {
            var categories = await _unitOfWork.GetRepository<Publisher>().GetAllAsync(filter);
            return categories.Select(c => new PublisherUpdateVM
            {
                Id = c.Id,
                Name = c.Name,
                Email = c.Email,
                Address = c.Address,
                Phone = c.Phone
            });
        }

        public async Task<PublisherVM> GetByIdAsync(object id)
        {
            var Publisher = await _unitOfWork.GetRepository<Publisher>().GetByIdAsync(id);
            return new PublisherVM
            {
                Id = Publisher.Id,
                Name = Publisher.Name,
                Email = Publisher.Email,
                Address = Publisher.Address,
                Phone = Publisher.Phone,
                UpdatedAt = Publisher.ModifiedDate,
                CreatedAt = Publisher.CreatedDate
            };
        }

        public async Task AddAsync(PublisherCreateVM entity)
        {
            var Publisher = new Publisher
            {
                Name = entity.Name,
                Email = entity.Email,
                Address = entity.Address,
                Phone = entity.Phone
            };
            await _unitOfWork.GetRepository<Publisher>().AddAsync(Publisher);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateAsync(PublisherUpdateVM entity)
        {
            var Publisher = new Publisher
            {
                Id = entity.Id,
                Name = entity.Name,
                Email = entity.Email,
                Address = entity.Address,
                Phone = entity.Phone,
                ModifiedDate = DateTime.UtcNow
            };
            await _unitOfWork.GetRepository<Publisher>().UpdateAsync(Publisher);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(PublisherUpdateVM entity)
        {
            var Publisher = new Publisher
            {
                Id = entity.Id,
                Name = entity.Name,
                Email = entity.Email,
                Address = entity.Address,
                Phone = entity.Phone
            };
            await _unitOfWork.GetRepository<Publisher>().DeleteAsync(Publisher);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(PublisherUpdateVM entity)
        {
            var Publisher = new Publisher
            {
                Id = entity.Id,
                Name = entity.Name,
                Email = entity.Email,
                Address = entity.Address,
                Phone = entity.Phone,
                IsDeleted = true,
                DeletedDate = DateTime.Now
            };
            await _unitOfWork.GetRepository<Publisher>().UpdateAsync(Publisher);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task ExecuteUpdateAsync(PublisherUpdateVM entity)
        {
            await _unitOfWork.GetRepository<Publisher>().GetDbSet()
                .Where(c => c.Id == entity.Id)
                .ExecuteUpdateAsync(c => c
                    .SetProperty(x => x.Name, entity.Name)
                    .SetProperty(x => x.Email, entity.Email)
                    .SetProperty(x => x.Address, entity.Address)
                    .SetProperty(x => x.Phone, entity.Phone)
                    .SetProperty(x => x.ModifiedDate, DateTime.UtcNow)
                );
        }

        public async Task ExecuteDeleteAsync(PublisherUpdateVM entity)
        {
            await _unitOfWork.GetRepository<Publisher>().GetDbSet()
                .Where(c => c.Id == entity.Id)
                .ExecuteDeleteAsync();
        }
    }
}
