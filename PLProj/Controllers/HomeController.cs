using BLLProject.Interfaces;
using BLLProject.Specifications;
using DALProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PLProj.Models;
using System.Linq;
using System.Security.Claims;
using Utility;

namespace PLProj.Controllers
{
	public class HomeController : Controller
	{
        private readonly IUnitOfWork _unitOfWork;
        public HomeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region Index
        public IActionResult Index()
		{
			var Techspec = new BaseSpecification<Technician>();
			Techspec.Includes.Add(t => t.User);
			Techspec.Includes.Add(t => t.Category);

            var Servspec = new BaseSpecification<Service>();
            Servspec.Includes.Add(t => t.Category);

            var HomeVM = new HomeViewModel()
			{
				Services = _unitOfWork.Repository<Service>()
                    .GetAllWithSpec(Servspec).Select(s => (ServiceViewModel)s).ToList(),
				Technicians = _unitOfWork.Repository<Technician>()
                    .GetAllWithSpec(Techspec).Select(s => (TechnicianViewModel)s).ToList()
			};

            //to set session
            var identity = (ClaimsIdentity)User.Identity;
            var claim = identity?.FindFirst(ClaimTypes.NameIdentifier);
            if (claim?.Value != null) // user login in
            {
                HttpContext.Session.SetInt32(SD.SessionCart,
                 _unitOfWork.Repository<ShoppingCart>().
                 GetAllWithSpec(new BaseSpecification<ShoppingCart>(u => u.UserId == claim.Value)).Count());
            }

            return View(HomeVM);
		}
        #endregion

        #region TechniciansPartial
        public IActionResult TechniciansPartial()
        {
            var spec = new BaseSpecification<Technician>();
            spec.Includes.Add(t => t.User);
            spec.Includes.Add(t => t.Category);
            var technicians = _unitOfWork.Repository<Technician>()
                .GetAllWithSpec(spec).Select(s => (TechnicianViewModel)s).ToList();
            return PartialView("_TechnicianListPartial", technicians);
        }
        #endregion

        #region AccessDeniedPath 

        public IActionResult AccessDenied()
        {
            return View();
        }


        #endregion
    }
}
