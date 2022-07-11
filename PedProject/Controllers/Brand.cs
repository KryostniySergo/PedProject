using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PedProject.Model;

namespace PedProject.Controllers;

//TODO -> ДОДЕЛАТЬ БД ПО ВИДОСУ (СДЕЛАТЬ ЗАВИСИМОСТИ)

public class Brand : Controller
{
    private readonly ApplicationContext db;
    
    public Brand(ApplicationContext context)
    {
        db = context;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Model.Brand brand)
    {
        db.Brands.Add(brand);
        await db.SaveChangesAsync();
        return Json(brand);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var types = await db.Brands.ToListAsync();

        return Json(types);
    }
}