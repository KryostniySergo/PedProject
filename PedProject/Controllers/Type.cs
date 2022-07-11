using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PedProject.Model;

namespace PedProject.Controllers;

public class Type : Controller
{
    private readonly ApplicationContext db;
    
    public Type(ApplicationContext context)
    {
        db = context;
    }

    [HttpPost]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> Create([FromBody] PedProject.Model.Type type)
    {
        db.Types.Add(type);
        await db.SaveChangesAsync();
        return Json(type);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var types = await db.Types.ToListAsync();

        return Json(types);
    }
}