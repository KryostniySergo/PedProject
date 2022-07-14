using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PedProject.Model;


namespace PedProject.Controllers;


public class Device : Controller
{
    private ApplicationContext db;
    
    public Device(ApplicationContext context)
    {
        db = context;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromForm] Model.Device device,[FromForm] string info, [FromServices] ILogger<Device> logger)
    {
        var uniqueFileName = GetUniqueFileName(device.Image.FileName);
        var uploads = Path.Combine(Environment.CurrentDirectory, "Pictures");
        device.ImagePath = uniqueFileName;
        
        await device.Image.CopyToAsync(new FileStream(Path.Combine(uploads, uniqueFileName), FileMode.Create));
        
        var deviceInfos = JsonSerializer.Deserialize<List<DeviceInfo>>(info);

        foreach (var item in deviceInfos)
        {
            device.DeviceInfo.Add(item);
        }

        await db.Devices.AddAsync(device);
        await db.SaveChangesAsync();
        
        return Json(device,
            new JsonSerializerOptions() { ReferenceHandler = ReferenceHandler.IgnoreCycles });
    }
    
    private string GetUniqueFileName(string fileName)
    {
        fileName = Path.GetFileName(fileName);
        return Guid.NewGuid() + Path.GetExtension(fileName);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll(int brandId, int typeId, int limit = 9, int page = 1)
    {
        List<Model.Device> devices;

        var offset = page * limit - limit;

        if (brandId == 0 && typeId == 0)
        {
            devices = await db.Devices.Skip(offset).Take(limit).ToListAsync();
        }
        else if (brandId != 0 && typeId == 0)
        { 
            devices = await db.Devices.Where(d => d.BrandId == brandId).Skip(offset).Take(limit).ToListAsync();
        }
        else if (brandId == 0 && typeId != 0)
        {
            devices = await db.Devices.Where(d => d.TypeId == typeId).Skip(offset).Take(limit).ToListAsync();
        }
        else
        {
            devices = await db.Devices.Where(d => d.BrandId == brandId && d.TypeId == typeId).Skip(offset).Take(limit).ToListAsync();
        }

        var totalCount = await db.Devices.CountAsync();
        
        foreach (var device in devices)
        {
            device.Count = totalCount;
        }

        return Json(devices);
    }

    public async Task<IActionResult> GetOne(int id)
    {
        var device = await db.Devices.FindAsync(id);
        if (device != null)
        {
            await db.DeviceInfos.Where(u => u.DeviceId == device.Id).LoadAsync();
        }

        return Json(device,  new JsonSerializerOptions() { ReferenceHandler = ReferenceHandler.IgnoreCycles });
    }
}