using BlazingQuiz.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;

namespace BlazingQuiz.Web.Auth;

public class QuizAuthStateProvider : AuthenticationStateProvider
{
    private const string AuthType = "quiz-auth";
    private const string UserDataKey = "userdata";

    private  Task<AuthenticationState> _authenticationState;
    private readonly IJSRuntime _jSRuntime;
    private readonly NavigationManager _navigationManager;

    public QuizAuthStateProvider(IJSRuntime jSRuntime , NavigationManager navigationManager)
    {
        _jSRuntime = jSRuntime;
        _navigationManager = navigationManager;
        SetAuthStateTask();

    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync() => _authenticationState;

    public LoggedInUser LoggedInUser {  get; private set  ; }
    public bool IsLoggedIn => LoggedInUser?.Id > 0;

    public async Task SetLoginAsync(LoggedInUser loggedInUser)
    {
        LoggedInUser = loggedInUser;
        SetAuthStateTask();
        NotifyAuthenticationStateChanged(_authenticationState);
        await _jSRuntime.InvokeVoidAsync("localStorage.setItem", UserDataKey, loggedInUser.ToJson());
    }

    public async Task SetLogoutAsync()
    {
        LoggedInUser = null;
        SetAuthStateTask();
        NotifyAuthenticationStateChanged(_authenticationState);
        await _jSRuntime.InvokeVoidAsync("localStorage.removeItem", UserDataKey);

    }

    public bool IsInitializing { get; private set; } = true;
    public async Task InitializeAsync()
    {
        try 
        {
            var userData = await _jSRuntime.InvokeAsync<string?>("localStorage.getItem", UserDataKey);
            if (string.IsNullOrWhiteSpace(userData))
            {
                RedirectToLogin();
                return;
            }
            var user = LoggedInUser.LoadFrom(userData);
            if (user == null || user.Id == 0)
            {
                RedirectToLogin();
                return;
            }
            // Check Token expiration
            if (!IsTokenValid(user.Token))
            {
                RedirectToLogin();
                return;
            }
            await SetLoginAsync(user);
        }
        finally 
        {
            IsInitializing = false;
        }
    }

    private void RedirectToLogin() 
    {
        _navigationManager.NavigateTo("auth/login");
    }



    private static bool IsTokenValid(string token) 
    {
        if (string.IsNullOrWhiteSpace(token))
            return false;

        var jwtHandler = new JwtSecurityTokenHandler();
      
        if (!jwtHandler.CanReadToken(token))
            return false;

        var jwt = jwtHandler.ReadJwtToken(token);
        var expClaim =  jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Exp);

        if (expClaim == null)
            return false;

        var expTime = long.Parse(expClaim.Value);
        
        var expDateTimeToUtc =  DateTimeOffset.FromUnixTimeSeconds(expTime).UtcDateTime;


        return expDateTimeToUtc > DateTime.UtcNow;
    }




    private void SetAuthStateTask()
    {
        if (IsLoggedIn)
        {
            var identity = new ClaimsIdentity(LoggedInUser.ToClaims(),AuthType);
            var principal = new ClaimsPrincipal(identity);
            var authState = new AuthenticationState(principal);

            _authenticationState = Task.FromResult(authState);
        }
        else
        {
            var identity = new ClaimsIdentity();
            var principal = new ClaimsPrincipal(identity);
            var authState = new AuthenticationState(principal);

            _authenticationState = Task.FromResult(authState);

        }

    }


}
