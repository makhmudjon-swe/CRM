using System.Diagnostics;
using System.Security.Claims;
using InventoryManagementService.Application.Interfaces;
using InventoryManagementService.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementService.Web.Controllers;

public class HomeController : Controller
{
    private readonly IInventoryService _inventoryService;
    private readonly ITagService _tagService;

    public HomeController(IInventoryService inventoryService, ITagService tagService)
    {
        _inventoryService = inventoryService;
        _tagService = tagService;
    }

    public async Task<IActionResult> Index()
    {
        var isAuthenticated = User.Identity?.IsAuthenticated == true;
        var isAdmin = User.IsInRole("Admin");
        var isCreator = User.IsInRole("Creator");

        if (isAuthenticated && !isAdmin && !isCreator)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var accessible = await _inventoryService.GetAccessibleAsync(userId);
            var accessibleList = accessible.ToList();
            ViewBag.LatestInventories = accessibleList.OrderByDescending(i => i.CreatedAt).Take(10);
            ViewBag.TopPopular = accessibleList.OrderByDescending(i => i.ItemCount).Take(5);
        }
        else
        {
            ViewBag.LatestInventories = await _inventoryService.GetLatestAsync(10);
            ViewBag.TopPopular = await _inventoryService.GetTopPopularAsync(5);
        }

        ViewBag.TagCloud = await _tagService.GetPopularAsync(30);
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
