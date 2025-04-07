using Microsoft.EntityFrameworkCore;
using Servidor.Data;
using Servidor.Models;

namespace Servidor.ErrosService
{
    public class PerfilCalculo
    {
        private readonly BancoContext _bancoContext;

        public PerfilCalculo(BancoContext bancoContext)
        {
            _bancoContext = bancoContext;
        }

        //============================== PERFIL DE CÁLCULO ==============================//
        public async Task GeradorPerfilCalculo()
        {
            var tabelaTxt = await _bancoContext.Contracheque.AsNoTracking().ToListAsync();
            var tabelaExcel = await _bancoContext.Administrativo.AsNoTracking().ToListAsync();

            var duplicados = tabelaExcel
                .Where(a => !string.IsNullOrWhiteSpace(a.Acoluna1) && !string.IsNullOrWhiteSpace(a.Acoluna2))
                .GroupBy(a => $"{a.Acoluna1.Trim()}{a.Acoluna2.Trim()}")
                .Where(g => g.Count() > 1)
                .ToList();

            var duplicidadesDiscrepantes = new List<string>();

            // Dicionário final ignorando duplicatas (pega a primeira ocorrência apenas)
            var administrativosMap = new Dictionary<string, (string ValorNumerico, AdministrativoModel Entidade)>();

            foreach (var grupo in tabelaExcel
                .Where(a => !string.IsNullOrWhiteSpace(a.Acoluna1) && !string.IsNullOrWhiteSpace(a.Acoluna2))
                .GroupBy(a => $"{a.Acoluna1.Trim()}{a.Acoluna2.Trim()}"))
            {
                var chave = grupo.Key;
                var primeiro = grupo.First();
                administrativosMap[chave] = (ExtrairNumeros(primeiro.Acoluna6), primeiro);

                if (grupo.Count() > 1)
                {
                    duplicidadesDiscrepantes.Add(chave);
                }
            }

            var discrepancias = new List<ContrachequeModel>();

            foreach (var linha in tabelaTxt)
            {
                if (string.IsNullOrWhiteSpace(linha.Ccoluna2) || string.IsNullOrWhiteSpace(linha.Ccoluna3))
                    continue;

                var chaveTxt = $"{linha.Ccoluna2.Trim()}{linha.Ccoluna3.Trim()}";
                var valorNumericoTxt = ExtrairNumeros(linha.Ccoluna18);

                if (administrativosMap.TryGetValue(chaveTxt, out var administrativo))
                {
                    if (administrativo.ValorNumerico != valorNumericoTxt)
                    {
                        discrepancias.Add(linha);
                    }
                }
                else
                {
                    // Se não encontrar a chave, também considera discrepância
                    discrepancias.Add(linha);
                }
            }

            var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            // Salvar discrepâncias de valor SEM duplicação por CPF + Matrícula
            if (discrepancias.Any())
            {
                var filePath = Path.Combine(desktopPath, "PERFIL DE CALCULO.txt");
                using var writer = new StreamWriter(filePath);

                var chavesRegistradas = new HashSet<string>();

                foreach (var item in discrepancias)
                {
                    var cpf = item.Ccoluna2?.Trim() ?? "";
                    var matricula = item.Ccoluna3?.Trim() ?? "";
                    var valor = item.Ccoluna18?.Trim() ?? "";

                    var chaveUnica = $"{cpf}{matricula}";

                    if (!chavesRegistradas.Contains(chaveUnica))
                    {
                        await writer.WriteLineAsync($"{cpf};{matricula};{valor}");
                        chavesRegistradas.Add(chaveUnica);
                    }
                }

                Console.WriteLine($"✅ Arquivo 'PERFIL DE CALCULO.txt' gerado com {chavesRegistradas.Count} discrepâncias únicas.");
            }
            else
            {
                Console.WriteLine("✅ Nenhuma discrepância de valor encontrada.");
            }
        
        }

        private string ExtrairNumeros(string input)
        {
            return string.IsNullOrWhiteSpace(input)
                ? ""
                : string.Concat(input.Where(char.IsDigit));
        }
    }
}
