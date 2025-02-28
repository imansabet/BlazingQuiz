using BlazingQuiz.Api.Data;
using BlazingQuiz.Api.Data.Entities;
using BlazingQuiz.Shared;
using BlazingQuiz.Shared.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BlazingQuiz.Api.Services;

public class AuthServices
{
    private readonly QuizContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IConfiguration _configuration;

    public AuthServices(QuizContext context , IPasswordHasher<User> passwordHasher , IConfiguration configuration)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _configuration = configuration;
    }
    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
    {
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == loginDto.UserName);
    
        if(user == null)
        {
            return new AuthResponseDto(default , "Invalid User Name");
        }

        if (!user.IsApproved)
            return new AuthResponseDto(default, "Your Acount Is Not Approved . ");

        var passwordResult = _passwordHasher.VerifyHashedPassword(user, user.HashedPassword, loginDto.Password);
    
        if(passwordResult == PasswordVerificationResult.Failed)
        {
            return new AuthResponseDto(default, "Wrong Password");

        }
        // Generate JWt
        var jwt = GenereateJwtToken(user);
        var loggedInUser = new LoggedInUser(user.Id, user.Name, user.Role, jwt);


        return new AuthResponseDto(loggedInUser);
    }


    public async Task<QuizApiResponse> RegisterAsync(RegisterDto registerDto)
    {
        if(await _context.Users.AnyAsync(u=> u.Email == registerDto.Email))
        {
            return QuizApiResponse.Fail("Email Already Exists , Try Logging in ");
        }
        var user = new User
        {
            Email = registerDto.Email,
            Name = registerDto.Name,
            Phone = registerDto.Phone,
            Role = nameof(UserRole.Student),
            IsApproved = false,

        };
        user.HashedPassword = _passwordHasher.HashPassword(user, registerDto.Password);
        _context.Users.Add(user);
        try
        {
            await _context.SaveChangesAsync();
            return QuizApiResponse.Success();
        }
        catch (Exception ex)
        {
            return QuizApiResponse.Fail(ex.Message);
        }
    }



    private string GenereateJwtToken(User user)
    {
        Claim[] claims =
            [
                new Claim(ClaimTypes.NameIdentifier , user.Id.ToString()),
                new Claim(ClaimTypes.Name , user.Name),
                new Claim(ClaimTypes.Role , user.Role),
            ];

        var secretKey = _configuration.GetValue<string>("Jwt:Secret");
        var symmetricKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secretKey));
        var signinCred = new SigningCredentials(symmetricKey, SecurityAlgorithms.HmacSha256);

        var jwtSecurityToken = new JwtSecurityToken(
            issuer: _configuration.GetValue<string>("Jwt:Issuer"),
            audience: _configuration.GetValue<string>("Jwt:Audience"),
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_configuration.GetValue<int>("Jwt:ExpireInMinutes")),
            signingCredentials: signinCred
            );

        var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        return token;
    }


}
