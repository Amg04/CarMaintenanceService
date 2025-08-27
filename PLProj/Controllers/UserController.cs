using BLLProject.Interfaces;
using BLLProject.Specifications;
using DALProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using PLProj.HelperClasses;
using PLProj.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utility;

namespace PLProj.Controllers
{
    [Authorize(Roles = SD.AdminRole)]
    public class UserController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _env;

        public UserController(UserManager<AppUser> userManager, IUnitOfWork unitOfWork, IWebHostEnvironment env)
        {
            _userManager = userManager;
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
        public IActionResult GetAll(string role)
        {
            List<UserViewModel> users;

            if (role == "all")
            {
                users = _unitOfWork.Repository<AppUser>().GetAll()
                         .Select(t => (UserViewModel)t).ToList();
            }
            else
            {
                users = _unitOfWork.Repository<AppUser>()
                    .GetAllWithSpec(new BaseSpecification<AppUser>(a => a.Role == role))
                         .Select(t => (UserViewModel)t).ToList();
            }

            return Json(new { data = users });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(string id)
        {
            var userToBeDeleted = _unitOfWork.Repository<AppUser>()
                .GetEntityWithSpec(new BaseSpecification<AppUser>(u => u.Id == id));

            if (userToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error While deleting" });
            }

            if(userToBeDeleted.Role == SD.TechnicianRole)
            {
                var technicianToBeDeleted = _unitOfWork.Repository<Technician>()
                    .GetEntityWithSpec(new BaseSpecification<Technician>(u => u.Id == id));

                ImageHelper.DeleteImage(technicianToBeDeleted.ImgPath, _env, "technician");
                _unitOfWork.Repository<Technician>().Delete(technicianToBeDeleted);
            }
            else if(userToBeDeleted.Role == SD.DriverRole)
            {
                var DriverToBeDeleted = _unitOfWork.Repository<Driver>()
                   .GetEntityWithSpec(new BaseSpecification<Driver>(u => u.Id == id));
                _unitOfWork.Repository<Driver>().Delete(DriverToBeDeleted);
            }
            _unitOfWork.Complete();

            var userDataFromUserTable = await _userManager.FindByIdAsync(userToBeDeleted.Id);

            if (userDataFromUserTable is not null)
            {
                var result = await _userManager.DeleteAsync(userDataFromUserTable);
                if (!result.Succeeded)
                {
                    return Json(new { success = false, message = "User deletion failed." });
                }
            }
            return Json(new { success = true, message = "Delete Successful" });
        }


        #endregion

        #region Customer

        #region Details
        public IActionResult CustomerDetails(string Id, string viewname = "CustomerDetails")
        {
            if (Id == null)
                return BadRequest();

            var spec = new BaseSpecification<AppUser>(e => e.Id == Id);
            spec.Includes.Add(e => e.Cars);
            var customer = _unitOfWork.Repository<AppUser>().GetEntityWithSpec(spec);
            if (customer is null)
                return NotFound();

            var customerVM = (CustomerViewModel)customer;

            customerVM.NumberOfCars = _unitOfWork.Repository<Car>()
                .GetAllWithSpec(new BaseSpecification<Car>(c => c.UserId == customer.Id)).Count();

            return View(viewname, customerVM);
        }

        #endregion

        #region Edit
        public IActionResult CustomerEdit(string Id)
        {
            return CustomerDetails(Id, nameof(CustomerEdit));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CustomerEdit([FromRoute] string Id, CustomerViewModel Tech)
        {
            if (Id != Tech.Id)
                return BadRequest();

            var CustomerFromDb = _unitOfWork.Repository<AppUser>()
                .GetEntityWithSpec(new BaseSpecification<AppUser>(t => t.Id == Id));
            var user = await _userManager.FindByIdAsync(CustomerFromDb.Id);

            if (!ModelState.IsValid)
                return View(Tech);

            try
            {
                if (user is not null)
                {
                    user.Name = Tech.Name;
                    user.Email = Tech.Email;
                    user.City = Tech.City;
                    user.Street = Tech.Street;
                    user.PhoneNumber = Tech.PhoneNumber;

                    var result = await _userManager.UpdateAsync(user);

                    if (result.Succeeded)
                    {
                        TempData["success"] = "Customer Has been Updated Successfully";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                            ModelState.AddModelError(string.Empty, error.Description);

                        return View(Tech);
                    }
                }
                else
                    return View(Tech);
            }
            catch (Exception ex)
            {
                if (_env.IsDevelopment())
                    ModelState.AddModelError(string.Empty, ex.Message);
                else
                    ModelState.AddModelError(string.Empty, "An Error Has Occurred during Updating the Department");

                return View(Tech);
            }
        }

        #endregion

        #endregion

        #region Technician

        #region Details
        public IActionResult TechnicianDetails(string Id, string viewname = "TechnicianDetails")
        {
            if (Id == null)
                return BadRequest();

            var spec = new BaseSpecification<Technician>(e => e.Id == Id);
            spec.Includes.Add(e => e.Category);
            spec.Includes.Add(e => e.User);
            var Tech = _unitOfWork.Repository<Technician>().GetEntityWithSpec(spec);
            if (Tech is null)
                return NotFound();
            return View(viewname, (TechnicianViewModel)Tech);
        }
        #endregion

        #region Edit
        public IActionResult TechnicianEdit(string Id)
        {
            ViewBag.CategoryList = _unitOfWork.Repository<Category>().GetAll()
                .Select(u => new SelectListItem { Text = u.Name, Value = u.Id.ToString() });

            return TechnicianDetails(Id, nameof(TechnicianEdit));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TechnicianEdit([FromRoute] string Id, TechnicianViewModel Tech, IFormFile? file)
        {
            if (Id != Tech.Id)
                return BadRequest();

            var TechFromDb = _unitOfWork.Repository<Technician>()
                .GetEntityWithSpec(new BaseSpecification<Technician>(t => t.Id == Id));
            var user = await _userManager.FindByIdAsync(TechFromDb.Id);

            if (!ModelState.IsValid)
            {
                ViewBag.CategoryList = _unitOfWork.Repository<Category>().GetAll()
                    .Select(u => new SelectListItem { Text = u.Name, Value = u.Id.ToString() });
                return View(Tech);
            }

            try
            {
                if (file != null)
                {
                    if (!string.IsNullOrEmpty(TechFromDb.ImgPath))
                    {
                        ImageHelper.DeleteImage(TechFromDb.ImgPath, _env, "technician");
                    }

                    var newImageUrl = ImageHelper.SaveImage(file, _env, "technician");
                    Tech.Img = newImageUrl;
                }
                else
                {
                    if (!string.IsNullOrEmpty(Tech.Img) && string.IsNullOrEmpty(TechFromDb.ImgPath))
                    {
                        ImageHelper.DeleteImage(TechFromDb.ImgPath, _env, "technician");
                        Tech.Img = null;
                    }
                    else
                    {
                        Tech.Img = TechFromDb.ImgPath;
                    }
                }

                TechFromDb.CategoryId = Tech.CategoryId;
                TechFromDb.Availability = Tech.Availability;
                TechFromDb.BirthDate = Tech.BirthDate;
                TechFromDb.ImgPath = Tech.Img;
                _unitOfWork.Repository<Technician>().Update(TechFromDb);
                _unitOfWork.Complete();

                if (user is not null)
                {
                    user.Name = Tech.Name;
                    user.Email = Tech.Email;
                    user.City = Tech.City;
                    user.Street = Tech.Street;
                    user.PhoneNumber = Tech.PhoneNumber;

                    var result = await _userManager.UpdateAsync(user);

                    if (result.Succeeded)
                    {
                        TempData["success"] = "Technician Has been Updated Successfully";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                            ModelState.AddModelError(string.Empty, error.Description);

                        ViewBag.CategoryList = _unitOfWork.Repository<Category>().GetAll()
                            .Select(u => new SelectListItem { Text = u.Name, Value = u.Id.ToString() });
                        return View(Tech);
                    }
                }
                else
                {

                    ViewBag.CategoryList = _unitOfWork.Repository<Category>().GetAll()
                        .Select(u => new SelectListItem { Text = u.Name, Value = u.Id.ToString() });
                    return View(Tech);
                }
            }
            catch (Exception ex)
            {
                if (_env.IsDevelopment())
                    ModelState.AddModelError(string.Empty, ex.Message);
                else
                    ModelState.AddModelError(string.Empty, "An Error Has Occurred during Updating the Department");

                ViewBag.CategoryList = _unitOfWork.Repository<Category>().GetAll()
                    .Select(u => new SelectListItem { Text = u.Name, Value = u.Id.ToString() });
                return View(Tech);
            }
        }
        #endregion

        #endregion

        #region Driver

        #region Details

        public IActionResult DriverDetails(string Id, string viewname = "DriverDetails")
        {
            if (Id == null)
                return BadRequest();

            var spec = new BaseSpecification<Driver>(e => e.Id == Id);
            spec.Includes.Add(e => e.User);
            var driver = _unitOfWork.Repository<Driver>().GetEntityWithSpec(spec);
            if (driver is null)
                return NotFound();

            return View(viewname, (DriverViewModel)driver);
        }
        #endregion

        #region Edit
        public IActionResult DriverEdit(string Id)
        {
            return DriverDetails(Id, nameof(DriverEdit));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DriverEdit([FromRoute] string Id, DriverViewModel Driver)
        {
            if (Id != Driver.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(Driver);

            var driverFromDb = _unitOfWork.Repository<Driver>()
                .GetEntityWithSpec(new BaseSpecification<Driver>(t => t.Id == Id));
            var user = await _userManager.FindByIdAsync(driverFromDb.Id);

            try
            {
                driverFromDb.Availability = Driver.Availability;
                driverFromDb.BirthDate = Driver.BirthDate;
                driverFromDb.License = Driver.License;
                driverFromDb.LicenseDate = Driver.LicenseDate;
                driverFromDb.LicenseExpDate = Driver.LicenseExpDate;
                _unitOfWork.Complete();

                if (user is not null)
                {
                    user.Name = Driver.Name;
                    user.Email = Driver.Email;
                    user.City = Driver.City;
                    user.Street = Driver.Street;
                    user.PhoneNumber = Driver.PhoneNumber;

                    var result = await _userManager.UpdateAsync(user);

                    if (result.Succeeded)
                    {
                        TempData["success"] = "Driver Has been Updated Successfully";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                            ModelState.AddModelError(string.Empty, error.Description);

                        return View(Driver);
                    }
                }
                else
                    return View(Driver);
            }
            catch (Exception ex)
            {
                if (_env.IsDevelopment())
                    ModelState.AddModelError(string.Empty, ex.Message);
                else
                    ModelState.AddModelError(string.Empty, "An Error Has Occurred during Updating the Driver");

                return View(Driver);
            }
        }

        #endregion     

        #endregion

        #region LockUnlock
        [HttpPost]
        public IActionResult LockUnlock(string? id)
        {
            var user = _unitOfWork.Repository<AppUser>()
                .GetEntityWithSpec(new BaseSpecification<AppUser>(a => a.Id == id));
            if (user == null)
            {
                return NotFound();
            }
            if (user.LockoutEnd == null || user.LockoutEnd < DateTime.Now)
            {
                user.LockoutEnd = DateTime.Now.AddYears(1); // locked 1 year
            }
            else
            {
                //user.LockoutEnd = DateTime.Now; // unlock
                // or
                user.LockoutEnd = null; // unlock

            }
            _unitOfWork.Complete();
            return Json(new { success = true, message = "User lock/unlock updated successfully." });
        }
        #endregion
    }
}
