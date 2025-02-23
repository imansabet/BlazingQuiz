using BlazingQuiz.Api.Services;
using BlazingQuiz.Shared;
using BlazingQuiz.Shared.DTOs;

namespace BlazingQuiz.Api.Endpoints;

public static class CategoryEndpoints
{
    public static IEndpointRouteBuilder MapCategoryEndpoints(this IEndpointRouteBuilder app)
    {
        var categoryGroup = app.MapGroup("/api/categories").RequireAuthorization();

        categoryGroup.MapGet("", async (CategoryService categoryService) =>
            Results.Ok(await categoryService.GetCategoriesAsync())
        );

        categoryGroup.MapPost("", async (CategoryDto categoryDto, CategoryService categoryService) =>

            Results.Ok(await categoryService.SaveCategoryAsync(categoryDto))
        ).RequireAuthorization(p => p.RequireRole(nameof(UserRole.Admin)));

        return app;
    }
}
