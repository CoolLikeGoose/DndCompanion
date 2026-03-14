using System.Security.Claims;
using DndCompanion.Application.Abstractions.Identity;
using DndCompanion.Application.Abstractions.Persistence;
using DndCompanion.Application.Features.Auth.Login;
using DndCompanion.Application.Features.Auth.Register;
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

var app = builder.Build();

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
    return Results.Redirect("/");
    
}).DisableAntiforgery();

app.MapPost("sessions/create", async (
    HttpContext HttpContext,
    [FromForm] string? masterDisplayName,
    [FromForm] string pinCode,
    CreateSessionService createSessionService) =>
{
    var result = await createSessionService.ExecuteAsync(new CreateSessionCommand(masterDisplayName, pinCode));

    if (!result.IsSuccess)
    {
        var error = Uri.EscapeDataString(result.ErrorMessage ?? "Session creation failed");
        return Results.Redirect($"/sessions/create?error={error}");
    }

    var invite = Uri.EscapeDataString(result.InviteCode ?? string.Empty);
    var pin = Uri.EscapeDataString(result.PinCode ?? string.Empty);

    return Results.Redirect($"/sessions/join?created=1&inviteCode={invite}&pinCode={pin}");
}).DisableAntiforgery();

app.MapPost("/sessions/join", async (
    [FromForm] string inviteCode,
    [FromForm] string displayName,
    [FromForm] string? pinCode,
    JoinSessionService joinSessionService) =>
{
    var result = await joinSessionService.ExecuteAsync(new JoinSessionCommand(inviteCode, displayName, pinCode));
    
    if (!result.IsSuccess)
    {
        var error = Uri.EscapeDataString(result.ErrorMessage ?? "Session join failed");
        return Results.Redirect($"/sessions/join?error={error}&inviteCode={Uri.EscapeDataString(inviteCode)}");
    }
    
    //TODO: Implement redirect to session page
    return Results.Redirect("/");
}).DisableAntiforgery();

app.Run();