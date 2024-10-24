using Bloggie.Web.Models.Domain;
using CloudinaryDotNet.Actions;

namespace Bloggie.Web.Models.ViewModels;

public class HomeViewModel
{
    public IEnumerable<BlogPost> BlogPosts { get; set; }
    public IEnumerable<Tag> Tags { get; set; }
}
