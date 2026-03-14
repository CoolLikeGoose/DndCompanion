using System.Security.Claims;
using DndCompanion.Application.Abstractions.Identity;
using DndCompanion.Application.Abstractions.Persistence;
using DndCompanion.Application.Features.Auth.Login;
using DndCompanion.Application.Features.Auth.Register;
using DndCompanion.Application.Features.Characters.CreateCharacter;
using DndCompanion.Application.Features.Characters.SelectCharacter;
using DndCompanion.Application.Features.Sessions.CreateSession;
using DndCompanion.Application.Features.Sessions.JoinSession;
using Infrastructure.Identity;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContext<DndCompanionDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/auth/login";
        options.LogoutPath = "/auth/logout";
        options.AccessDeniedPath = "/auth/login";
    });

builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<RegisterUserService>();
builder.Services.AddScoped<LoginUserService>();

builder.Services.AddScoped<ICurrentUser, HttpContextCurrentUser>();
builder.Services.AddScoped<ISessionRepository, SessionRepository>();
builder.Services.AddScoped<CreateSessionService>();
builder.Services.AddScoped<JoinSessionService>();

builder.Services.AddScoped<ICharacterRepository, CharacterRepository>();
builder.Services.AddScoped<CreateCharacterService>();
builder.Services.AddScoped<SelectCharacterService>();

var app = builder.Build();

const string LastSessionIdCookie = "dnd.lastSessionId";
const string LastParticipantIdCookie = "dnd.lastParticipantId";

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Auth endpoints
app.MapPost("/auth/login", async (
    HttpContext httpContext,
    [FromForm] string email,
    [FromForm] string password,
    LoginUserService loginUserService) =>
{
    var result = await loginUserService.ExecuteAsync(new LoginUserCommand(email, password));

    if (!result.IsSuccess)
    {
        var error = Uri.EscapeDataString(result.ErrorMessage ?? "Login failed");
        return Results.Redirect($"/auth/login?error={error}");
    }

    var claims = new List<Claim>
    {
        new(ClaimTypes.NameIdentifier, result.UserId!.Value.ToString()),
        new(ClaimTypes.Name, result.UserName!),
        new(ClaimTypes.Email, result.Email!)
    };
    
    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    var principal = new ClaimsPrincipal(identity);

    await httpContext.SignInAsync(
        CookieAuthenticationDefaults.AuthenticationScheme,
        principal);
    
    return Results.Redirect("/");

}).DisableAntiforgery();

app.MapPost("/auth/logout", async (HttpContext httpContext) =>
{
    await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    ClearLastSessionCookies(httpContext);
    return Results.Redirect("/");
    
}).DisableAntiforgery();

app.MapPost("/sessions/create", async (
    HttpContext httpContext,
    [FromForm] string? masterDisplayName,
    [FromForm] string? pinCode,
    CreateSessionService createSessionService) =>
{
    var result = await createSessionService.ExecuteAsync(new CreateSessionCommand(masterDisplayName, pinCode));

    if (!result.IsSuccess)
    {
        var error = Uri.EscapeDataString(result.ErrorMessage ?? "Session creation failed");
        return Results.Redirect($"/sessions/create?error={error}");
    }

    if (result.SessionId is { } createdSessionId && result.ParticipantId is { } createdParticipantId)
    {
        SaveLastSessionCookies(httpContext, createdSessionId, createdParticipantId);
        return Results.Redirect($"/sessions/{createdSessionId}?participantId={createdParticipantId}");
    }

    var invite = Uri.EscapeDataString(result.InviteCode ?? string.Empty);
    var pin = Uri.EscapeDataString(result.PinCode ?? string.Empty);

    return Results.Redirect($"/sessions/join?created=1&inviteCode={invite}&pinCode={pin}");
}).DisableAntiforgery();

app.MapPost("/sessions/join", async (
    HttpContext httpContext,
    [FromForm] string inviteCode,
    [FromForm] string? displayName,
    [FromForm] string? pinCode,
    JoinSessionService joinSessionService) =>
{
    var result = await joinSessionService.ExecuteAsync(new JoinSessionCommand(inviteCode, displayName, pinCode));
    
    if (!result.IsSuccess)
    {
        var error = Uri.EscapeDataString(result.ErrorMessage ?? "Session join failed");
        return Results.Redirect($"/sessions/join?error={error}&inviteCode={Uri.EscapeDataString(inviteCode)}");
    }
    
    var sessionIdValue = result.SessionId!.Value;
    var participantIdValue = result.ParticipantId!.Value;

    SaveLastSessionCookies(httpContext, sessionIdValue, participantIdValue);

    var sessionId = Uri.EscapeDataString(sessionIdValue.ToString());
    var participantId = Uri.EscapeDataString(participantIdValue.ToString());
    
    return Results.Redirect($"/sessions/{sessionId}/lobby?participantId={participantId}");
}).DisableAntiforgery();

app.MapGet("/sessions/resume", (HttpContext httpContext) =>
{
    var sessionIdRaw = httpContext.Request.Cookies[LastSessionIdCookie];
    var participantIdRaw = httpContext.Request.Cookies[LastParticipantIdCookie];

    if (!Guid.TryParse(sessionIdRaw, out var sessionId) || !Guid.TryParse(participantIdRaw, out var participantId))
    {
        ClearLastSessionCookies(httpContext);
        return Results.Redirect("/");
    }

    return Results.Redirect($"/sessions/{sessionId}?participantId={participantId}");
});

void SaveLastSessionCookies(HttpContext httpContext, Guid sessionId, Guid participantId)
{
    var options = new CookieOptions
    {
        HttpOnly = true,
        Secure = !app.Environment.IsDevelopment(),
        SameSite = SameSiteMode.Lax,
        Expires = DateTimeOffset.UtcNow.AddDays(7)
    };

    httpContext.Response.Cookies.Append(LastSessionIdCookie, sessionId.ToString(), options);
    httpContext.Response.Cookies.Append(LastParticipantIdCookie, participantId.ToString(), options);
}

void ClearLastSessionCookies(HttpContext httpContext)
{
    httpContext.Response.Cookies.Delete(LastSessionIdCookie);
    httpContext.Response.Cookies.Delete(LastParticipantIdCookie);
}

app.Run();