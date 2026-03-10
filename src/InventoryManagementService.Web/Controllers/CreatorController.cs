using InventoryManagementService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace InventoryManagementService.Web.Controllers
{
    [Authorize(Roles = "Creator,Admin")]
    public class CreatorController : Controller
    {
        private readonly IInventoryService _inventoryService;

        public CreatorController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        // Barcha inventarlarni ko'rsatish (Admin uchun foydali, Creator uchun ham ruxsat berilgan)
        public async Task<IActionResult> Index()
        {
            var inventories = await _inventoryService.GetAllAsync();
            return View(inventories);
        }

    
        public async Task<IActionResult> MyInventories()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

        
            var myInventories = await _inventoryService.GetAllAsync();
          
            // var all = await _inventoryService.GetAllAsync();
            // var myInventories = all.Where(i => i.OwnerId == userId).ToList();

            return View("MyInventories", myInventories);  // alohida view bo'lsa yaxshi
        }

        // Bitta inventarni o'chirish
        [HttpPost]
        [ValidateAntiForgeryToken]   // xavfsizlik uchun qo'shish tavsiya etiladi
        public async Task<IActionResult> DeleteInventory(int id)
        {
            var inventory = await _inventoryService.GetByIdAsync(id);
            if (inventory == null)
            {
                return NotFound();
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Xavfsizlik: faqat o'z inventarini o'chirsin (Creator uchun)
            // Admin esa hammasini o'chirishi mumkin
            if (!User.IsInRole("Admin") && inventory.OwnerId != currentUserId)
            {
                return Forbid();   // yoki TempData["Error"] bilan xabar chiqarib Redirect
            }

            await _inventoryService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));   // yoki MyInventories ga qaytish ham mumkin
        }
    }
}