using System.Security.Claims;
using InventoryManagementService.Application.DTOs;
using InventoryManagementService.Application.Interfaces;
using InventoryManagementService.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementService.Web.Controllers;

[Authorize]
public class ItemController : Controller
{
    private readonly IItemService _itemService;
    private readonly IInventoryService _inventoryService;
    private readonly IInventoryAuthorizationService _authService;
    private readonly UserManager<ApplicationUser> _userManager;

    public ItemController(IItemService itemService, IInventoryService inventoryService,
        IInventoryAuthorizationService authService, UserManager<ApplicationUser> userManager)
    {
        _itemService = itemService;
        _inventoryService = inventoryService;
        _authService = authService;
        _userManager = userManager;
    }

    private string? UserId => User.FindFirstValue(ClaimTypes.NameIdentifier);
    private bool IsAdmin => User.IsInRole("Admin");
    private bool IsCreator => User.IsInRole("Creator");

    private async Task<bool> CanManageAsync()
    {
        if (IsAdmin || IsCreator) return true;
        var user = await _userManager.FindByIdAsync(UserId!);
        return user?.CanManageInventories == true;
    }

    public async Task<IActionResult> Details(int id)
    {
        var item = await _itemService.GetByIdAsync(id, UserId);
        if (item == null) return NotFound();

        if (!await _authService.CanViewInventoryAsync(item.InventoryId, UserId!, IsAdmin, IsCreator))
            return Forbid();

        var canManage = await CanManageAsync();
        var inventory = await _inventoryService.GetByIdAsync(item.InventoryId);
        ViewBag.Inventory = inventory;
        ViewBag.CanWriteItems = await _authService.CanWriteItemsAsync(item.InventoryId, UserId!, IsAdmin, IsCreator, canManage);

        return View(item);
    }

    public async Task<IActionResult> Create(int inventoryId)
    {
        var inventory = await _inventoryService.GetByIdAsync(inventoryId);
        if (inventory == null) return NotFound();

        var canManage = await CanManageAsync();
        if (!await _authService.CanWriteItemsAsync(inventoryId, UserId!, IsAdmin, IsCreator, canManage))
            return Forbid();

        ViewBag.Inventory = inventory;
        return View(new CreateItemDto { InventoryId = inventoryId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateItemDto dto)
    {
        var canManage = await CanManageAsync();
        if (!await _authService.CanWriteItemsAsync(dto.InventoryId, UserId!, IsAdmin, IsCreator, canManage))
            return Forbid();

        if (!ModelState.IsValid)
        {
            var inventory = await _inventoryService.GetByIdAsync(dto.InventoryId);
            ViewBag.Inventory = inventory;
            return View(dto);
        }

        var item = await _itemService.CreateAsync(dto);
        return RedirectToAction(nameof(Details), new { id = item.Id });
    }

    public async Task<IActionResult> Edit(int id)
    {
        var item = await _itemService.GetByIdAsync(id, UserId);
        if (item == null) return NotFound();

        var canManage = await CanManageAsync();
        if (!await _authService.CanWriteItemsAsync(item.InventoryId, UserId!, IsAdmin, IsCreator, canManage))
            return Forbid();

        var inventory = await _inventoryService.GetByIdAsync(item.InventoryId);
        ViewBag.Inventory = inventory;

        var dto = new UpdateItemDto
        {
            Id = item.Id,
            Name = item.Name,
            CustomId = item.CustomId,
            InventoryId = item.InventoryId,
            CustomString1Value = item.CustomString1Value,
            CustomString2Value = item.CustomString2Value,
            CustomString3Value = item.CustomString3Value,
            CustomText1Value = item.CustomText1Value,
            CustomText2Value = item.CustomText2Value,
            CustomText3Value = item.CustomText3Value,
            CustomInt1Value = item.CustomInt1Value,
            CustomInt2Value = item.CustomInt2Value,
            CustomInt3Value = item.CustomInt3Value,
            CustomLink1Value = item.CustomLink1Value,
            CustomLink2Value = item.CustomLink2Value,
            CustomLink3Value = item.CustomLink3Value,
            CustomBool1Value = item.CustomBool1Value,
            CustomBool2Value = item.CustomBool2Value,
            CustomBool3Value = item.CustomBool3Value
        };
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateItemDto dto)
    {
        var canManage = await CanManageAsync();
        if (!await _authService.CanWriteItemsAsync(dto.InventoryId, UserId!, IsAdmin, IsCreator, canManage))
            return Forbid();

        if (!ModelState.IsValid)
        {
            var inventory = await _inventoryService.GetByIdAsync(dto.InventoryId);
            ViewBag.Inventory = inventory;
            return View(dto);
        }

        await _itemService.UpdateAsync(dto);
        return RedirectToAction(nameof(Details), new { id = dto.Id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, int inventoryId)
    {
        var canManage = await CanManageAsync();
        if (!await _authService.CanWriteItemsAsync(inventoryId, UserId!, IsAdmin, IsCreator, canManage))
            return Forbid();

        await _itemService.DeleteAsync(id);
        return RedirectToAction("Details", "Inventory", new { id = inventoryId });
    }
}
