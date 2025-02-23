using BlazingQuiz.Api.Data;
using BlazingQuiz.Api.Data.Entities;
using BlazingQuiz.Shared.DTOs;
using Microsoft.EntityFrameworkCore;

namespace BlazingQuiz.Api.Services;

public class CategoryService
{
    private readonly QuizContext _context;

    public CategoryService(QuizContext context)
    {
        _context = context;
    }

    public async Task<QuizApiResponse> SaveCategoryAsync(CategoryDto categoryDto)
    {
        
        if(await _context.Categories
            .AsNoTracking()
            .AnyAsync(c => c.Name == categoryDto.Name && c.Id != categoryDto.Id))
        {
            // duplicated Category
            return QuizApiResponse.Fail("This Category Already Exists. ");
        }
        // create new category
        if (categoryDto.Id == 0)
        {
            var category = new Category
            {
                Name = categoryDto.Name
            };
            _context.Categories.Add(category);
        }
        // updating existing category
        else
        {
            var existingCategory = await _context.Categories.FirstOrDefaultAsync(c => c.Id == categoryDto.Id);
            if (existingCategory == null)
            {
                return QuizApiResponse.Fail("Category doesn't Exist. ");
            }
            existingCategory.Name = categoryDto.Name;
            _context.Categories.Update(existingCategory);
        }
        await _context.SaveChangesAsync();
        return QuizApiResponse.Success();
    }

    public async Task<CategoryDto[]> GetCategoriesAsync() =>
        await _context.Categories.AsNoTracking()
        .Select(c => new CategoryDto
        {
            Name = c.Name,
            Id = c.Id
        }).ToArrayAsync();

}
