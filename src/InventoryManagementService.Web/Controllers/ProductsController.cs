using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WholesaleCRM.Application.Interfaces;
using WholesaleCRM.Application.Requests;

namespace WholesaleCRM.Web.Controllers;

[Authorize]
public class ProductsController : Controller
{
    private readonly IProductService _products;

    public ProductsController(IProductService products) => _products = products;

    public async Task<IActionResult> Index(int? categoryId)
    {
        ViewBag.Categories = await _products.GetCategoriesAsync();
        ViewBag.SelectedCategory = categoryId;
        var list = await _products.GetAllAsync(categoryId);
        return View(list);
    }

    public async Task<IActionResult> Create()
    {
        await LoadCategories();
        return View(new ProductRequest { IsActive = true });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductRequest req)
    {
        if (!ModelState.IsValid)
        {
            await LoadCategories();
            return View(req);
        }
        await _products.CreateAsync(req);
        TempData["Success"] = "Mahsulot qo'shildi.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var dto = await _products.GetByIdAsync(id);
        if (dto == null) return NotFound();

        await LoadCategories(dto.ProductCategoryId);
        var req = new ProductRequest
        {
            Name = dto.Name,
            SKU = dto.SKU,
            Description = dto.Description,
            ProductCategoryId = dto.ProductCategoryId,
            UnitPrice = dto.UnitPrice,
            StockQuantity = dto.StockQuantity,
            IsActive = dto.IsActive
        };
        ViewBag.ProductId = id;
        return View(req);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ProductRequest req)
    {
        if (!ModelState.IsValid)
        {
            await LoadCategories(req.ProductCategoryId);
            ViewBag.ProductId = id;
            return View(req);
        }
        await _products.UpdateAsync(id, req);
        TempData["Success"] = "Mahsulot yangilandi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _products.DeleteAsync(id);
        TempData["Success"] = "Mahsulot o'chirildi.";
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadCategories(int? selected = null)
    {
        var cats = await _products.GetCategoriesAsync();
        ViewBag.Categories = new SelectList(cats, "Id", "Name", selected);
    }
}
