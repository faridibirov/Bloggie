using Bloggie.Web.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Bloggie.Web.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;

    public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async  Task<IActionResult> Register(RegisterViewModel registerViewModel)
    {

        var identityUser = new IdentityUser
        {
            UserName = registerViewModel.Username,
            Email = registerViewModel.Email
        };

       var identityResult =  await _userManager.CreateAsync(identityUser, registerViewModel.Password);

        if (identityResult.Succeeded)
        {
            // assign this user the "User" role

            var roleIdentityResult = await _userManager.AddToRoleAsync(identityUser, "User");

            if (roleIdentityResult.Succeeded)
            {
                // Show success notification
                return RedirectToAction("Register");
            }
        }

        // Show error notification
        return View();
    }

    [HttpGet]
    public IActionResult Login(string ReturnUrl)
    {
        var model = new LoginViewModel
        {
            ReturnUrl = ReturnUrl
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel loginViewModel)
    {
        var signInResult = await _signInManager.PasswordSignInAsync(loginViewModel.Username, loginViewModel.Password, false, false);

        if (signInResult.Succeeded && signInResult!=null)
        {
            if(!string.IsNullOrWhiteSpace(loginViewModel.ReturnUrl))
            {
                return Redirect(loginViewModel.ReturnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();

        return RedirectToAction("Index", "Home");
    }


    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }

}
