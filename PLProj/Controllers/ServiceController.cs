using BLLProject.Interfaces;
using BLLProject.Specifications;
using DALProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PLProj.HelperClasses;
using System.Linq;
using Utility;

namespace PLProj.Controllers
{
    [Authorize(Roles = SD.AdminRole)]
    public class ServiceController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _env;

        public ServiceController(IUnitOfWork unitOfWork, IWebHostEnvironment env)
        {
            _unitOfWork = unitOfWork;
            _env = env;
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
            var spec = new BaseSpecification<Service>();
            spec.Includes.Add(s => s.Category);
            var Services = _unitOfWork.Repository<Service>()
                .GetAllWithSpec(spec).Select(s => (ServiceViewModel)s).ToList();
            return Json(new { data = Services });
        }

        #endregion

        #region CreateServic

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ServiceViewModel serv, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string? imageUrl = null;
                if (file != null)
                {
                    imageUrl = ImageHelper.SaveImage(file, _env, "service");
                }

                serv.ImgPath = imageUrl;
                _unitOfWork.Repository<Service>().Add((Service)serv);
                var count = _unitOfWork.Complete();
                if (count > 0)
                {
                    TempData["success"] = "Service has been Added Successfully";
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(serv);
        }

        #endregion

        #region Details

        public IActionResult Details(int? Id, string viewName = "Details")
        {
            if (!Id.HasValue)
                return BadRequest();

            var spec = new BaseSpecification<Service>
            (e => e.Id == Id.Value);
            spec.Includes.Add(e => e.Category);
            var service = _unitOfWork.Repository<Service>().GetEntityWithSpec(spec);

            if (service is null)
                return NotFound();

            return View(viewName, (ServiceViewModel)service);
        }

        #endregion

        #region Edit

        public IActionResult Edit(int? Id)
        {
            return Details(Id, nameof(Edit));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit([FromRoute] int? id, ServiceViewModel serv, IFormFile? file)
        {
            if (id != serv.Id)  
                return BadRequest();

            if (ModelState.IsValid)
            {
                var oldService = _unitOfWork.Repository<Service>().Get(id.Value);    

                if (file != null)
                {
                    if (!string.IsNullOrEmpty(oldService.ImgPath))
                    {
                        ImageHelper.DeleteImage(oldService.ImgPath, _env, "Service");
                    }

                    var newImageUrl = ImageHelper.SaveImage(file, _env, "Service");
                    serv.ImgPath = newImageUrl;
                }
                else
                {
                    if ( !string.IsNullOrEmpty(serv.ImgPath) && string.IsNullOrEmpty(oldService.ImgPath))
                    {
                        ImageHelper.DeleteImage(oldService.ImgPath, _env, "Service");
                        serv.ImgPath = null;
                    }
                    else
                    {
                        serv.ImgPath = oldService.ImgPath;
                    }
                }

                oldService.Name = serv.Name;
                oldService.Price = serv.Price;
                oldService.Description = serv.Description;
                oldService.ImgPath = serv.ImgPath;
                oldService.CategoryId = serv.CategoryId;
                oldService.RecommendedKilometres = serv.RecommendedKilometres;

                _unitOfWork.Repository<Service>().Update(oldService);
                _unitOfWork.Complete();

                TempData["success"] = "Service updated successfully!";
                return RedirectToAction(nameof(Index));
            }

            return View(serv);
        }

        #endregion

        #region Delete

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            if (!id.HasValue)
                return BadRequest();

            var ServiceToBeDeleted = _unitOfWork.Repository<Service>().Get(id.Value);
            if (ServiceToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error While deleting" });
            }

            ImageHelper.DeleteImage(ServiceToBeDeleted.ImgPath, _env, "Service");
            _unitOfWork.Repository<Service>().Delete(ServiceToBeDeleted);
            _unitOfWork.Complete();

            return Json(new { success = true, message = "Delete Successful" });
        }

        #endregion

    }
}
