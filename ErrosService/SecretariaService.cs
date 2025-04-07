using Microsoft.EntityFrameworkCore;
using Servidor.Data;
using Servidor.Models;

public class SecretariaService
{
    private readonly BancoContext _context;

    public SecretariaService(BancoContext context)
    {
        _context = context;
    }

    public async Task GerarSecretariasAsync()
    {
        var tabelaTxt = await _context.Contracheque.AsNoTracking().ToListAsync();
        var tabelaExcel = await _context.Administrativo.AsNoTracking().ToListAsync();

        // Cria um Lookup (permite múltiplas entradas com a mesma chave)
        var administrativosLookup = tabelaExcel
            .Where(a => !string.IsNullOrWhiteSpace(a.Acoluna1) && !string.IsNullOrWhiteSpace(a.Acoluna2))
            .ToLookup(
                a => $"{a.Acoluna1.Trim()}{a.Acoluna2.Trim()}",
                a => (a.Acoluna4 ?? "").Trim()
            );

        var discrepancias = new List<ContrachequeModel>();

        foreach (var linha in tabelaTxt)
        {
            var cpf = linha.Ccoluna2?.Trim();
            var matricula = linha.Ccoluna3?.Trim();
            var secretariaTxt = (linha.Ccoluna21 ?? "").Trim();

            if (string.IsNullOrEmpty(cpf) || string.IsNullOrEmpty(matricula))
                continue;

            var chave = $"{cpf}{matricula}";

            // Verifica se existe a chave no Administrativo
            if (administrativosLookup.Contains(chave))
            {
                var secretariaExcel = administrativosLookup[chave];

                // Se nenhuma das secretarias do Excel bater com a do TXT, é discrepância
                if (!secretariaExcel.Any(sec => sec == secretariaTxt))
                {
                    discrepancias.Add(linha);
                }
            }
        }

        if (!discrepancias.Any())
        {
            Console.WriteLine("✅ Nenhuma discrepância encontrada.");
            return;
        }

        var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        var filePath = Path.Combine(desktopPath, "SECRETARIAS.txt");

        try
        {
            using (var writer = new StreamWriter(filePath))
            {
                foreach (var item in discrepancias)
                {
                    await writer.WriteLineAsync($"{item.Ccoluna2?.Trim()};{item.Ccoluna3?.Trim()};{item.Ccoluna21?.Trim()}");
                }
            }

            Console.WriteLine($"✅ Arquivo 'SECRETARIAS.txt' gerado com {discrepancias.Count} discrepâncias.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erro ao salvar o arquivo: {ex.Message}");
        }
    }
}
