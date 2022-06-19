using Microsoft.EntityFrameworkCore;

namespace PedProject.Model;

public sealed class ApplicationContext : DbContext
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Basket> Baskets { get; set; }= null!;
    public DbSet<Basket_device> BasketDevices { get; set; }= null!;
    public DbSet<Device> Devices { get; set; }= null!;
    public DbSet<DeviceInfo> DeviceInfos { get; set; }= null!;
    public DbSet<Rating> Ratings { get; set; }= null!;
    public DbSet<Brand> Brands { get; set; }= null!;
    public DbSet<Type> Types { get; set; }= null!;

    public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options)
    {
        //Database.EnsureDeleted();
        Database.EnsureCreated();   // создаем базу данных при первом обращении
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=PedMagaz;Username=postgres;Password=12345");
        //optionsBuilder.LogTo(Console.WriteLine);
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Device>()
            .HasOne(u => u.DeviceInfo)
            .WithOne(u => u.Device)
            .HasForeignKey<DeviceInfo>(u => u.DeviceId);
        
        modelBuilder.Entity<User>()
            .HasOne(u => u.Basket)
            .WithOne(u => u.User)
            .HasForeignKey<Basket>(u => u.UserId);
        
        modelBuilder.Entity<User>().Property(u => u.Role).HasDefaultValue("USER");
        modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
        
        modelBuilder.Entity<Device>().HasIndex(u => u.Name).IsUnique();
        modelBuilder.Entity<Device>().Property(u => u.Rating).HasDefaultValue(0);
        
        modelBuilder.Entity<Brand>().HasIndex(u => u.Name).IsUnique();
        
        modelBuilder.Entity<Type>().HasIndex(u => u.Name).IsUnique();
    }
}