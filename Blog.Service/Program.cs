using Blog.Domain.Services.User;
using Blog.Repositary;
using Blog.Service.Middlewares;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

static void ConfigureServices(IServiceCollection services)
{
    services.AddScoped<IUserService, UserService>();
}

var builder = WebApplication.CreateBuilder(args);
var Configuration = builder.Configuration;

// Set Authentication
// Must have unique name (Scheme)
builder.Services
    .AddAuthentication(defaultScheme: Configuration["Auth:DefaultScheme"]!)
    .AddCookie(Configuration["Auth:DefaultScheme"]!, options =>
    {
        options.Cookie.Name = Configuration["Auth:DefaultCookieName"];
        options.ExpireTimeSpan = TimeSpan.FromDays(1.0);

        options.Cookie.HttpOnly = false;

        options.LoginPath = new PathString("/login");
    })
    .AddCookie(Configuration["Auth:TempCookieName"]!)
    .AddOpenIdConnect(Configuration["Vas3kAuth:Scheme"]!, options =>
    {
        options.Authority = Configuration["Vas3kAuth:Authority"];

        options.ClientId = Configuration["Vas3kAuth:ClientId"];
        options.ClientSecret = Configuration["Vas3kAuth:ClientSecret"];

        // Set the callback path, so it will call back to.
        options.CallbackPath = new PathString(Configuration["Vas3kAuth:Callback"]);

        // Set response type to code
        options.ResponseType = OpenIdConnectResponseType.Code;

        // Configure the scope
        options.Scope.Clear();
        options.Scope.Add("openid");

        options.SaveTokens = true;

        options.Events.OnAuthorizationCodeReceived = async (context) =>
        {
            var request = context.HttpContext.Request;
            var redirectUri = context.Properties?.Items[OpenIdConnectDefaults.RedirectUriForCodePropertiesKey] ?? "/";
            var code = context.ProtocolMessage.Code;

            using var client = new HttpClient();
            var discoResponsee = await client.GetDiscoveryDocumentAsync(options.Authority);

            var tokenResponse = await client.RequestAuthorizationCodeTokenAsync(
                new AuthorizationCodeTokenRequest
                {
                    Address = discoResponsee.TokenEndpoint,
                    ClientId = options.ClientId!,
                    ClientSecret = options.ClientSecret,
                    Code = code,
                    RedirectUri = redirectUri,
                }
            );

            if (tokenResponse.IsError)
            {
                // Error handler
                throw new Exception("Bad auth. Can't exchange code for access token and id token");
            }

            var accessToken = tokenResponse.AccessToken ?? string.Empty;
            var idToken = tokenResponse.IdentityToken ?? string.Empty;

            context.HandleCodeRedemption(accessToken, idToken);
        };

        options.MapInboundClaims = false;
        options.SignInScheme = Configuration["Auth:TempCookieName"];
    });

// Add polices
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("IsAdmin", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("isAdmin", "yes");
    });
});

// Add AppDbContext
builder.Services.AddDbContext<AppDbContext>();

// Configure application services
ConfigureServices(builder.Services);
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<UserMiddleware>();

#pragma warning disable ASP0014 // Suggest using top level route registrations
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "api/{controller}/{action=Index}/{id?}"
    );
});
#pragma warning restore ASP0014 // Suggest using top level route registrations

if (app.Environment.IsDevelopment())
{
    app.UseSpa(spa =>
    {
        spa.UseProxyToSpaDevelopmentServer("http://localhost:5173");
    });
}
else
{
    app.MapFallbackToFile("index.html");
}

app.Run();
