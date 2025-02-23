using System.ComponentModel.DataAnnotations;

namespace BlazingQuiz.Shared.DTOs;

public class CategoryDto
{
    public int Id { get; set; }
    [Required,MaxLength(50)]
    public string Name { get; set; }

}
