
using Microsoft.EntityFrameworkCore;
using TramitesWeb.Models;

namespace TramitesWeb.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Solicitud> Solicitudes { get; set; }
        = null!;
}
