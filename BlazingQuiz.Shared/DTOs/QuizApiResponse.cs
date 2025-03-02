namespace BlazingQuiz.Shared.DTOs;

public record QuizApiResponse(bool IsSuccess, string? ErrorMessage)
{
    public static QuizApiResponse Success() => new(true, null);
    public static QuizApiResponse Fail(string errorMessage) => new(false, errorMessage);
}


public record QuizApiResponse<TData>(TData Data,bool IsSuccess, string? ErrorMessage)
{
    public static QuizApiResponse<TData> Success(TData data ) => new(data,true, null);
    public static QuizApiResponse<TData> Fail(string errorMessage) => new(default,false, errorMessage);
}