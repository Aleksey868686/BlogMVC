using Blog.Models;
using Blog.Services;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers;

public class CommentController : Controller
{
    private readonly CommentService _commentService;

    public CommentController(CommentService commentService)
    {
        _commentService = commentService;
    }

    public async Task<IActionResult> Index()
    {
        var comments = await _commentService.GetAllCommentsAsync();
        return View(comments);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
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

    public async Task<IActionResult> Edit(Guid id)
    {
        var comment = await _commentService.GetCommentByIdAsync(id);
        if (comment == null)
        {
            return NotFound();
        }
        return View(comment);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, Comment comment)
    {
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
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        await _commentService.DeleteCommentAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
