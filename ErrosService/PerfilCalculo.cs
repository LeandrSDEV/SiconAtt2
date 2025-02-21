using Microsoft.EntityFrameworkCore;
using Servidor.Data;
using Servidor.Models.Enums;

namespace Servidor.ErrosService
{
    public class PerfilCalculo
    {
        private readonly BancoContext _bancoContext;

        public PerfilCalculo(BancoContext bancoContext)
        {
            _bancoContext = bancoContext;
        }

        //==================================  CALCULO =================================//

        public async Task GeradorPerfilCalculo(Status statusSelecionado)
        {
            // Obter os dados da tabela Administrativo
            var TabelaExcel = await _bancoContext.Administrativo.ToListAsync();

            var resultado = new List<string>();

            // Lógica para determinar o valor esperado baseado no statusSelecionado e Acoluna5
            foreach (var linha in TabelaExcel)
            {
                // Normalizar Acoluna6 para comparação, ignorando texto adicional
                var valorAcoluna6 = linha.Acoluna6?.Split('-').LastOrDefault()?.Trim();

                // Lógica para determinar o valor esperado com base no statusSelecionado
                var valorResultado = statusSelecionado switch
                {
                    Status.PREF_Cupira_PE => linha.Acoluna5.Trim() switch
                    {
                        "33" => "926",
                        "2" => "926",
                        _ => "943"
                    },
                    Status.PREF_Alcinópolis_MS => linha.Acoluna5.Trim() switch
                    {
                        _ => "794"
                    },
                    Status.PREF_Cansanção_BA => linha.Acoluna5.Trim() switch
                    {
                        "10" => "128",
                        _ => "833"
                    },
                    Status.PREF_Abare_BA => linha.Acoluna5.Trim() switch
                    {
                        "2" => "678",
                        _ => "679"
                    },
                    Status.PREF_Cafarnaum_BA => linha.Acoluna5.Trim() switch
                    {
                        "1" => "936",
                        "10" => "936",
                        _ => "937"
                    },
                    Status.PREF_Indiaporã_SP => linha.Acoluna5.Trim() switch
                    {
                        "2" => "964",
                        _ => "965"
                    },
                    Status.PREF_Anadia_AL => linha.Acoluna5.Trim() switch
                    {
                        "2" => "171",
                        _ => "329"
                    },
                    Status.PREF_BeloMonte_AL => linha.Acoluna5.Trim() switch
                    {                       
                    "1" => "636",
                    "4" => "636",
                    "2" => "624",
                        _ => "625"
                    },
                    Status.PREF_CabaceiraDoParaguacu_BA => linha.Acoluna5.Trim() switch
                    {                       
                    "1" => "987",
                    "2" => "987",
                    "15" => "987",
                        _ => "988"
                    },
                    Status.FUNPREBO_Bodoco_PE => linha.Acoluna5.Trim() switch
                    {                      
                        _ => "391"
                    },
                    Status.PREF_Remanso_BA => linha.Acoluna5.Trim() switch
                    {                   
                        _ => "353"
                    },
                    Status.PREF_Bodoco_PE => linha.Acoluna5.Trim() switch
                    {
                        "2" => "390",
                        "1" => "390",
                        _ => "392"
                    },
                    Status.FMS_Cupira_PE => linha.Acoluna5.Trim() switch
                    {
                        "2" => "928",
                        _ => "996"
                    },
                    Status.FAPEN_SaoJoseDaSaje_AL => linha.Acoluna5.Trim() switch
                    {
                        "1" => "766",
                        _ => "767"
                    },
                    Status.PREF_Miranda_MS => linha.Acoluna5.Trim() switch
                    {
                        "1" => "299",
                        "2" => "299",
                        "4" => "299",
                        "9" => "299",
                        _ => "309"
                    },
                    Status.PREF_SantaMariaDaVitoria_BA => linha.Acoluna5.Trim() switch
                    {
                        "1" => "1009",
                        "4" => "1009",
                        "10" => "1009",
                        "13" => "1009",
                        _ => "1010"
                    },
                    Status.PREF_Catu_BA => linha.Acoluna5.Trim() switch
                    {
                        "17" => "994",
                        "10" => "994",
                        _ => "995"
                    },
                    Status.FUNDO_Moncao_MA => linha.Acoluna5.Trim() switch
                    {
                        "2" => "925",
                        "16" => "925",
                        _ => "938"
                    },
                    Status.PREF_GirauDoPonciano => linha.Acoluna5.Trim() switch
                    {
                        "1" => "793",
                        "2" => "793",
                        "4" => "793",
                        _ => "743"
                    },
                    // Adicione mais status e suas respectivas lógicas conforme necessário
                    _ => throw new ArgumentException("Status inválido.")
                };

                // Verificar se a linha já possui o valor correto na Acoluna6 (ignorar se já está correto)
                if (valorAcoluna6 == valorResultado)
                {
                    continue; // Ignorar a linha se já está com o valor esperado em Acoluna6
                }

                // Adicionar a linha no formato esperado ao resultado
                resultado.Add($"{linha.Acoluna1};{linha.Acoluna2};{valorResultado}");
            }

            // Se não houver resultado, não criar o arquivo
            if (!resultado.Any())
            {
                Console.WriteLine("Nenhuma discrepância encontrada. Arquivo não foi gerado.");
                return;
            }

            // Gerar o arquivo PERFIL_DE_CALCULO.txt
            var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var filePath = Path.Combine(desktopPath, "PERFIL DE CALCULO.txt");

            await File.WriteAllLinesAsync(filePath, resultado);

            Console.WriteLine($"Arquivo salvo em: {filePath}");
        }
    }
}
