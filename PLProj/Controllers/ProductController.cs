using BLLProject.Interfaces;
using BLLProject.Specifications;
using DALProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PLProj.HelperClasses;
using PLProj.Models;
using System.Linq;
using Utility;

namespace PLProj.Controllers
{
    [Authorize(Roles = SD.AdminRole)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private IWebHostEnvironment _webHost;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHost)
        {
            _unitOfWork = unitOfWork;
            _webHost = webHost;
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
            var spec = new BaseSpecification<Product>();
            spec.Includes.Add(s => s.ProductCategory);
            var products = _unitOfWork.Repository<Product>().GetAllWithSpec(spec);
            return Json(new { data = products });
        }
        #endregion

        #region Create
        public IActionResult Create()
        {
            ViewBag.CategoryList = _unitOfWork.Repository<ProductCategory>()
                .GetAll()
                .Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString(),
                });

            ViewBag.ModelList = _unitOfWork.Repository<Model>()
             .GetAll()
             .Select(u => new SelectListItem
             {
                 Text = u.Name,
                 Value = u.Id.ToString(),
             });

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ProductViewModel ProductVM, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string? imageUrl = null;
                if (file != null)
                {
                    imageUrl = ImageHelper.SaveImage(file, _webHost, "product");
                }
                ProductVM.ImgPath = imageUrl;
                var product = (Product)ProductVM;
                _unitOfWork.Repository<Product>().Add(product);
                _unitOfWork.Complete();

                var selectedModelIds = ProductVM.SelectedModelIds;
                // لو المستخدم مخترش حاجة، نجيب كل الموديلات
                if (selectedModelIds == null || !selectedModelIds.Any())
                {
                    selectedModelIds = _unitOfWork.Repository<Model>()
                        .GetAll()
                        .Select(m => m.Id)
                        .ToList();
                }

               

                foreach (var modelId in selectedModelIds)
                {
                    _unitOfWork.Repository<ModelPart>().Add(new ModelPart
                    {
                        ModelId = modelId,
                        ProductId = product.Id
                    });
                }
                _unitOfWork.Complete();

                TempData["success"] = "Product has been Added Successfully";
                return RedirectToAction("Index");
            }

            ViewBag.CategoryList = _unitOfWork.Repository<ProductCategory>()
                .GetAll()
                .Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString(),
                });


            ViewBag.ModelList = _unitOfWork.Repository<Model>()
             .GetAll()
             .Select(u => new SelectListItem
             {
                 Text = u.Name,
                 Value = u.Id.ToString(),
             });

            return View(ProductVM);
        }
        #endregion

        #region Details

        public IActionResult Details(int? Id, string viewName = "Details")
        {
            if (!Id.HasValue)
                return BadRequest();

            var spec = new BaseSpecification<Product>(c => c.Id == Id);
            spec.Includes.Add(p => p.ProductCategory);
            spec.ComplexIncludes.Add(p => p.Include(ps => ps.ModelParts)
                     .ThenInclude(mp => mp.Model));

            var partService = _unitOfWork.Repository<Product>()
                .GetEntityWithSpec(spec);

            if (partService is null)
                return NotFound();

            ViewBag.CategoryList = _unitOfWork.Repository<ProductCategory>()
          .GetAll()
          .Select(u => new SelectListItem
          {
              Text = u.Name,
              Value = u.Id.ToString(),
          });
            ViewBag.ModelList = _unitOfWork.Repository<Model>()
   .GetAll()
   .Select(u => new SelectListItem
   {
       Text = u.Name,
       Value = u.Id.ToString(),
   });

            return View(viewName, (ProductViewModel)partService);
        }

        #endregion

        #region Edit
        public IActionResult Edit(int? Id)
        {
            return Details(Id, nameof(Edit));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ProductViewModel ProductVM, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                var oldPart = _unitOfWork.Repository<Product>().Get(ProductVM.Id);

                if (file != null)
                {
                    if (!string.IsNullOrEmpty(oldPart.ImgPath))
                    {
                        ImageHelper.DeleteImage(oldPart.ImgPath, _webHost, "product");
                    }

                    var newImageUrl = ImageHelper.SaveImage(file, _webHost, "product");
                    ProductVM.ImgPath = newImageUrl;
                }
                else
                {
                    if (!string.IsNullOrEmpty(ProductVM.ImgPath) && string.IsNullOrEmpty(oldPart.ImgPath))
                    {
                        ImageHelper.DeleteImage(oldPart.ImgPath, _webHost, "product");
                        ProductVM.ImgPath = null;
                    }
                    else
                    {
                        ProductVM.ImgPath = oldPart.ImgPath;
                    }
                }

                oldPart.Name = ProductVM.Name;
                oldPart.Description = ProductVM.Description;
                oldPart.ImgPath = ProductVM.ImgPath;
                oldPart.Price = ProductVM.Price;
                oldPart.ProdCatIegoryd = ProductVM.ProdCategoryId;

                _unitOfWork.Complete();

                var product = (Product)ProductVM;
                var selectedModelIds = ProductVM.SelectedModelIds;
                
                // حذف العلاقات القديمة
                var existingRelations = _unitOfWork.Repository<ModelPart>()
                    .GetAll().Where(mp => mp.ProductId == product.Id);
                _unitOfWork.Repository<ModelPart>().RemoveRange(existingRelations);

                foreach (var modelId in selectedModelIds)
                {
                    _unitOfWork.Repository<ModelPart>().Add(new ModelPart
                    {
                        ModelId = modelId,
                        ProductId = product.Id
                    });
                }
                _unitOfWork.Complete();

                TempData["success"] = "Product has been Updated Successfully";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.CategoryList = _unitOfWork.Repository<ProductCategory>()
            .GetAll()
            .Select(u => new SelectListItem
            {
            Text = u.Name,
            Value = u.Id.ToString(),
            });
            ViewBag.ModelList = _unitOfWork.Repository<Model>()
            .GetAll()
            .Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString(),
            });
            return View(ProductVM);
        }

        #endregion

        #region Delete
        public IActionResult Delete(int? id)
        {
            if (!id.HasValue)
                return BadRequest();

            var partToBeDelete = _unitOfWork.Repository<Product>().Get(id.Value);
            if (partToBeDelete == null)
            {
                return Json(new { success = false, message = "Error While deleting" });
            }

            var relations = _unitOfWork.Repository<ModelPart>()
             .GetAll().Where(mp => mp.ProductId == partToBeDelete.Id);
            _unitOfWork.Repository<ModelPart>().RemoveRange(relations);

            ImageHelper.DeleteImage(partToBeDelete.ImgPath, _webHost, "product");
            _unitOfWork.Repository<Product>().Delete(partToBeDelete);
            _unitOfWork.Complete();

            return Json(new { success = true, message = "Delete Successful" });
        }
        #endregion
    }
}
