using HeroEngine.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace HeroEngine.Core.Data;

public class HeroEngineContext : DbContext
{
    public DbSet<HeroEntity> Heroes { get; set; }
    public DbSet<Ability> Abilities { get; set; }
    public DbSet<HeroClass> HeroClasses { get; set; }

    public HeroEngineContext(DbContextOptions<HeroEngineContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Relació HeroClass → HeroEntity (1:N)
        modelBuilder.Entity<HeroEntity>()
            .HasOne(h => h.HeroClass)
            .WithMany(hc => hc.Heroes)
            .HasForeignKey(h => h.HeroClassId)
            .OnDelete(DeleteBehavior.Restrict); // Bloqueig d'eliminació (Tasca 8.3)

        // Relació HeroEntity → Ability (1:N)
        modelBuilder.Entity<Ability>()
            .HasOne(a => a.Hero)
            .WithMany(h => h.Abilities)
            .HasForeignKey(a => a.HeroId)
            .OnDelete(DeleteBehavior.Cascade); // Eliminar heroi → elimina les seves habilitats
    }
}
