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
        // Carregar os dados do Contracheque
        var TabelaTxt = await _context.Contracheque
            .Select(c => new
            {
                CPF = c.Ccoluna2.Trim(),
                Matricula = c.Ccoluna3.TrimStart('0'),
                Categoria = c.Ccoluna16.Trim(),
                Concatenado = $"{c.Ccoluna2.Trim()}{c.Ccoluna3.TrimStart('0')}{c.Ccoluna16.Trim()}"
            })
            .ToListAsync();

        // Carregar os dados do Administrativo
        var TabelaAdministrativo = await _context.Administrativo
            .Select(a => new
            {
                CPF = a.Acoluna1.Trim(),
                Matricula = a.Acoluna2.TrimStart('0'),
                Categoria = a.Acoluna5.Trim(),
                Concatenado = $"{a.Acoluna1.Trim()}{a.Acoluna2.TrimStart('0')}{a.Acoluna5.Trim()}"
            })
            .ToListAsync();

        // Identificar as discrepâncias comparando os valores concatenados
        var discrepancias = TabelaTxt
            .Where(txt => !TabelaAdministrativo.Any(admin => admin.Concatenado == txt.Concatenado))
            .ToList();

        if (discrepancias.Any())
        {
            var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var filePath = Path.Combine(desktopPath, "CATEGORIA.txt");

            using var writer = new StreamWriter(filePath);
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                foreach (var item in discrepancias)
                {
                    // Escrever no arquivo
                    await writer.WriteLineAsync($"{item.CPF};{item.Matricula};{item.Categoria}");

                    // Atualizar no banco de dados
                    var administrativo = await _context.Administrativo
                        .FirstOrDefaultAsync(a =>
                            a.Acoluna1.Trim() == item.CPF &&
                            a.Acoluna2.TrimStart('0') == item.Matricula
                        );

                    if (administrativo != null)
                    {
                        administrativo.Acoluna5 = item.Categoria;
                    }
                }

                // Salvar alterações no banco de dados
                await _context.SaveChangesAsync();
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
