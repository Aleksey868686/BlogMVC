using Blog.Models;
using Blog.Services;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers;

public class PostController : Controller
{
    private readonly PostService _postService;

    public PostController(PostService postService)
    {
        _postService = postService;
    }

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

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
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

    public async Task<IActionResult> Edit(Guid id)
    {
        var post = await _postService.GetPostByIdAsync(id);
        if (post == null)
        {
            return NotFound();
        }
        return View(post);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, Post post)
    {
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
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        await _postService.DeletePostAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
