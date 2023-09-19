using AutoMapper;
using Mango.Services.ProductAPI.Data;
using Mango.Services.ProductAPI.Models;
using Mango.Services.ProductAPI.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.ProductAPI.Controllers
{
    [Route("api/product")]
    [ApiController]
    public class ProductAPIController : ControllerBase
    {
        private readonly AppDbContext _db;
        private ResponseDto _responseDto;
        private IMapper _mapper;

        public ProductAPIController(AppDbContext appDbContext, IMapper mapper)
        {
            _db = appDbContext;
            _responseDto = new ResponseDto();
            _mapper = mapper;
        }

        [HttpGet]
        public ResponseDto Get()
        {
            try
            {
                var products = _db.Products.ToList();
                _responseDto.Result = _mapper.Map<IEnumerable<ProductDto>>(products);
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccessful = false;
                _responseDto.Message = ex.Message;
            }

            return _responseDto;
        }

        [HttpGet]
        [Route("{id:int}")]
        public ResponseDto Get(int id)
        {
            try
            {
                var product = _db.Products.First(p => p.ProductId == id);
                _responseDto.Result = _mapper.Map<ProductDto>(product);
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccessful = false;
                _responseDto.Message = ex.Message;
            }

            return _responseDto;
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public ResponseDto Post([FromBody] ProductDto productDto)
        {
            try
            {
                var product = _mapper.Map<Product>(productDto);
                _db.Products.Add(product);
                _db.SaveChanges();

                _responseDto.Result = productDto;
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccessful = false;
                _responseDto.Message = ex.Message;
            }
            
            return _responseDto;
        }

        [HttpPut]
        [Authorize(Roles = "ADMIN")]
        public ResponseDto Put([FromBody] ProductDto productDto)
        {
            try
            {
                var product = _mapper.Map<Product>(productDto);
                _db.Products.Update(product);
                _db.SaveChanges();

                _responseDto.Result = productDto;
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccessful = false;
                _responseDto.Message = ex.Message;
            }

            return _responseDto;
        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "ADMIN")]
        public ResponseDto Delete(int id)
        {
            try
            {
                var product = _db.Products.First(p => p.ProductId == id);
                _db.Products.Remove(product);
                _db.SaveChanges();

                _responseDto.Message = "Product deleted successfully";
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccessful = false;
                _responseDto.Message = ex.Message;
            }

            return _responseDto;
        }
    }
}
