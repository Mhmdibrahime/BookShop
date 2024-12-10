using Services.RateService.RateServiceVM;

namespace Services.RateService
{
    public interface IRateService
    {
        Task AddRateAsync(RateVM rateVM);
        Task UpdateRateAsync(RateVM rateVM);

    }
}