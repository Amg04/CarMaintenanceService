using BLLProject.Interfaces;
using DALProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PLProj.Models;
using Utility;

namespace PLProj.Controllers
{
    [Authorize(Roles =SD.AdminRole)]
    public class ProductCategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProductCategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region Index
        public IActionResult Index()
        {
            return View();
        }
        #endregion

        #region  API Calls (Data Tables)

        [HttpGet]
        public IActionResult GetAll()
        {
            var categories = _unitOfWork.Repository<ProductCategory>().GetAll();
            return Json(new { data = categories });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            if (!id.HasValue)
                return BadRequest();

            var categoryToBeDelete = _unitOfWork.Repository<ProductCategory>().Get(id.Value);
            if (categoryToBeDelete == null)
                return Json(new { success = false, message = "Error While deleting" });

            _unitOfWork.Repository<ProductCategory>().Delete(categoryToBeDelete);
            _unitOfWork.Complete();

            return Json(new { success = true, message = "Delete Successful" });
        }

        #endregion

        #region Create

        public IActionResult Create()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ProductCategoryVM category)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Repository<ProductCategory>().Add((ProductCategory)category);
                _unitOfWork.Complete();
                TempData["success"] = "Product Category has been Added Successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        #endregion

        #region Edit
        public IActionResult Edit(int? id)
        {
            if (!id.HasValue)
                return BadRequest();

            var category = _unitOfWork.Repository<ProductCategory>().Get(id.Value);
            return View((ProductCategoryVM)category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ProductCategoryVM category)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Repository<ProductCategory>().Update((ProductCategory)category);
                _unitOfWork.Complete();
                TempData["success"] = "Product Category has been Added Successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        #endregion
    }
}
