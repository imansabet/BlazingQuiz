using BlazingQuiz.Shared;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Security.Claims;
using System.Text.Json;

namespace BlazingQuiz.Web.Auth;

public class QuizAuthStateProvider : AuthenticationStateProvider
{
    private const string AuthType = "quiz-auth";
    private const string UserDataKey = "userdata";

    private  Task<AuthenticationState> _authenticationState;
    private readonly IJSRuntime _jSRuntime;

    public QuizAuthStateProvider(IJSRuntime jSRuntime)
    {
        _jSRuntime = jSRuntime;
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
                return;
            }
            var user = LoggedInUser.LoadFrom(userData);
            if (user == null || user.Id == 0)
            {
                return;
            }
            await SetLoginAsync(user);
        }
        finally 
        {
            IsInitializing = false;
        }
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
