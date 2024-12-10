using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Entities.Domains;
using Services.PublisherServices.PublisherViewModels;

namespace Services.PublisherServices
{
    public interface IPublisherService
    {
        Task<IEnumerable<PublisherUpdateVM>> GetAllAsync(
            Expression<Func<Publisher, bool>>? filter = null);
        Task<PublisherVM> GetByIdAsync(object id);
        Task AddAsync(PublisherCreateVM entity);
        Task UpdateAsync(PublisherUpdateVM entity);
        Task DeleteAsync(PublisherUpdateVM entity);
        Task SoftDeleteAsync(PublisherUpdateVM entity);
        Task ExecuteUpdateAsync(PublisherUpdateVM entity);
        Task ExecuteDeleteAsync(PublisherUpdateVM entity);
    }
}
