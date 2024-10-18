using Bloggie.Web.Data;
using Bloggie.Web.Models.Domain;
using Bloggie.Web.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Bloggie.Web.Repositories;

public class TagRepository : ITagRepository
{

    private readonly BloggieDbContext _bloggieDbContext;

    public TagRepository(BloggieDbContext bloggieDbContext)
    {
        _bloggieDbContext = bloggieDbContext;
    }

    public async Task<Tag> AddAsync(Tag tag)
    {
        await _bloggieDbContext.Tags.AddAsync(tag);
        await _bloggieDbContext.SaveChangesAsync();

        return tag;
    }

    public async Task<Tag?> DeleteAsync(Guid id)
    {
        var tagFromDb = await _bloggieDbContext.Tags.FindAsync(id);

        if (tagFromDb != null)
        {
            _bloggieDbContext.Tags.Remove(tagFromDb);
            await _bloggieDbContext.SaveChangesAsync();

            return tagFromDb;
        }

        return null;
    }

    public async Task<IEnumerable<Tag>> GetAllAsync()
    {
        return await _bloggieDbContext.Tags.ToListAsync();
    }

    public async Task<Tag?> GetAsync(Guid id)
    {
        return await _bloggieDbContext.Tags.FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<Tag?> UpdateAsync(Tag tag)
    {
        var existingTag = await _bloggieDbContext.Tags.FindAsync(tag.Id);

        if(existingTag!=null)
        {
            existingTag.Name = tag.Name;
            existingTag.DisplayName = tag.DisplayName;

            await _bloggieDbContext.SaveChangesAsync();

            return existingTag;
        }

        return null;
    }
}
