using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WholesaleCRM.Application.Interfaces;
using WholesaleCRM.Application.Requests;

namespace WholesaleCRM.Web.Controllers;

[Authorize]
public class ContactsController : Controller
{
    private readonly IContactService _contacts;
    private readonly ICustomerService _customers;

    public ContactsController(IContactService contacts, ICustomerService customers)
    {
        _contacts = contacts;
        _customers = customers;
    }

    public async Task<IActionResult> Index()
    {
        var list = await _contacts.GetAllAsync();
        return View(list);
    }

    public async Task<IActionResult> Create(int? customerId)
    {
        await LoadCustomers(customerId);
        return View(new ContactRequest { CustomerId = customerId ?? 0 });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ContactRequest req)
    {
        if (!ModelState.IsValid)
        {
            await LoadCustomers(req.CustomerId);
            return View(req);
        }
        await _contacts.CreateAsync(req);
        TempData["Success"] = "Kontakt muvaffaqiyatli qo'shildi.";
        if (req.CustomerId > 0)
            return RedirectToAction("Details", "Customers", new { id = req.CustomerId });
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var dto = await _contacts.GetByIdAsync(id);
        if (dto == null) return NotFound();

        await LoadCustomers(dto.CustomerId);
        var req = new ContactRequest
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Phone = dto.Phone,
            Position = dto.Position,
            CustomerId = dto.CustomerId,
            IsPrimary = dto.IsPrimary
        };
        ViewBag.ContactId = id;
        return View(req);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ContactRequest req)
    {
        if (!ModelState.IsValid)
        {
            await LoadCustomers(req.CustomerId);
            ViewBag.ContactId = id;
            return View(req);
        }
        await _contacts.UpdateAsync(id, req);
        TempData["Success"] = "Kontakt yangilandi.";
        return RedirectToAction("Details", "Customers", new { id = req.CustomerId });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, int customerId)
    {
        await _contacts.DeleteAsync(id);
        TempData["Success"] = "Kontakt o'chirildi.";
        return RedirectToAction("Details", "Customers", new { id = customerId });
    }

    private async Task LoadCustomers(int? selectedId = null)
    {
        var customers = await _customers.GetAllAsync();
        ViewBag.Customers = new SelectList(customers, "Id", "CompanyName", selectedId);
    }
}
