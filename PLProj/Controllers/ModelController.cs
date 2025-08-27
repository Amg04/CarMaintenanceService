using BLLProject.Interfaces;
using BLLProject.Specifications;
using DALProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using Utility;

namespace PLProj.Controllers
{
    [Authorize(Roles = SD.AdminRole)]
    public class ModelController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public ModelController(
            UserManager<AppUser> userManager,
            IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        #region Index
        public IActionResult Index()
        {
            return View();
        }
        #endregion

        #region API Calls (Data Tables)

        [HttpGet]
        public IActionResult GetAll()
        {
            var spec = new BaseSpecification<Model>();
            spec.Includes.Add(s => s.Brand);
            var Models = _unitOfWork.Repository<Model>()
                .GetAllWithSpec(spec).Select(s => (ModelViewModel)s).ToList();
            return Json(new { data = Models });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            if (!id.HasValue)
                return BadRequest();

            var ModelToBeDeleted = _unitOfWork.Repository<Model>().Get(id.Value);
            if (ModelToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error While deleting" });
            }

            _unitOfWork.Repository<Model>().Delete(ModelToBeDeleted);
            _unitOfWork.Complete();

            return Json(new { success = true, message = "Delete Successful" });
        }

        #endregion

        #region Add

        public IActionResult Create()
        {
            IEnumerable<SelectListItem> brandList = _unitOfWork.Repository<Brand>()
                .GetAll()
                // Projections in EF core
                .Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString(),
                });

            ViewBag.brandList = brandList;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ModelViewModel model)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Repository<Model>().Add((Model)model);
                var count = _unitOfWork.Complete();
                if (count > 0)
                {
                    TempData["success"] = "Model has been Added Successfully";
                    return RedirectToAction(nameof(Index));
                }
            }

            IEnumerable<SelectListItem> brandList = _unitOfWork.Repository<Brand>()
            .GetAll()
            // Projections in EF core
            .Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString(),
            });

            ViewBag.brandList = brandList;

            return View(model);
        }

        #endregion

        #region Edit

        public IActionResult Edit(int? id)
        {
            IEnumerable<SelectListItem> brandList = _unitOfWork.Repository<Brand>()
             .GetAll()
             // Projections in EF core
             .Select(u => new SelectListItem
             {
                 Text = u.Name,
                 Value = u.Id.ToString(),
             });

            ViewBag.brandList = brandList;


            if (!id.HasValue)
                return BadRequest();

            var spec = new BaseSpecification<Model>
            (e => e.Id == id.Value);
            spec.Includes.Add(e => e.Brand);
            var model = _unitOfWork.Repository<Model>().GetEntityWithSpec(spec);

            if (model is null)
                return NotFound();
            return View((ModelViewModel)model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit([FromRoute] int? id, ModelViewModel obj)
        {
            if (id != obj.Id)  //in case the user (hacker) edit the Id
                return BadRequest();

            if (ModelState.IsValid)
            {
                _unitOfWork.Repository<Model>().Update((Model)obj);
                int count = _unitOfWork.Complete();
                if (count > 0)
                {
                    TempData["success"] = "Model Updated Successfully";
                    return RedirectToAction(nameof(Index));
                }
            }
            

            ViewBag.brandList = _unitOfWork.Repository<Brand>().GetAll()
                .Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString(),
                });
            return View(obj);
        }

        #endregion

        }
    }
