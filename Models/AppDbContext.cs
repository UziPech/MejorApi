using Microsoft.EntityFrameworkCore;

namespace ParInpar.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<PalabraVerificada> PalabrasVerificadas { get; set; } = null!;
        public DbSet<NumeroVerificado> Numeros { get; set; } = null!;
        public DbSet<TextoCifrado> Cifrados { get; set; } = null!;
        public DbSet<Usuario> Usuarios { get; set; } = null!;
    }
}

