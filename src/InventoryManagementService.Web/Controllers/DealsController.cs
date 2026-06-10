using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WholesaleCRM.Application.Interfaces;
using WholesaleCRM.Application.Requests;
using WholesaleCRM.Domain.Entities;
using WholesaleCRM.Domain.Enums;

namespace WholesaleCRM.Web.Controllers;

[Authorize]
public class DealsController : Controller
{
    private readonly IDealService _deals;
    private readonly ICustomerService _customers;
    private readonly UserManager<AppUser> _userManager;

    public DealsController(IDealService deals, ICustomerService customers, UserManager<AppUser> userManager)
    {
        _deals = deals;
        _customers = customers;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var all = (await _deals.GetAllAsync()).ToList();
        ViewBag.Statuses = Enum.GetValues<DealStatus>().Cast<int>().ToList();
        return View(all);
    }

    public async Task<IActionResult> Details(int id)
    {
        var dto = await _deals.GetByIdAsync(id);
        if (dto == null) return NotFound();
        return View(dto);
    }

    public async Task<IActionResult> Create(int? customerId)
    {
        await LoadDropdowns(customerId);
        return View(new DealRequest { CustomerId = customerId ?? 0, ExpectedCloseDate = DateTime.Now.AddDays(30) });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(DealRequest req)
    {
        if (!ModelState.IsValid)
        {
            await LoadDropdowns(req.CustomerId);
            return View(req);
        }
        var dto = await _deals.CreateAsync(req);
        TempData["Success"] = $"'{dto.Title}' bitimi yaratildi.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var dto = await _deals.GetByIdAsync(id);
        if (dto == null) return NotFound();

        await LoadDropdowns(dto.CustomerId);
        var req = new DealRequest
        {
            Title = dto.Title,
            CustomerId = dto.CustomerId,
            AssignedToId = dto.AssignedToId,
            Status = dto.StatusValue,
            TotalAmount = dto.TotalAmount,
            ExpectedCloseDate = dto.ExpectedCloseDate,
            Notes = dto.Notes
        };
        ViewBag.DealId = id;
        return View(req);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, DealRequest req)
    {
        if (!ModelState.IsValid)
        {
            await LoadDropdowns(req.CustomerId);
            ViewBag.DealId = id;
            return View(req);
        }
        await _deals.UpdateAsync(id, req);
        TempData["Success"] = "Bitim yangilandi.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, int status)
    {
        await _deals.UpdateStatusAsync(id, status);
        return Ok();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _deals.DeleteAsync(id);
        TempData["Success"] = "Bitim o'chirildi.";
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadDropdowns(int? customerId = null)
    {
        var customers = await _customers.GetAllAsync();
        ViewBag.Customers = new SelectList(customers, "Id", "CompanyName", customerId);
        ViewBag.Users = new SelectList(_userManager.Users.ToList(), "Id", "FullName");
        ViewBag.Statuses = Enum.GetValues<DealStatus>()
            .Select(s => new SelectListItem { Value = ((int)s).ToString(), Text = GetStatusName(s) })
            .ToList();
    }

    private static string GetStatusName(DealStatus s) => s switch
    {
        DealStatus.Lead => "Yangi Lead",
        DealStatus.Qualified => "Qualified",
        DealStatus.Proposal => "Taklif",
        DealStatus.Negotiation => "Muzokaralar",
        DealStatus.Won => "Yutildi",
        DealStatus.Lost => "Yutqizildi",
        _ => s.ToString()
    };
}
