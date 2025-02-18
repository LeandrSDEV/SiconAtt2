using Microsoft.EntityFrameworkCore;
using Servidor.Data;

public class MatriculaService
{
    private readonly BancoContext _context;

    public MatriculaService(BancoContext context)
    {
        _context = context;
    }

    public async Task GerarMatriculasAsync()
    {
        // Carregar os dados das tabelas
        var contracheque = await _context.Contracheque.ToListAsync();
        var administrativo = await _context.Administrativo.ToListAsync();

        // Concatenar colunas e remover zeros à esquerda
        var contrachequeConcatenado = contracheque
            .Select(c => new
            {
                CPF = c.Ccoluna2.Trim(),
                ValorOriginal = c.Ccoluna3.TrimStart('0'), // Removendo zeros à esquerda
                ValorConcatenado = $"{c.Ccoluna2.Trim()}{c.Ccoluna3.TrimStart('0')}"
            })
            .ToList();

        var administrativoConcatenado = administrativo
            .Select(a => new
            {
                CPF = a.Acoluna1.Trim(),
                ValorOriginal = a.Acoluna2.TrimStart('0'), // Removendo zeros à esquerda
                ValorConcatenado = $"{a.Acoluna1.Trim()}{a.Acoluna2.TrimStart('0')}"
            })
            .ToList();

        // Listas de discrepâncias
        var discrepancias = new List<(string CPF, string ValorOriginal, string ValorNovo)>();

        // Etapa 2: Remover registros iguais
        var contrachequeRestante = contrachequeConcatenado
            .Where(c => !administrativoConcatenado.Any(a => a.ValorConcatenado == c.ValorConcatenado))
            .ToList();

        var administrativoRestante = administrativoConcatenado
            .Where(a => !contrachequeConcatenado.Any(c => c.ValorConcatenado == a.ValorConcatenado))
            .ToList();

        // Etapa 3: Substituir valores únicos
        var cpfsUnicosContracheque = contrachequeRestante
            .GroupBy(c => c.CPF)
            .Where(g => g.Count() == 1)
            .Select(g => g.Key);

        foreach (var cpf in cpfsUnicosContracheque)
        {
            var valorContracheque = contrachequeRestante.First(c => c.CPF == cpf);
            var valorAdministrativo = administrativoRestante.FirstOrDefault(a => a.CPF == cpf);

            if (valorAdministrativo != null)
            {
                discrepancias.Add((cpf, valorAdministrativo.ValorOriginal, valorContracheque.ValorOriginal));
                administrativoRestante.Remove(valorAdministrativo);
            }
        }

        // Etapa 4 e 5: Tratar valores múltiplos
        var cpfsDuplicadosContracheque = contrachequeRestante
            .GroupBy(c => c.CPF)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key);

        foreach (var cpf in cpfsDuplicadosContracheque)
        {
            var valoresContracheque = contrachequeRestante.Where(c => c.CPF == cpf).ToList();
            var valoresAdministrativo = administrativoRestante.Where(a => a.CPF == cpf).ToList();

            for (int i = 0; i < Math.Min(valoresContracheque.Count, valoresAdministrativo.Count); i++)
            {
                discrepancias.Add((
                    cpf,
                    valoresAdministrativo[i].ValorOriginal,
                    valoresContracheque[i].ValorOriginal
                ));

                administrativoRestante.Remove(valoresAdministrativo[i]);
            }
        }

        // Etapa 7: Gerar o arquivo de discrepâncias apenas se houver conteúdo
        if (discrepancias.Any())
        {
            var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var filePath = Path.Combine(desktopPath, "MATRICULA.txt");

            await using (var writer = new StreamWriter(filePath))
            {
                foreach (var discrepancia in discrepancias)
                {
                    // Gerar apenas CPF, valor original, e valor novo, sem zeros à esquerda
                    await writer.WriteLineAsync($"{discrepancia.CPF};{discrepancia.ValorOriginal};{discrepancia.ValorNovo}");
                }
            }

            Console.WriteLine($"Arquivo de discrepâncias salvo em: {filePath}");
        }
        else
        {
            Console.WriteLine("Nenhuma discrepância encontrada. Arquivo não gerado.");
        }

        // Etapa 8: Atualizar os valores no banco de dados
        foreach (var discrepancia in discrepancias)
        {
            // Localizar o registro correspondente no Administrativo
            var registroParaAtualizar = administrativo.FirstOrDefault(a =>
                a.Acoluna1.Trim() == discrepancia.CPF &&
                a.Acoluna2.TrimStart('0') == discrepancia.ValorOriginal);

            if (registroParaAtualizar != null)
            {
                // Atualizar o valor na tabela Administrativo
                registroParaAtualizar.Acoluna2 = discrepancia.ValorNovo.PadLeft(6, '0'); // Opcional: ajustar formato, se necessário
            }
        }

        // Salvar as alterações no banco de dados
        await _context.SaveChangesAsync();
        Console.WriteLine("Tabela Administrativo atualizada com sucesso.");
    }
}
