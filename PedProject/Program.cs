using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PedProject;
using PedProject.Model;

var builder = WebApplication.CreateBuilder(
    new WebApplicationOptions { WebRootPath = "Pictures"});

string connection = builder.Configuration.GetConnectionString("DefaultConnection");


builder.Services.AddMvc();
builder.Services.AddDbContext<ApplicationContext>(options => options.UseNpgsql(connection));
builder.Services.AddCors();
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

app.UseCors(builder2 => builder2.AllowAnyOrigin().AllowAnyHeader());
app.UseDeveloperExceptionPage();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();


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