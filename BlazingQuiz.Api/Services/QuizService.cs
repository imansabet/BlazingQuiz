using BlazingQuiz.Api.Data;
using BlazingQuiz.Api.Data.Entities;
using BlazingQuiz.Shared.DTOs;
using Microsoft.EntityFrameworkCore;

namespace BlazingQuiz.Api.Services;

public class QuizService
{
    private readonly QuizContext _context;

    public QuizService(QuizContext context)
    {
        _context = context;
    }
    public async Task<QuizApiResponse> SaveQuizAsync(SaveQuizDto saveQuizDto)
    {
        var questions = saveQuizDto.Questions.Select(q => new Question
        {
            Id = q.Id,
            Text = q.Text,
            Options = q.Options.Select(o => new Option
            {
                Text = o.Text,
                IsCorrect = o.IsCorrect
            }).ToArray()
        }).ToArray();

        if (saveQuizDto.Id == Guid.Empty)
        {
            // Create New Quiz        
            var quiz = new Quiz
            {
                CategoryId = saveQuizDto.CategoryId,
                IsActive = saveQuizDto.IsActive,
                Name = saveQuizDto.Name,
                TimeInMinutes = saveQuizDto.TimeInMinutes,
                TotalQuestion = saveQuizDto.TotalQuestion,
                Questions = questions
            };
            _context.Quizzes.Add(quiz);
        }
        else
        {
            //Update Existing Quiz
            var existingQuiz = await _context.Quizzes
                .FirstOrDefaultAsync(q => q.Id == saveQuizDto.Id);

            if (existingQuiz == null) 
            {
                return QuizApiResponse.Fail("Quiz Not Found !");
            }
            existingQuiz.CategoryId = saveQuizDto.CategoryId;
            existingQuiz.IsActive = saveQuizDto.IsActive;
            existingQuiz.Name = saveQuizDto.Name;
            existingQuiz.TotalQuestion  = saveQuizDto.TotalQuestion;
            existingQuiz.TimeInMinutes = saveQuizDto.TimeInMinutes;
            existingQuiz.Questions = questions;

        }
        try 
        {
            await _context.SaveChangesAsync();
            return QuizApiResponse.Success();
        }
        catch (Exception ex) 
        {
            return QuizApiResponse.Fail(ex.Message);
        }
    }

}