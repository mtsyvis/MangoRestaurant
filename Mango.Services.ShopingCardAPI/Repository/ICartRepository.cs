using Mango.Services.ShopingCardAPI.Models.Dtos;

namespace Mango.Services.ShopingCardAPI.Repository
{
    public interface ICartRepository
    {
        Task<CartDto> GetCartByUserId(string id);
        Task<CartDto> CreateUpdateCart(CartDto cartDto);
        Task<bool> RemoveFromCart(int cartDetailsId);
        Task<bool> ClearCart(string userId);
    }
}
