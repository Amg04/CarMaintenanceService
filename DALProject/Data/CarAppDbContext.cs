using DALProject.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection;

namespace DALProject.Data
{
    public class CarAppDbContext : IdentityDbContext<AppUser>
    {
        public CarAppDbContext(DbContextOptions<CarAppDbContext> options) : base(options)
        {

        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            // اي بروبرتي نوعها ديسمل بحدد حجها
            configurationBuilder.Properties<decimal>().HavePrecision(8, 2);
        }

        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<Model> Models { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Category> Categroies { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<ProductCategory> ProductCategorys { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ShoppingCart> shoppingCarts { get; set; }
        public DbSet<OrderDetail> OrderDetials { get; set; }
        public DbSet<OrderHeader> OrderHeaders { get; set; }
        public DbSet<ModelPart> ModelParts { get; set; }

        #region TPC
        public DbSet<AppUser> Customers { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Technician> Technicians { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region Seed Data

            modelBuilder.Entity<Brand>().HasData(
                new Brand { Id = 1, Name = "Toyota" },
                new Brand { Id = 2, Name = "Hyundai" },
                new Brand { Id = 3, Name = "BMW" });

            modelBuilder.Entity<Model>().HasData(
                new Model { Id = 1, Name = "Corolla", BrandId = 1 },
                new Model { Id = 2, Name = "Elantra", BrandId = 2 },
                new Model { Id = 3, Name = "X5", BrandId = 3 });

            modelBuilder.Entity<Color>().HasData(
                new Color { Id = 1, Name = "Black" },
                new Color { Id = 2, Name = "White" },
                new Color { Id = 3, Name = "Red" });

            modelBuilder.Entity<Category>().HasData(
              new Category { Id = 1, Name = "Electrical" },
              new Category { Id = 2, Name = "Mechanical" }
          );

        modelBuilder.Entity<Service>().HasData(
      new Service
      {
          Id = 1,
          Name = "Oil Change",
          Price = 300,
          Description = "Complete oil change",
          ImgPath = "oil.jpg",
          CategoryId = 1
      },
      new Service
      {
          Id = 2,
          Name = "Brake Service",
          Price = 500,
          Description = "Brake inspection and replacement",
          ImgPath = "brake.jpg",
          CategoryId = 2
      },
      new Service
      {
          Id = 3,
          Name = "Tire Rotation",
          Price = 200,
          Description = "Rotation of tires for even wear",
          ImgPath = "tire.jpg",
          CategoryId = 2
      }
  );

            modelBuilder.Entity<ProductCategory>().HasData(
                new ProductCategory
                {
                    Id = 1,
                    Name = "Engine Maintenance",
                    Description = "Services related to engine performance and upkeep",
                    CreatedTime = DateTime.Now
                },
                new ProductCategory
                {
                    Id = 2,
                    Name = "Fuel System",
                    Description = "Services related to the fuel system",
                    CreatedTime = DateTime.Now
                }
            );



            modelBuilder.Entity<Product>().HasData(
    new Product
    {
        Id = 1,
        Name = "Oil Change + Filter",
        Description = "Change oil and replace filter",
        ImgPath = "oil_combo.jpg",
        Price = 450,
        ProdCatIegoryd = 1,
    },
    new Product
    {
        Id = 2,
        Name = "Brake Maintenance",
        Description = "Replace brake pads and clean brakes",
        ImgPath = "brake_combo.jpg",
        Price = 600,
        ProdCatIegoryd = 1,
    },
    new Product
    {
        Id = 3,
        Name = "Air Filter Replacement",
        Description = "Install new engine air filter",
        ImgPath = "air.jpg",
        Price = 180,
        ProdCatIegoryd = 1,
    },
    new Product
    {
        Id = 4,
        Name = "Spark Plug Replacement",
        Description = "Replace old spark plugs",
        ImgPath = "spark.jpg",
        Price = 200,
        ProdCatIegoryd = 1,
    },
    new Product
    {
        Id = 5,
        Name = "Fuel Filter Service",
        Description = "Replace fuel filter to improve fuel flow",
        ImgPath = "fuel.jpg",
        Price = 250,
        ProdCatIegoryd = 2,
    },
    new Product
    {
        Id = 6,
        Name = "Timing Belt Replacement",
        Description = "Install new timing belt",
        ImgPath = "belt.jpg",
        Price = 800,
        ProdCatIegoryd = 1,
    }
);

            #endregion

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
