using BlazingQuiz.Api.Services;
using BlazingQuiz.Shared.DTOs;
using BlazingQuiz.Shared;

namespace BlazingQuiz.Api.Endpoints;

public static class QuizEndpoints
{
    public static IEndpointRouteBuilder MapQuizEndpoints(this IEndpointRouteBuilder app)
    {
        var quizGroup = app.MapGroup("/api/quizes").RequireAuthorization();

        quizGroup.MapPost("", async (SaveQuizDto saveQuizDto, QuizService quizService) =>
        {
            if (saveQuizDto.Questions.Count == 0)
                return Results.BadRequest("Please Provide Questions");

            if (saveQuizDto.Questions.Count != saveQuizDto.TotalQuestion)
                return Results.BadRequest("Total Questions Does'nt match with Provided Questions");


             return Results.Ok(await quizService.SaveQuizAsync(saveQuizDto));
        });

        quizGroup.MapGet("", async (QuizService service) =>
                Results.Ok(await service.GetQuizesAsync()));

        quizGroup.MapGet("{quizId:guid}/questions", async (Guid quizId , QuizService service) =>
        {
            return Results.Ok(await service.GetQuizQuestionsAsync(quizId));
        });


        return app;
    }
}
