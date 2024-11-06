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

    public async Task<IEnumerable<Tag>> GetAllAsync(string? searchQuery, string? sortBy = null, string? sortDirection = null)
    {
        var query = _bloggieDbContext.Tags.AsQueryable();

        // Filtering
        if (string.IsNullOrWhiteSpace(searchQuery) == false)
        {
            query = query.Where(x => x.Name.Contains(searchQuery) || 
                                     x.DisplayName.Contains(searchQuery));
        }

        //Sorting
        if (string.IsNullOrWhiteSpace(sortBy)==false)
        {
            var isDesc = string.Equals(sortDirection, "Desc", StringComparison.OrdinalIgnoreCase);

            if(string.Equals(sortBy, "Name", StringComparison.OrdinalIgnoreCase))
            {
            query = isDesc ? query.OrderByDescending(x => x.Name) : query.OrderBy(x=>x.Name);
            }

            if (string.Equals(sortBy, "DisplayName", StringComparison.OrdinalIgnoreCase))
            {
                query = isDesc ? query.OrderByDescending(x => x.DisplayName) : query.OrderBy(x => x.DisplayName);
            }
        }

        //Pagination

        return await query.ToListAsync();

       // return await _bloggieDbContext.Tags.ToListAsync();
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
