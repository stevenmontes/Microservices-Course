using Mango.Web.Models;

namespace Mango.Web.Service.IService
{
    public interface ICartService
    {
        Task<ResponseDto?> ApplyCouponAsync(CartDto cart);
        Task<ResponseDto?> GetCartAsync(string userId);
        Task<ResponseDto?> UpsertAsync(CartDto cart);
        Task<ResponseDto?> RemoveCartAsync(int cartDetailsId);
    }
}
