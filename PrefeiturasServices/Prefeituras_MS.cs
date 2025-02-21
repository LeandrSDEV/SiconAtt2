using Servidor.Models.Enums;
using Servidor.Models;

public class ContrachequeMS
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
            Ccoluna10 = "MS",
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
//======================================    ALCINOPÓLIS    ============================================\\

public class AlcinopolisService
{
    private static readonly Dictionary<string, string> Vinculo = new()
    {
        { "Comissionado", "7" },
        { "Concursado", "2" },
        { "Tempo determinado/Processo seletivo simplificado", "5" },
        { "Prefeito", "13" },
        { "Vice-Prefeito", "13" },
        { "Conselho tutelar", "17" },
        { "Membro de Conselho", "17" }
    };

    public Task<List<ContrachequeModel>> ProcessarArquivoAsync(string[] colunas, Status status)
    {
        var contracheque = ContrachequeMS.CriarContracheque(colunas, "ALCINOPOLIS");

        if (contracheque.Ccoluna1 == "PREFEITURA MUNICIPAL DE ALCINOPOLIS")
        {
            contracheque.Ccoluna21 = "1";
        }

        if (contracheque.Ccoluna1 == "FUNDO MUN M.D.ED.V.P.ED-FUNDEB")
        {
            contracheque.Ccoluna21 = "2";
        }

        if (Vinculo.ContainsKey(colunas[16].Trim()))
        {
            contracheque.Ccoluna16 = Vinculo[colunas[16].Trim()];
        }

        switch (contracheque.Ccoluna16)
        {           
            default:
                contracheque.Ccoluna18 = "794";
                break;
        }


        return Task.FromResult(new List<ContrachequeModel> { contracheque });
    }
}

//======================================    MIRANDA    ============================================\\

public class MirandaService
{
    private static readonly Dictionary<string, string> Vinculo = new()
    {
        { "EFETIVOS", "2" },    
        { "COMISSIONADO", "7" },    
        { "ELETIVO", "13" },    
        { "CELETISTAS", "9" },    
        { "CONTRATADOS", "5" },    
        { "PENSIONISTAS", "1" },    
        { "APOSENTADOS", "4" },    
        { "CONVOCADOS", "7" },    
    };

    public Task<List<ContrachequeModel>> ProcessarArquivoAsync(string[] colunas, Status status)
    {
        var contracheque = ContrachequeMS.CriarContracheque(colunas, "MIRANDA");

        if (contracheque.Ccoluna1 == "PREFEITURA DO MUNICIPIO DE MIRANDA")
        {
            contracheque.Ccoluna21 = "1";
        }

        if (Vinculo.ContainsKey(colunas[16].Trim()))
        {
            contracheque.Ccoluna16 = Vinculo[colunas[16].Trim()];
        }

        switch (contracheque.Ccoluna16)
        {   
            case "1":
            case "2":
            case "4":
            case "9":
                contracheque.Ccoluna18 = "299";
                break;
            default:
                contracheque.Ccoluna18 = "309";
                break;
        }


        return Task.FromResult(new List<ContrachequeModel> { contracheque });
    }
}
