using System.ComponentModel.DataAnnotations;

namespace dotnetwebapi.DTOs.Posts;

public class CreatePostDto
{
    [Required]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 200 characters")]
    public string Title { get; set; } = string.Empty;
    
    [Required]
    [StringLength(5000, MinimumLength = 10, ErrorMessage = "Content must be between 10 and 5000 characters")]
    public string Content { get; set; } = string.Empty;
}
