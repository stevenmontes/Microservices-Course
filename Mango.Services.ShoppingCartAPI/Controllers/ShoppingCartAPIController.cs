﻿using AutoMapper;
using Mango.Services.ShoppingCartAPI.Data;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.Dto;
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

        public ShoppingCartAPIController(AppDbContext db, IMapper mapper)
        {
            _appDbContext = db;
            _mapper = mapper;
            _responseDto = new ResponseDto();
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
                    await _appDbContext.SaveChangesAsync();
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
                        await _appDbContext.SaveChangesAsync();
                    }
                    else
                    {
                        // Update count in Cart Details
                        cartDto.CartDetails.First().Count += cartDetailsFromDb.Count;
                        cartDto.CartDetails.First().CartHeaderId = cartDetailsFromDb.CartHeaderId;
                        cartDto.CartDetails.First().CartDetailId = cartDetailsFromDb.CartDetailId;
                        CartDetails cartDetails = _mapper.Map<CartDetails>(cartDto.CartDetails.First());
                        _appDbContext.CartDetails.Update(cartDetails);
                        await _appDbContext.SaveChangesAsync();
                    }

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
    }
}