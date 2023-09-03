using Mango.Web.Models;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Web.Controllers
{
    public class CouponController : Controller
    {
        private readonly ICouponService _couponService;

        public CouponController(ICouponService couponService)
        {
            _couponService = couponService;
        }

        public async Task<IActionResult> CouponIndex()
        {
            List<CouponDto>? couponDtos = new();
            ResponseDto? response = await _couponService.GetAllCouponsAsync();

            if (response != null && response.IsSuccessful)
            {
                couponDtos = JsonConvert.DeserializeObject<List<CouponDto>>(Convert.ToString(response.Result)!);
            }
            else
            {
                TempData["error"] = response.Message;
            }

            return View(couponDtos);
        }

        public async Task<IActionResult> CouponCreate() => View();

        [HttpPost]
        public async Task<IActionResult> CouponCreate(CouponDto coupon)
        {
            if (ModelState.IsValid)
            {
                ResponseDto? response = await _couponService.CreateCouponAsync(coupon);

                if (response != null && response.IsSuccessful)
                {
                    TempData["sucess"] = "Coupon created successfully";
                    return RedirectToAction(nameof(CouponIndex));
                }
                else
                {
                    TempData["error"] = response.Message;
                }
            }
            return View();
        }

        public async Task<IActionResult> CouponDelete(int couponId)
        {
            ResponseDto? response = await _couponService.GetCouponByIdAsync(couponId);

            if (response != null && response.IsSuccessful)
            {
                CouponDto model = JsonConvert.DeserializeObject<CouponDto>(Convert.ToString(response.Result)!);
                return View(model);
            }
            else
            {
                TempData["error"] = response.Message;
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> CouponDelete(CouponDto coupon)
        {
            ResponseDto? response = await _couponService.DeleteCouponAsync(coupon.CouponId);

            if (response != null && response.IsSuccessful)
            {
                TempData["sucess"] = "Coupon deleted successfully";
                return RedirectToAction(nameof(CouponIndex));
            }
            else
            {
                TempData["error"] = response.Message;
            }

            return View();
        }
    }
}
