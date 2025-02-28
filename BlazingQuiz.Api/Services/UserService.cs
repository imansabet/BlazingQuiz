using BlazingQuiz.Api.Data;
using BlazingQuiz.Shared;
using BlazingQuiz.Shared.DTOs;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BlazingQuiz.Api.Services;

public class UserService
{
    private readonly QuizContext _context;

    public UserService(QuizContext context)
    {
        _context = context;
    }
    public async Task<PagedResult<UserDto>> GetUsersAsync(UserApprovalFilter userApprovalFilter  , int startIndex, int pageSize) 
    {
        var query = _context.Users.Where(u => u.Role != nameof(UserRole.Admin)).AsQueryable();
        if(userApprovalFilter != UserApprovalFilter.All) 
        {
            if (userApprovalFilter == UserApprovalFilter.ApprovedOnly)
                query = query.Where(u => u.IsApproved);

            else
                query = query.Where(u => !u.IsApproved);
        }
        var total = await query.CountAsync();

        var users = await query.OrderByDescending(u => u.Id)
            .Skip(startIndex)
            .Take(pageSize)
            .Select(u => new UserDto(u.Id,u.Name,u.Email,u.Phone,u.IsApproved))
            .ToArrayAsync();

        return new PagedResult<UserDto>(users, total);
    }

    public async Task ToggleUserApprovalStatus(int userId) 
    {
        var dbuser = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (dbuser != null) 
        {
            dbuser.IsApproved = !dbuser.IsApproved;
            await _context.SaveChangesAsync();
        }
    }


}
