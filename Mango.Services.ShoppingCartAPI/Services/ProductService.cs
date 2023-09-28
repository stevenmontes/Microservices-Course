using Mango.Services.ShoppingCartAPI.Models.Dto;
using Mango.Services.ShoppingCartAPI.Services.IServices;
using Newtonsoft.Json;

namespace Mango.Services.ShoppingCartAPI.Services
{
    public class ProductService : IProductService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ProductService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IEnumerable<ProductDto>> GetProductDtosAsync()
        {
            var client = _httpClientFactory.CreateClient("Product");
            var response = await client.GetAsync($"/api/product");
            var apiContent = await response.Content.ReadAsStringAsync();
            var responseDto = JsonConvert.DeserializeObject<ResponseDto>(apiContent);

            if (responseDto!.IsSuccessful)
            {
                return JsonConvert.DeserializeObject<IEnumerable<ProductDto>>(responseDto!.Result.ToString());
            }

            return new List<ProductDto>();
        }
    }
}
