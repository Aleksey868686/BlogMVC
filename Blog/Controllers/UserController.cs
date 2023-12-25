using Blog.Models;
using Blog.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers;

public class UserController : Controller
{
    private readonly UserService _userService;

    public UserController(UserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var users = await _userService.GetAllUsersAsync();
        return View(users);
    }

    [HttpGet]
    [Authorize(Policy = "Administrator")]
    public async Task<IActionResult> Details(Guid id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }
        return View(user);
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(User user)
    {
        if (ModelState.IsValid)
        {
            await _userService.AddUserAsync(user);
            return RedirectToAction(nameof(Index));
        }
        return View(user);
    }

    [HttpGet]
    [Authorize(Policy = "User")]
    public async Task<IActionResult> Edit(Guid id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }
        return View(user);
    }

    [HttpPost]
    [Authorize(Policy = "User")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, User user)
    {
        if (id != user.Id)
        {
            return BadRequest();
        }

        if (ModelState.IsValid)
        {
            await _userService.UpdateUserAsync(user);
            return RedirectToAction(nameof(Index));
        }
        return View(user);
    }

    [HttpGet]
    [Authorize(Policy = "User")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }
        return View(user);
    }

    [HttpPost, ActionName("Delete")]
    [Authorize(Policy = "User")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        await _userService.DeleteUserAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
