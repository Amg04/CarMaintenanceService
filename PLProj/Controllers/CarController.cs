using BLLProject.Interfaces;
using BLLProject.Specifications;
using DALProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using PLProj.Email;
using PLProj.Models;
using Stripe;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PLProj.Controllers
{
    [Authorize]
    public class CarController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _env;
        private readonly IEmailSender _emailSender;

        public CarController(UserManager<AppUser> userManager,
            IUnitOfWork unitOfWork,
            IWebHostEnvironment env,
             IEmailSender emailSender)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _env = env;
            _emailSender = emailSender;
        }

        #region Index

        public IActionResult Index()
        {
            var userId = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value;
            var spec = new BaseSpecification<Car>(c => c.UserId == userId);
            spec.Includes.Add(c => c.Color);
            spec.ComplexIncludes.Add(c => c.Include(m => m.Model)
                         .ThenInclude(b => b.Brand));

            var myCarList = _unitOfWork.Repository<Car>().GetAllWithSpec(spec)
                .Select(s => (CarViewModel)s).ToList();

            return View(myCarList);
        }

        #endregion

        #region Create

        public IActionResult Create()
        {
            PopulateDropDownLists();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CarViewModel car)
        {

            var userId = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value;

            if (ModelState.IsValid)
            {
                car.UserId = userId;
                _unitOfWork.Repository<Car>().Add((Car)car);
                var count = _unitOfWork.Complete();
                if (count > 0)
                {
                    TempData["success"] = "car has been Added Successfully";
                    return RedirectToAction(nameof(Index));
                }
            }

            PopulateDropDownLists(brandId: car.BrandId);
            return View(car);
        }

        #endregion

        #region Edit
       
        public IActionResult Edit(int? id, string viewname = "Edit")
        {
            if (!id.HasValue)
                return BadRequest();

            var spec = new BaseSpecification<Car>(c => c.Id == id);
            spec.Includes.Add(c => c.Color);
            spec.ComplexIncludes.Add(c => c.Include(m => m.Model)
                         .ThenInclude(b => b.Brand));

            var car = _unitOfWork.Repository<Car>().GetEntityWithSpec(spec);
            
            if (car is null)
                return NotFound();

            var carViewModel = (CarViewModel)car;
            carViewModel.BrandId = car.Model?.BrandId ?? 0;
            PopulateDropDownLists(carViewModel.BrandId);

            return View(viewname, carViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit([FromRoute] int? id, CarViewModel obj)
        {
            if (id != obj.Id) 
                return BadRequest();

            var userId = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value;

            try
            {
                obj.UserId = userId;
                _unitOfWork.Repository<Car>().Update((Car)obj);
                _unitOfWork.Complete();
                TempData["success"] = "car Updated Successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                if (_env.IsDevelopment())
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "An Error Has Occurred during Updating the Car");
                }

                PopulateDropDownLists(obj.BrandId);
                return View(obj);
            }

        }

        #endregion

        #region Delete

        public IActionResult Delete(int? id)
        {
            return  Edit(id, nameof(Delete));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(CarViewModel obj)
        {
            try
            {
                var CarFromDB = _unitOfWork.Repository<Car>()
                    .GetEntityWithSpec(new BaseSpecification<Car>(c => c.Id == obj.Id));

                if (CarFromDB == null)
                    return NotFound();

                _unitOfWork.Repository<Car>().Delete(CarFromDB);
                _unitOfWork.Complete();
                TempData["success"] = "car Deleted Successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {

                if (_env.IsDevelopment())
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "An Error Has Occurred during Deleted the car");
                }
                return View(obj);
            }
        }

        #endregion

        #region DeleteAll

        [HttpPost]
        public IActionResult DeleteAll()
        {
            var userId = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value;
            
            var spec = new BaseSpecification<Car>(c => c.UserId == userId);
            var myCarList = _unitOfWork.Repository<Car>().GetAllWithSpec(spec);

            foreach (var car in myCarList)
            {
                _unitOfWork.Repository<Car>().Delete(car);
            }
            _unitOfWork.Complete();

            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region GetModelsByBrandId
        [HttpGet]
        public IActionResult GetModelsByBrandId(int BrandId)
        {
            var spec = new BaseSpecification<Model>(e => e.BrandId == BrandId);
            var Models = _unitOfWork.Repository<Model>().GetAllWithSpec(spec)
                .Select(e => new { Id = e.Id, Name = e.Name }).ToList();

            return new JsonResult(Models);
        }
        #endregion

        #region method
        private void PopulateDropDownLists(int? brandId = null)
        {
            ViewBag.BrandList = _unitOfWork.Repository<Brand>().GetAll()
               .Select(u => new SelectListItem
               {
                   Text = u.Name,
                   Value = u.Id.ToString(),
               }).ToList();

            if (brandId.HasValue)
            {
                var models = _unitOfWork.Repository<Model>()
                    .GetAllWithSpec(new BaseSpecification<Model>(m => m.BrandId == brandId.Value)).ToList();
                ViewBag.ModelList = new SelectList(models, "Id", "Name");
            }
            else
            {
                ViewBag.ModelList = new SelectList(Enumerable.Empty<SelectListItem>());
            }

            ViewBag.ColorList = _unitOfWork.Repository<Color>().GetAll()
                .Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString(),
                }).ToList();
        }
        #endregion

    }
}
