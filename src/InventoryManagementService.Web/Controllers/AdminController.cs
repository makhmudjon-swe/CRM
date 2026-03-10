using System.Security.Claims;
using InventoryManagementService.Application.Interfaces;
using InventoryManagementService.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementService.Web.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IInventoryService _inventoryService;

    public AdminController(UserManager<ApplicationUser> userManager, IInventoryService inventoryService)
    {
        _userManager = userManager;
        _inventoryService = inventoryService;
    }

    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    private static readonly string[] AllRoles = ["Admin", "Creator", "User"];

    public async Task<IActionResult> Index()
    {
        var users = await _userManager.Users.ToListAsync();
        ViewBag.Users = users;

        var userRoles = new Dictionary<string, string>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            userRoles[user.Id] = roles.FirstOrDefault() ?? "User";
        }
        ViewBag.UserRoles = userRoles;

        var inventories = await _inventoryService.GetAllAsync();
        ViewBag.Inventories = inventories;
        return View();
    }

    public async Task<IActionResult> UserDetails(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        ViewBag.TargetUser = user;
        var roles = await _userManager.GetRolesAsync(user);
        ViewBag.UserRole = roles.FirstOrDefault() ?? "User";

        var allInventories = await _inventoryService.GetAllAsync();
        ViewBag.UserInventories = allInventories.Where(i => i.OwnerId == id).ToList();

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ToggleBlock(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user != null)
        {
            user.IsBlocked = !user.IsBlocked;
            await _userManager.UpdateAsync(user);
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> ChangeRole(string userId, string role)
    {
        if (!AllRoles.Contains(role))
            return BadRequest("Invalid role.");

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return NotFound();

        // Remove all existing roles
        var currentRoles = await _userManager.GetRolesAsync(user);
        if (currentRoles.Any())
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

        // Assign the new role
        await _userManager.AddToRoleAsync(user, role);

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> ToggleManageAccess(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return NotFound();

        var currentRoles = await _userManager.GetRolesAsync(user);
        var currentRole = currentRoles.FirstOrDefault() ?? "User";

        // Admins  always has full access — no toggle needed
        if (currentRole == "Admin" || currentRole == "Creator")
            return RedirectToAction(nameof(Index));

        // Toggle the flag without changing the role
        user.CanManageInventories = !user.CanManageInventories;
        await _userManager.UpdateAsync(user);

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> DeleteUser(string userId)
    {
        if (userId == UserId)
        {
            TempData["Error"] = "You cannot delete your own account.";
            return RedirectToAction(nameof(Index));
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user != null)
        {
            await _userManager.DeleteAsync(user);
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> DeleteInventory(int id)
    {
        await _inventoryService.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
