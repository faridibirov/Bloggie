using Bloggie.Web.Data;
using Bloggie.Web.Models.Domain;
using Bloggie.Web.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Bloggie.Web.Controllers;

public class AdminTagsController : Controller
{
    private readonly BloggieDbContext _bloggieDbContext;

    public AdminTagsController(BloggieDbContext bloggieDbContext)
    {
        _bloggieDbContext = bloggieDbContext;
    }

    [HttpGet]
    public IActionResult Add()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Add(AddTagRequest addTagRequest)
    {

        // Mapping AddTagRequest to TAg domain model
        var tag = new Tag
        {
            Name = addTagRequest.Name,
            DisplayName = addTagRequest.DisplayName

        };

        _bloggieDbContext.Tags.Add(tag);
        _bloggieDbContext.SaveChanges();

        return RedirectToAction("List");
    }

    [HttpGet]
    public IActionResult List()
    {
        var tags = _bloggieDbContext.Tags.ToList();

        return View(tags);
    }



    [HttpGet]
    public IActionResult Edit(Guid id)
    {
        var tagfromDb = _bloggieDbContext.Tags.FirstOrDefault(t => t.Id == id);


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
    public IActionResult Edit(EditTagRequest editTagRequest)
    {

        var tag = new Tag
        {
            Id = editTagRequest.Id,
            Name = editTagRequest.Name,
            DisplayName = editTagRequest.DisplayName
        };

        var existingTag = _bloggieDbContext.Tags.Find(tag.Id);

        if (existingTag != null)
        {
            existingTag.Name = tag.Name;
            existingTag.DisplayName = tag.DisplayName;

            _bloggieDbContext.SaveChanges();

            return RedirectToAction("List");
        }

        return RedirectToAction("Edit", new { id = editTagRequest.Id });
    }

    [HttpPost]
    public IActionResult Delete(EditTagRequest editTagRequest)
    {
        var tagFromDb = _bloggieDbContext.Tags.Find(editTagRequest.Id);

        if (tagFromDb != null)
        {
            _bloggieDbContext.Tags.Remove(tagFromDb);
            _bloggieDbContext.SaveChanges();

            return RedirectToAction("List");
        }

        return RedirectToAction("Edit", new { id = editTagRequest.Id });
    }
}
