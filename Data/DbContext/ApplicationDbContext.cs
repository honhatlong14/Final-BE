using Data.Entities;
using Data.Entities.BaseEntity;
using Data.Entities.User;
using Microsoft.EntityFrameworkCore;

namespace Data.DbContext;

public class ApplicationDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Comment>().HasOne(u => u.User).WithMany(c => c.Comments)
            .HasForeignKey("UserId").OnDelete(DeleteBehavior.NoAction); 
    }

    public DbSet<User> Users { set; get; }
    public DbSet<Address> Addresses { set; get; }
    public DbSet<Book> Books { set; get; }
    public DbSet<BookCategory> BookCategories { set; get; }
    public DbSet<Cart> Carts { set; get; }
    public DbSet<Category> Categories { set; get; }
    public DbSet<Comment> Comments { set; get; }
    public DbSet<Image> Images { set; get; }
    public DbSet<Post> Posts { set; get; }
    public DbSet<Order> Orders { set; get; }
    public DbSet<OrderDetail> OrderDetails { set; get; }
    public DbSet<Stall> Stalls { set; get; }
    public DbSet<WishList> WishLists { set; get; }
    public DbSet<Chat> Chats { get; set; }
    public DbSet<ChatMessage> ChatMessages { get; set; }
    public DbSet<ChatUser> ChatUsers { get; set; }
}