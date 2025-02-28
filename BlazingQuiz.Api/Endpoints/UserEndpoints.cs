using BlazingQuiz.Api.Services;
using BlazingQuiz.Shared;
using BlazingQuiz.Shared.DTOs;
using Microsoft.AspNetCore.Identity.Data;

namespace BlazingQuiz.Api.Endpoints;

public static class UserEndpoints 
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app) 
    {
        var userGroup = app.MapGroup("/api/users").RequireAuthorization(p => p.RequireRole(nameof(UserRole.Admin)));

        userGroup.MapGet("", async (UserApprovalFilter userApprovalFilter, int startIndex,int pageSize , UserService userService )    =>
        {
            //var approvalFilter = Enum.Parse<UserApprovalFilter>(filter);
           return  Results.Ok(await userService.GetUsersAsync(userApprovalFilter, startIndex, pageSize));
        });

        userGroup.MapPatch("{userId:int}/toggle-status", async (int userId, UserService service) => 
        {
            await service.ToggleUserApprovalStatus(userId);
            return Results.Ok();
        });

            return app;
    }

}