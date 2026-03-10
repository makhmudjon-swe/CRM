using System.Security.Claims;
using InventoryManagementService.Application.DTOs;
using InventoryManagementService.Application.Interfaces;
using InventoryManagementService.Domain.Entities;
using InventoryManagementService.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementService.Web.Controllers;

[Route("api")]
[ApiController]
public class ApiController : ControllerBase
{
    private readonly ITagService _tagService;
    private readonly ILikeService _likeService;
    private readonly ICommentService _commentService;
    private readonly IInventoryService _inventoryService;
    private readonly IItemService _itemService;
    private readonly IInventoryAccessService _accessService;
    private readonly IInventoryAuthorizationService _authService;
    private readonly IImageService _imageService;
    private readonly ICustomIdService _customIdService;
    private readonly UserManager<ApplicationUser> _userManager;

    public ApiController(ITagService tagService, ILikeService likeService, ICommentService commentService,
        IInventoryService inventoryService, IItemService itemService,
        IInventoryAccessService accessService, IInventoryAuthorizationService authService,
        IImageService imageService, ICustomIdService customIdService,
        UserManager<ApplicationUser> userManager)
    {
        _tagService = tagService;
        _likeService = likeService;
        _commentService = commentService;
        _inventoryService = inventoryService;
        _itemService = itemService;
        _accessService = accessService;
        _authService = authService;
        _imageService = imageService;
        _customIdService = customIdService;
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

    [HttpGet("tags/search")]
    public async Task<IActionResult> SearchTags([FromQuery] string q)
    {
        var tags = await _tagService.SearchAsync(q ?? "");
        return Ok(tags.Select(t => t.Name));
    }

     

    [HttpPost("images/upload")]
    [Authorize]
    [RequestSizeLimit(5 * 1024 * 1024)]
    public async Task<IActionResult> UploadImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { error = "No file provided." });

        var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
        if (!allowedTypes.Contains(file.ContentType))
            return BadRequest(new { error = "Only JPEG, PNG, GIF, and WebP images are allowed." });

        using var stream = file.OpenReadStream();
        var url = await _imageService.UploadAsync(stream, file.FileName);
        return Ok(new { url });
    }

    [HttpPost("likes/toggle")]
    [Authorize]
    public async Task<IActionResult> ToggleLike([FromBody] ToggleLikeRequest request)
    {
        var isLiked = await _likeService.ToggleAsync(request.ItemId, UserId!);
        var count = await _likeService.GetCountAsync(request.ItemId);
        return Ok(new { isLiked, count });
    }

    [HttpGet("comments/{itemId}")]
    public async Task<IActionResult> GetComments(int itemId)
    {
        var comments = await _commentService.GetByItemAsync(itemId);
        return Ok(comments);
    }

    [HttpPost("comments")]
    [Authorize]
    public async Task<IActionResult> AddComment([FromBody] CreateCommentDto dto)
    {
        var comment = await _commentService.CreateAsync(dto, UserId!);
        return Ok(comment);
    }

    [HttpDelete("comments/{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteComment(int id)
    {
        await _commentService.DeleteAsync(id);
        return Ok();
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string q)
    {
        var inventories = await _inventoryService.SearchAsync(q ?? "");
        var items = await _itemService.SearchAsync(q ?? "", UserId);
        return Ok(new { inventories, items });
    }

    // --- Access Management Endpoints ---

    [HttpGet("inventory/{id}/access")]
    [Authorize]
    public async Task<IActionResult> GetAccess(int id)
    {
        if (!await _authService.CanManageAccessAsync(id, UserId!, IsAdmin, IsCreator, await CanManageAsync()))
            return Forbid();

        var accessList = await _accessService.GetByInventoryAsync(id);
        return Ok(accessList);
    }

    [HttpPost("inventory/{id}/access")]
    [Authorize]
    public async Task<IActionResult> GrantAccess(int id, [FromBody] GrantAccessRequest request)
    {
        if (!await _authService.CanManageAccessAsync(id, UserId!, IsAdmin, IsCreator, await CanManageAsync()))
            return Forbid();

        await _accessService.GrantAccessAsync(id, request.UserId, AccessLevel.ReadWrite);
        return Ok(new { success = true });
    }

    [HttpDelete("inventory/{id}/access/{userId}")]
    [Authorize]
    public async Task<IActionResult> RevokeAccess(int id, string userId)
    {
        if (!await _authService.CanManageAccessAsync(id, UserId!, IsAdmin, IsCreator, await CanManageAsync()))
            return Forbid();

        await _accessService.RevokeAccessAsync(id, userId);
        return Ok(new { success = true });
    }

    [HttpPost("inventory/{id}/toggle-public")]
    [Authorize]
    public async Task<IActionResult> TogglePublic(int id)
    {
        if (!await _authService.CanManageAccessAsync(id, UserId!, IsAdmin, IsCreator, await CanManageAsync()))
            return Forbid();

        var inventory = await _inventoryService.GetByIdAsync(id);
        if (inventory == null) return NotFound();

        await _accessService.SetPublicAsync(id, !inventory.IsPublic);
        return Ok(new { isPublic = !inventory.IsPublic });
    }

    [HttpGet("users/search")]
    [Authorize]
    public async Task<IActionResult> SearchUsers([FromQuery] string q)
    {
        if (string.IsNullOrWhiteSpace(q) || q.Length < 2)
            return Ok(Array.Empty<object>());

        var users = await _userManager.Users
            .Where(u => (u.DisplayName != null && u.DisplayName.Contains(q)) || (u.Email != null && u.Email.Contains(q)))
            .Take(10)
            .Select(u => new { u.Id, u.DisplayName, u.Email })
            .ToListAsync();

        return Ok(users);
    }

    // --- Custom ID Format Endpoints ---

    [HttpGet("inventory/{id}/custom-id-format")]
    [Authorize]
    public async Task<IActionResult> GetCustomIdFormat(int id)
    {
        var format = await _customIdService.GetFormatAsync(id);
        return Ok(format);
    }

    [HttpPost("inventory/{id}/custom-id-format")]
    [Authorize]
    public async Task<IActionResult> SaveCustomIdFormat(int id, [FromBody] SaveCustomIdFormatRequest request)
    {
        if (!await _authService.CanManageAccessAsync(id, UserId!, IsAdmin, IsCreator, await CanManageAsync()))
            return Forbid();

        var format = await _customIdService.SaveFormatAsync(id, request);
        return Ok(format);
    }

    [HttpDelete("inventory/{id}/custom-id-format")]
    [Authorize]
    public async Task<IActionResult> DeleteCustomIdFormat(int id)
    {
        if (!await _authService.CanManageAccessAsync(id, UserId!, IsAdmin, IsCreator, await CanManageAsync()))
            return Forbid();

        await _customIdService.DeleteFormatAsync(id);
        return Ok(new { success = true });
    }

    [HttpPost("custom-id/preview")]
    [Authorize]
    public IActionResult PreviewCustomId([FromBody] SaveCustomIdFormatRequest request)
    {
        var preview = _customIdService.PreviewId(request.Elements);
        return Ok(new { preview });
    }

    // --- Admin: user-centric access management ---

    [HttpGet("admin/user/{userId}/access")]
    [Authorize(Roles = "Admin,Creator")]
    public async Task<IActionResult> GetUserAccess(string userId)
    {
        var accessList = await _accessService.GetByUserAsync(userId);
        return Ok(accessList);
    }

    [HttpPost("admin/user/{userId}/grant-access")]
    [Authorize(Roles = "Admin,Creator")]
    public async Task<IActionResult> AdminGrantAccess(string userId, [FromBody] AdminGrantAccessRequest request)
    {
        await _accessService.GrantAccessAsync(request.InventoryId, userId, AccessLevel.ReadWrite);
        return Ok(new { success = true });
    }

    [HttpDelete("admin/user/{userId}/access/{inventoryId}")]
    [Authorize(Roles = "Admin,Creator")]
    public async Task<IActionResult> AdminRevokeAccess(string userId, int inventoryId)
    {
        await _accessService.RevokeAccessAsync(inventoryId, userId);
        return Ok(new { success = true });
    }

    [HttpGet("admin/inventories/search")]
    [Authorize(Roles = "Admin,Creator")]
    public async Task<IActionResult> SearchInventories([FromQuery] string q)
    {
        if (string.IsNullOrWhiteSpace(q) || q.Length < 2)
            return Ok(Array.Empty<object>());

        var inventories = await _inventoryService.SearchAsync(q);
        return Ok(inventories.Take(10).Select(i => new { i.Id, i.Name, i.OwnerName }));
    }
}

public class ToggleLikeRequest
{
    public int ItemId { get; set; }
}

public class GrantAccessRequest
{
    public string UserId { get; set; } = string.Empty;
}

public class AdminGrantAccessRequest
{
    public int InventoryId { get; set; }
}
