using Mango.Services.ShopingCardAPI.Models.Dto;
using Mango.Services.ShopingCardAPI.Models.Dtos;
using Microsoft.AspNetCore.Authentication;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Mango.Services.ShopingCardAPI.Repository
{
    public class CouponRepository : ICouponRepository
    {
        private readonly HttpClient client;
        private readonly IHttpContextAccessor httpContextAccessor;

        public CouponRepository(HttpClient client, IHttpContextAccessor httpContextAccessor)
        {
            this.client = client;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<CouponDto> GetCouponAsync(string couponName)
        {
            var token = await httpContextAccessor.HttpContext.GetTokenAsync("access_token");
            this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync($"/api/coupon/{couponName}");
            var apiContent = await response.Content.ReadAsStringAsync();
            var resp = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
            if (resp.IsSuccess)
            {
                return JsonConvert.DeserializeObject<CouponDto>(Convert.ToString(resp.Result));
            }
            return new CouponDto();
        }
    }
}
