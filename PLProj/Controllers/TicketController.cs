using BLLProject.Interfaces;
using BLLProject.Specifications;
using DALProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PLProj.Email;
using PLProj.Models;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Utility;

namespace PLProj.Controllers
{
    [Authorize]
    public class TicketController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _configuration;
        private readonly IEmailSender _emailSender;

        public TicketController(UserManager<AppUser> userManager,
            IUnitOfWork unitOfWork,
            IWebHostEnvironment env,
             IConfiguration configuration,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _env = env;
           _configuration = configuration;
           _emailSender = emailSender;
        }

        #region AllTicket
        public IActionResult AllTicket()
        {
            return View();
        }
        #endregion

        #region API Calls (Data Tables)
        [HttpGet]
        public IActionResult GetAll(string status)
        {
            var claim = ((ClaimsIdentity)User.Identity)?.FindFirst(ClaimTypes.NameIdentifier);

            var spec = new BaseSpecification<Ticket>(t => t.PaymentStatus != null);
            spec.Includes.Add(t => t.Service);
            spec.Includes.Add(t => t.Car);

            if (User.IsInRole(SD.CustomerRole))
            {
                spec.Criteria = c => c.Car.UserId == claim.Value && c.PaymentStatus != null; 
            }
            else if(User.IsInRole(SD.TechnicianRole))
            {
                var techClaim = claim.Value;

                spec.Includes.Add(t => t.Appointments);

                spec.Criteria = t =>
                    t.Appointments.Any(a => a.TechnicianId == techClaim);
            }
            else if (User.IsInRole(SD.DriverRole))
            {
                var driverClaim = claim.Value;

                spec.Includes.Add(t => t.Appointments);

                spec.Criteria = t =>
                    t.Appointments.Any(a => a.DriverId != null && a.DriverId == driverClaim);
            }

            var objTicket = _unitOfWork.Repository<Ticket>().GetAllWithSpec(spec);

            switch (status)
            {
                case "New":
                    objTicket = objTicket.Where(e => e.stateType == StateType.New);
                    break;
                case "Assigned":
                    objTicket = objTicket.Where(e => e.stateType == StateType.Assigned);
                    break;
                case "Finished":
                    objTicket = objTicket.Where(e => e.stateType == StateType.Finished);
                    break;
                default:
                    break;
            }

            return Json(new
            {
                data = objTicket.Select(t => new
                {
                    t.Id,
                    Service = new { t.Service.Name },
                    Car = new { t.Car.PlateNumber },
                    StateType = t.stateType.ToString()
                })
            });

        }
        #endregion

        #region Details

        public IActionResult Details(int id)
        {
            var spec = new BaseSpecification<Ticket>(t => t.Id == id);
            spec.Includes.Add(c => c.Service);

            spec.ComplexIncludes.Add(c => c.Include(t => t.Appointments)
                    .ThenInclude(a => a.Driver)
                    .ThenInclude(d => d.User));

            spec.ComplexIncludes.Add(c => c.Include(t => t.Appointments)
                .ThenInclude(a => a.Technician)
                .ThenInclude(t => t.User));

            spec.ComplexIncludes.Add(c => c.Include(m => m.Car)
                         .ThenInclude(b => b.Model)
                         .ThenInclude(b => b.Brand));

            spec.ComplexIncludes.Add(c => c.Include(m => m.Car)
                       .ThenInclude(b => b.AppUser));

            var ticketList = _unitOfWork.Repository<Ticket>().GetEntityWithSpec(spec);
            return View(ticketList);
        }

        #endregion

        #region Admin

        #region AddAppointment

        [Authorize(Roles = SD.AdminRole)]
        public IActionResult AddAppointment(int? id)
        {
            if (!id.HasValue)
                return BadRequest();

            var Techspec = new BaseSpecification<Technician>();
            Techspec.Includes.Add(T => T.User);
            ViewBag.TechList = _unitOfWork.Repository<Technician>().GetAllWithSpec(Techspec)
           .Select(u => new SelectListItem
           {
               Text = u.User.Name,
               Value = u.Id.ToString(),
           });

            var Driverspec = new BaseSpecification<Driver>();
            Driverspec.Includes.Add(T => T.User);
            ViewBag.DriverList = _unitOfWork.Repository<Driver>().GetAllWithSpec(Driverspec)
           .Select(u => new SelectListItem
           {
               Text = u.User.Name,
               Value = u.Id.ToString(),
           });

            ViewBag.Ticketid = id.Value;

            return View();
        }

        [Authorize(Roles = SD.AdminRole)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddAppointment([FromRoute] int? id, AddAppointmentViewModel model)
        {
            if (!id.HasValue)
                return BadRequest();

            model.TicketId = id.Value;
            if (ModelState.IsValid)
            {
                var spec = new BaseSpecification<Ticket>(e => e.Id == id);
                spec.Includes.Add(t => t.Appointments);
                var ticket = _unitOfWork.Repository<Ticket>().GetEntityWithSpec(spec);

                bool isFirstAppointment = ticket is not null && ticket.Appointments?.Any() == false;

                _unitOfWork.Repository<Appointment>().Add((Appointment)model);

                if (isFirstAppointment)
                {
                    ticket.StartDateTime = model.StartDateTime;
                    ticket.stateType = StateType.Assigned;
                    _unitOfWork.Repository<Ticket>().Update(ticket);
                }
                _unitOfWork.Complete();

                TempData["success"] = "Appointment has been Added Successfully";
                return RedirectToAction(nameof(AllTicket));

            }

            var Techspec = new BaseSpecification<Technician>();
            Techspec.Includes.Add(T => T.User);
            ViewBag.TechList = _unitOfWork.Repository<Technician>().GetAllWithSpec(Techspec)
           .Select(u => new SelectListItem
           {
               Text = u.User.Name,
               Value = u.Id.ToString(),
           });

            var Driverspec = new BaseSpecification<Driver>();
            Driverspec.Includes.Add(T => T.User);
            ViewBag.DriverList = _unitOfWork.Repository<Driver>().GetAllWithSpec(Driverspec)
           .Select(u => new SelectListItem
           {
               Text = u.User.Name,
               Value = u.Id.ToString(),
           });

            ViewBag.Ticketid = id.Value;

            return View(model);

        }

        #endregion

        #region EditTicket
        
        [Authorize(Roles = SD.AdminRole)]
        public IActionResult EditTicket(int? id)
        {
            if (!id.HasValue)
                return BadRequest();

            var ticket = _unitOfWork.Repository<Ticket>().Get(id.Value);

            if(ticket == null)
                return NotFound();

            return View((EditTickerViewModel)ticket);
        }

        [Authorize(Roles = SD.AdminRole)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditTicket(EditTickerViewModel obj)
        {
            if (!Enum.IsDefined(typeof(StateType), obj.StateType))
            {
                ModelState.AddModelError("stateType", "Invalid state selected.");
                return View(obj);
            }

            var ticket = _unitOfWork.Repository<Ticket>().Get(obj.id);

            if (ticket == null)
                return NotFound();

            if (ModelState.IsValid)
            {
                ticket.StartDateTime = obj.StartDataTime;
                ticket.EndDateTime = obj.EndDateTime;
                ticket.stateType = obj.StateType;
                ticket.FinalReport = obj.FinalReport;
                _unitOfWork.Complete();
                 TempData["success"] = "Ticket Updated Successfully";
                return RedirectToAction(nameof(Details), new { obj.id });
            }

            return View(obj);
        }
        #endregion

        #region EditAppointment

        [Authorize(Roles = SD.AdminRole)]
        public IActionResult EditAppointment(int id)
        {
            var appointment = _unitOfWork.Repository<Appointment>().Get(id);

            if (appointment == null)
                return NotFound();

            var specDriver = new BaseSpecification<Driver>();
            specDriver.Includes.Add(d => d.User);
            var specTech = new BaseSpecification<Technician>();
            specTech.Includes.Add(t => t.User);

            var viewModel = new EditAppointmentViewModel()
            {
                id = appointment.Id,
                StartDateTime = appointment.StartDateTime,
                EndDateTime = appointment.EndDateTime,
                PartialReport = appointment.PartialReport,
                TicketId = appointment.TicketId,
                TechId = appointment.TechnicianId,
                DriverId = appointment.DriverId,
                Driver = _unitOfWork.Repository<Driver>().GetAllWithSpec(specDriver)
                    .Select(u => new SelectListItem
                    {
                        Text = u.User.Name,
                        Value = u.Id.ToString(),
                    }).ToList(),
                Technician = _unitOfWork.Repository<Technician>().GetAllWithSpec(specTech)
                    .Select(u => new SelectListItem
                    {
                        Text = u.User.Name,
                        Value = u.Id.ToString(),
                    }).ToList()
            };

            return View(viewModel);
        }

        [Authorize(Roles = SD.AdminRole)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditAppointment(EditAppointmentViewModel viewModel)
        {
            var appointment = _unitOfWork.Repository<Appointment>().Get(viewModel.id);

            if (appointment == null)
                return NotFound();

            if (ModelState.IsValid)
            {
                appointment.StartDateTime = viewModel.StartDateTime;
                appointment.EndDateTime = viewModel.EndDateTime;
                appointment.PartialReport = viewModel.PartialReport;
                appointment.TechnicianId = viewModel.TechId;
                appointment.DriverId = viewModel.DriverId;

                _unitOfWork.Complete();
                TempData["success"] = "Appointment Updated Successfully";
                return RedirectToAction(nameof(Details), new { id = appointment.TicketId });
            }

            var specDriver = new BaseSpecification<Driver>();
            specDriver.Includes.Add(d => d.User);
            var specTech = new BaseSpecification<Technician>();
            specTech.Includes.Add(t => t.User);

            viewModel = new EditAppointmentViewModel()
            {
                id = appointment.Id,
                StartDateTime = appointment.StartDateTime,
                EndDateTime = appointment.EndDateTime,
                PartialReport = appointment.PartialReport,
                TicketId = appointment.TicketId,
                TechId = appointment.TechnicianId,
                DriverId = appointment.DriverId,
                Driver = _unitOfWork.Repository<Driver>().GetAllWithSpec(specDriver)
                    .Select(u => new SelectListItem
                    {
                        Text = u.User.Name,
                        Value = u.Id.ToString(),
                    }).ToList(),
                Technician = _unitOfWork.Repository<Technician>().GetAllWithSpec(specTech)
                    .Select(u => new SelectListItem
                    {
                        Text = u.User.Name,
                        Value = u.Id.ToString(),
                    }).ToList()
            };

            return View(viewModel);
        }

        #endregion

        #endregion

        #region Customer

        #region AddTicket
       
        [HttpGet]
        public IActionResult AddTicket(int? id)
        {
            if (User.IsInRole(SD.TechnicianRole))
            {
                var ticketFromDB = _unitOfWork.Repository<Ticket>().Get(id.Value);

                ViewBag.ServiceList = _unitOfWork.Repository<Service>().GetAll();
                return View((TicketViewModelCustomer)ticketFromDB);
            }
            else
            {
                LoadDropdowns(id);
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTicket(TicketViewModelCustomer ticketVM)
        {
            if (ModelState.IsValid)
            {
                ticketVM.stateType = StateType.New;
                ticketVM.CreatedAt = DateTime.Now;
                var ticket = (Ticket)ticketVM;
                _unitOfWork.Repository<Ticket>().Add(ticket);
                _unitOfWork.Complete();

                var service = _unitOfWork.Repository<Service>().Get(ticket.ServiceId);

                var items = new List<(string, decimal, int)> { (service.Name, service.Price, 1) };

                if (User.IsInRole(SD.TechnicianRole))
                {
                    var stripeSessionService = new StripeSessionService(_configuration);
                    var stripeSession = stripeSessionService.CreateSession(
                                ticket.Id,
                                items,
                                successPath: "/Ticket/TicketConfirmation",
                                cancelPath: "/Ticket/AllTicket"
                                );

                    _unitOfWork.TicketRepository.UpdateStripePaymentId(
                        ticket.Id, stripeSession.Id, stripeSession.PaymentIntentId);
                    _unitOfWork.Complete();

                    // Get Customer Email
                    var car = _unitOfWork.Repository<Car>().Get(ticket.CarId);

                    if (car != null)
                    {
                        var userSpec = new BaseSpecification<AppUser>(u => u.Id == car.UserId);
                        var user = _unitOfWork.Repository<AppUser>().GetEntityWithSpec(userSpec);

                        if (user != null && !string.IsNullOrEmpty(user.Email))
                        {
                            await _emailSender.SendEmailAsync(
                                user.Email,
                                "Stripe Payment Link",
                                $@"
                                    <p>Dear {user.Name},</p>
                                    <p>Thank you for choosing our service.</p>

                                    <p><strong>Service:</strong> {service.Name}</p>
                                    <p><strong>Price:</strong> {service.Price:C}</p>

                                    <p>Please complete your payment by clicking the button below:</p>

                                    <p style='margin: 20px 0;'>
                                        <a href='{stripeSession.Url}' style='
                                            display: inline-block;
                                            padding: 10px 20px;
                                            background-color: #28a745;
                                            color: white;
                                            text-decoration: none;
                                            border-radius: 5px;
                                            font-weight: bold;
                                        '>Pay Now</a>
                                    </p>
                                    <p><strong>Note:</strong> This payment link will expire in <strong>2 days</strong>. If payment is not completed within this time, the link will no longer be valid.</p>

                                    <p>If you have any questions, feel free to contact our support team.</p>

                                    <p>Best regards,<br/>Car Maintenance Team</p>
                                "
                            );
                        }
                        else
                        {
                            Console.WriteLine("User or Email is null.");
                        }
                    }

                    TempData["success"] = "Ticket created and payment link sent to the customer.";
                    return RedirectToAction(nameof(AllTicket));
                }
                else
                {
                    var stripeSessionService = new StripeSessionService(_configuration);
                    var stripeSession = stripeSessionService.CreateSession(
                                ticket.Id,
                                items,
                                successPath: "/Ticket/TicketConfirmation",
                                cancelPath: "/Ticket/AllTicket"
                                );

                    _unitOfWork.TicketRepository.UpdateStripePaymentId
                        (ticket.Id, stripeSession.Id, stripeSession.PaymentIntentId);
                    _unitOfWork.Complete();

                    Response.Headers["Location"] = stripeSession.Url.ToString();
                    return new StatusCodeResult(303);
                }
            }

            LoadDropdowns(null);
            return View();
        }
        #endregion

        #region  Confirmation

        public IActionResult TicketConfirmation(int id)
        {
            Ticket ticket = _unitOfWork.Repository<Ticket>().Get(id);
            var service = new SessionService();
            Session session = service.Get(ticket.SessionId);
            if (session.PaymentStatus.ToLower() == "paid")
            {
                _unitOfWork.TicketRepository.UpdateStripePaymentId(id, session.Id, session.PaymentIntentId);
                _unitOfWork.TicketRepository.UpdateStatus(id, SD.Approve);
                _unitOfWork.Complete();
            }
            return View(id);
        }

        #endregion

        #region TicketDetails
        public IActionResult TicketDetails([FromRoute] int? id)
        {
            if (!id.HasValue)
                return BadRequest();

            var spec = new BaseSpecification<Ticket>(t => t.Id == id);
            spec.Includes.Add(t => t.Service);
            spec.Includes.Add(t => t.Car);
            var ticket = _unitOfWork.Repository<Ticket>().GetEntityWithSpec(spec);

            if (ticket == null)
                return NotFound();

            return View(ticket);
        }

        #endregion

        #region  AppointmentDetails
        public IActionResult AppointmentDetails([FromRoute] int? id)
        {
            if (!id.HasValue)
                return BadRequest();

            var spec = new BaseSpecification<Appointment>(c => c.Id == id);
            spec.ComplexIncludes.Add(c => c.Include(m => m.Technician)
                .ThenInclude(m => m.User));
            spec.ComplexIncludes.Add(c => c.Include(m => m.Driver)
               .ThenInclude(m => m.User));

            var appointment = _unitOfWork.Repository<Appointment>().GetEntityWithSpec(spec);

            if (appointment == null)
                return NotFound();

            return View((AppointmentDetailViewModel)appointment);
        }
        #endregion

        #region  AddFeedback
        public IActionResult AddFeedback([FromRoute] int? id)
        {
            if (!id.HasValue)
                return BadRequest();

            var ticket = _unitOfWork.Repository<Ticket>().Get(id.Value);

            if (ticket == null)
                return NotFound();

            return View((AddFeedBackViewModel)ticket);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddFeedback(AddFeedBackViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var ticket = _unitOfWork.Repository<Ticket>().Get(viewModel.Id);

                if (ticket == null)
                    return NotFound();

                ticket.Feedback = viewModel.FeedBack;
                _unitOfWork.Complete();
                TempData["success"] = "Feedback added successfully";
                return RedirectToAction(nameof(AllTicket));
            }
            return View(viewModel);
        }
        #endregion

        #region Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var ticket = _unitOfWork.Repository<Ticket>().Get(id);

            if (ticket == null)
            {
                TempData["error"] = "Ticket not found.";
                return RedirectToAction(nameof(AllTicket));
            }

            if (ticket.stateType != StateType.New)
            {
                TempData["error"] = "You can only delete tickets with status 'New'.";
                return RedirectToAction(nameof(AllTicket));
            }

            _unitOfWork.Repository<Ticket>().Delete(ticket);
            _unitOfWork.Complete();

            TempData["success"] = "Ticket deleted successfully.";
            return RedirectToAction(nameof(AllTicket));
        }
        #endregion

        #region Methods
        private void LoadDropdowns(int? id)
        {
            IEnumerable<Service> serviceList;

            if (id != null)
            {
                var selectedService = _unitOfWork.Repository<Service>().Get(id.Value);

                serviceList = new List<Service> { selectedService }; // تغليف الكائن في List
            }
            else
            {
                serviceList = _unitOfWork.Repository<Service>().GetAll();
            }

            ViewBag.ServiceList = serviceList;

            var identity = (ClaimsIdentity)User.Identity;
            var claim = identity?.FindFirst(ClaimTypes.NameIdentifier);

            ViewBag.CarList = _unitOfWork.Repository<Car>()
                .GetAllWithSpec(new BaseSpecification<Car>(c => c.UserId == claim.Value));
        }

        #endregion

        #endregion

        #region Technician

        #region FinishAppointment

        [Authorize(Roles = SD.TechnicianRole)]
        public IActionResult FinishAppointment(int id)
        {
            var appointment = _unitOfWork.Repository<Appointment>().Get(id);

            if (appointment == null)
                return NotFound();

            var viewModel = new FinishAppointmentViewModel
            {
                Id = appointment.Id,
                PartialReport = appointment.PartialReport,
                EndDateTime = null,
                TicketId = appointment.TicketId,
            };

            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = SD.TechnicianRole)]
        public IActionResult FinishAppointment(FinishAppointmentViewModel model)
        {
            if (ModelState.IsValid)
            {
                var appointment = _unitOfWork.Repository<Appointment>().Get(model.Id);

                if (appointment != null)
                {
                    appointment.EndDateTime = model.EndDateTime;
                    appointment.PartialReport = model.PartialReport;
                }
                else
                {
                    return NotFound();
                }

                if (model.IsLastAppointment) // true
                {
                    var ticket = _unitOfWork.Repository<Ticket>().Get(appointment.TicketId);

                    ticket.EndDateTime = model.EndDateTime;
                    ticket.FinalReport = model.FinalReport;
                    ticket.stateType = StateType.Finished;
                }
                _unitOfWork.Complete();

                TempData["success"] = "Appointment has been finished successfully";
                return RedirectToAction(nameof(Details), new { id = appointment.TicketId });

            }

            return View(model);
        }
        #endregion

        #endregion

    }
}
