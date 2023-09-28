using AutoMapper;
using Mango.Services.ShoppingCartAPI.Data;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.Dto;
using Mango.Services.ShoppingCartAPI.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.PortableExecutable;

namespace Mango.Services.ShoppingCartAPI.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class ShoppingCartAPIController : ControllerBase
    {
        private ResponseDto _responseDto;
        private IMapper _mapper;
        private readonly AppDbContext _appDbContext;
        private IProductService _productService;

        public ShoppingCartAPIController(AppDbContext db, IMapper mapper, IProductService productService)
        {
            _appDbContext = db;
            _mapper = mapper;
            _responseDto = new ResponseDto();
            _productService = productService;
        }

        [HttpGet("GetCart/{userId}")]
        public async Task<ResponseDto> GetCart(string userId)
        {
            try
            {
                CartDto cart = new()
                {
                    CartHeader = _mapper.Map<CartHeaderDto>(_appDbContext.CartHeaders.First(u => u.UserId == userId)),
                };

                cart.CartDetails = _mapper.Map<IEnumerable<CartDetailsDto>>(_appDbContext.CartDetails
                    .Where(u => u.CartHeaderId == cart.CartHeader.CartHeaderId));

                var productDtos = await _productService.GetProductDtosAsync();

                foreach (var item in cart.CartDetails)
                {
                    item.Product = productDtos.FirstOrDefault(p => p.ProductId == item.ProductId);
                    cart.CartHeader.CartTotal += item.Count * item.Product.Price;
                }

                _responseDto.Result = cart;
            }
            catch (Exception ex)
            {
                _responseDto.Message = ex.Message;
                _responseDto.IsSuccessful = false;
            }

            return _responseDto;
        }

        [HttpPost("CartUpsert")]
        public async Task<ResponseDto> Upsert(CartDto cartDto)
        {
            try
            {
                var cartHeaderFromDb = await _appDbContext.CartHeaders
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.UserId == cartDto.CartHeader.UserId);

                if (cartHeaderFromDb == null)
                {
                    // Create Header and Details
                    CartHeader cartHeader = _mapper.Map<CartHeader>(cartDto.CartHeader);
                    _appDbContext.CartHeaders.Add(cartHeader);
                    await _appDbContext.SaveChangesAsync();

                    cartDto.CartDetails.First().CartHeaderId = cartHeader.CartHeaderId;
                    CartDetails cartDetails = _mapper.Map<CartDetails>(cartDto.CartDetails.First());
                    _appDbContext.CartDetails.Add(cartDetails);
                }
                else
                {
                    // Check if details has same product
                    var cartDetailsFromDb = await _appDbContext.CartDetails.AsNoTracking()
                        .FirstOrDefaultAsync(
                        u => u.ProductId == cartDto.CartDetails.First().ProductId &&
                        u.CartHeaderId == cartHeaderFromDb.CartHeaderId);

                    if (cartDetailsFromDb == null)
                    {
                        // Create CartDetails
                        cartDto.CartDetails.First().CartHeaderId = cartHeaderFromDb.CartHeaderId;
                        CartDetails cartDetails = _mapper.Map<CartDetails>(cartDto.CartDetails.First());
                        _appDbContext.CartDetails.Add(cartDetails);
                    }
                    else
                    {
                        // Update count in Cart Details
                        cartDto.CartDetails.First().Count += cartDetailsFromDb.Count;
                        cartDto.CartDetails.First().CartHeaderId = cartDetailsFromDb.CartHeaderId;
                        cartDto.CartDetails.First().CartDetailId = cartDetailsFromDb.CartDetailId;
                        CartDetails cartDetails = _mapper.Map<CartDetails>(cartDto.CartDetails.First());
                        _appDbContext.CartDetails.Update(cartDetails);
                    }

                    await _appDbContext.SaveChangesAsync();
                    _responseDto.Result = cartDto;
                }
            }
            catch (Exception ex)
            {
                _responseDto.Message = ex.Message;
                _responseDto.IsSuccessful = false;
            }

            return _responseDto;
        }

        [HttpPost("RemoveCart")]
        public async Task<ResponseDto> RemoveCart([FromBody] int cartDetailsId)
        {
            try
            {
                CartDetails cartDetails = _appDbContext.CartDetails.First(c => c.CartDetailId == cartDetailsId);
                var totalCountCartItem = _appDbContext.CartDetails.Count(c => c.CartHeaderId == cartDetails.CartHeaderId);
                _appDbContext.Remove(cartDetails);

                if (totalCountCartItem == 1)
                {
                    var cartHeaderToRemove = await _appDbContext.CartHeaders
                        .FirstOrDefaultAsync(u => u.CartHeaderId == cartDetails.CartHeaderId);

                    _appDbContext.CartHeaders.Remove(cartHeaderToRemove);
                }

                await _appDbContext.SaveChangesAsync();
                _responseDto.Result = true;
            }
            catch (Exception ex)
            {
                _responseDto.Message = ex.Message;
                _responseDto.IsSuccessful = false;
            }

            return _responseDto;
        }
    }
}
