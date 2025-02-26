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
    public List<QuestionDto> Questions { get; set; } = [];

    public  string? Validate()
    {
        if (TotalQuestion != Questions.Count) 
           return ("Number Of Question Does not Match With Total Questions !");


        if (Questions.Any(q => string.IsNullOrWhiteSpace(q.Text)))
            return "Question Text Is Required !";


        if (Questions.Any(q => q.Options.Count < 2))
            return  "Options Are Required : At Least 2 Options ! ";


        if (Questions.Any(q => !q.Options.Any(o => o.IsCorrect)))
            return "You Shoul Define Correct Answer ! ";

        if (Questions.Any(q => q.Options.Any(o => string.IsNullOrWhiteSpace(o.Text))))
            return "All Options Must Have Text!";

        return null;


    }


}
