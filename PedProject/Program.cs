using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using PedProject;
using PedProject.Model;

var builder = WebApplication.CreateBuilder(
    new WebApplicationOptions { WebRootPath = "Pictures"}); 


builder.Services.AddMvc();
builder.Services.AddDbContext<ApplicationContext>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigin", corsPolicyBuilder => corsPolicyBuilder.AllowAnyOrigin());
});
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = AuthOptions.ISSUER,
        ValidateAudience = true,
        ValidAudience = AuthOptions.AUDIENCE,
        ValidateLifetime = true,
        IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
        ValidateIssuerSigningKey = true,
    };
});


var app = builder.Build();

app.UseDeveloperExceptionPage();
app.UseStaticFiles();
app.UseCors();


app.MapGet("/", () =>
{
    return "hi";
});

app.MapControllerRoute(
    name: "device",
    pattern: "{controller=Device}/{action=GetOne}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action}/");


app.Run();