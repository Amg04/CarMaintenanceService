using BLLProject.Interfaces;
using BLLProject.Repositories;
using DALProject.Data;
using DALProject.DBInitializer;
using DALProject.Models;
using Hangfire;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PLProj.Email;
using PLProj.Jops;
using Stripe;
using System;
using System.Threading.Tasks;

namespace PLProj
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            
            #region Stripe
           
            builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));

            #endregion;

            #region IEmailSender

            builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
            builder.Services.AddTransient<IEmailSender, SmtpEmailSender>();

            #endregion

            #region ConfigureServices

            builder.Services.AddControllersWithViews();
            
            #region Dbcontext
            builder.Services.AddDbContext<CarAppDbContext>(optionsBuilder =>
            {
                optionsBuilder.UseSqlServer(builder.Configuration.GetConnectionString("CS"));
                //optionsBuilder.UseLazyLoadingProxies(true);
            });
            #endregion

            builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.Password.RequiredUniqueChars = 2;
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequiredLength = 5;

                options.SignIn.RequireConfirmedAccount = false;
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;
            }).AddEntityFrameworkStores<CarAppDbContext>()
            .AddDefaultTokenProviders();


            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/LogOut";
                options.AccessDeniedPath = "/Home/AccessDenied";
                options.SlidingExpiration = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
            });


            builder.Services.AddScoped<IDBInitializer, DBInitializer>();

            #region Jop
            builder.Services.AddHangfire(x =>x.UseSqlServerStorage(builder.Configuration.GetConnectionString("CS")));
            builder.Services.AddHangfireServer();
            builder.Services.AddScoped<TicketCleanupJob>();        
            builder.Services.AddScoped<EmailService>();
            #endregion

            #region faceBook

            //builder.Services.AddAuthentication(options =>
            //{
            //    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //    options.DefaultChallengeScheme = FacebookDefaults.AuthenticationScheme;
            //})
            //.AddCookie()
            //.AddFacebook(facebookOptions =>
            //{
            //    facebookOptions.AppId = "1773982039991815";
            //    facebookOptions.AppSecret = "cfcb83aacb241fff1b8c8721bf835c8d";
            //    facebookOptions.Scope.Add("email");
            //});

            #endregion


            builder.Services.AddRazorPages();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(100); 
                options.Cookie.HttpOnly = true; 
                options.Cookie.IsEssential = true; 
            });

            builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
            #endregion

            var app = builder.Build();

            #region  Jop

            // Add the job safely here using the service provider
            using (var scope = app.Services.CreateScope())
            {
                var jobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();
            
                var ticketCleanupJob = scope.ServiceProvider.GetRequiredService<TicketCleanupJob>();
                jobManager.AddOrUpdate<TicketCleanupJob>(
                    "delete-unpaid-tickets",
                    x => x.RemoveUnpaidTickets(),
                    Cron.Weekly(DayOfWeek.Sunday, 3)
                );
            
                var emailService = scope.ServiceProvider.GetRequiredService<EmailService>();
                jobManager.AddOrUpdate<EmailService>(
                    "SendWeeklyKilometreReminder",
                    x => x.SendKilometreReminderEmails(),
                    Cron.Weekly(DayOfWeek.Sunday, 15)
                );
            }

            #endregion

            #region Configure

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios,
                // see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:secretKey").Get<string>();
            
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSession();
            await SeedDatabaseAsync();

            // Jop
            app.UseHangfireDashboard();

            app.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{Id?}");
            
            #endregion

            app.Run();

            async Task SeedDatabaseAsync()
            {
                using (var scope = app.Services.CreateScope())
                {
                    var dbInitializer = scope.ServiceProvider.GetRequiredService<IDBInitializer>();
                    await dbInitializer.Initialize();
                }
            }
        }
    }
}

