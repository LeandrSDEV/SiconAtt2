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

        //==================================  CÁLCULO =================================//

        public async Task GeradorPerfilCalculo(Status statusSelecionado)
        {
            var tabelaExcel = await _bancoContext.Administrativo.ToListAsync();
            var resultado = new List<string>();

            foreach (var linha in tabelaExcel)
            {
                var valorAcoluna6 = linha.Acoluna6?.Split('-').LastOrDefault()?.Trim();
                var acoluna5 = linha.Acoluna5.Trim(); // Normaliza o valor para evitar múltiplos trims no switch

                var valorResultado = statusSelecionado switch
                {
                    Status.PREF_XiqueXique_BA => "265",
                    Status.PREF_Alcinópolis_MS => "794",
                    Status.FUNPREBO_Bodoco_PE => "391",
                    Status.PREF_Remanso_BA => "353",
                    Status.PREF_Vicosa_AL => "149",
                    Status.PREF_Cupira_PE => acoluna5 switch
                    {
                        "33" or "2" => "926",
                        _ => "943"
                    },                    
                    Status.PREF_Cansanção_BA => acoluna5 switch
                    {
                        "10" => "128",
                        _ => "833"
                    },
                    Status.PREF_Abare_BA => acoluna5 switch
                    {
                        "2" => "678",
                        _ => "679"
                    },
                    Status.PREF_Cafarnaum_BA => acoluna5 switch
                    {
                        "1" or "10" => "936",
                        _ => "937"
                    },
                    Status.PREF_Indiaporã_SP => acoluna5 switch
                    {
                        "2" => "964",
                        _ => "965"
                    },
                    Status.PREF_Anadia_AL => acoluna5 switch
                    {
                        "2" => "171",
                        _ => "329"
                    },
                    Status.PREF_BeloMonte_AL => acoluna5 switch
                    {
                        "1" or "4" => "636",
                        "2" => "624",
                        _ => "625"
                    },
                    Status.PREF_CabaceiraDoParaguacu_BA => acoluna5 switch
                    {
                        "1" or "2" or "15" => "987",
                        _ => "988"
                    },
                    Status.PREF_Bodoco_PE => acoluna5 switch
                    {
                        "1" or "2" => "390",
                        _ => "392"
                    },
                    Status.FMS_Cupira_PE => acoluna5 switch
                    {
                        "2" => "928",
                        _ => "996"
                    },
                    Status.PREF_Canarana_BA => acoluna5 switch
                    {
                        "10" => "264",
                        _ => "269"
                    },
                    Status.FAPEN_SaoJoseDaSaje_AL => acoluna5 switch
                    {
                        "1" or "4" => "766",
                        _ => "767"
                    },
                    Status.PREF_Miranda_MS => acoluna5 switch
                    {
                        "1" or "2" or "4" or "9" => "299",
                        _ => "309"
                    },
                    Status.PREF_SantaMariaDaVitoria_BA => acoluna5 switch
                    {
                        "1" or "4" or "10" or "13" => "1009",
                        _ => "1010"
                    },
                    Status.PREF_Catu_BA => acoluna5 switch
                    {
                        "10" or "17" => "994",
                        _ => "995"
                    },
                    Status.FUNDO_Moncao_MA => acoluna5 switch
                    {
                        "2" or "16" => "925",
                        _ => "938"
                    },
                    Status.PREF_Lamarao_BA => acoluna5 switch
                    {
                        "10" or "15" => "953",
                        _ => "954"
                    },
                    Status.PREF_GirauDoPonciano => acoluna5 switch
                    {
                        "1" or "2" or "4" => "793",
                        _ => "743"
                    },
                    Status.PREF_Cambira_PR => (acoluna5, linha.Acoluna4.Trim()) switch
                    {
                        ("1", "PREFEITURA") or ("13", "PREFEITURA") or ("17", "PREFEITURA")
                        or ("4", "PREFEITURA") or ("9", "PREFEITURA") or ("10", "PREFEITURA") or ("7", "PREFEITURA") => "1014",

                        ("1", "SAÚDE") or ("13", "SAÚDE") or ("17", "SAÚDE")
                        or ("4", "SAÚDE") or ("9", "SAÚDE") or ("10", "SAÚDE") or ("7", "SAÚDE") => "1015",

                        ("1", "EDUCAÇÃO") or ("13", "EDUCAÇÃO") or ("17", "EDUCAÇÃO" )
                        or ("4", "EDUCAÇÃO") or ("9", "EDUCAÇÃO") or ("10", "EDUCAÇÃO") or ("7", "EDUCAÇÃO") => "1016",

                        _ => "1017"
                    },
                    _ => throw new ArgumentException("Status inválido.")
                };

                if (valorAcoluna6 == valorResultado)
                    continue; // Ignora se já está correto

                resultado.Add($"{linha.Acoluna1};{linha.Acoluna2};{valorResultado}");
            }

            if (!resultado.Any())
            {
                Console.WriteLine("Nenhuma discrepância encontrada. Arquivo não foi gerado.");
                return;
            }

            var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var filePath = Path.Combine(desktopPath, "PERFIL DE CALCULO.txt");

            await File.WriteAllLinesAsync(filePath, resultado);

            Console.WriteLine($"Arquivo salvo em: {filePath}");
        }
    }
}
