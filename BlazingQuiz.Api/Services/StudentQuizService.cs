using BlazingQuiz.Api.Data;
using BlazingQuiz.Api.Data.Entities;
using BlazingQuiz.Shared.DTOs;
using Microsoft.EntityFrameworkCore;

namespace BlazingQuiz.Api.Services;

public class StudentQuizService
{
    private readonly QuizContext _context;

    public StudentQuizService(QuizContext context)
    {
        _context = context;
    }

    public async Task<QuizListDto[]> GetActiveQuizesAsync(int categoryId)
    {
        var query = _context.Quizzes.Where(q => q.IsActive);
        if (categoryId > 0)
        {
            query = query.Where(q => q.CategoryId == categoryId);
        }
        var quizes = await query.Select(q => new QuizListDto
        {
            CategoryId = q.CategoryId,
            CategoryName = q.Category.Name,
            Name = q.Category.Name,
            TimeInMinutes = q.TimeInMinutes,
            TotalQuestion = q.TotalQuestion,
            Id = q.Id,
        }).ToArrayAsync();

        return quizes;
    }

    public async Task<QuizApiResponse<int>> StartQuizAsync(int studentId,Guid quizId) 
    {
        try 
        {
            var studentQuiz = new StudentQuiz
            {
                QuizId = quizId,
                StudentId = studentId,
                Status = nameof(StudentQuizStatus.Started),
                StartedOn = DateTime.UtcNow,
            };
            _context.StudentQuizzes.Add(studentQuiz);
            await _context.SaveChangesAsync();

            return QuizApiResponse<int>.Success(studentQuiz.Id);
        }
        catch (Exception ex) 
        {
            return QuizApiResponse<int>.Fail(ex.Message);
        }
        
    }

    public async Task<QuizApiResponse<QuestionDto?>> GetNextQuestionForQuizAsync(int studentQuizId , int studentId) 
    {
        var studentQuiz = await _context.StudentQuizzes
            .Include(s => s.StudentQuizQuestions)
            .FirstOrDefaultAsync(s => s.Id == studentQuizId);

        if(studentQuiz == null) 
        {
            return QuizApiResponse<QuestionDto?>.Fail("Quiz Does'nt Exists .");
        }
        if(studentQuiz.StudentId != studentId) 
        {
            return QuizApiResponse<QuestionDto?>.Fail("Invalid Request .");

        }

        var questionsServed = studentQuiz.StudentQuizQuestions
                                .Select(s => s.QuestionId)
                                .ToArray();

        var nextQuestion = await _context.Questions
            .Where(q => q.QuizId == studentQuiz.QuizId)
            .Where(q => !questionsServed.Contains(q.Id))
            .OrderBy(q => Guid.NewGuid())
            .Take(1)
            .Select(q => new QuestionDto
            {
                Id = q.Id,
                Text = q.Text,
                Options = q.Options.Select(o => new OptionDto
                {
                    Id = o.Id,
                    Text = o.Text,
                }).ToList()
            })
            .FirstOrDefaultAsync();

        if (nextQuestion == null)
            return QuizApiResponse<QuestionDto?>.Fail("No More Questions Left For This Quiz.");

        try 
        {
            var studentQuizQuestion = new StudentQuizQuestion
            {
                StudentQuizId = studentQuizId,
                QuestionId = nextQuestion.Id,
            };
            _context.StudentQuizQuestions.Add(studentQuizQuestion);
            await _context.SaveChangesAsync();
            return QuizApiResponse<QuestionDto?>.Success(nextQuestion);

        }
        catch (Exception ex) 
        {
            return QuizApiResponse<QuestionDto?>.Fail(ex.Message);

        }

    }

    public async Task<QuizApiResponse> SaveQuestionResponseAsync(StudentQuizQuestionResponseDto quizResponseDto , int studentId) 
    {
        var studentQuiz = await _context.StudentQuizzes.AsTracking()
          .FirstOrDefaultAsync(s => s.Id == quizResponseDto.StudentQuizId);

        if (studentQuiz == null)
        {
            return QuizApiResponse.Fail("Quiz Does'nt Exists .");
        }
        if (studentQuiz.StudentId != studentId)
        {
            return QuizApiResponse.Fail("Invalid Request .");

        }
        var isSelectedOptionCorrect = await
                            _context
                            .Options.Where(o => o.QuestionId == quizResponseDto.QuestionId && o.Id == quizResponseDto.OptionId)
                            .Select(o => o.IsCorrect)
                            .FirstOrDefaultAsync();

        if (isSelectedOptionCorrect) 
        {
            studentQuiz.Score++;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) 
            {
                return QuizApiResponse.Fail(ex.Message);
            }   
        
        }
        return QuizApiResponse.Success();

    }

    public async Task<QuizApiResponse> SubmitQuizAsync(int studentQuizId , int studentId)
        =>  await CompleteQuizAsync(studentQuizId, DateTime.UtcNow, nameof(StudentQuizStatus.Completed),studentId);

    

    public async Task<QuizApiResponse> ExitQuizAsync(int studentQuizId , int studentId) 
        => await CompleteQuizAsync(studentQuizId, null, nameof(StudentQuizStatus.Exited), studentId);



    public async Task<QuizApiResponse> AutoSubmitQuizAsync(int studentQuizId , int studentId) 
        => await CompleteQuizAsync(studentQuizId, DateTime.UtcNow, nameof(StudentQuizStatus.AutoSubmitted), studentId);


    private async Task<QuizApiResponse> CompleteQuizAsync(int studentQuizId , DateTime? completedOn , string status , int studentId) 
    {
        var studentQuiz = await _context.StudentQuizzes.AsTracking()
              .FirstOrDefaultAsync(s => s.Id == studentQuizId);

        if (studentQuiz == null)
        {
            return QuizApiResponse.Fail("Quiz Does'nt Exists .");
        }
        if (studentQuiz.CompletedOn.HasValue || studentQuiz.Status != nameof(StudentQuizStatus.Exited))
        {
            return QuizApiResponse.Fail("Quiz already Completed ");
        }
        if (studentQuiz.StudentId != studentId)
        {
            return QuizApiResponse.Fail("Invalid Request .");

        }
        try
        {
            studentQuiz.CompletedOn = completedOn;
            studentQuiz.Status = status;

            var studentQuizQuestions = await
                _context.StudentQuizQuestions
                .Where(q => q.StudentQuizId == studentQuizId)
                .ToListAsync();

            _context.StudentQuizQuestions.RemoveRange(studentQuizQuestions);
            await _context.SaveChangesAsync();
            return QuizApiResponse.Success();

        }
        catch (Exception ex)
        {
            return QuizApiResponse.Fail(ex.Message);
        }
    }

}
