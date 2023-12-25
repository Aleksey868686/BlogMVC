using Blog.Extensions;
using Blog.Models;
using Blog.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers;

public class PostController : Controller
{
    private readonly PostService _postService;

    public PostController(PostService postService)
    {
        _postService = postService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var posts = await _postService.GetAllPostsAsync();
        return View(posts);
    }

    [HttpGet]
    public async Task<IActionResult> PostsByUser(Guid userId)
    {
        if (userId == Guid.Empty)
        {
            return BadRequest("Invalid User ID");
        }

        var posts = await _postService.GetPostsByUserIdAsync(userId);
        return View(posts);
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
    public async Task<IActionResult> Create(Post post)
    {
        if (ModelState.IsValid)
        {
            await _postService.AddPostAsync(post);
            return RedirectToAction(nameof(Index));
        }
        return View(post);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var post = await _postService.GetPostByIdAsync(id);
        if (post == null)
        {
            return NotFound();
        }

        var currentUserId = User.GetUserId(); // Extension method to get user ID from claims
        if (!await _postService.IsUserPostOwnerAsync(currentUserId, id) && !User.IsInRole("Administrator") && !User.IsInRole("Moderator"))
        {
            return Forbid();
        }
  
        return View(post);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, Post post)
    {
        var currentUserId = User.GetUserId();
        if (!await _postService.IsUserPostOwnerAsync(currentUserId, id) && !User.IsInRole("Administrator") && !User.IsInRole("Moderator"))
        {
            return Forbid();
        }

        if (id != post.Id)
        {
            return BadRequest();
        }

        if (ModelState.IsValid)
        {
            await _postService.UpdatePostAsync(post);
            return RedirectToAction(nameof(Index));
        }

        return View(post);
    }

    [HttpGet]
    public async Task<IActionResult> Delete(Guid id)
    {
        var post = await _postService.GetPostByIdAsync(id);
        if (post == null)
        {
            return NotFound();
        }
        return View(post);
    }

    [HttpPost, ActionName("Delete")]
    [Authorize(Policy = "User")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        await _postService.DeletePostAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
