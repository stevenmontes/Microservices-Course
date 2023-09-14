using Mango.Web.Models;
using Mango.Web.Service;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> ProductIndex()
        {
            List<ProductDto>? productDtos = new();
            ResponseDto? response = await _productService.GetProductAsync();

            if (response != null && response.IsSuccessful)
            {
                productDtos = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(response.Result)!);
            }
            else
            {
                TempData["error"] = response.Message;
            }

            return View(productDtos);
        }

        [HttpGet]
        public IActionResult ProductCreate() => View();

        [HttpPost]
        public async Task<IActionResult> ProductCreate(ProductDto product)
        {
            if (ModelState.IsValid)
            {
                ResponseDto? response = await _productService.CreateProductAsync(product);

                if (response != null && response.IsSuccessful)
                {
                    TempData["sucess"] = "Product created successfully";
                    return RedirectToAction(nameof(ProductIndex));
                }
                else
                {
                    TempData["error"] = response.Message;
                }
            }

            return View(product);
        }

        [HttpGet]
        public async Task<IActionResult> ProductEdit(int productId)
        {
            var response = await _productService.GetProductAsync(productId);

            if (response != null && response.IsSuccessful)
            {
                var productDto = JsonConvert.DeserializeObject<ProductDto?>(response.Result.ToString());
                return View(productDto);
            }
            else
            {
                TempData["error"] = response.Message;
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> ProductEdit(ProductDto product)
        {
            if (ModelState.IsValid)
            {
                ResponseDto? response = await _productService.UpdateProductAsync(product);

                if (response != null && response.IsSuccessful)
                {
                    TempData["sucess"] = "Product updated successfully";
                    return RedirectToAction(nameof(ProductIndex));
                }
                else
                {
                    TempData["error"] = response.Message;
                }
            }

            return View(product);
        }

		public async Task<IActionResult> ProductDelete(int productId)
		{
            var response = await _productService.GetProductAsync(productId);

            if (response != null && response.IsSuccessful)
            {
                var productDto = JsonConvert.DeserializeObject<ProductDto?>(response.Result.ToString());
                return View(productDto);
            }
            else
            {
                TempData["error"] = response.Message;
            }

            return NotFound();
        }

		[HttpPost]
        public async Task<IActionResult> ProductDelete(ProductDto product)
        {
            if (ModelState.IsValid)
            {
                ResponseDto? response = await _productService.DeleteProductAsync(product.ProductId);

                if (response != null && response.IsSuccessful)
                {
                    TempData["sucess"] = "Product deleted successfully";
                    return RedirectToAction(nameof(ProductIndex));
                }
                else
                {
                    TempData["error"] = response.Message;
                }
            }

            return View(product);
        }
    }
}
