using BLLProject.Interfaces;
using BLLProject.Specifications;
using DALProject.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PLProj.Email;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLProj.Jops
{
    public class EmailService  
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailSender _emailSender;
        private readonly string _domain;

        public EmailService(IUnitOfWork unitOfWork, IEmailSender emailSender, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _emailSender = emailSender;
            _domain = configuration["Stripe:Domain"]
                ?? throw new InvalidOperationException("Domain is not configured in appsettings.");
        }

        public async Task SendKilometreReminderEmails()
        {
            var usersWithCarsAndTicketsSpec = new BaseSpecification<AppUser>
                        (u => u.Cars.Any(car => car.Tickets.Any(ticket => ticket.PaymentStatus != null)));
            usersWithCarsAndTicketsSpec.Includes.Add(u => u.Cars);
            usersWithCarsAndTicketsSpec.ComplexIncludes.Add(c => c.Include(c => c.Cars)
                        .ThenInclude(m => m.Model)
                        .ThenInclude(b => b.Brand));
            usersWithCarsAndTicketsSpec.ComplexIncludes.Add(c => c.Include(u => u.Cars)
                        .ThenInclude(c => c.Tickets));
            var users = _unitOfWork.Repository<AppUser>().GetAllWithSpec(usersWithCarsAndTicketsSpec);

           
            foreach (var user in users)
            {
                if (!string.IsNullOrEmpty(user.Email))
                {
                    var carsWithTickets = user.Cars.Where(c => c.Tickets != null && c.Tickets.
                                Any(t => t.PaymentStatus != null)).ToList();
                    if(carsWithTickets.Any())
                    {
                        StringBuilder emailBody = new StringBuilder();
                        emailBody.Append("<p>Please update the current kilometre reading for your cars:</p><ul>");

                        foreach (var car in carsWithTickets)
                        {
                            emailBody.Append(

                                 $@"<li>
                                     Car: {car.PlateNumber} - {car.Model?.Name} {car.Model?.Brand?.Name} <br/>
                                    <a href='{_domain}/Kilometre/Update?carId={car.Id}'>
                                    Click here to update the current kilometre reading
                                 </a>
                                 </li>
                                 ");
                        }

                        emailBody.Append("</ul>");

                        await _emailSender.SendEmailAsync(
                            user.Email,
                            "Weekly Kilometre Update Reminder",
                            emailBody.ToString()
                        );
                    }
                }
            }
        }
    }

}
