using AutoMapper;
using GeekShopping.CartAPI.Data.DTO;
using GeekShopping.CartAPI.Model;
using GeekShopping.CartAPI.Model.Context;
using Microsoft.EntityFrameworkCore;

namespace GeekShopping.CartAPI.Repository
{
    public class CartRepository : ICartRepository
    {
        private readonly MySqlContext _context;
        private IMapper _mapper;

        public CartRepository(MySqlContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<bool> ApplyCoupon(string userId, string couponCode)
        {
            var cartHeader = await _context.CartHeaders.FirstOrDefaultAsync(_ => _.UserId == userId);

            if (cartHeader != null)
            {
                cartHeader.CouponCode = couponCode;
                _context.CartHeaders.Update(cartHeader);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<bool> RemoveCoupon(string userId)
        {
            var cartHeader = await _context.CartHeaders.FirstOrDefaultAsync(_ => _.UserId == userId);

            if (cartHeader != null)
            {
                cartHeader.CouponCode = string.Empty;
                _context.CartHeaders.Update(cartHeader);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<bool> ClearCart(string userId)
        {
            var cartHeader = await _context.CartHeaders.FirstOrDefaultAsync(_ => _.UserId == userId);

            if (cartHeader != null)
            {
                _context.CartDetails.RemoveRange(
                    _context.CartDetails.Where(_ => _.CartHeaderId == cartHeader.Id)
                    );

                _context.CartHeaders.Remove(cartHeader);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<CartDTO> FindCartByUserId(string userId)
        {
            Cart cart = new()
            {
                CartHeader = await _context.CartHeaders.FirstOrDefaultAsync(_ => _.UserId == userId),
            };
            cart.CartDetails = _context.CartDetails
                .Where(_ => _.CartHeaderId == cart.CartHeader.Id)
                .Include(_ => _.Product);

            if(cart.CartHeader != null &&  cart.CartDetails != null)
                return _mapper.Map<CartDTO>(cart);

            return null;
        }

        public async Task<bool> RemoveFromCart(long cartDetailsId)
        {
            try
            {
                CartDetail cartDetail = await _context.CartDetails.FirstOrDefaultAsync(_ => _.Id == cartDetailsId);

                int total = _context.CartDetails.Where(_ => _.CartHeaderId == cartDetail.CartHeaderId).Count();

                _context.CartDetails.Remove(cartDetail);
                if (total == 1)
                {
                    var cartHeaderToRemove = await _context.CartHeaders.FirstOrDefaultAsync(_ => _.Id == cartDetail.CartHeaderId);

                    _context.CartHeaders.Remove(cartHeaderToRemove);
                }

                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<CartDTO> SaveOrUpdateCart(CartDTO cartDTO)
        {
            Cart cart = _mapper.Map<Cart>(cartDTO);

            var product = await _context.Products.FirstOrDefaultAsync(_ =>
            _.Id == cartDTO.CartDetails.FirstOrDefault().ProductId);

            if (product == null)
            {
                _context.Products.Add(cart.CartDetails.FirstOrDefault().Product);
                await _context.SaveChangesAsync();
            }

            var cartHeader = await _context.CartHeaders.AsNoTracking().FirstOrDefaultAsync(_ => _.UserId == cart.CartHeader.UserId);

            if (cartHeader == null)
            {
                _context.CartHeaders.Add(cart.CartHeader);
                await _context.SaveChangesAsync();
                cart.CartDetails.FirstOrDefault().CartHeaderId = cart.CartHeader.Id;
                cart.CartDetails.FirstOrDefault().Product = null;
                _context.CartDetails.Add(cart.CartDetails.FirstOrDefault());
                await _context.SaveChangesAsync();
            }
            else
            {
                var cartDetail = await _context.CartDetails.AsNoTracking().FirstOrDefaultAsync(_ =>
                _.ProductId == cart.CartDetails.FirstOrDefault().ProductId
                && _.CartHeaderId == cartHeader.Id);

                if (cartDetail == null)
                {
                    cart.CartDetails.FirstOrDefault().CartHeaderId = cartHeader.Id;
                    cart.CartDetails.FirstOrDefault().Product = null;
                    _context.CartDetails.Add(cart.CartDetails.FirstOrDefault());
                    await _context.SaveChangesAsync();
                }
                else
                {
                    cart.CartDetails.FirstOrDefault().Product = null;
                    cart.CartDetails.FirstOrDefault().Count += cartDetail.Count;
                    cart.CartDetails.FirstOrDefault().Id = cartDetail.Id;
                    cart.CartDetails.FirstOrDefault().CartHeaderId = cartDetail.CartHeaderId;

                    _context.CartDetails.Update(cart.CartDetails.FirstOrDefault());

                    await _context.SaveChangesAsync();
                }
            }

            return _mapper.Map<CartDTO>(cart);
        }
    }
}