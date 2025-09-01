using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using dotnetwebapi.Data;
using dotnetwebapi.DTOs.Posts;
using dotnetwebapi.Models;

namespace dotnetwebapi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PostsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public PostsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET: api/posts
    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PostResponseDto>>> GetPosts()
    {
        return await _context.Posts
            .Include(p => p.User)
            .Select(p => new PostResponseDto
            {
                Id = p.Id,
                Title = p.Title,
                Content = p.Content,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                UserId = p.UserId,
                UserDisplayName = p.User.DisplayName ?? p.User.UserName!
            })
            .ToListAsync();
    }

    // GET: api/posts/5
    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<ActionResult<PostResponseDto>> GetPost(int id)
    {
        var post = await _context.Posts
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (post == null)
        {
            return NotFound();
        }

        return new PostResponseDto
        {
            Id = post.Id,
            Title = post.Title,
            Content = post.Content,
            CreatedAt = post.CreatedAt,
            UpdatedAt = post.UpdatedAt,
            UserId = post.UserId,
            UserDisplayName = post.User.DisplayName ?? post.User.UserName!
        };
    }

    // POST: api/posts
    [HttpPost]
    public async Task<ActionResult<PostResponseDto>> CreatePost(CreatePostDto createPostDto)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        var post = new Post
        {
            Title = createPostDto.Title,
            Content = createPostDto.Content,
            UserId = user.Id,
            CreatedAt = DateTime.UtcNow
        };

        _context.Posts.Add(post);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetPost), new { id = post.Id }, new PostResponseDto
        {
            Id = post.Id,
            Title = post.Title,
            Content = post.Content,
            CreatedAt = post.CreatedAt,
            UserId = post.UserId,
            UserDisplayName = user.DisplayName ?? user.UserName!
        });
    }

    // PUT: api/posts/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePost(int id, CreatePostDto updatePostDto)
    {
        var post = await _context.Posts.FindAsync(id);
        if (post == null)
        {
            return NotFound();
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null || post.UserId != user.Id)
        {
            return Forbid();
        }

        post.Title = updatePostDto.Title;
        post.Content = updatePostDto.Content;
        post.UpdatedAt = DateTime.UtcNow;

        _context.Entry(post).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!PostExists(id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    // DELETE: api/posts/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePost(int id)
    {
        var post = await _context.Posts.FindAsync(id);
        if (post == null)
        {
            return NotFound();
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null || (post.UserId != user.Id && !await _userManager.IsInRoleAsync(user, "Admin")))
        {
            return Forbid();
        }

        _context.Posts.Remove(post);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool PostExists(int id)
    {
        return _context.Posts.Any(e => e.Id == id);
    }
}
