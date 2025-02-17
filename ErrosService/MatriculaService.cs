using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Servidor.Data;
using Microsoft.EntityFrameworkCore;

public class MatriculaService
{
    private readonly BancoContext _context;

    public MatriculaService(BancoContext context)
    {
        _context = context;
    }

    public async Task GerarMatriculasAsync()
    {
        // Carregar os dados das tabelas ordenadas
        var TabelaTxt = await _context.Contracheque
            .OrderBy(c => c.Ccoluna2.TrimStart('0'))
            .ThenBy(c => c.Ccoluna3.TrimStart('0'))
            .ToListAsync();

        var TabelaExcel = await _context.Administrativo
            .OrderBy(a => a.Acoluna1.TrimStart('0'))
            .ThenBy(a => a.Acoluna2.TrimStart('0'))
            .ToListAsync();

        // Preparar uma lista para armazenar as discrepâncias
        var discrepancias = new List<(string Ccoluna2, string Acoluna2, string Ccoluna3)>();

        // Agrupar os dados por CPF para processar cada grupo
        var gruposContracheque = TabelaTxt
            .GroupBy(c => c.Ccoluna2.TrimStart('0'))
            .ToDictionary(g => g.Key, g => g.OrderBy(c => c.Ccoluna3.TrimStart('0')).ToList());

        var gruposAdministrativo = TabelaExcel
            .GroupBy(a => a.Acoluna1.TrimStart('0'))
            .ToDictionary(g => g.Key, g => g.OrderBy(a => a.Acoluna2.TrimStart('0')).ToList());

        // Verificar discrepâncias entre Contracheque e Administrativo
        foreach (var grupo in gruposContracheque)
        {
            var cpf = grupo.Key;
            var contracheques = grupo.Value;

            if (gruposAdministrativo.TryGetValue(cpf, out var administrativos))
            {
                // Ajustar para comparar apenas o mínimo de linhas correspondentes
                var count = Math.Min(contracheques.Count, administrativos.Count);

                for (int i = 0; i < count; i++)
                {
                    var matriculaContracheque = contracheques[i].Ccoluna3.TrimStart('0');
                    var matriculaAdministrativo = administrativos[i].Acoluna2.TrimStart('0');

                    // Registrar discrepância apenas se as matrículas não forem iguais
                    if (matriculaContracheque != matriculaAdministrativo)
                    {
                        discrepancias.Add((
                            Ccoluna2: cpf,
                            Acoluna2: matriculaAdministrativo,
                            Ccoluna3: matriculaContracheque
                        ));
                    }
                }

                // Ignorar linhas extras no Administrativo
                // Nenhuma ação é necessária aqui
            }
            else
            {
                // Caso o CPF do Contracheque não exista no Administrativo
                foreach (var contracheque in contracheques)
                {
                    discrepancias.Add((
                        Ccoluna2: cpf,
                        Acoluna2: "Não encontrado no Administrativo",
                        Ccoluna3: contracheque.Ccoluna3.TrimStart('0')
                    ));
                }
            }
        }

        // Se não houver discrepâncias, encerrar a execução
        if (!discrepancias.Any())
        {
            Console.WriteLine("Nenhuma discrepância encontrada. Arquivo não foi gerado.");
            return;
        }

        // Ordenar as discrepâncias por CPF e matrícula para garantir a ordem correta
        var discrepanciasOrdenadas = discrepancias
            .OrderBy(d => d.Ccoluna2)  // Ordena pelo CPF
            .ThenBy(d => d.Acoluna2)  // Ordena pela matrícula do Administrativo
            .ToList();

        // Gerar o arquivo MATRICULA.txt
        var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        var filePath = Path.Combine(desktopPath, "MATRICULA.txt");

        // Escrever as discrepâncias finais no arquivo
        await using (var writer = new StreamWriter(filePath))
        {
            foreach (var discrepancia in discrepanciasOrdenadas)
            {
                await writer.WriteLineAsync($"{discrepancia.Ccoluna2};{discrepancia.Acoluna2};{discrepancia.Ccoluna3}");
            }
        }

        // Verificar se o arquivo foi gerado corretamente
        if (new FileInfo(filePath).Length == 0)
        {
            File.Delete(filePath); // Excluir o arquivo vazio
            Console.WriteLine("Arquivo vazio detectado e excluído.");
            return;
        }

        Console.WriteLine($"Arquivo salvo em: {filePath}");

        // Atualizar os valores no banco de dados com as discrepâncias encontradas
        foreach (var discrepancia in discrepanciasOrdenadas)
        {
            var administrativoParaAtualizar = TabelaExcel.FirstOrDefault(a =>
                a.Acoluna1.TrimStart('0') == discrepancia.Ccoluna2 &&
                a.Acoluna2.TrimStart('0') == discrepancia.Acoluna2);

            if (administrativoParaAtualizar != null)
            {
                // Atualizar a matrícula
                administrativoParaAtualizar.Acoluna2 = discrepancia.Ccoluna3;
            }
        }

        // Salvar alterações no banco
        await _context.SaveChangesAsync();
        Console.WriteLine("Tabela Administrativo atualizada com sucesso.");
    }

}
