using Microsoft.EntityFrameworkCore;
using Aserto.TodoApp.Domain.Models;
namespace Aserto.TodoApp.Persistence.Contexts
{
  public class AppDbContext : DbContext
  {
    public DbSet<Todo> Todos { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
      base.OnModelCreating(builder);

      builder.Entity<Todo>().ToTable("Todos");
      builder.Entity<Todo>().HasKey(p => p.ID);
      builder.Entity<Todo>().Property(p => p.ID).IsRequired().ValueGeneratedOnAdd();
      builder.Entity<Todo>().Property(p => p.Title).IsRequired();
      builder.Entity<Todo>().Property(p => p.OwnerID).IsRequired();
      builder.Entity<Todo>().Property(p => p.Completed).IsRequired();
    }
  }
}