using IMCApp.Modelos;
using IMCApp.Utilidades;
using Microsoft.EntityFrameworkCore;

namespace IMCApp.DataAccess
{
    public class PacienteDBContext : DbContext
    {
        public DbSet<Paciente> Pacientes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string conexionDB = $"Filename={ConexionDB.devolverRuta("pacientes.db")}";
            optionsBuilder.UseSqlite(conexionDB);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Paciente>(entity =>
            {
                entity.HasKey(col => col.IdPaciente);
                entity.Property(col => col.IdPaciente).IsRequired().ValueGeneratedOnAdd();
            });
        }


    }
}
