﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazingQuiz.Api.Data.Entities;

public class Quiz
{
    public Guid Id { get; set; }
    [MaxLength(100)]
    public string Name { get; set; }
    public int CategoryId { get; set; }
    public int TotalQuestion { get; set; }
    public int TimeInMinutes { get; set; }
    public bool IsActive { get; set; }


    [ForeignKey(nameof(CategoryId))]
    public virtual Category Category { get; set; }

    public ICollection<Question> Questions { get; set; } = [];

}
