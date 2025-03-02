using BlazingQuiz.Api.Data.Entities;
using BlazingQuiz.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace BlazingQuiz.Api.Data;

public class QuizContext : DbContext
{
    private readonly IPasswordHasher<User> _passwordHasher;

    public QuizContext(DbContextOptions<QuizContext> options , IPasswordHasher<User> passwordHasher) : base(options)
    {
        _passwordHasher = passwordHasher;
    }


    public DbSet<Category> Categories { get; set; }
    public DbSet<Option> Options { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<Quiz> Quizzes { get; set; }
    public DbSet<StudentQuiz> StudentQuizzes { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<StudentQuizQuestion> StudentQuizQuestions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.ConfigureWarnings(warnings =>
            warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
    
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StudentQuizQuestion>()
            .HasKey(s => new { s.StudentQuizId, s.QuestionId });

        base.OnModelCreating(modelBuilder);
        var adminUser = new User
        {
            Id = 1,
            Name = "Iman Sabet",
            Email = "admin@gmail.com",
            Phone = "1230",
            Role = nameof(UserRole.Admin),
            IsApproved = true
           
        };
        adminUser.HashedPassword = _passwordHasher.HashPassword(adminUser, "Aa123456");

        modelBuilder.Entity<User>()
            .HasData(adminUser);


        //modelBuilder.Entity<User>().ToTable("Users");

    }

}
