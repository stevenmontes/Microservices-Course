using Mango.Web.Models;
using Mango.Web.Service.IService;
using static Mango.Web.Utility.StaticDetails;

namespace Mango.Web.Service
{
    public class CartService : ICartService
    {
        private readonly IBaseService _baseService;

        public CartService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ResponseDto?> ApplyCouponAsync(CartDto cart)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.POST,
                Url = $"{CartAPIBase}/api/cart/ApplyCoupon",
                Data = cart
            });
        }

        public async Task<ResponseDto?> GetCartAsync(string userId)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.GET,
                Url = $"{CartAPIBase}/api/cart/GetCart/{userId}"
            });
        }

        public async Task<ResponseDto?> RemoveCartAsync(int cartDetailsId)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.POST,
                Url = $"{CartAPIBase}/api/cart/RemoveCart",
                Data = cartDetailsId
            });
        }

        public async Task<ResponseDto?> UpsertAsync(CartDto cart)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.POST,
                Url = $"{CartAPIBase}/api/cart/CartUpsert",
                Data = cart
            });
        }
    }
}
