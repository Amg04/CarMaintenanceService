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
    public class ColorController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ColorController(IUnitOfWork unitOfWork)
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
            var Colors = _unitOfWork.Repository <Color>().GetAll().Select(s => (ColorViewModel)s).ToList();
            return Json(new { data = Colors });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            if (!id.HasValue)
                return BadRequest();

            var ColorToBeDeleted = _unitOfWork.Repository<Color>().Get(id.Value);
            if (ColorToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error While deleting" });
            }

            _unitOfWork.Repository<Color>().Delete(ColorToBeDeleted);
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
        public IActionResult Create(ColorViewModel model)
        {

            if (ModelState.IsValid)
            {
                _unitOfWork.Repository<Color>().Add((Color)model);
                var count = _unitOfWork.Complete();
                if (count > 0)
                {
                    TempData["Message"] = "Color has been Added Successfully";
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

            var color = _unitOfWork.Repository<Color>().Get( id.Value);

            if (color is null)
                return NotFound();

            return View((ColorViewModel)color);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit([FromRoute] int? id, ColorViewModel obj)
        {
            if (id != obj.Id) 
                return BadRequest();

            if (ModelState.IsValid)
            {
                _unitOfWork.Repository<Color>().Update((Color)obj);
                _unitOfWork.Complete();
                TempData["success"] = "Color Updated Successfully";
                return RedirectToAction(nameof(Index));
            }

            return View(obj);
        }

        #endregion

    }
}
