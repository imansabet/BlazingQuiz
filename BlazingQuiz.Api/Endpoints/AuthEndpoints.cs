﻿using BlazingQuiz.Api.Services;
using BlazingQuiz.Shared.DTOs;

namespace BlazingQuiz.Api.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/login", async (LoginDto loginDto, AuthServices authService) =>
            Results.Ok(await authService.LoginAsync(loginDto))
            );

        return app;
    }
}
