﻿using CRBClient.Helpers;
using CRBClient.Models;
using CRBClient.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CRBClient.Controllers;

public class AccountController : Controller
{
    private readonly ILogger<AccountController> _logger;

    private readonly IUserService _userService;

    public AccountController(ILogger<AccountController> logger,
        IUserService userService)
    {
        _logger = logger;
        _userService = userService;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View("LoginView");
    }

    [HttpPost]
    public async Task<ActionResult> LoginAsync(UserViewModel user)
    {
        try
        {
            await _userService.LoginUserAsync(user);
        }
        catch (CustomException)
        {
            return View("LoginView");
        }

        return Redirect("/Home/Index");
    }

    [HttpPost]
    public ActionResult Registration(UserViewModel user)
    {
        return Redirect("/Home/Index");
    }
}
