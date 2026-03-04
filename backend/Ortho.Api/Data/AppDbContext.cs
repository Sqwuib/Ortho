using Microsoft.EntityFrameworkCore;
using Ortho.Api.Models;

namespace Ortho.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Project> Projects => Set<Project>();
}