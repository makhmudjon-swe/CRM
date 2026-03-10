using System.Security.Claims;
using InventoryManagementService.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementService.Web.Controllers;

public class SearchController : Controller
{
    private readonly IInventoryService _inventoryService;
    private readonly IItemService _itemService;

    public SearchController(IInventoryService inventoryService, IItemService itemService)
    {
        _inventoryService = inventoryService;
        _itemService = itemService;
    }

    public async Task<IActionResult> Index(string? q)
    {
        if (string.IsNullOrWhiteSpace(q))
        {
            ViewBag.Query = "";
            ViewBag.Inventories = Enumerable.Empty<object>();
            ViewBag.Items = Enumerable.Empty<object>();
            return View();
        }

        var userId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
        ViewBag.Query = q;
        ViewBag.Inventories = await _inventoryService.SearchAsync(q);
        ViewBag.Items = await _itemService.SearchAsync(q, userId);
        return View();
    }
}
