using BLLProject.Interfaces;
using BLLProject.Specifications;
using DALProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PLProj.Models;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Utility;


namespace PLProj.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly UserManager<AppUser> _userManager;

        public CartController(IUnitOfWork unitOfWork,
            IConfiguration configuration,
            UserManager<AppUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _userManager = userManager;
        }

        #region Index
        public IActionResult Index()
        {
            var userId = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value;

            var spec = new BaseSpecification<ShoppingCart>(d => d.UserId == userId);
            spec.Includes.Add(s => s.Product);

            ShoppingCartVM cartVM = new ShoppingCartVM
            {
                CartsList = _unitOfWork.Repository<ShoppingCart>().GetAllWithSpec(spec)
            };

            cartVM.TotalCarts = cartVM.CartsList.Sum(item => item.count * item.Product.Price);

            return View(cartVM);
        }
        #endregion

        #region Summary
        public async Task<IActionResult> Summary()
        {
            var userId = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value;

            var CartSpec = new BaseSpecification<ShoppingCart>(d => d.UserId == userId);
            CartSpec.Includes.Add(s => s.Product);
            ShoppingCartVM cartVM = new ShoppingCartVM
            {
                CartsList = _unitOfWork.Repository<ShoppingCart>().GetAllWithSpec(CartSpec),
                OrderHeader = new OrderHeader(),
            };

            var user = await _userManager.GetUserAsync(User);

            if (cartVM.OrderHeader != null)
            {
                cartVM.OrderHeader.Name = user.Name;
                cartVM.OrderHeader.City = user.City;
                cartVM.OrderHeader.Street = user.Street;
                cartVM.OrderHeader.PhoneNumber = user.PhoneNumber;
            }
            else
            {
                ModelState.AddModelError("", "Some required data is missing.");
                return View(cartVM);
            }

            cartVM.OrderHeader.TotalPrice = cartVM.CartsList.Sum(item => item.count * item.Product.Price);

            return View(cartVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Summary(ShoppingCartVM shoppingCartVM)
        {
            var userId = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value;

            var spec = new BaseSpecification<ShoppingCart>(d => d.UserId == userId);
            spec.Includes.Add(s => s.Product);
            shoppingCartVM.CartsList = _unitOfWork.Repository<ShoppingCart>().GetAllWithSpec(spec);

            var existingOrder = _unitOfWork.Repository<OrderHeader>()
                .GetEntityWithSpec(new BaseSpecification<OrderHeader>
                (u => u.UserId == userId && u.PaymentStatus == SD.Pending));

            if (existingOrder != null)
            {
                // تحديث الطلب الحالي
                existingOrder.Name = shoppingCartVM.OrderHeader.Name;
                existingOrder.Street = shoppingCartVM.OrderHeader.Street;
                existingOrder.City = shoppingCartVM.OrderHeader.City;
                existingOrder.PhoneNumber = shoppingCartVM.OrderHeader.PhoneNumber;
                existingOrder.TotalPrice = 0;
                foreach (var item in shoppingCartVM.CartsList)
                {
                    existingOrder.TotalPrice += (item.count * item.Product.Price);
                }

                existingOrder.OrderDate = DateTime.Now;
                _unitOfWork.Repository<OrderHeader>().Update(existingOrder);
                _unitOfWork.Complete();

                // حذف تفاصيل الطلب القديمة المرتبطة بالطلب (إن وجدت)
                var oldOrderDetails = _unitOfWork.Repository<OrderDetail>()
                    .GetAllWithSpec(new BaseSpecification<OrderDetail>(od => od.OrderHeaderId == existingOrder.Id));

                _unitOfWork.Repository<OrderDetail>().RemoveRange(oldOrderDetails);
                shoppingCartVM.OrderHeader = existingOrder;
            }
            else
            {
                // إنشاء طلب جديد
                shoppingCartVM.OrderHeader = new OrderHeader()
                {
                    UserId = userId,
                    OrderDate = DateTime.Now,
                    OrderStatus = SD.Pending,
                    PaymentStatus = SD.Pending,
                    Name = shoppingCartVM.OrderHeader.Name,
                    Street = shoppingCartVM.OrderHeader.Street,
                    City = shoppingCartVM.OrderHeader.City,
                    PhoneNumber = shoppingCartVM.OrderHeader.PhoneNumber,
                };

                shoppingCartVM.OrderHeader.TotalPrice = shoppingCartVM.CartsList
                    .Sum(item => item.count * item.Product.Price);

                _unitOfWork.Repository<OrderHeader>().Add(shoppingCartVM.OrderHeader);
            }
            _unitOfWork.Complete();

            foreach (var item in shoppingCartVM.CartsList)
            {
                OrderDetail orderDetial = new OrderDetail()
                {
                    ProductId = item.ProductId,
                    OrderHeaderId = shoppingCartVM.OrderHeader.Id,
                    Price = item.Product.Price,
                    Count = item.count
                };
                _unitOfWork.Repository<OrderDetail>().Add(orderDetial);
            }
            _unitOfWork.Complete();

            var items = shoppingCartVM.CartsList
                        .Select(p => (p.Product.Name, p.Product.Price, p.count))
                        .ToList();

            var stripeSessionService = new StripeSessionService(_configuration);
            var session = stripeSessionService.CreateSession(
                        shoppingCartVM.OrderHeader.Id,
                        items,
                        successPath: "/Cart/OrderConfirmation",
                        cancelPath: "/Cart/index"
                        );

            _unitOfWork.OrderHeaderRepository.UpdateStripePaymentId
                (shoppingCartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
            _unitOfWork.Complete();

            #region old
            //var domain = _configuration["Stripe:Domain"];
            //var option = new SessionCreateOptions
            //{
            //    PaymentMethodTypes = new List<string> { "card" },
            //    LineItems = new List<SessionLineItemOptions>(),
            //    Mode = "payment",
            //    SuccessUrl = domain + $"Cart/OrderConfirmation?Id={shoppingCartVM.OrderHeader.Id}",
            //    CancelUrl = domain + $"Cart/index",
            //};
            //foreach (var item in shoppingCartVM.CartsList)
            //{
            //    var sessionLineOption = new SessionLineItemOptions
            //    {
            //        PriceData = new SessionLineItemPriceDataOptions
            //        {
            //            UnitAmount = (long)(item.Product.Price * 100),
            //            Currency = "usd",
            //            ProductData = new SessionLineItemPriceDataProductDataOptions
            //            {
            //                Name = item.Product.Name,
            //            }
            //        },
            //        Quantity = item.count,
            //    };
            //    option.LineItems.Add(sessionLineOption);
            //}

            //var service = new SessionService();
            //Session session = service.Create(option);

            //_unitOfWork.OrderHeaderRepository.UpdateStripePaymentId
            //    (shoppingCartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
            //_unitOfWork.Complete();

            //Response.Headers["Location"] = session.Url.ToString();
            ////Response.Headers.Add("Location", session.Url);
            //return new StatusCodeResult(303);
            #endregion

            Response.Headers["Location"] = session.Url.ToString();
            return new StatusCodeResult(303);
        }
        #endregion

        #region OrderConfirmation
        public IActionResult OrderConfirmation(int id)
        {
            OrderHeader orderHeader = _unitOfWork.Repository<OrderHeader>().Get(id);
            var service = new SessionService();
            Session session = service.Get(orderHeader.SessionId);
            if (session.PaymentStatus.ToLower() == "paid")
            {
                _unitOfWork.OrderHeaderRepository.UpdateStripePaymentId(id, session.Id, session.PaymentIntentId);
                _unitOfWork.OrderHeaderRepository.UpdateStatus(id, SD.Approve, SD.Approve);
                _unitOfWork.Complete();
            }
            HttpContext.Session.Clear();
            List<ShoppingCart> shoppingCarts = _unitOfWork.Repository<ShoppingCart>()
                .GetAllWithSpec(new BaseSpecification<ShoppingCart>
                (u => u.UserId == orderHeader.UserId)).ToList();
            if (shoppingCarts.Any())
            {
                _unitOfWork.ShoppingCart.RemoveRange(shoppingCarts);
            }
            _unitOfWork.Complete();
            return View(id);
        } 
        #endregion

        #region Plus
        public IActionResult Plus(int cartid)
        {
            var shoopingcart = _unitOfWork.Repository<ShoppingCart>().Get( cartid);
            _unitOfWork.ShoppingCart.IncreaseCount(shoopingcart, 1);
            _unitOfWork.Complete();
            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Minus
        public IActionResult Minus(int cartid)
        {
            var shoopingcart = _unitOfWork.Repository<ShoppingCart>().Get(cartid);

            if (shoopingcart == null)
            {
                return NotFound();
            }

            var currentCartCount = _unitOfWork.Repository<ShoppingCart>()
                    .GetAllWithSpec(new BaseSpecification<ShoppingCart>(u => u.UserId == shoopingcart.UserId)).Count();

            if (shoopingcart.count <= 1)
            {
                HttpContext.Session.SetInt32(SD.SessionCart, Math.Max(0, currentCartCount - 1));
                _unitOfWork.ShoppingCart.Delete(shoopingcart);
            }
            else
            {
                _unitOfWork.ShoppingCart.DecreaseCount(shoopingcart, 1);
            }

            _unitOfWork.Complete();
            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Remove
        public IActionResult Remove(int cartid)
        {
            var shoopingcart = _unitOfWork.Repository<ShoppingCart>().Get(cartid);

            var currentCartCount = _unitOfWork.Repository<ShoppingCart>()
                .GetAllWithSpec(new BaseSpecification<ShoppingCart>(u => u.UserId == shoopingcart.UserId)).Count();

            HttpContext.Session.SetInt32(SD.SessionCart, Math.Max(0, currentCartCount - 1));

            _unitOfWork.ShoppingCart.Delete(shoopingcart);
            _unitOfWork.Complete();
            return RedirectToAction(nameof(Index));
        }
        #endregion

    }
}

