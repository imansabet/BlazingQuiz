﻿using BlazingQuiz.Shared.DTOs;
using Refit;

namespace BlazingQuiz.Web.Apis;


[Headers("Authorization: Bearer ")]
public interface IQuizApi
{
    [Post("/api/quizes")]
    Task<QuizApiResponse> SaveQuizAsync(SaveQuizDto saveQuizDto);

    [Get("/api/quizes")]
    Task<QuizListDto[]> GetQuizesAsync();

    [Get("/api/quizes/{quizId}/questions")]
    Task<QuestionDto[]> GetQuizQuestionsAsync(Guid quizId);

    [Get("/api/quizes/{quizId}")]
    Task<SaveQuizDto?> GetQuizToEditAsync(Guid quizId);

}
