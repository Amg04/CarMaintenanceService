using BLLProject.Interfaces;
using BLLProject.Specifications;
using DALProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PLProj.Models;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Utility;

namespace PLProj.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        # region Index

        public IActionResult Index()
        {
            return View();
        }

        #endregion

        #region  API Calls (Data Tables)
        public IActionResult GetAll(string status)
        {
            IEnumerable<OrderHeader> orderHeaders;
          
            if (User.IsInRole(SD.AdminRole))
            {
                var AdminSpec = new BaseSpecification<OrderHeader>();
                AdminSpec.Includes.Add(o => o.AppUser);

                orderHeaders = _unitOfWork.Repository<OrderHeader>()
                    .GetAllWithSpec(AdminSpec).ToList();
            }
            else if(User.IsInRole(SD.DriverRole))
            {
                var clamisIdentity = (ClaimsIdentity)User.Identity;
                var userId = clamisIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

                var driverSpec = new BaseSpecification<OrderHeader>(o => o.DriverId == userId);
                driverSpec.Includes.Add(o => o.AppUser);

                orderHeaders = _unitOfWork.Repository<OrderHeader>()
                    .GetAllWithSpec(driverSpec).ToList();
            }
            else
            {
                var clamisIdentity = (ClaimsIdentity)User.Identity;
                var userId = clamisIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

                var userSpec = new BaseSpecification<OrderHeader>(u => u.UserId == userId);
                userSpec.Includes.Add(o => o.AppUser);
                orderHeaders = _unitOfWork.Repository<OrderHeader>().GetAllWithSpec(userSpec);
            }

            switch (status)
            {
                case SD.Pending:
                    orderHeaders = orderHeaders.Where(u => u.PaymentStatus == SD.Pending);
                    break;
                case SD.Approve:
                    orderHeaders = orderHeaders.Where(u => u.OrderStatus == SD.Approve);
                    break;
                case SD.Proccessing:
                    orderHeaders = orderHeaders.Where(u => u.OrderStatus == SD.Proccessing);
                    break;
                case SD.Shipped:
                    orderHeaders = orderHeaders.Where(u => u.OrderStatus == SD.Shipped);
                    break;
                case SD.Delivered:
                    orderHeaders = orderHeaders.Where(u => u.OrderStatus == SD.Delivered);
                    break;
                case SD.Cancelled:
                    orderHeaders = orderHeaders.Where(u => u.OrderStatus == SD.Cancelled);
                    break;
                default:
                    break;
            }

            return Json(new { data = orderHeaders });
        }

        #endregion

        #region Details

        public IActionResult Details(int Id)
        {
            var specOrdeHeader = new BaseSpecification<OrderHeader>(u => u.Id == Id);
            specOrdeHeader.Includes.Add(o => o.AppUser);
           
            

            var specOrderDetial = new BaseSpecification<OrderDetail>(x => x.OrderHeaderId == Id);
            specOrderDetial.Includes.Add(o => o.Product);

            OrderVM orderVM = new OrderVM()
            {
                OrderHeader = _unitOfWork.Repository<OrderHeader>().GetEntityWithSpec(specOrdeHeader),
                OrderDetials = _unitOfWork.Repository<OrderDetail>().GetAllWithSpec(specOrderDetial)

            };

            var driverSpec = new BaseSpecification<Driver>();
            driverSpec.Includes.Add(d => d.User);
            ViewBag.driverList = _unitOfWork.Repository<Driver>()
               .GetAllWithSpec(driverSpec)
               .Select(u => new SelectListItem
               {
                   Text = u.User.Name,
                   Value = u.Id.ToString(),
               });


            var spec = new BaseSpecification<Driver>(d => d.Id == orderVM.OrderHeader.DriverId);
            spec.Includes.Add(d => d.User);
            var driver = _unitOfWork.Repository<Driver>().GetEntityWithSpec(spec);
            ViewBag.driverName = driver?.User?.Name;
            ViewBag.driverNumber = driver?.User?.PhoneNumber;

            return View(orderVM);
        }

        #endregion

        #region UpdateOrderDetail

        [Authorize(Roles = SD.AdminRole)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateOrderDetail(OrderVM orderVM)
        {
            var orderfromdb = _unitOfWork.Repository<OrderHeader>().Get(orderVM.OrderHeader.Id);
            orderfromdb.Name = orderVM.OrderHeader.Name;
            orderfromdb.PhoneNumber = orderVM.OrderHeader.PhoneNumber;
            orderfromdb.Street = orderVM.OrderHeader.Street;
            orderfromdb.City = orderVM.OrderHeader.City;
            orderfromdb.DriverId = orderVM.OrderHeader.DriverId;
            orderfromdb.TrackingNumber = orderVM.OrderHeader.TrackingNumber;
            _unitOfWork.OrderHeaderRepository.Update(orderfromdb);
            _unitOfWork.Complete();
            TempData["success"] = "Order Details has Updated Successfully";

            return RedirectToAction(nameof(Details), new { Id = orderfromdb.Id });

        }

        #endregion

        #region StartProcessing

        [Authorize(Roles = SD.AdminRole)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult StartProcessing(OrderVM orderVM)
        {
            _unitOfWork.OrderHeaderRepository.UpdateStatus(orderVM.OrderHeader.Id, SD.Proccessing, null);

            var orderHeader = _unitOfWork.Repository<OrderHeader>().Get(orderVM.OrderHeader.Id);

            var techSpec = new BaseSpecification<Ticket>(t => t.Car.UserId == orderVM.OrderHeader.UserId && t.CarId > 0);
            techSpec.Includes.Add(t => t.Appointments);
            var tickets = _unitOfWork.Repository<Ticket>()
                .GetAllWithSpec(techSpec);

            var nearestAppointment = tickets
                .SelectMany(t => t.Appointments)
                .Where(a =>
                    a.StartDateTime.HasValue &&
                    a.StartDateTime.Value > DateTime.Now &&
                    !string.IsNullOrEmpty(a.DriverId)) 
                .OrderBy(a => a.StartDateTime)
                .FirstOrDefault();

            if (nearestAppointment != null)
            {
                orderHeader.DriverId = nearestAppointment.DriverId;
                _unitOfWork.Repository<OrderHeader>().Update(orderHeader);
            }

            _unitOfWork.Complete();
            TempData["success"] = "Start Process Successfully";

            return RedirectToAction(nameof(Details), new { Id = orderVM.OrderHeader.Id });

        }

        #endregion

        #region StartShip

        [Authorize(Roles = SD.AdminRole)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult StartShip(OrderVM orderVM)
        {
            if (string.IsNullOrWhiteSpace(orderVM.OrderHeader.TrackingNumber))
            {
                ModelState.AddModelError("OrderHeader.TrackingNumber", "Please enter a tracking number before shipping.");
            }
            if (orderVM.OrderHeader.DriverId == null)
            {
                ModelState.AddModelError("OrderHeader.DriverId", "Please select a driver before shipping.");
            }
            if (!ModelState.IsValid)
            {
                var driverSpec = new BaseSpecification<Driver>();
                driverSpec.Includes.Add(d => d.User);
                ViewBag.driverList = _unitOfWork.Repository<Driver>()
                   .GetAllWithSpec(driverSpec)
                   .Select(u => new SelectListItem
                   {
                       Text = u.User.Name,
                       Value = u.Id.ToString(),
                   });

                return View(nameof(Details), orderVM);
            }

            var orderfromdb = _unitOfWork.Repository<OrderHeader>()
              .GetEntityWithSpec(new BaseSpecification<OrderHeader>(u => u.Id == orderVM.OrderHeader.Id));
           
            
            
            orderfromdb.TrackingNumber = orderVM.OrderHeader.TrackingNumber;
            orderfromdb.DriverId = orderVM.OrderHeader.DriverId;
            orderfromdb.ShippingDate = DateTime.Now;
            orderfromdb.OrderStatus = SD.Shipped;
            _unitOfWork.Repository<OrderHeader>().Update(orderfromdb);
            _unitOfWork.Complete();
            TempData["success"] = "Order Shipped Successfully";
            return RedirectToAction(nameof(Details), new { Id = orderVM.OrderHeader.Id });

        }

        #endregion

        #region CancelOrder

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CancelOrder(OrderVM orderVM)
        {
            var orderfromdb = _unitOfWork.Repository<OrderHeader>().Get(orderVM.OrderHeader.Id);
            
            if (orderfromdb.PaymentStatus == SD.Approve)
            {
                var option = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderfromdb.PaymentIntentId
                };
                var service = new RefundService();
                Refund refund = service.Create(option);
                _unitOfWork.OrderHeaderRepository.UpdateStatus(orderfromdb.Id, SD.Cancelled, SD.Refund);
            }
            else
            {
                _unitOfWork.OrderHeaderRepository.UpdateStatus(orderfromdb.Id, SD.Cancelled, SD.Cancelled);

            }
            _unitOfWork.Complete();
            TempData["success"] = "Order Cancelled Successfully";

            return RedirectToAction(nameof(Details), new { Id = orderVM.OrderHeader.Id });

        }

        #endregion

        #region OrderDelivery

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = SD.DriverRole)]
        public IActionResult OrderDelivery(OrderVM orderVM)
        {
           
                var order = _unitOfWork.Repository<OrderHeader>().Get(orderVM.OrderHeader.Id);
                if (order != null)
                {
                    order.OrderStatus = SD.Delivered;

                    _unitOfWork.Repository<OrderHeader>().Update(order);
                    _unitOfWork.Complete();

                    TempData["success"] = "Order has been Delivered successfully";
                    return RedirectToAction(nameof(Details), new { Id = order.Id });
                }
            return View(nameof(Details), orderVM);
        }

        #endregion

    }
}
