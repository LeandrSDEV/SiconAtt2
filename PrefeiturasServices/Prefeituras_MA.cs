using Servidor.Models.Enums;
using Servidor.Models;

public class ContrachequeMA
{
    public static ContrachequeModel CriarContracheque(string[] colunas, string municipio)
    {
        return new ContrachequeModel
        {
            Ccoluna1 = colunas[7],
            Ccoluna2 = colunas[3],
            Ccoluna3 = colunas[4],
            Ccoluna4 = colunas[5],
            Ccoluna5 = "Rua A",
            Ccoluna6 = "S/N",
            Ccoluna7 = "CASA",
            Ccoluna8 = "CENTRO",
            Ccoluna9 = municipio,
            Ccoluna10 = "MA",
            Ccoluna11 = "99999999",
            Ccoluna12 = "99999999999",
            Ccoluna13 = "99999999999",
            Ccoluna14 = "99999999999",
            Ccoluna15 = colunas[9],
            Ccoluna16 = string.IsNullOrEmpty(colunas[16]) ? "14" : colunas[16],
            Ccoluna17 = "0",
            Ccoluna18 = colunas[18],
            Ccoluna19 = "0",
            Ccoluna20 = "Teste@gmail.com",
            Ccoluna21 = colunas[19],
            Ccoluna22 = "0",
            Ccoluna23 = colunas[10],
            Ccoluna24 = "0",
            Ccoluna25 = "0"
        };
    }
}
//======================================    FUNDO DE SAÚDE DE MONÇÃO    ============================================\\

public class FundoMoncaoService
{
    private static readonly Dictionary<string, string> Vinculo = new()
    {
        { "CONTRATADO", "5" },
        { "COMISSIONADOS", "7" },
        { "CONCURSADOS", "2" },
        { "ESTAVEIS", "16" },
    };

    public Task<List<ContrachequeModel>> ProcessarArquivoAsync(string[] colunas, Status status)
    {
        var contracheque = ContrachequeMA.CriarContracheque(colunas, "MONCAO");

        if (contracheque.Ccoluna1 == "FUNDO MUNICIPAL DE SAUDE")
        {
            contracheque.Ccoluna21 = "7";
        }

        if (Vinculo.ContainsKey(colunas[16].Trim()))
        {
            contracheque.Ccoluna16 = Vinculo[colunas[16].Trim()];
        }

        switch (contracheque.Ccoluna16)
        {
            case "2":
            case "16":
                contracheque.Ccoluna18 = "925";
                break;
            default:
                contracheque.Ccoluna18 = "938";
                break;
        }


        return Task.FromResult(new List<ContrachequeModel> { contracheque });
    }
}
