using Entities.Domains;
using Entities.Repositories;
using Services.RateService.RateServiceVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Services.RateService
{
    public class RateService : IRateService
    {
        private readonly IUnitOfWork unitOfWork;

        public RateService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task AddRateAsync(RateVM rateVM)
        {

            var rateVMS = new Rate
            {
                BookId = rateVM.BookId,
                Rating = rateVM.Rating,
                CreatedDate = rateVM.CreatedDate,
                UserId = rateVM.UserId,
                ModifiedDate = rateVM.ModifiedDate,
            };
            await unitOfWork.GetRepository<Rate>().AddAsync(rateVMS);
            unitOfWork.SaveChangesAsync().GetAwaiter().GetResult();
        }

        public async Task UpdateRateAsync(RateVM rateVM)
        {

            var rateVMS = new Rate
            {
                Rating = rateVM.Rating,
                CreatedDate = rateVM.CreatedDate,
                ModifiedDate = rateVM.ModifiedDate,
            };
            await unitOfWork.GetRepository<Rate>().UpdateAsync(rateVMS);
            await unitOfWork.SaveChangesAsync();
        }
    }
}
