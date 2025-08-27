using BLLProject.Interfaces;
using BLLProject.Specifications;
using DALProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PLProj.Email;
using PLProj.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLProj.Controllers
{
    [Authorize]
    public class KilometreController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailSender _emailSender;

        public KilometreController(IUnitOfWork unitOfWork, IEmailSender emailSender)
        {
            _unitOfWork = unitOfWork;
            _emailSender = emailSender;
        }

        #region  Update

        public IActionResult Update(int? carId)
        {
            if (!carId.HasValue)
                return BadRequest();

            var car = _unitOfWork.Repository<Car>().Get(carId.Value);

            if (car == null)
                return NotFound();

            var viewModel = new KilometreViewModel
            {
                CarId = car.Id,
                PlateNumber = car.PlateNumber
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(KilometreViewModel obj)
        {
            if (ModelState.IsValid)
            {
                var km = new KiloMetres
                {
                    CarId = obj.CarId,
                    kiloMetre = obj.kiloMetre,
                };

                _unitOfWork.Repository<KiloMetres>().Add(km);
                _unitOfWork.Complete();

                var tickSpec = new BaseSpecification<Ticket>(t => t.CarId == obj.CarId && t.PaymentStatus != null);
                tickSpec.Includes.Add(t => t.Service);
                var tickets = _unitOfWork.Repository<Ticket>().GetAllWithSpec(tickSpec);

                var resultList = new List<(string serviceName, string status)>();

                if (tickets.Any())
                {
                    foreach (var ticket in tickets)
                    {
                        if (ticket.Service?.RecommendedKilometres > 0)
                        {
                            var targetKm = ticket.CurrentKilometres + ticket.Service.RecommendedKilometres;
                            string status;

                            if (obj.kiloMetre >= targetKm)
                                status = "⚠️ Overdue";
                            else if (targetKm - obj.kiloMetre <= 500)
                                status = "⏳ Almost due";
                            else
                                status = "✅ Still early";

                            resultList.Add((ticket.Service.Name, status));
                        }
                    }

                }

                var user = _unitOfWork.Repository<AppUser>()
                        .GetEntityWithSpec(new BaseSpecification<AppUser>(u => u.Cars.Any(c => c.Id == obj.CarId)));

                if (user != null)
                {
                    StringBuilder messageBody = new StringBuilder();

                    if (!tickets.Any())
                    {
                        messageBody.Append($@"
                        <p>Hello <strong>{user.Name}</strong>,</p>
                        <p>The car with plate number <strong>{obj.PlateNumber}</strong> is not linked to any service ticket in the system.</p>
                        <p>Please make sure to add services for proper tracking and notifications.</p>
                        <p style='margin-top: 20px;'>Thank you,<br/>Car Maintenance Team</p>
                        ");
                    }
                    else
                    {
                        messageBody.Append($@"
                        <!DOCTYPE html>
                        <html>
                        <head>
                          <meta charset='UTF-8'>
                          <style>
                            body {{
                                font-family: Arial, sans-serif;
                                background-color: #f4f4f4;
                                padding: 20px;
                                color: #333;
                            }}
                            .container {{
                                background-color: #ffffff;
                                padding: 20px;
                                border-radius: 8px;
                                box-shadow: 0 0 10px rgba(0,0,0,0.1);
                                max-width: 600px;
                                margin: auto;
                            }}
                            h2 {{
                                color: #007BFF;
                            }}
                            table {{
                                width: 100%;
                                border-collapse: collapse;
                                margin-top: 20px;
                            }}
                            th, td {{
                                padding: 10px;
                                border: 1px solid #ddd;
                                text-align: left;
                            }}
                            th {{
                                background-color: #007BFF;
                                color: white;
                            }}
                            .status {{
                                font-weight: bold;
                            }}
                          </style>
                        </head>
                        <body>
                          <div class='container'>
                            <h2>Service Analysis Report</h2>
                            <p>Hello <strong>{user.Name}</strong>,</p>
                            <p>Here is the status of services for your car <strong>{obj.PlateNumber}</strong> based on the current kilometre: <strong>{obj.kiloMetre}</strong>.</p>
                        
                         <table>
                              <tr>
                                <th>Service</th>
                                <th>Status</th>
                              </tr>");

                    foreach (var item in resultList)
                    {
                        messageBody.Append($@"
                                <tr>
                                  <td>{item.serviceName}</td>
                                  <td class='status'>{item.status}</td>
                                </tr>");
                    }

                    messageBody.Append(@"
                            </table>
                            <p style='margin-top: 20px;'>Thank you,<br/>Car Maintenance Team</p>
                            </div>
                            </body>
                            </html>");
                    }


                    await _emailSender.SendEmailAsync(
                        user.Email,
                        $"🚗 Service Status Update for {obj.PlateNumber} - {DateTime.Now:dd MMM yyyy}",
                        messageBody.ToString()
                    );
                }

                TempData["success"] = "Kilometre updated successfully and email sent.";
                return RedirectToAction("Index", "Home");
            }

            return View(obj);
        }

        #endregion

    }
}
