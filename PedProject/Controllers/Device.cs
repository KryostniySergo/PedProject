using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PedProject.Model;


namespace PedProject.Controllers;

//TODO -> https://stackoverflow.com/questions/35379309/how-to-upload-files-in-asp-net-core [ГАЙД НА ФАЙЛЫ]

public class Device : Controller
{
    private ApplicationContext db;
    
    public Device(ApplicationContext context)
    {
        db = context;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromForm] Model.Device device, [FromForm] DeviceInfo deviceInfo)
    {
        var uniqueFileName = GetUniqueFileName(device.Image.FileName);
        var uploads = Path.Combine(Environment.CurrentDirectory, "Pictures");
        device.ImagePath = Path.Combine(uploads, uniqueFileName); // КАК ПУТЬ
        
        await device.Image.CopyToAsync(new FileStream(device.ImagePath, FileMode.Create));

        device.DeviceInfo = deviceInfo;
        
        await db.Devices.AddAsync(device);
        await db.SaveChangesAsync();

        return Json(device,
            new JsonSerializerOptions() { ReferenceHandler = ReferenceHandler.IgnoreCycles });  //TODO -> ПОДУМАТЬ;
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

        //Примерно 50 минута видио про магаз
        var offset = page * limit - limit;

        if (brandId == 0 && typeId == 0)
        {
            devices = await db.Devices.Skip(offset).Take(limit).ToListAsync(); //Если нет бренда и типа / выводим всё
        }
        else if (brandId != 0 && typeId == 0)
        { 
            devices = await db.Devices.Where(d => d.BrandId == brandId).Skip(offset).Take(limit).ToListAsync(); //Если нет типа / выводим бренды
        }
        else if (brandId == 0 && typeId != 0)
        {
            devices = await db.Devices.Where(d => d.TypeId == typeId).Skip(offset).Take(limit).ToListAsync();
        }
        else
        {
            devices = await db.Devices.Where(d => d.BrandId == brandId && d.TypeId == typeId).Skip(offset).Take(limit).ToListAsync();
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