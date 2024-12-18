﻿using Bloggie.Web.Data;
using Bloggie.Web.Models.Domain;
using Bloggie.Web.Models.ViewModels;
using Bloggie.Web.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bloggie.Web.Controllers;

[Authorize(Roles = "Admin")]
public class AdminTagsController : Controller
{
    private readonly ITagRepository _tagRepository;

    public AdminTagsController(ITagRepository tagRepository)
    {
        _tagRepository = tagRepository;
    }

 
    [HttpGet]
    public IActionResult Add()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Add(AddTagRequest addTagRequest)
    {
        ValidateAddTagRequest(addTagRequest);

        if(ModelState.IsValid==false)
        {
            return View();
        }

        // Mapping AddTagRequest to TAg domain model
        var tag = new Tag
        {
            Name = addTagRequest.Name,
            DisplayName = addTagRequest.DisplayName

        };

        await _tagRepository.AddAsync(tag);

        return RedirectToAction("List");
    }

    

    [HttpGet]
    public async Task<IActionResult> List(string? searchQuery, string? sortBy, string? sortDirection, int pageSize=3, int pageNumber=1 )
    {

        var totalRecords = await _tagRepository.CountAsync();

        var totalPages = Math.Ceiling((decimal)totalRecords / pageSize);


        if(pageNumber>totalPages)
        {
            pageNumber--;
        }


        if (pageNumber < 1)
        {
            pageNumber++;
        }

        ViewBag.TotalPages = totalPages;

        ViewBag.SearchQuery = searchQuery;
        ViewBag.SortBy = sortBy;
        ViewBag.SortDirection = sortDirection;

        ViewBag.PageSize = pageSize;
        ViewBag.PageNumber = pageNumber;

        var tags = await _tagRepository.GetAllAsync(searchQuery, sortBy, sortDirection, pageNumber, pageSize);

        return View(tags);
    }



    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var tagfromDb = await _tagRepository.GetAsync(id);


        if (tagfromDb != null)
        {
            var editTagRequest = new EditTagRequest
            {
                Id = tagfromDb.Id,
                Name = tagfromDb.Name,
                DisplayName = tagfromDb.DisplayName
            };

            return View(editTagRequest);
        }

        return View(null);

    }


    [HttpPost]
    public async Task<IActionResult> Edit(EditTagRequest editTagRequest)
    {

        var tag = new Tag
        {
            Id = editTagRequest.Id,
            Name = editTagRequest.Name,
            DisplayName = editTagRequest.DisplayName
        };

        var updatedTag = await _tagRepository.UpdateAsync(tag);

        if (updatedTag != null)
        {
            return RedirectToAction("List");

        }

        else
        {
            return RedirectToAction("Edit", new { id = editTagRequest.Id });

        }

    }

    [HttpPost]
    public async Task<IActionResult> Delete(EditTagRequest editTagRequest)
    {

        var deletedTag =  await _tagRepository.DeleteAsync(editTagRequest.Id);

        if(deletedTag!=null)
        {
           return RedirectToAction("List");
        }

        return RedirectToAction("Edit", new { id = editTagRequest.Id });
    }

    private void ValidateAddTagRequest(AddTagRequest request)
    {
       if (request.Name is not null && request.DisplayName is not null)
        {
            if(request.Name==request.DisplayName)
            {
                ModelState.AddModelError("DisplayName", "Name cannot be the same as DisplayName");
            }
        }
    }
}
