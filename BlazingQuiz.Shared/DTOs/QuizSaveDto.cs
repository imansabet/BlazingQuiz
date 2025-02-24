using System.ComponentModel.DataAnnotations;

namespace BlazingQuiz.Shared.DTOs;

public  class SaveQuizDto
{
    public Guid Id { get; set; }
    [Required,MaxLength(100)]
    public string Name { get; set; }
    [Range(1,int.MaxValue,ErrorMessage ="Category Is Required . ")]
    public int CategoryId { get; set; }
    [Range(1, int.MaxValue, ErrorMessage = "Question Numbers Must Greater Than 0!  ")]
    public int TotalQuestion { get; set; }
    [Range(1,120, ErrorMessage = "Provide Valid Time in Minutes .  ")]
    public int TimeInMinutes { get; set; }
    public bool IsActive { get; set; }
    public List<QuestionDto> Questions { get; set; }
}
public class OptionDto 
{
    public int Id { get; set; }
    [Required,MaxLength(200)]
    public string Text { get; set; }
    public bool IsCorrect { get; set; }
}
public class QuestionDto
{
    public int Id { get; set; }
    [Required,MaxLength(200)]
    public string Text { get; set; }
    public List<OptionDto> Options { get; set; }
}