using Bloggie.Web.Data;
using Bloggie.Web.Models.Domain;

namespace Bloggie.Web.Repositories;

public class BlogPostCommentRepository : IBlogPostCommentRepository
{
    private readonly BloggieDbContext _bloggieDbContext;

    public BlogPostCommentRepository(BloggieDbContext bloggieDbContext)
    {
       _bloggieDbContext = bloggieDbContext;
    }

    public async Task<BlogPostComment> AddAsync(BlogPostComment blogPostComment)
    {
        await _bloggieDbContext.BlogPostComments.AddAsync(blogPostComment);
        await _bloggieDbContext.SaveChangesAsync();

        return blogPostComment;
    }
}
