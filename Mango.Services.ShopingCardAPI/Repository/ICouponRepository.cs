using Mango.Services.ShopingCardAPI.Models.Dto;

namespace Mango.Services.ShopingCardAPI.Repository
{
    public interface ICouponRepository
    {
        Task<CouponDto> GetCouponAsync(string couponName);
    }
}
