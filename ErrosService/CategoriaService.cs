using Microsoft.EntityFrameworkCore;
using Servidor.Data;
using System.Linq;

public class CategoriaService
{
    private readonly BancoContext _context;

    public CategoriaService(BancoContext context)
    {
        _context = context;
    }

    public async Task GerarVinculoAsync()
    {
        // Carregar apenas os dados necessários
        var TabelaTxt = await _context.Contracheque
            .Select(c => new { c.Ccoluna2, c.Ccoluna3, c.Ccoluna16 })
            .ToListAsync();

        var TabelaExcel = await _context.Administrativo
            .Select(a => new { a.Id, a.Acoluna1, a.Acoluna5 })
            .ToListAsync();

        // Identificar as discrepâncias
        var discrepancias = TabelaTxt
    .Where(c =>
        TabelaExcel.Any(a => a.Acoluna1 == c.Ccoluna2) && // Verificar se Ccoluna2 existe na Acoluna1
        !TabelaExcel.Any(a =>
            a.Acoluna1 == c.Ccoluna2 && // Correspondência Acoluna1 <-> Ccoluna2
            a.Acoluna5.Trim().Equals(c.Ccoluna16.Trim(), StringComparison.OrdinalIgnoreCase) // Comparação Acoluna5 <-> Ccoluna16
        )
    )
    .ToList();

        if (discrepancias.Any())
        {
            // Agrupar discrepâncias por Ccoluna2
            var discrepanciasAgrupadas = discrepancias.GroupBy(d => d.Ccoluna2);

            var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var filePath = Path.Combine(desktopPath, "CATEGORIA.txt");

            using var writer = new StreamWriter(filePath);

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                foreach (var grupo in discrepanciasAgrupadas)
                {
                    foreach (var item in grupo)
                    {
                        // Escrever no arquivo
                        await writer.WriteLineAsync($"{item.Ccoluna2};{item.Ccoluna3};{item.Ccoluna16}");

                        // Verificar e atualizar Acoluna5 no banco de dados
                        var administrativo = TabelaExcel
                            .FirstOrDefault(a => a.Acoluna1 == item.Ccoluna2);

                        if (administrativo != null && administrativo.Acoluna5 != item.Ccoluna16.Trim())
                        {
                            var itemParaAtualizar = await _context.Administrativo
                                .FirstOrDefaultAsync(a => a.Id == administrativo.Id);

                            if (itemParaAtualizar != null)
                            {
                                itemParaAtualizar.Acoluna5 = item.Ccoluna16.Trim();
                                _context.Administrativo.Update(itemParaAtualizar);
                            }
                        }
                    }
                }

                // Salvar alterações no banco
                await _context.SaveChangesAsync();

                // Confirmar transação
                await transaction.CommitAsync();

                Console.WriteLine($"Arquivo salvo em: {filePath}");
            }
            catch (Exception ex)
            {
                // Reverter transação em caso de erro
                await transaction.RollbackAsync();
                Console.WriteLine($"Erro ao processar: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine("Nenhuma discrepância encontrada. Arquivo não gerado.");
        }
    }
}
