using BLLProject.Interfaces;
using BLLProject.Specifications;
using DALProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Utility;

namespace PLProj.Controllers
{
    [Authorize(Roles = SD.AdminRole)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
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
            var categories = _unitOfWork.Repository<Category>().GetAll().Select(s => (CategoryViewModel)s).ToList();
            return Json(new { data = categories });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            if (!id.HasValue)
                return BadRequest();

            var CategoryToBeDeleted = _unitOfWork.Repository<Category>().Get(id.Value);
            if (CategoryToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error While deleting" });
            }

            _unitOfWork.Repository<Category>().Delete(CategoryToBeDeleted);
            _unitOfWork.Complete();

            return Json(new { success = true, message = "Delete Successful" });
        }

        #endregion

        #region Add
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CategoryViewModel category)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Repository<Category>().Add((Category)category);
                var count = _unitOfWork.Complete();
                if (count > 0)
                {
                    TempData["success"] = "Category has been Added Successfully";
                    return RedirectToAction(nameof(Index));
                }
            }

            return View(category);
        }

        #endregion

        #region Edit

        public IActionResult Edit(int? id)
        {
            if (!id.HasValue)
                return BadRequest();

            var category = _unitOfWork.Repository<Category>().Get(id.Value);

            if (category is null)
                return NotFound();

            return View((CategoryViewModel)category);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit([FromRoute] int? id, CategoryViewModel obj)
        {
            if (id != obj.Id)  //in case the user (hacker) edit the Id
                return BadRequest();

            if (ModelState.IsValid)
            {
                _unitOfWork.Repository<Category>().Update((Category)obj);
                int count = _unitOfWork.Complete();
                if (count > 0)
                {
                    TempData["success"] = "Category Updated Successfully";
                    return RedirectToAction(nameof(Index));
                }
            }

            return View(obj);
        }

        #endregion

    }
}  


