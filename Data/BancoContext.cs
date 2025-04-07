using Microsoft.EntityFrameworkCore;
using Servidor.Models;

namespace Servidor.Data
{
    public class BancoContext : DbContext
    {
        public BancoContext(DbContextOptions<BancoContext> options) : base(options) { }

        public DbSet<ContrachequeModel> Contracheque { get; set; }

        public DbSet<AdministrativoModel> Administrativo { get; set; }
        public DbSet<SelectOptionModel> SelectOptions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ContrachequeModel>().ToTable("Contracheque");
            modelBuilder.Entity<AdministrativoModel>().ToTable("Administrativo");
            modelBuilder.Entity<SelectOptionModel>().ToTable("SelectOptions");
 
        }
        public static void Seed(BancoContext context)
        {
            if (!context.SelectOptions.Any())
            {
                var selectOption = new List<SelectOptionModel>
            {
                new SelectOptionModel { Nome = "Município de Anadia/BA", ValorColuna9 = "ANADIA", ValorColuna10 = "BA" },
                new SelectOptionModel { Nome = "Município de Cansanção/BA", ValorColuna9 = "CANSANCAO", ValorColuna10 = "BA" },
                new SelectOptionModel { Nome = "Município de Abaré/BA", ValorColuna9 = "ABARE", ValorColuna10 = "BA" },
                new SelectOptionModel { Nome = "Município de Deodápolis/BA", ValorColuna9 = "DEODAPOLIS", ValorColuna10 = "BA" },
                new SelectOptionModel { Nome = "Município de Limoeiro/PE", ValorColuna9 = "LIMOEIRO", ValorColuna10 = "PE" },
                new SelectOptionModel { Nome = "Município de Alcinópolis/MS", ValorColuna9 = "ALCINOPOLIS", ValorColuna10 = "MS" },
                new SelectOptionModel { Nome = "Município de Cupira/PE", ValorColuna9 = "CUPIRA", ValorColuna10 = "PE" },
                new SelectOptionModel { Nome = "Município de Aracatu/BA", ValorColuna9 = "ARACATU", ValorColuna10 = "BA" },
            };

                context.SelectOptions.AddRange(selectOption);
                context.SaveChanges();
            }
        }
    }
}
