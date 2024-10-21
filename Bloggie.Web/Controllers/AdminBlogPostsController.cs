using Bloggie.Web.Models.Domain;
using Bloggie.Web.Models.ViewModels;
using Bloggie.Web.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Bloggie.Web.Controllers;

public class AdminBlogPostsController : Controller
{
    private readonly ITagRepository _tagRepository;
    private readonly IBlogPostRepository _blogPostRepository;

    public AdminBlogPostsController(ITagRepository tagRepository, IBlogPostRepository blogPostRepository)
    {
        _tagRepository = tagRepository;
        _blogPostRepository = blogPostRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Add()
    {
        //get tags from repository

        var tags = await _tagRepository.GetAllAsync();

        var model = new AddBlogPostRequest

        {
            Tags = tags.Select(t => new SelectListItem { Text = t.Name, Value = t.Id.ToString() })
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Add(AddBlogPostRequest addBlogPostRequest)

    {
        //Map view model to domain model

        var blogPost = new BlogPost
        {
            Heading = addBlogPostRequest.Heading,
            PageTitle = addBlogPostRequest.PageTitle,
            Content = addBlogPostRequest.Content,
            ShortDescription = addBlogPostRequest.ShortDescription,
            FeaturedImageUrl = addBlogPostRequest.FeaturedImageUrl,
            UrlHandle = addBlogPostRequest.UrlHandle,
            PublishedDate = addBlogPostRequest.PublishedDate,
            Author = addBlogPostRequest.Author,
            Visible = addBlogPostRequest.Visible
        };

        // Maps Tags from selected tags
        var selectedTags = new List<Tag>();

        foreach (var selectedTagId in addBlogPostRequest.SelectedTags)
        {
            var selectedTagIdAsGuid = Guid.Parse(selectedTagId);
            var existingTag = await _tagRepository.GetAsync(selectedTagIdAsGuid);

            if (existingTag != null)
            {
                selectedTags.Add(existingTag);
            }
            // Mapping tags back to domain model

        }
        blogPost.Tags = selectedTags;

        await _blogPostRepository.AddAsync(blogPost);
        return RedirectToAction("Add");

    }

    [HttpGet]
    public async Task<IActionResult> List()
    {
        // Call the repository
        var blogPosts = await _blogPostRepository.GetAllAsync();

        return View(blogPosts);
    }


    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var blogPostfromDb = await _blogPostRepository.GetAsync(id);
        var tags = await _tagRepository.GetAllAsync();

        if (blogPostfromDb != null)
        {
            var editBlogPostRequest = new EditBlogPostRequest
            {
                Id = blogPostfromDb.Id,
                Heading = blogPostfromDb.Heading,
                PageTitle = blogPostfromDb.PageTitle,
                Content = blogPostfromDb.Content,
                ShortDescription = blogPostfromDb.ShortDescription,
                FeaturedImageUrl = blogPostfromDb.FeaturedImageUrl,
                UrlHandle = blogPostfromDb.UrlHandle,
                PublishedDate = blogPostfromDb.PublishedDate,
                Author = blogPostfromDb.Author,
                Visible = blogPostfromDb.Visible,
                Tags = tags.Select(t => new SelectListItem { Text = t.Name, Value = t.Id.ToString() }),
                SelectedTags = blogPostfromDb.Tags.Select(t => t.Id.ToString()).ToArray()
            };


            return View(editBlogPostRequest);
        }

        return View(null);

    }


    [HttpPost]
    public async Task<IActionResult> Edit(EditBlogPostRequest editBlogPostRequest)
    {
        // map view model back to domain model
        var blogPost = new BlogPost
        {
            Id = editBlogPostRequest.Id,
            Heading = editBlogPostRequest.Heading,
            PageTitle = editBlogPostRequest.PageTitle,
            Content = editBlogPostRequest.Content,
            ShortDescription = editBlogPostRequest.ShortDescription,
            FeaturedImageUrl = editBlogPostRequest.FeaturedImageUrl,
            UrlHandle = editBlogPostRequest.UrlHandle,
            PublishedDate = editBlogPostRequest.PublishedDate,
            Author = editBlogPostRequest.Author,
            Visible = editBlogPostRequest.Visible
        };

        // Map tags into domain model

        var selectedTags = new List<Tag>();
        foreach (var selectedTag in editBlogPostRequest.SelectedTags)
        {
            if (Guid.TryParse(selectedTag, out var tag))
            {
                var foundTag = await _tagRepository.GetAsync(tag);

                if (foundTag != null)
                {
                    selectedTags.Add(foundTag);
                }
            }
        }


        blogPost.Tags = selectedTags;

        //Submit information to repository to update
        var updatedBlogPost = await _blogPostRepository.UpdateAsync(blogPost);

        if (updatedBlogPost != null)
        {
            return RedirectToAction("List");
        }

        return RedirectToAction("Edit");

    }

}
