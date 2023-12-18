using FPTStore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace FPTStore.DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers {get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<OrderHeader> OrderHeaders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Category>().HasData(
                new Category { CategoryId = 1, CategoryName = "Action", DisplayOrder = 1 },
                new Category { CategoryId = 2, CategoryName = "SciFi", DisplayOrder = 2 },
                new Category { CategoryId = 3, CategoryName = "History", DisplayOrder = 3 }
                
                );
            modelBuilder.Entity<Category>().HasData(new Category
                { CategoryId = 4, CategoryName = "IT", DisplayOrder = 4 });
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    ProductId = 1,
                    ProductTitle = "Fortune of Time",
                    ProductAuthor = "Billy Spark",
                    ProductDescription = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
                    ISBN = "SWD9999001",
                    ListPrice = 99,
                    Price = 90,
                    Price50 = 85,
                    Price100 = 80,
                    CategoryId = 1,
                    ImageUrl = ""
                },
                new Product
                {
                    ProductId = 2,
                    ProductTitle = "Dark Skies",
                    ProductAuthor = "Nancy Hoover",
                    ProductDescription = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
                    ISBN = "CAW777777701",
                    ListPrice = 40,
                    Price = 30,
                    Price50 = 25,
                    Price100 = 20,
                    CategoryId = 2,
                    ImageUrl = ""
                },
                new Product
                {
                    ProductId = 3,
                    ProductTitle = "Vanish in the Sunset",
                    ProductAuthor = "Julian Button",
                    ProductDescription = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
                    ISBN = "RITO5555501",
                    ListPrice = 55,
                    Price = 50,
                    Price50 = 40,
                    Price100 = 35,
                    CategoryId = 2,
                    ImageUrl = ""
                },
                new Product
                {
                    ProductId = 4,
                    ProductTitle = "Cotton Candy",
                    ProductAuthor = "Abby Muscles",
                    ProductDescription = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
                    ISBN = "WS3333333301",
                    ListPrice = 70,
                    Price = 65,
                    Price50 = 60,
                    Price100 = 55,
                    CategoryId = 3,
                    ImageUrl = ""
                },
                new Product
                {
                    ProductId = 5,
                    ProductTitle = "Rock in the Ocean",
                    ProductAuthor = "Ron Parker",
                    ProductDescription = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
                    ISBN = "SOTJ1111111101",
                    ListPrice = 30,
                    Price = 27,
                    Price50 = 25,
                    Price100 = 20,
                    CategoryId = 2,
                    ImageUrl = ""
                },
                new Product
                {
                    ProductId = 6,
                    ProductTitle = "Leaves and Wonders",
                    ProductAuthor = "Laura Phantom",
                    ProductDescription = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
                    ISBN = "FOT000000001",
                    ListPrice = 25,
                    Price = 23,
                    Price50 = 22,
                    Price100 = 20,
                    CategoryId = 3,
                    ImageUrl = ""
                },
                new Product
                {
                    ProductId = 7,
                    ProductTitle = "Eternal Echoes",
                    ProductAuthor = "Lily Harmony",
                    ProductDescription = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed ut diam nec ligula euismod consequat. In hac habitasse platea dictumst.",
                    ISBN = "EE999888801",
                    ListPrice = 60,
                    Price = 55,
                    Price50 = 50,
                    Price100 = 45,
                    CategoryId = 2,
                    ImageUrl = ""
                },
                new Product
                {
                    ProductId = 8,
                    ProductTitle = "Whispers in the Wind",
                    ProductAuthor = "Owen Zephyr",
                    ProductDescription = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed ut diam nec ligula euismod consequat. In hac habitasse platea dictumst.",
                    ISBN = "WITW777777701",
                    ListPrice = 45,
                    Price = 40,
                    Price50 = 35,
                    Price100 = 30,
                    CategoryId = 1,
                    ImageUrl = ""
                },
                new Product
                {
                    ProductId = 9,
                    ProductTitle = "Mystical Meadows",
                    ProductAuthor = "Aria Moon",
                    ProductDescription = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed ut diam nec ligula euismod consequat. In hac habitasse platea dictumst.",
                    ISBN = "MM111122223301",
                    ListPrice = 75,
                    Price = 70,
                    Price50 = 65,
                    Price100 = 60,
                    CategoryId = 2,
                    ImageUrl = ""
                },
                new Product
                {
                    ProductId = 10,
                    ProductTitle = "Sunset Serenity",
                    ProductAuthor = "Ethan Sunshine",
                    ProductDescription = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed ut diam nec ligula euismod consequat. In hac habitasse platea dictumst.",
                    ISBN = "SSSS888888801",
                    ListPrice = 35,
                    Price = 32,
                    Price50 = 30,
                    Price100 = 28,
                    CategoryId = 1,
                    ImageUrl = ""
                },
                new Product
                {
                    ProductId = 11,
                    ProductTitle = "Introduction to Algorithms",
                    ProductAuthor = "Thomas H. Cormen",
                    ProductDescription = "A comprehensive introduction to algorithms and data structures.",
                    ISBN = "ITA111122223301",
                    ListPrice = 90,
                    Price = 85,
                    Price50 = 80,
                    Price100 = 75,
                    CategoryId = 4,
                    ImageUrl = ""
                },
                new Product
                {
                    ProductId = 12,
                    ProductTitle = "Clean Code: A Handbook of Agile Software Craftsmanship",
                    ProductAuthor = "Robert C. Martin",
                    ProductDescription = "Guidelines for writing clean, maintainable, and efficient code.",
                    ISBN = "CCASH777777701",
                    ListPrice = 55,
                    Price = 50,
                    Price50 = 45,
                    Price100 = 40,
                    CategoryId = 4,
                    ImageUrl = ""
                },
                new Product
                {
                    ProductId = 13,
                    ProductTitle = "The Pragmatic Programmer: Your Journey to Mastery",
                    ProductAuthor = "David Thomas, Andrew Hunt",
                    ProductDescription = "Practical advice on software development and becoming an effective programmer.",
                    ISBN = "PRAGMA3333333301",
                    ListPrice = 70,
                    Price = 65,
                    Price50 = 60,
                    Price100 = 55,
                    CategoryId = 4,
                    ImageUrl = ""
                },
                new Product
                {
                    ProductId = 14,
                    ProductTitle = "Design Patterns: Elements of Reusable Object-Oriented Software",
                    ProductAuthor = "Erich Gamma, Richard Helm, Ralph Johnson, John Vlissides",
                    ProductDescription = "Classic book on object-oriented design patterns.",
                    ISBN = "DPATTERN1111111101",
                    ListPrice = 40,
                    Price = 35,
                    Price50 = 30,
                    Price100 = 25,
                    CategoryId = 4,
                    ImageUrl = ""
                }
            );
        }
    }
}
