using BLLProject.Interfaces;
using BLLProject.Specifications;
using DALProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PLProj.Models;
using System.Linq;
using System.Security.Claims;
using Utility;
using X.PagedList;

namespace PLProj.Controllers
{
    [Authorize]
    public class StoreController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public StoreController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region Index
        public IActionResult Index(int? page)
        {
            int pageSize = 6;
            int pageNumber = page ?? 1;

            var partServices = _unitOfWork.Repository<Product>().GetAll().Select(s => (ProductViewModel)s)
                .ToPagedList(pageNumber, pageSize);

            var identity = (ClaimsIdentity)User.Identity;
            var claim = identity?.FindFirst(ClaimTypes.NameIdentifier);
            if (claim?.Value != null) // user login in
            {
                HttpContext.Session.SetInt32(SD.SessionCart,
                 _unitOfWork.Repository<ShoppingCart>().
                 GetAllWithSpec(new BaseSpecification<ShoppingCart>(u => u.UserId == claim.Value)).Count());
            }


            return View(partServices);
        }
        #endregion

        #region  Details       

        public IActionResult Details(int Id)
        {
            var spec = new BaseSpecification<Product>(e => e.Id == Id);
            spec.Includes.Add(e => e.ProductCategory);
            spec.ComplexIncludes.Add(p => p.Include(ps => ps.ModelParts)
                   .ThenInclude(mp => mp.Model));
            ShoppingCart shoppingCart = new ShoppingCart()
            {
                Product = _unitOfWork.Repository<Product>().GetEntityWithSpec(spec),
                ProductId = Id,
                count = 1
            };
            return View((CartVM)shoppingCart);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Details(CartVM shoppingCart)
        {
            if (!ModelState.IsValid)
                return View();

            var userId = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value;
            shoppingCart.UserId = userId;

            var cartSpec = new BaseSpecification<ShoppingCart>(u =>
                u.UserId == userId &&
                u.ProductId == shoppingCart.ProductId);
            cartSpec.ComplexIncludes.Add(c => c.Include(m => m.Product)
                       .ThenInclude(b => b.ProductCategory));

            var cartObj = _unitOfWork.Repository<ShoppingCart>().GetEntityWithSpec(cartSpec);

            if (cartObj == null)
            {
                _unitOfWork.Repository<ShoppingCart>().Add((ShoppingCart)shoppingCart);

                TempData["success"] = "Cart Added successfully!";

                HttpContext.Session.SetInt32(SD.SessionCart,
                            _unitOfWork.Repository<ShoppingCart>()
                            .GetAllWithSpec(new BaseSpecification<ShoppingCart>(u => u.UserId == userId)).Count());
            }
            else
            {

                _unitOfWork.ShoppingCart.IncreaseCount(cartObj, shoppingCart.count);
            }

            _unitOfWork.Complete();

            return RedirectToAction(nameof(Index));
        }
        #endregion
    }
}
