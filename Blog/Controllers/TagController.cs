﻿using Blog.Models;
using Blog.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers;

public class TagController : Controller
{
    private readonly TagService _tagService;

    public TagController(TagService tagService)
    {
        _tagService = tagService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var tags = await _tagService.GetAllTagsAsync();
        return View(tags);
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
    public async Task<IActionResult> Create(Tag tag)
    {
        if (ModelState.IsValid)
        {
            await _tagService.AddTagAsync(tag);
            return RedirectToAction(nameof(Index));
        }
        return View(tag);
    }

    [HttpGet]
    [Authorize(Policy = "Administrator")]
    public async Task<IActionResult> Edit(Guid id)
    {
        var tag = await _tagService.GetTagByIdAsync(id);
        if (tag == null)
        {
            return NotFound();
        }
        return View(tag);
    }

    [HttpPost]
    [Authorize(Policy = "Administrator")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, Tag tag)
    {
        if (id != tag.Id)
        {
            return BadRequest();
        }

        if (ModelState.IsValid)
        {
            await _tagService.UpdateTagAsync(tag);
            return RedirectToAction(nameof(Index));
        }
        return View(tag);
    }

    [HttpGet]
    [Authorize(Policy = "Administrator")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var tag = await _tagService.GetTagByIdAsync(id);
        if (tag == null)
        {
            return NotFound();
        }
        return View(tag);
    }

    [HttpPost, ActionName("Delete")]
    [Authorize(Policy = "Administrator")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        await _tagService.DeleteTagAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
