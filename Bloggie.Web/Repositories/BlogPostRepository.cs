using Bloggie.Web.Data;
using Bloggie.Web.Models.Domain;
using Bloggie.Web.Models.ViewModels;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;

namespace Bloggie.Web.Repositories;

public class BlogPostRepository : IBlogPostRepository
{
    private readonly BloggieDbContext _bloggieDbContext;

    public BlogPostRepository(BloggieDbContext bloggieDbContext)
    {
        _bloggieDbContext = bloggieDbContext;
    }

    public async Task<BlogPost> AddAsync(BlogPost blogPost)
    {
        await _bloggieDbContext.AddAsync(blogPost);

        await _bloggieDbContext.SaveChangesAsync();

        return blogPost;
    }

    public Task<BlogPost?> DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<BlogPost>> GetAllAsync()
    {
        return await _bloggieDbContext.BlogPosts.Include(t=>t.Tags).ToListAsync();
    }

    public async Task<BlogPost?> GetAsync(Guid id)
    {
       return await _bloggieDbContext.BlogPosts.Include(t=>t.Tags).FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<BlogPost?> UpdateAsync(BlogPost blogPost)
    {
        var blogfromDb = await _bloggieDbContext.BlogPosts.Include(t=>t.Tags).FirstOrDefaultAsync(b => b.Id == blogPost.Id);

        if (blogfromDb!=null)
        {
            blogfromDb.Id = blogPost.Id;
            blogfromDb.Heading = blogPost.Heading;
            blogfromDb.PageTitle = blogPost.PageTitle;
            blogfromDb.Content = blogPost.Content;
            blogfromDb.ShortDescription = blogPost.ShortDescription;
            blogfromDb.FeaturedImageUrl = blogPost.FeaturedImageUrl;
            blogfromDb.UrlHandle = blogPost.UrlHandle;
            blogfromDb.PublishedDate = blogPost.PublishedDate;
            blogfromDb.Author = blogPost.Author;
            blogfromDb.Visible = blogPost.Visible;

            await _bloggieDbContext.SaveChangesAsync();
            return blogfromDb;
        }
        return null;
    }
}
