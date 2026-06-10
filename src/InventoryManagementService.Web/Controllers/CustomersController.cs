using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WholesaleCRM.Application.Interfaces;
using WholesaleCRM.Application.Requests;
using WholesaleCRM.Domain.Entities;

namespace WholesaleCRM.Web.Controllers;

[Authorize]
public class CustomersController : Controller
{
    private readonly ICustomerService _customers;
    private readonly UserManager<AppUser> _userManager;

    public CustomersController(ICustomerService customers, UserManager<AppUser> userManager)
    {
        _customers = customers;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(string? search)
    {
        ViewBag.Search = search;
        var list = await _customers.GetAllAsync(search);
        return View(list);
    }

    public async Task<IActionResult> Details(int id)
    {
        var dto = await _customers.GetByIdAsync(id);
        if (dto == null) return NotFound();
        return View(dto);
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.Users = await _userManager.Users.ToListAsync();
        return View(new CustomerRequest());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CustomerRequest req)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Users = await _userManager.Users.ToListAsync();
            return View(req);
        }
        var dto = await _customers.CreateAsync(req);
        TempData["Success"] = $"'{dto.CompanyName}' muvaffaqiyatli qo'shildi.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var dto = await _customers.GetByIdAsync(id);
        if (dto == null) return NotFound();

        ViewBag.Users = await _userManager.Users.ToListAsync();
        var req = new CustomerRequest
        {
            CompanyName = dto.CompanyName,
            Industry = dto.Industry,
            Email = dto.Email,
            Phone = dto.Phone,
            Address = dto.Address,
            City = dto.City,
            Country = dto.Country,
            Status = dto.StatusValue,
            AssignedToId = dto.AssignedToId,
            Notes = dto.Notes
        };
        ViewBag.CustomerId = id;
        return View(req);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CustomerRequest req)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Users = await _userManager.Users.ToListAsync();
            ViewBag.CustomerId = id;
            return View(req);
        }
        await _customers.UpdateAsync(id, req);
        TempData["Success"] = "Mijoz ma'lumotlari yangilandi.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _customers.DeleteAsync(id);
        TempData["Success"] = "Mijoz o'chirildi.";
        return RedirectToAction(nameof(Index));
    }

    private async Task<List<AppUser>> ToListAsync() =>
        await Task.FromResult(_userManager.Users.ToList());
}

internal static class UserManagerExtensions
{
    public static async Task<List<T>> ToListAsync<T>(this IQueryable<T> query)
        => await Task.FromResult(query.ToList());
}
