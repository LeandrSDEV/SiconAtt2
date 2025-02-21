using Microsoft.EntityFrameworkCore;
using Servidor.Data;
using Servidor.Models;

public class ServidorService
{
    private readonly BancoContext _context;

    public ServidorService(BancoContext context)
    {
        _context = context;
    }

    public async Task GerarEncontradoAsync()
    {
        // Carregar os dados das tabelas
        var contracheques = await _context.Contracheque.AsNoTracking().ToListAsync();
        var administrativos = await _context.Administrativo.AsNoTracking().ToListAsync();

        // Normalizar os dados
        var administrativosNormalizados = administrativos
            .Select(a => a.Acoluna1?.TrimStart('0').Trim())
            .ToList();

        var cColuna2List = contracheques
            .Select(c => c.Ccoluna2?.TrimStart('0').Trim())
            .ToList();

        var discrepancias = new List<ContrachequeModel>();

        // Identificar valores duplicados em Ccoluna2
        var duplicatasCcoluna2 = cColuna2List
            .GroupBy(c => c)
            .Select(g => new { Valor = g.Key, QuantidadeC = g.Count() })
            .ToList();

        foreach (var duplicata in duplicatasCcoluna2)
        {
            // Contar as ocorrências em Administrativo
            int ocorrenciasEmAdministrativo = administrativos
                .Count(a => a.Acoluna1 != null && a.Acoluna1.TrimStart('0').Trim() == duplicata.Valor);

            // Se a quantidade em Contracheque for maior que em Administrativo
            if (duplicata.QuantidadeC > ocorrenciasEmAdministrativo)
            {
                // Selecionar as linhas de Contracheque correspondentes a essa duplicata
                var linhasDuplicadas = contracheques
                    .Where(c => c.Ccoluna2 != null && c.Ccoluna2.TrimStart('0').Trim() == duplicata.Valor)
                    .ToList();

                // Filtrar as linhas que ainda não têm correspondência em Administrativo
                var linhasExtras = linhasDuplicadas
                    .Where(c => !administrativos.Any(a =>
                        a.Acoluna1 != null && a.Acoluna1.TrimStart('0').Trim() == (c.Ccoluna2 != null ? c.Ccoluna2.TrimStart('0').Trim() : null) &&
                        a.Acoluna2 != null && a.Acoluna2.TrimStart('0').Trim() == (c.Ccoluna3 != null ? c.Ccoluna3.TrimStart('0').Trim() : null)))
                    .ToList();

                foreach (var linha in linhasExtras)
                {
                    Console.WriteLine($"Discrepância encontrada: {linha.Ccoluna2};{linha.Ccoluna3}");
                    discrepancias.Add(linha);

                    // Verificar se já existe no banco antes de adicionar
                    bool exists = _context.Administrativo.Any(a =>
                        a.Acoluna1 != null && a.Acoluna1.TrimStart('0').Trim() == (linha.Ccoluna2 != null ? linha.Ccoluna2.TrimStart('0').Trim() : null) &&
                        a.Acoluna2 != null && a.Acoluna2.TrimStart('0').Trim() == (linha.Ccoluna3 != null ? linha.Ccoluna3.TrimStart('0').Trim() : null));

                    if (!exists)
                    {
                        var novaLinha = new AdministrativoModel
                        {
                            Acoluna1 = linha.Ccoluna2,
                            Acoluna2 = linha.Ccoluna3,
                            Acoluna3 = linha.Ccoluna4,
                            Acoluna4 = linha.Ccoluna21,
                            Acoluna5 = linha.Ccoluna16,
                            Acoluna6 = linha.Ccoluna18
                        };

                        Console.WriteLine($"Adicionando ao banco: {novaLinha.Acoluna1}, {novaLinha.Acoluna2}, {novaLinha.Acoluna3}");
                        _context.Administrativo.Add(novaLinha);
                    }
                }
            }
        }

        // Salvar as discrepâncias no banco de dados
        if (discrepancias.Any())
        {
            try
            {
                await _context.SaveChangesAsync();
                Console.WriteLine("Discrepâncias salvas no banco de dados com sucesso.");
            }
            catch (DbUpdateException dbEx)
            {
                Console.WriteLine($"Erro ao salvar no banco de dados: {dbEx.Message}");
                Console.WriteLine($"Detalhes: {dbEx.InnerException?.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro inesperado: {ex.Message}");
            }

            // Gerar o arquivo SERVIDOR.txt
            var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var filePath = Path.Combine(desktopPath, "SERVIDOR.txt");

            await using (var writer = new StreamWriter(filePath))
            {
                foreach (var linha in discrepancias)
                {
                    await writer.WriteLineAsync(FormatarLinha(linha));
                }
            }

            Console.WriteLine($"Arquivo salvo em: {filePath}");
        }
        else
        {
            Console.WriteLine("Nenhuma discrepância encontrada. Arquivo não gerado.");
        }
    }

    private string FormatarLinha(ContrachequeModel c)
    {
        return string.Join(';', new[]
        {
            c.Ccoluna1 ?? "",
            c.Ccoluna2 ?? "",
            c.Ccoluna3 ?? "",
            c.Ccoluna4 ?? "",
            c.Ccoluna5 ?? "",
            c.Ccoluna6 ?? "",
            c.Ccoluna7 ?? "",
            c.Ccoluna8 ?? "",
            c.Ccoluna9 ?? "",
            c.Ccoluna10 ?? "",
            c.Ccoluna11 ?? "",
            c.Ccoluna12 ?? "",
            c.Ccoluna13 ?? "",
            c.Ccoluna14 ?? "",
            c.Ccoluna15 ?? "",
            c.Ccoluna16 ?? "",
            c.Ccoluna17 ?? "",
            c.Ccoluna18 ?? "",
            c.Ccoluna19 ?? "",
            c.Ccoluna20 ?? "",
            c.Ccoluna21 ?? "",
            c.Ccoluna22 ?? "",
            c.Ccoluna23 ?? "",
            c.Ccoluna24 ?? "",
            c.Ccoluna25 ?? ""
         
        });
    }
}