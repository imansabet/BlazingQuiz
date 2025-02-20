using BlazingQuiz.Api.Data;
using BlazingQuiz.Api.Data.Entities;
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
        var user = await _context.User
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == loginDto.UserName);
    
        if(user == null)
        {
            return new AuthResponseDto(default , "Invalid User Name");
        }

        var passwordResult = _passwordHasher.VerifyHashedPassword(user, user.HashedPassword, loginDto.Password);
    
        if(passwordResult == PasswordVerificationResult.Failed)
        {
            return new AuthResponseDto(default, "Wrong Password");

        }
        // Generate JWt
        var jwt = GenereateJwtToken(user);

        return new AuthResponseDto(jwt);
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
