using Microsoft.EntityFrameworkCore;
using Servidor.Data;
using Servidor.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public class ServidorService
{
    private readonly BancoContext _context;
    private readonly string _caminhoSaida;

    public ServidorService(BancoContext context)
    {
        _context = context;
        _caminhoSaida = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        // Garante que a pasta exista
        if (!Directory.Exists(_caminhoSaida))
        {
            Directory.CreateDirectory(_caminhoSaida);
        }
    }

    public async Task GerarEncontradoAsync()
    {
        var contracheques = await _context.Contracheque.AsNoTracking().ToListAsync();
        var administrativos = await _context.Administrativo.AsNoTracking().ToListAsync();

        var contagemContracheque = contracheques
            .GroupBy(c => c.Ccoluna2?.TrimStart('0').Trim())
            .ToDictionary(g => g.Key, g => g.Count());

        var contagemAdministrativo = administrativos
            .GroupBy(a => a.Acoluna1?.TrimStart('0').Trim())
            .ToDictionary(g => g.Key, g => g.Count());

        var discrepancias = new List<ContrachequeModel>();

        // Agrupar discrepâncias por CPF
        var contrachequesAgrupados = contracheques
            .GroupBy(c => c.Ccoluna2?.TrimStart('0').Trim());

        foreach (var grupo in contrachequesAgrupados)
        {
            var cpf = grupo.Key;
            if (string.IsNullOrEmpty(cpf)) continue;

            var qtdContracheque = grupo.Count();
            var qtdAdministrativo = contagemAdministrativo.ContainsKey(cpf) ? contagemAdministrativo[cpf] : 0;
            var diferenca = qtdContracheque - qtdAdministrativo;

            if (diferenca <= 0) continue;

            var linhasParaAdicionar = grupo.Take(diferenca).ToList();

            foreach (var linha in linhasParaAdicionar)
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

                _context.Administrativo.Add(novaLinha);
                discrepancias.Add(linha); // só adiciona a linha que será salva
            }
        }

        if (discrepancias.Any())
        {
            try
            {
                await _context.SaveChangesAsync();
                Console.WriteLine("Discrepâncias salvas no banco de dados com sucesso.");
            }
            catch (DbUpdateException dbEx)
            {
                Console.WriteLine($"Erro ao salvar no banco: {dbEx.Message}");
                Console.WriteLine($"Detalhes: {dbEx.InnerException?.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro inesperado: {ex.Message}");
            }

            // Gerar arquivo apenas com as linhas adicionadas
            var filePath = Path.Combine(_caminhoSaida, "SERVIDOR.txt");
            await using (var writer = new StreamWriter(filePath))
            {
                foreach (var linha in discrepancias)
                {
                    await writer.WriteLineAsync(FormatarLinha(linha));
                }
            }

            Console.WriteLine($"Arquivo SERVIDOR.txt gerado com {discrepancias.Count} linhas em: {filePath}");
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
