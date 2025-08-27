using BLLProject.Interfaces;
using DALProject.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PLProj.Email;
using PLProj.HelperClasses;
using PLProj.Models.Account;
using System;
using System.Linq;
using System.Threading.Tasks;
using Utility;

namespace PLProj.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IWebHostEnvironment _env;
        private readonly IEmailSender _emailSender;

        public AccountController(UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            IUnitOfWork unitOfWork,
            RoleManager<IdentityRole> roleManager,
            IWebHostEnvironment env,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _unitOfWork = unitOfWork;
            _roleManager = roleManager;
            _env = env;
            _emailSender = emailSender;
        }

        #region Register
        public IActionResult Register()
        {

            var registerVM = new RegisterViewModel()
            {
                RoleList = _roleManager.Roles.Select(x => x.Name)
                .Select(i => new SelectListItem
                {
                    Text = i,
                    Value = i
                }),
                CategoryList = _unitOfWork.Repository<Category>().GetAll()
              .Select(u => new SelectListItem
              {
                  Text = u.Name,
                  Value = u.Id.ToString(),
              })
            };

            return View(registerVM);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model , IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                var user = new AppUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Name = model.Name,
                    PhoneNumber = model.PhoneNumber,
                    City = model.City,
                    Street = model.Street,
                    Role = model.Role ?? SD.CustomerRole
                };


                // check before Add User 
                #region Check

                if (model.Role == SD.TechnicianRole)
                {
                    if (!model.CategoryId.HasValue)
                    {
                        ModelState.AddModelError("CategoryId", "Category is required for technicians.");
                        PopulateDropdowns(model);
                        return View(model);
                    }
                }

                if (model.Role == SD.DriverRole)
                {
                    if (string.IsNullOrWhiteSpace(model.License) ||
                                !model.LicenseDate.HasValue ||
                                !model.LicenseExpDate.HasValue)
                    {
                        ModelState.AddModelError("", "License information is required for drivers.");
                        PopulateDropdowns(model);
                        return View(model);
                    }
                }

                #endregion

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    if (model.Role == null || model.Role == SD.CustomerRole)
                    {
                        await _userManager.AddToRoleAsync(user, SD.CustomerRole);
                    }
                    else if (model.Role == SD.AdminRole)
                    {
                        await _userManager.AddToRoleAsync(user, SD.AdminRole);
                    }
                    else if (model.Role == SD.TechnicianRole)
                    {
                        await _userManager.AddToRoleAsync(user, SD.TechnicianRole);
                        string imageUrl = null;
                        if (file != null)
                        {
                            imageUrl = ImageHelper.SaveImage(file, _env, "technician");
                        }

                        model.Img = imageUrl;

                        var technician = new Technician()
                        {
                            CategoryId = model.CategoryId.Value,
                            Availability = model.Availability,
                            BirthDate = model.BirthDate ?? DateOnly.MinValue,
                            ImgPath = model.Img,
                            Id = user.Id,
                        };

                        _unitOfWork.Repository<Technician>().Add(technician);
                    }
                    else if (model.Role == SD.DriverRole)
                    {
                        await _userManager.AddToRoleAsync(user, SD.DriverRole);
                        var driver = new Driver
                        {
                            Id = user.Id,
                            Availability = model.Availability,
                            BirthDate = model.BirthDate ?? DateOnly.MinValue,
                            License = model.License,
                            LicenseDate = model.LicenseDate.Value,
                            LicenseExpDate = model.LicenseExpDate.Value
                        };
                        _unitOfWork.Repository<Driver>().Add(driver);
                    }

                    _unitOfWork.Complete();
                    var isAdmin = User.IsInRole(SD.AdminRole);

                    TempData["Success"] = $"{model.Role ?? "Your"} account has been created successfully." +
                                          (isAdmin ? "" : " Please log in.");

                    return isAdmin ? RedirectToAction(nameof(UserController.Index), "User") : RedirectToAction(nameof(Login));
                }
                else
                {
                    foreach (var error in result.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);

                    PopulateDropdowns(model);
                    return View(model);
                }

            }
            else
            {
                PopulateDropdowns(model);
                return View(model);
            }
        }
       
        #endregion

        #region login
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user is null)
                    user = await _userManager.FindByNameAsync(model.Email); // user name
                if (user is not null)
                {
                    var flag = await _userManager.CheckPasswordAsync(user, model.Password);
                    if (flag)
                    {
                        var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);

                        if (result.Succeeded)
                            return RedirectToAction(nameof(HomeController.Index), "Home");
                    }
                }
                ModelState.AddModelError(string.Empty, "Email Or Password isn't Correct");
            }
            return View(model);
        }
        #endregion

        #region LogOut
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            HttpContext.Session.Clear();
            return RedirectToAction(nameof(Login));
        }

        #endregion

        #region ForgotPassword

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return RedirectToAction(nameof(ForgotPasswordConfirmation));
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Action(nameof(ResetPassword), "Account",
                new { token, email = user.Email }, protocol: HttpContext.Request.Scheme);

            await _emailSender.SendEmailAsync(
                model.Email,"Reset Password",
                $@"
                <div style='font-family: Arial, sans-serif; font-size: 14px; color: #333;'>
                    <h2 style='color: #2c3e50;'>Reset Your Password</h2>
                    <p>Hello,</p>
                    <p>We received a request to reset your password. 
                       Please click the button below to proceed:</p>
                    <a href='{callbackUrl}' 
                       style='display: inline-block; padding: 10px 20px; margin-top: 10px;
                              background-color: #007BFF; color: white; text-decoration: none;
                              border-radius: 5px; font-weight: bold;'>
                        Reset Password
                    </a>
                    <p style='margin-top: 20px; font-size: 12px; color: #888;'>
                        If you did not request a password reset, please ignore this email.
                    </p>
                </div>
                "
            );

            return RedirectToAction(nameof(ForgotPasswordConfirmation));
        }

        #endregion

        #region ResetPassword
       
        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            if (token == null || email == null)
                return RedirectToAction("Index", "Home");

            var model = new ResetPasswordViewModel { Token = token, Email = email };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return RedirectToAction(nameof(ForgotPasswordConfirmation));

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
            if (result.Succeeded)
                return RedirectToAction(nameof(ForgotPasswordConfirmation));

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }

        #endregion

        #region ForgotPasswordConfirmation

        [HttpGet]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        #endregion

        #region Method
        private void PopulateDropdowns(RegisterViewModel model)
        {
            model.RoleList = _roleManager.Roles.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Name
            });

            model.CategoryList = _unitOfWork.Repository<Category>().GetAll()
                .Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
        }
        #endregion



    }
}
