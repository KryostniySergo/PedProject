using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PedProject.Model;

namespace PedProject.Controllers;

public class User : Controller
{
    private ApplicationContext db;

    public User(ApplicationContext db)
    {
        this.db = db;
    }

    [HttpPost]
    public async Task<IActionResult> Registration([FromBody] Model.User? user, [FromServices] ILogger<User> _logger)
    {
        //if (!ModelState.IsValid) return ValidationProblem("Вы не ввели данные в поля");

        var candidate = await db.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
        if (candidate is not null)
        {
            _logger.LogInformation("Пользователь с таки email уже сущ");
            return  Unauthorized("Пользователь с таки email уже сущ");
        }

        var hashPassword = BCrypt.Net.BCrypt.HashPassword(user.Password, 5);

        user.Password = hashPassword;
        user.Basket = new Basket();
        await db.Users.AddAsync(user);
        await db.SaveChangesAsync();

        var claims = new List<Claim>
        {
            new Claim("id", user.Id.ToString()),
            new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email),
            new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role)
        };

        ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

        var response = new
        {
            acces_token = JWTtokken.GetTokken(claimsIdentity.Claims),
            username = claimsIdentity.Name
        };

        return Json(response, new JsonSerializerOptions() { ReferenceHandler = ReferenceHandler.IgnoreCycles });
    }
    
    public IActionResult Login(string username)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, username),
            
        };

        return Content(JWTtokken.GetTokken(claims));
    }

    public IActionResult Auth([FromQuery] Model.User user)
    {
        if (!ModelState.IsValid) return ValidationProblem("Вы не ввели данные в поля");

        return Json(user);
    }
}
