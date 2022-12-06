using AutoMapper;
using Mango.Services.ShopingCardAPI.DbContexts;
using Mango.Services.ShopingCardAPI.Models;
using Mango.Services.ShopingCardAPI.Models.Dtos;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.ShopingCardAPI.Repository
{
    public class CartRepository : ICartRepository
    {
        private readonly ApplicationDbContext _db;
        private IMapper _mapper;

        public CartRepository(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<bool> ClearCart(string userId)
        {
            var cartHeaderInDb = await _db.CartHeaders.FirstOrDefaultAsync(u => u.UserId == userId);
            if (cartHeaderInDb == null) { return false; }
            else
            {
                _db.CartDetails.RemoveRange(_db.CartDetails.Where(u => u.CartHeaderId == cartHeaderInDb.CartHeaderId));
                _db.CartHeaders.Remove(cartHeaderInDb);
                await _db.SaveChangesAsync();
                return true;
            }
        }

        public async Task<CartDto> CreateUpdateCart(CartDto cartDto)
        {
            Cart cart = _mapper.Map<Cart>(cartDto);
            //check if product exist in database, if not create
            var productInDb = await _db.Products.FirstOrDefaultAsync(u => u.ProductId == cartDto.CartDetails.FirstOrDefault().ProductId);
            if (productInDb == null)
            {
                _db.Products.Add(cart.CartDetails.FirstOrDefault().Product);
                await _db.SaveChangesAsync();
            }

            //check if header is null
            var cartHeaderInDb = await _db.CartHeaders.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == cart.CartHeader.UserId);
            if (cartHeaderInDb == null)
            {
                _db.CartHeaders.Add(cart.CartHeader);
                await _db.SaveChangesAsync();
                cart.CartDetails.FirstOrDefault().CartHeaderId = cart.CartHeader.CartHeaderId;
                cart.CartDetails.FirstOrDefault().Product = null;
                _db.CartDetails.Add(cart.CartDetails.FirstOrDefault());
                await _db.SaveChangesAsync();
            }
            else
            {
                //if header is not null
                // check if details has same product
                var cartDetailsInDb = await _db.CartDetails.AsNoTracking().FirstOrDefaultAsync(
                    u => u.ProductId == cart.CartDetails.FirstOrDefault().ProductId && u.CartHeaderId == cartHeaderInDb.CartHeaderId);
                if (cartDetailsInDb == null)
                {
                    //create details
                    cart.CartDetails.FirstOrDefault().CartHeaderId = cartHeaderInDb.CartHeaderId;
                    cart.CartDetails.FirstOrDefault().Product = null;
                    _db.CartDetails.Add(cart.CartDetails.FirstOrDefault());
                    await _db.SaveChangesAsync();
                }
                else
                {
                    //update the count / cart details
                    cart.CartDetails.FirstOrDefault().Count += cartDetailsInDb.Count;
                    cart.CartDetails.FirstOrDefault().Product = null;
                    _db.CartDetails.Update(cart.CartDetails.FirstOrDefault());
                    await _db.SaveChangesAsync();
                }
            }

            return _mapper.Map<CartDto>(cart);
        }

        public async Task<CartDto> GetCartByUserId(string id)
        {
            Cart cart = new()
            {
                CartHeader = await _db.CartHeaders.FirstOrDefaultAsync(u => u.UserId == id)
            };

            cart.CartDetails = _db.CartDetails.Where(u => u.CartHeaderId == cart.CartHeader.CartHeaderId).Include(u => u.Product);

            return _mapper.Map<CartDto>(cart);
        }

        public async Task<bool> RemoveFromCart(int cartDetailsId)
        {
            try
            {
                CartDetails cartDetails = await _db.CartDetails
                    .FirstOrDefaultAsync(u => u.CartDetailsId == cartDetailsId);

                int totalCountOfCartItems = _db.CartDetails
                    .Where(u => u.CartHeaderId == cartDetails.CartHeaderId).Count();

                _db.CartDetails.Remove(cartDetails);
                if (totalCountOfCartItems == 1)
                {
                    var cartHeaderToRemove = await _db.CartHeaders
                        .FirstOrDefaultAsync(u => u.CartHeaderId == cartDetails.CartHeaderId);

                    _db.CartHeaders.Remove(cartHeaderToRemove);
                }
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                // todo log exception
                return false;
            }
        }
    }
}
