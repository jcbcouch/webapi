namespace dotnetwebapi.DTOs.Posts;

public class PostResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string? UserDisplayName { get; set; }
}
