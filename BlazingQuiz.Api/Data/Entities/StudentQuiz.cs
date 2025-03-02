using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazingQuiz.Api.Data.Entities;


// many to many  :   user <=> quiz
public class StudentQuiz
{
    [Key]
    public int Id { get; set; }
    public int StudentId { get; set; }
    public Guid QuizId { get; set; }
    public DateTime StartedOn { get; set; }
    public DateTime? CompletedOn { get; set; }
    public int Score { get; set; }


    [AllowedValues(nameof(StudentQuizStatus.Started)
        , nameof(StudentQuizStatus.Completed)
        , nameof(StudentQuizStatus.Exited)
        , nameof(StudentQuizStatus.AutoSubmitted))]
    public string Status { get; set; } = nameof(StudentQuizStatus.Started);


    [ForeignKey(nameof(StudentId))]
    public virtual User Student { get; set; }

    [ForeignKey(nameof(QuizId))]
    public virtual Quiz Quiz { get; set; }


    public virtual ICollection<StudentQuizQuestion> StudentQuizQuestions { get; set; } = [];

}
