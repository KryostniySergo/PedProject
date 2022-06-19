using System.ComponentModel.DataAnnotations.Schema;

namespace PedProject.Model;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }

    public Basket? Basket { get; set; }
    public List<Rating> Ratings { get; set; } = new();
}

public class Basket
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }

    public List<Basket_device> BasketDevices { get; set; } = new();
}

public class Basket_device
{
    public int Id { get; set; }
    
    public int DeviceId { get; set; }
    public Device? Device { get; set; }
    
    public int BasketId { get; set; }
    public Basket? Basket { get; set; }
}

public class Device
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public int Price { get; set; }
    public int Rating { get; set; } = 0;
    public string ImagePath { get; set; }
    [NotMapped]
    public IFormFile Image{ get; set; }
    
    public int TypeId{ get; set; }
    public Type? Type { get; set; }

    public int BrandId { get; set; }
    public Brand? Brand { get; set; }

    public List<Basket_device> BasketDevices { get; set; } = new();
    public DeviceInfo? DeviceInfo { get; set; }
}

public class DeviceInfo
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }

    public int DeviceId { get; set; }
    public Device? Device { get; set; }
}

public class Rating
{
    public int Id { get; set; }
    public int Rate { get; set; }
    
    public int UserId { get; set; }
    public User? User { get; set; }
    
    public int DeviceId { get; set; }
    public Device? Device { get; set; }
}

public class Brand
{
    public int Id { get; set; }
    public string? Name { get; set; }

    public List<Device> Devices { get; set; } = new();
}

public class Type
{
    public int Id { get; set; }
    public string? Name { get; set; }

    public List<Device> Devices { get; set; } = new();
}