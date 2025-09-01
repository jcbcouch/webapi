using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace dotnetwebapi.Models;

public class ApplicationUser : IdentityUser
{
    // Add custom properties here if needed
    public string? DisplayName { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation property for one-to-many relationship with Posts
    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
}
