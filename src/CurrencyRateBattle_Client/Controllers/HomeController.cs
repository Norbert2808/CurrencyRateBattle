using CRBClient.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Sockets;
using CRBClient.Services.Interfaces;
using CRBClient.Helpers;
using CRBClient.Dto;

namespace CRBClient.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    private readonly IRoomService _roomService;

    private readonly IUserService _userService;

    private readonly ICurrencyStateService _currencyStateService;

    private const int PageSize = 4;

    private List<RoomViewModel> _roomStorage = new();

    public HomeController(ILogger<HomeController> logger,
        IRoomService roomService,
        IUserService userService,
        ICurrencyStateService currencyStateService)
    {
        _logger = logger;
        _roomService = roomService;
        _userService = userService;
        _currencyStateService = currencyStateService;
    }

    public async Task<IActionResult> Index()
    {
        _logger.LogInformation("Home page with all rooms");
        ViewBag.Balance = await _userService.GetUserBalanceAsync();

        return View();
    }

    public async Task<IActionResult> Main(string searchNameString,
        string searchStartDateString,
        string searchEndDateString,
        int? page)
    {
        try
        {
            ViewBag.Balance = await _userService.GetUserBalanceAsync();
            var currState = await _currencyStateService.GetCurrencyRatesAsync();
            ViewBag.CurrencyRates = currState;
            ViewBag.Title = "Main Page";

            ViewData["CurrentNameFilter"] = searchNameString;
            ViewData["CurrentStartDateFilter"] = searchStartDateString;
            ViewData["CurrentEndDateFilter"] = searchEndDateString;

            var filter = new FilterDto(searchNameString, searchStartDateString, searchEndDateString);
            _roomStorage = filter.CheckFilter()
                ? await _roomService.GetFilteredCurrencyAsync(filter)
                : await _roomService.GetRoomsAsync(false);
        }
        catch (GeneralException)
        {
            _logger.LogDebug("User unauthorized");
            return Redirect("/Account/Authorization");
        }
        catch (SocketException ex)
        {
            _logger.LogError("{Msg}", ex.Message);
            return View("Error", new ErrorViewModel { RequestId = ex.Message });
        }

        var pageSize = 4;
        var roomViewModels = new PaginationList<RoomViewModel>();
        return View(await roomViewModels.CreateAsync(_roomStorage, page ?? 1, pageSize));
    }

    public async Task<IActionResult> Profile()
    {
        AccountInfoViewModel accountInfo;
        try
        {
            ViewBag.Balance = await _userService.GetUserBalanceAsync();
            ViewBag.Title = "User Profile";

            accountInfo = await _userService.GetAccountInfoAsync();
        }
        catch (GeneralException)
        {
            _logger.LogDebug("User unauthorized");
            return Redirect("/Account/Authorization");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError("{Msg}", ex.Message);
            return View("Error", new ErrorViewModel { RequestId = ex.Message });
        }
        catch (SocketException ex)
        {
            _logger.LogError("{Msg}", ex.Message);
            return View("Error", new ErrorViewModel { RequestId = ex.Message });
        }

        _logger.LogInformation("User profile page");
        return View(accountInfo);
    }

    public IActionResult Logout()
    {
        _userService.Logout();
        HttpContext.Session.Clear();

        _logger.LogInformation("User logout");
        return Redirect("/Account/Authorization");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

