using BlazingQuiz.Shared;
using BlazingQuiz.Shared.DTOs;
using Refit;

namespace BlazingQuiz.Web.Apis;

[Headers("Authorization: Bearer ")]
public interface IUserApi 
{
    [Get("/api/users")]
    Task<PagedResult<UserDto>> GetUsersAsync(UserApprovalFilter userApprovalFilter,int startIndex, int pageSize);
    [Patch("/api/users/{userId}/toggle-status")]
    Task ToggleUserApprovalStatus(int userId);
}