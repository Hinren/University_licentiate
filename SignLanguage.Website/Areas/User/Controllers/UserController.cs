﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SignLanguage.EF;
using SignLanguage.Logic;
using SignLanguage.Models;
using SignLanguage.Website.Controllers;
using SignLanguage.Website.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SignLanguage.Website.Areas.User.Controllers
{
    //TODO  actuaaly i have static model to rember user in system, but is not good solve
    //i need change this to get user more discreetly later read more about authentication and change this
    [Area("User")]
    public class UserController : BaseController
    {
        private UserManager<ApplicationUser> userManager { get; }
        private SignInManager<ApplicationUser> signInUser { get; }
        public UserController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInUser)
        {
            this.userManager = userManager;
            this.signInUser = signInUser;
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Register()
        {
            UserRegister userRegister = new UserRegister();
            return View(userRegister);
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserRegister userRegister)
        {
            var testtt = await userManager.GetUserAsync(User);

            if (ModelState.IsValid)
            {
                string message = "";
                try
                {
                    message = "User already registered";

                    ApplicationUser user = await userManager.FindByNameAsync(userRegister.Login);

                    if (user == null)
                    {
                        user = new ApplicationUser();
                        user.UserName = userRegister.Login;
                        user.PasswordHash = userRegister.Password;
                        user.Email = userRegister.Email;
                        user.EmailConfirmed = true;
                        user.UserId = Guid.NewGuid().ToString("N");

                        IdentityResult result = await userManager.CreateAsync(user, userRegister.Password);

                        message = "User was created";
                    }
                }
                catch (Exception ex)
                {
                    message += ex.Message;
                }

                SetMessageInfo(message);
                return View();
            }
            else
            {
                SetMessageWarning("", ModelState.Values.SelectMany(v => v.Errors).Where(me => me != null).Select(me => me.ErrorMessage));
                return View();
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Login()
        {
            UserLogin userLogin = new UserLogin();
            return View(userLogin);
        }

        [HttpPost]
        public async Task<IActionResult> Login(UserLogin userLogin)
        {
            if (ModelState.IsValid)
            {
                var result = await signInUser.PasswordSignInAsync(userLogin.Login, userLogin.Password, false, false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    SetMessageWarning("Podano niewłaściwe hasło i login");
                    return View();
                }
            }
            else
            {
                SetMessageWarning("", ModelState.Values.SelectMany(v => v.Errors).Where(me => me != null).Select(me => me.ErrorMessage));
                return View();
            }
        }

        public async Task<IActionResult> Logout()
        {
            await signInUser.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

    }
}