using Azure;
using Mango.Services.CouponAPI.Models.Dto;
using Mango.Services.CouponAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.CouponAPI.Controllers
{
    [Route("api/coupon")]
    [ApiController]
    public class CouponController : ControllerBase
    {
        private readonly ICouponRepository couponRepository;
        protected ResponseDto response;

        public CouponController(ICouponRepository couponRepository)
        {
            this.couponRepository = couponRepository;
            this.response = new ResponseDto();
        }

        [Authorize]
        [HttpGet("{code}")]
        public async Task<object> GetDiscountForCode(string code)
        {
            try
            {
                CouponDto couponDto = await couponRepository.GetCouponByCode(code);
                response.Result = couponDto;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return response;
        }
    }
}
