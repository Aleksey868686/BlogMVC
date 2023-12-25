using Blog.Extensions;
using Blog.Models;
using Blog.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers;

public class CommentController : Controller
{
    private readonly CommentService _commentService;

    public CommentController(CommentService commentService)
    {
        _commentService = commentService;
    }

    [HttpGet]
    [Authorize(Policy = "User")]
    public async Task<IActionResult> Index()
    {
        var comments = await _commentService.GetAllCommentsAsync();
        return View(comments);
    }

    [HttpGet]
    [Authorize(Policy = "User")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [Authorize(Policy = "User")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Comment comment)
    {
        if (ModelState.IsValid)
        {
            await _commentService.AddCommentAsync(comment);
            return RedirectToAction(nameof(Index));
        }
        return View(comment);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var comment = await _commentService.GetCommentByIdAsync(id);
        if (comment == null)
        {
            return NotFound();
        }

        var currentUserId = User.GetUserId(); // Extension method to get user ID from claims
        if (!await _commentService.IsUserCommentOwnerAsync(currentUserId, id) && !User.IsInRole("Administrator") && !User.IsInRole("Moderator"))
        {
            return Forbid();
        }

        return View(comment);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, Comment comment)
    {
        var currentUserId = User.GetUserId(); // Extension method to get user ID from claims
        if (!await _commentService.IsUserCommentOwnerAsync(currentUserId, id) && !User.IsInRole("Administrator") && !User.IsInRole("Moderator"))
        {
            return Forbid();
        }

        if (id != comment.Id)
        {
            return BadRequest();
        }

        if (ModelState.IsValid)
        {
            await _commentService.UpdateCommentAsync(comment);
            return RedirectToAction(nameof(Index));
        }
        return View(comment);
    }

    [HttpGet]
    [Authorize(Policy = "User")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var comment = await _commentService.GetCommentByIdAsync(id);
        if (comment == null)
        {
            return NotFound();
        }
        return View(comment);
    }

    [HttpPost, ActionName("Delete")]
    [Authorize(Policy = "User")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        await _commentService.DeleteCommentAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
