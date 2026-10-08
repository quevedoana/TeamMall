using Microsoft.EntityFrameworkCore;
using TeaMall.Models;

namespace TeaMall.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }

        public DbSet<Local> Locales { get; set; }

        public DbSet<Publicacion> Publicaciones { get; set; }

        public DbSet<PublicacionLocal> PublicacionesLocales { get; set; }

        public DbSet<Inasistencia> Inasistencias { get; set; }

        public DbSet<Notificacion> Notificaciones { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Usuario -> Local
            modelBuilder.Entity<Usuario>()
                .HasOne<Local>()
                .WithMany()
                .HasForeignKey(u => u.IdLocal)
                .OnDelete(DeleteBehavior.SetNull);

            // Publicacion -> Usuario
            modelBuilder.Entity<Publicacion>()
                .HasOne<Usuario>()
                .WithMany()
                .HasForeignKey(p => p.IdUsuario)
                .OnDelete(DeleteBehavior.Restrict);

            // PublicacionLocal
            modelBuilder.Entity<PublicacionLocal>()
                .HasKey(pl => new
                {
                    pl.IdPublicacion,
                    pl.IdLocal
                });

            modelBuilder.Entity<PublicacionLocal>()
                .HasOne<Publicacion>()
                .WithMany()
                .HasForeignKey(pl => pl.IdPublicacion)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PublicacionLocal>()
                .HasOne<Local>()
                .WithMany()
                .HasForeignKey(pl => pl.IdLocal)
                .OnDelete(DeleteBehavior.Cascade);

            // Inasistencia -> Usuario
            modelBuilder.Entity<Inasistencia>()
                .HasOne<Usuario>()
                .WithMany()
                .HasForeignKey(i => i.IdUsuario)
                .OnDelete(DeleteBehavior.Restrict);

            // Notificacion -> Usuario
            modelBuilder.Entity<Notificacion>()
                .HasOne<Usuario>()
                .WithMany()
                .HasForeignKey(n => n.IdUsuario)
                .OnDelete(DeleteBehavior.Cascade);

            // Notificacion -> Publicacion
            modelBuilder.Entity<Notificacion>()
                .HasOne<Publicacion>()
                .WithMany()
                .HasForeignKey(n => n.IdPublicacion)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}