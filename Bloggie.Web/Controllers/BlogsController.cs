using Bloggie.Web.Models.Domain;
using Bloggie.Web.Models.ViewModels;
using Bloggie.Web.Repositories;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Bloggie.Web.Controllers;

public class BlogsController : Controller
{
    private readonly IBlogPostRepository _blogPostRepository;
    private readonly IBlogPostLikeRepository _blogPostLikeRepository;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IBlogPostCommentRepository _blogPostCommentRepository;

    public BlogsController(IBlogPostRepository blogPostRepository, IBlogPostLikeRepository blogPostLikeRepository,
        SignInManager<IdentityUser> signInManager,
        UserManager<IdentityUser> userManager,
        IBlogPostCommentRepository blogPostCommentRepository )
    {
        _blogPostRepository = blogPostRepository;
        _blogPostLikeRepository = blogPostLikeRepository;
        _signInManager = signInManager;
        _userManager = userManager;
        _blogPostCommentRepository = blogPostCommentRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string urlHandle)
    {
        var liked = false;

        var blogPost = await _blogPostRepository.GetByUrlHandleAsync(urlHandle);

        var blogDetailsViewModel = new BlogDetailsViewModel();

        if (blogPost != null)
        {
            var totalLikes = await _blogPostLikeRepository.GetTotalLikes(blogPost.Id);

            if(_signInManager.IsSignedIn(User))
            {
                // Get Like for this blog for this user

                var likesForBlog =  await _blogPostLikeRepository.GetLikesForBlog(blogPost.Id);

                var userId = _userManager.GetUserId(User);

                if (userId!=null)
                {
                    var likeFromUser = likesForBlog.FirstOrDefault(l => l.UserId == Guid.Parse(userId));

                    liked = likeFromUser != null;

                }
            }

            // Get commetns for blog post

            var blogCommentsDomainModel = await _blogPostCommentRepository.GetCommentByBlogIdAsync(blogPost.Id);

            var blogCommentsForView = new List<BlogComment>();
            
            foreach (var blogComment in blogCommentsDomainModel) 
            {
                blogCommentsForView.Add(new BlogComment
                {
                    Description = blogComment.Description,
                    DateAdded = blogComment.DateAdded,
                    Username = (await _userManager.FindByIdAsync(blogComment.UserId.ToString())).UserName
                });
            }


             blogDetailsViewModel = new BlogDetailsViewModel
            {
                Id = blogPost.Id,
                Heading = blogPost.Heading,
                PageTitle = blogPost.PageTitle,
                Content = blogPost.Content,
                ShortDescription = blogPost.ShortDescription,
                FeaturedImageUrl = blogPost.FeaturedImageUrl,
                UrlHandle = blogPost.UrlHandle,
                PublishedDate = blogPost.PublishedDate,
                Author = blogPost.Author,
                Visible = blogPost.Visible,
                Tags = blogPost.Tags,
                TotalLikes = totalLikes,
                Liked = liked,
                Comments = blogCommentsForView  
              
            };
        }

        return View(blogDetailsViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Index(BlogDetailsViewModel blogDetailsViewModel)
    {
        if (_signInManager.IsSignedIn(User))
        {
            var domainmodel = new BlogPostComment
            {
                BlogPostId = blogDetailsViewModel.Id,
                Description = blogDetailsViewModel.CommentDescription,
                UserId = Guid.Parse(_userManager.GetUserId(User)),
                DateAdded = DateTime.Now
            };

            await _blogPostCommentRepository.AddAsync(domainmodel);

            return RedirectToAction("Index", "Blogs",
               new {urlHandle = blogDetailsViewModel.UrlHandle} );
        }

        return View();
       

    }
}
