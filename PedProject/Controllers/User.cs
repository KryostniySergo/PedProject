using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
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
    public async Task<IActionResult> Registration([FromBody] Model.User? user)
    {
        //if (!ModelState.IsValid) return ValidationProblem("Вы не ввели данные в поля");

        var candidate = await db.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
        if (candidate is not null)
        {
            return  Unauthorized("Пользователь с таки email уже сущ");
        }

        var hashPassword = BCrypt.Net.BCrypt.HashPassword(user.Password, 5);

        user.Password = hashPassword;
        user.Basket = new Basket();
        await db.Users.AddAsync(user);
        await db.SaveChangesAsync();

        ClaimsIdentity claimsIdentity = JWTtokken.GetClaimIdentity(user.Id.ToString(), user.Email, user.Role);
        
        var response = new
        {
            acces_token = JWTtokken.GetTokken(claimsIdentity.Claims),
        };

        return Json(response, new JsonSerializerOptions() { ReferenceHandler = ReferenceHandler.IgnoreCycles });
    }
    
    public async Task<IActionResult> Login([FromBody] Model.User candidate)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == candidate.Email);
        if (user is null) return Unauthorized("Пользователь с таким email не найден");

        if (BCrypt.Net.BCrypt.EnhancedVerify(candidate.Password, user.Password))
        {
            return Unauthorized("Неверный пароль");
        }

        ClaimsIdentity claimsIdentity = JWTtokken.GetClaimIdentity(user.Id.ToString(), user.Email, user.Role);
        
        var response = new
        {
            acces_token = JWTtokken.GetTokken(claimsIdentity.Claims),
        };

        return Json(response, new JsonSerializerOptions() { ReferenceHandler = ReferenceHandler.IgnoreCycles });
    }

    [Authorize]
    public IActionResult Auth([FromServices] ILogger<User> logger)
    {
        var user = User.Claims.ToList();

        var claimsIdentity =
            JWTtokken.GetClaimIdentity(user[0].Value, user[1].Value, user[2].Value);
        
        var response = new
        {
            acces_token = JWTtokken.GetTokken(claimsIdentity.Claims),
        };

        return Json(response);
    }
}
