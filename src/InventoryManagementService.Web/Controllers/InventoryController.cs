    using System.Security.Claims;
using InventoryManagementService.Application.DTOs;
using InventoryManagementService.Application.Interfaces;
using InventoryManagementService.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementService.Web.Controllers;

[Authorize]
public class InventoryController : Controller
{
    private readonly IInventoryService _inventoryService;
    private readonly ICategoryService _categoryService;
    private readonly ITagService _tagService;
    private readonly IItemService _itemService;
    private readonly IInventoryAuthorizationService _authService;
    private readonly UserManager<ApplicationUser> _userManager;

    public InventoryController(IInventoryService inventoryService, ICategoryService categoryService,
        ITagService tagService, IItemService itemService, IInventoryAuthorizationService authService,
        UserManager<ApplicationUser> userManager)
    {
        _inventoryService = inventoryService;
        _categoryService = categoryService;
        _tagService = tagService;
        _itemService = itemService;
        _authService = authService;
        _userManager = userManager;
    }

    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;
    private bool IsAdmin => User.IsInRole("Admin");
    private bool IsCreator => User.IsInRole("Creator");

    private async Task<bool> CanManageAsync()
    {
        if (IsAdmin || IsCreator) return true;
        var user = await _userManager.FindByIdAsync(UserId);
        return user?.CanManageInventories == true;
    }

    public async Task<IActionResult> Index()
    {
        IEnumerable<InventoryDto> inventories;
        if (IsAdmin || IsCreator)
        {
            inventories = await _inventoryService.GetAllAsync();
        }
        else
        {
            inventories = await _inventoryService.GetAccessibleAsync(UserId);
        }

        ViewBag.CanCreate = await CanManageAsync();
        return View(inventories);
    }

    public async Task<IActionResult> Details(int id)
    {
        var inventory = await _inventoryService.GetByIdAsync(id);
        if (inventory == null) return NotFound();

        if (!await _authService.CanViewInventoryAsync(id, UserId, IsAdmin, IsCreator))
            return Forbid();

        var canManage = await CanManageAsync();
        ViewBag.Items = await _itemService.GetByInventoryAsync(id, UserId);
        ViewBag.CanEdit = await _authService.CanEditInventoryAsync(id, UserId, IsAdmin, IsCreator, canManage);
        ViewBag.CanWriteItems = await _authService.CanWriteItemsAsync(id, UserId, IsAdmin, IsCreator, canManage);

        return View(inventory);
    }

    public async Task<IActionResult> Create()
    {
        if (!await CanManageAsync())
            return Forbid();

        ViewBag.Categories = await _categoryService.GetAllAsync();
        return View(new CreateInventoryDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateInventoryDto dto)
    {
        if (!await CanManageAsync())
            return Forbid();

        if (!ModelState.IsValid)
        {
            ViewBag.Categories = await _categoryService.GetAllAsync();
            return View(dto);
        }

        var inventory = await _inventoryService.CreateAsync(dto, UserId);
        return RedirectToAction(nameof(Details), new { id = inventory.Id });
    }

    public async Task<IActionResult> Edit(int id)
    {
        var inventory = await _inventoryService.GetByIdAsync(id);
        if (inventory == null) return NotFound();

        var canManage = await CanManageAsync();
        if (!await _authService.CanEditInventoryAsync(id, UserId, IsAdmin, IsCreator, canManage))
            return Forbid();

        ViewBag.Categories = await _categoryService.GetAllAsync();
        var dto = new UpdateInventoryDto
        {
            Id = inventory.Id,
            Name = inventory.Name,
            Description = inventory.Description,
            ImageUrl = inventory.ImageUrl,
            CategoryId = inventory.CategoryId,
            IsPublic = inventory.IsPublic,
            Tags = inventory.Tags,
            CustomString1Enabled = inventory.CustomString1Enabled,
            CustomString1Name = inventory.CustomString1Name,
            CustomString2Enabled = inventory.CustomString2Enabled,
            CustomString2Name = inventory.CustomString2Name,
            CustomString3Enabled = inventory.CustomString3Enabled,
            CustomString3Name = inventory.CustomString3Name,
            CustomText1Enabled = inventory.CustomText1Enabled,
            CustomText1Name = inventory.CustomText1Name,
            CustomText2Enabled = inventory.CustomText2Enabled,
            CustomText2Name = inventory.CustomText2Name,
            CustomText3Enabled = inventory.CustomText3Enabled,
            CustomText3Name = inventory.CustomText3Name,
            CustomInt1Enabled = inventory.CustomInt1Enabled,
            CustomInt1Name = inventory.CustomInt1Name,
            CustomInt2Enabled = inventory.CustomInt2Enabled,
            CustomInt2Name = inventory.CustomInt2Name,
            CustomInt3Enabled = inventory.CustomInt3Enabled,
            CustomInt3Name = inventory.CustomInt3Name,
            CustomLink1Enabled = inventory.CustomLink1Enabled,
            CustomLink1Name = inventory.CustomLink1Name,
            CustomLink2Enabled = inventory.CustomLink2Enabled,
            CustomLink2Name = inventory.CustomLink2Name,
            CustomLink3Enabled = inventory.CustomLink3Enabled,
            CustomLink3Name = inventory.CustomLink3Name,
            CustomBool1Enabled = inventory.CustomBool1Enabled,
            CustomBool1Name = inventory.CustomBool1Name,
            CustomBool2Enabled = inventory.CustomBool2Enabled,
            CustomBool2Name = inventory.CustomBool2Name,
            CustomBool3Enabled = inventory.CustomBool3Enabled,
            CustomBool3Name = inventory.CustomBool3Name
        };
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateInventoryDto dto)
    {
        var canManage = await CanManageAsync();
        if (!await _authService.CanEditInventoryAsync(dto.Id, UserId, IsAdmin, IsCreator, canManage))
            return Forbid();

        if (!ModelState.IsValid)
        {
            ViewBag.Categories = await _categoryService.GetAllAsync();
            return View(dto);
        }

        await _inventoryService.UpdateAsync(dto);
        return RedirectToAction(nameof(Details), new { id = dto.Id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var inventory = await _inventoryService.GetByIdAsync(id);
        if (inventory == null) return NotFound();

        var canManage = await CanManageAsync();
        if (!await _authService.CanDeleteInventoryAsync(id, UserId, IsAdmin, IsCreator, canManage))
            return Forbid();

        await _inventoryService.DeleteAsync(id);
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public async Task<IActionResult> AutoSave([FromBody] UpdateInventoryDto? dto)
    {
        if (dto == null)
            return Json(new { success = false, error = "invalid", message = "Invalid data." });

        var canManage = await CanManageAsync();
        if (!await _authService.CanEditInventoryAsync(dto.Id, UserId, IsAdmin, IsCreator, canManage))
            return Json(new { success = false, error = "forbidden", message = "You don't have permission to edit this inventory." });

        try
        {
            await _inventoryService.UpdateAsync(dto);
            return Json(new { success = true });
        }
        catch (DbUpdateConcurrencyException)
        {
            return Json(new { success = false, error = "concurrency", message = "Another user has modified this inventory. Please reload." });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, error = "general", message = ex.Message });
        }
    }
}
