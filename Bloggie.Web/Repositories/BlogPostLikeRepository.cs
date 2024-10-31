
using Bloggie.Web.Data;
using Bloggie.Web.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace Bloggie.Web.Repositories;

public class BlogPostLikeRepository : IBlogPostLikeRepository
{
    private readonly BloggieDbContext _bloggieDbContext;

    public BlogPostLikeRepository(BloggieDbContext bloggieDbContext)
    {
        _bloggieDbContext = bloggieDbContext;
    }

    public async Task<BlogPostLike> AddLikeForBlog(BlogPostLike blogPostLike)
    {
        await _bloggieDbContext.BlogPostLikes.AddAsync(blogPostLike);

        await _bloggieDbContext.SaveChangesAsync();
        return blogPostLike;
    }

    public async Task<IEnumerable<BlogPostLike>> GetLikesForBlog(Guid blogPostId)
    {
       return await _bloggieDbContext.BlogPostLikes.Where(x=>x.BlogPostId == blogPostId).ToListAsync();   
    }

    public async Task<int> GetTotalLikes(Guid blogPostId)
    {
      return  await _bloggieDbContext.BlogPostLikes.CountAsync(x=>x.BlogPostId==blogPostId);
    }
}
