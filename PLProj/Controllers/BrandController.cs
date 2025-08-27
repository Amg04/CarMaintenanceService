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
    public class BrandController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public BrandController(IUnitOfWork unitOfWork)
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
            var brands = _unitOfWork.Repository<Brand>().GetAll().Select(s => (BrandViewModel)s).ToList();
            return Json(new { data = brands });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var BrandToBeDeleted = _unitOfWork.Repository<Brand>()
                .GetEntityWithSpec(new BaseSpecification<Brand>(u => u.Id == id));
            if (BrandToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error While deleting" });
            }

            _unitOfWork.Repository<Brand>().Delete(BrandToBeDeleted);
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
        public IActionResult Create(BrandViewModel model)
        {

            if (ModelState.IsValid)
            {
                _unitOfWork.Repository<Brand>().Add((Brand)model);
                var count = _unitOfWork.Complete();
                if (count > 0)
                {
                    TempData["success"] = "Brand has been Added Successfully";
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(model);
        }

        #endregion

        #region Edit

        public IActionResult Edit(int? id)
        {
            if (!id.HasValue)
                return BadRequest();

            var brand = _unitOfWork.Repository<Brand>()
                .GetEntityWithSpec(new BaseSpecification<Brand>(e => e.Id == id.Value));

            if (brand is null)
                return NotFound();

            return View((BrandViewModel)brand);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit([FromRoute] int? id, BrandViewModel obj)
        {
            if (id != obj.Id)  //in case the user (hacker) edit the Id
                return BadRequest();

            if (ModelState.IsValid)
            {
                _unitOfWork.Repository<Brand>().Update((Brand)obj);
                int count = _unitOfWork.Complete();
                if (count > 0)
                {
                    TempData["success"] = "Brand Updated Successfully";
                    return RedirectToAction(nameof(Index));
                }
            }

            return View(obj);
        }

        #endregion

    }
}
