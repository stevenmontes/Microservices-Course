using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;
using static Mango.Web.Utility.StaticDetails;

namespace Mango.Web.Service
{
    public class ProductService : IProductService
    {
        private readonly IBaseService _baseService;

        public ProductService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public Task<ResponseDto?> DeleteProductAsync(int id)
        {
            return _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.DELETE,
                Url = $"{ProductAPIBase}/api/product/{id}"
            });
        }

        public Task<ResponseDto?> GetProductAsync()
        {
            return _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.GET,
                Url = $"{ProductAPIBase}/api/product/"
            });
        }

        public Task<ResponseDto?> GetProductAsync(int id)
        {
            return _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.GET,
                Url = $"{ProductAPIBase}/api/product/{id}"
            });
        }

        public Task<ResponseDto?> CreateProductAsync(ProductDto productDto)
        {
            return _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.POST,
                Url = $"{ProductAPIBase}/api/product/",
                Data = productDto
            });
        }

        public Task<ResponseDto?> UpdateProductAsync(ProductDto productDto)
        {
            return _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.PUT,
                Url = $"{ProductAPIBase}/api/product/",
                Data = productDto
            });
        }
    }
}
