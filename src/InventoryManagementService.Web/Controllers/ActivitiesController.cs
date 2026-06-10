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
public class ActivitiesController : Controller
{
    private readonly IActivityService _activities;
    private readonly ICustomerService _customers;
    private readonly IDealService _deals;
    private readonly UserManager<AppUser> _userManager;

    public ActivitiesController(IActivityService activities, ICustomerService customers,
        IDealService deals, UserManager<AppUser> userManager)
    {
        _activities = activities;
        _customers = customers;
        _deals = deals;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var list = await _activities.GetAllAsync();
        return View(list);
    }

    public async Task<IActionResult> Create(int? customerId, int? dealId)
    {
        await LoadDropdowns(customerId, dealId);
        return View(new ActivityRequest
        {
            CustomerId = customerId,
            DealId = dealId,
            ActivityDate = DateTime.Now
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ActivityRequest req)
    {
        if (!ModelState.IsValid)
        {
            await LoadDropdowns(req.CustomerId, req.DealId);
            return View(req);
        }

        var user = await _userManager.GetUserAsync(User);
        req.UserId = user?.Id;

        await _activities.CreateAsync(req);
        TempData["Success"] = "Faoliyat qo'shildi.";

        if (req.CustomerId.HasValue)
            return RedirectToAction("Details", "Customers", new { id = req.CustomerId });
        if (req.DealId.HasValue)
            return RedirectToAction("Details", "Deals", new { id = req.DealId });
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, int? customerId)
    {
        await _activities.DeleteAsync(id);
        TempData["Success"] = "Faoliyat o'chirildi.";
        if (customerId.HasValue)
            return RedirectToAction("Details", "Customers", new { id = customerId });
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadDropdowns(int? customerId = null, int? dealId = null)
    {
        var customers = await _customers.GetAllAsync();
        var deals = await _deals.GetAllAsync();
        ViewBag.Customers = new SelectList(customers, "Id", "CompanyName", customerId);
        ViewBag.Deals = new SelectList(deals, "Id", "Title", dealId);
        ViewBag.Types = Enum.GetValues<ActivityType>()
            .Select(t => new SelectListItem
            {
                Value = ((int)t).ToString(),
                Text = t switch
                {
                    ActivityType.Call => "Qo'ng'iroq",
                    ActivityType.Email => "Email",
                    ActivityType.Meeting => "Uchrashuv",
                    ActivityType.Note => "Eslatma",
                    ActivityType.Task => "Vazifa",
                    _ => t.ToString()
                }
            }).ToList();
    }
}
