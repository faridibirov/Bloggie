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

        return View("Add");
    }
}
