using Servidor.Models.Enums;
using Servidor.Models;

public class ContrachequePE
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
            Ccoluna10 = "PE",
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
//======================================    CUPIRA    ============================================\\

public class CupiraService
{
    private static readonly Dictionary<string, string> Vinculo = new()
    {
        { "Cargo Comissionado", "7" },
        { "Cargo Efetivo", "2" },
        { "SERVIDOR EFETIVO CEDIDO DE OUTRA ENTIDADE", "33" },
        { "EFETIVO CEDIDO LAGOA DOS GATOS", "33" },
        { "ELETIVOS", "13" },
        { "Contratados", "5" },
        { "PENSIONISTA", "1" },
        { "INATIVOS", "28" }
    };

    public Task<List<ContrachequeModel>> ProcessarArquivoAsync(string[] colunas, Status status)
    {
        var contracheque = ContrachequePE.CriarContracheque(colunas, "CUPIRA");

        if (contracheque.Ccoluna1 == "PREFEITURA MUNICIPAL DE CUPIRA")
        {
            contracheque.Ccoluna21 = "1";
        }

        if (Vinculo.ContainsKey(colunas[16].Trim()))
        {
            contracheque.Ccoluna16 = Vinculo[colunas[16].Trim()];
        }

        switch (contracheque.Ccoluna16)
        {           
            case "33":
            case "2":
                contracheque.Ccoluna18 = "926";
                break;
            default:
                contracheque.Ccoluna18 = "943";
                break;
        }


        return Task.FromResult(new List<ContrachequeModel> { contracheque });
    }
}
//======================================    FUNPREBO - BODOCÓ    ============================================\\

public class FUNBodocoService
{
    private static readonly Dictionary<string, string> Vinculo = new()
    {        
        { "PREVIDENCIA - RPPS - Aposentados", "4" },
        { "PREVIDENCIA - RPPS - Pensionistas", "1" },
        { "COMISSIONADO", "7" }
    };

    public Task<List<ContrachequeModel>> ProcessarArquivoAsync(string[] colunas, Status status)
    {
        var contracheque = ContrachequePE.CriarContracheque(colunas, "BODOCO");

        if (contracheque.Ccoluna1 == "FUNDO PREVIDENCIARIO DE BODOCÓ")
        {
            contracheque.Ccoluna21 = "1";
        }

        if (Vinculo.ContainsKey(colunas[16].Trim()))
        {
            contracheque.Ccoluna16 = Vinculo[colunas[16].Trim()];
        }

        switch (contracheque.Ccoluna16)
        {
            default:
                contracheque.Ccoluna18 = "391";
                break;
        }


        return Task.FromResult(new List<ContrachequeModel> { contracheque });
    }
}
//======================================    PREFEITURA DE BODOCÓ    ============================================\\

public class BodocoService
{
    private static readonly Dictionary<string, string> Vinculo = new()
    {
        { "CONSELHO TUTELAR DE BODOCO", "17" },
        { "COMISSIONADO", "7" },
        { "EFETIVOS", "2" },
        { "EXECUTIVO MUNICIPAL", "13" },
        { "CEDIDOS DE SERRITA-PE", "33" },
        { "BENEFICIO - AUXILIO RECLUSÃO", "13" },
        { "PENSAO SOCIAL", "1" },
        { "PERMUTA DE  IPUBI", "6" },
        { "CEDIDOS PICOS - PI", "33" },
        { "CONTRATADOS", "5" },
        { "CEDIDOS DE OURICURI", "33" },
        { "CEDIDOS - TRINDADE-PE", "33" },
        { "CEDIDOS ARARIPE-CE", "33" },
        { "PERMUTA DE  GRANITO", "6" },
        { "CEDIDOS -SANTA CRUZ PE", "33" },
    };

    public Task<List<ContrachequeModel>> ProcessarArquivoAsync(string[] colunas, Status status)
    {
        var contracheque = ContrachequePE.CriarContracheque(colunas, "BODOCO");

        if (contracheque.Ccoluna1 == "PREFEITURA MUNICIPAL DE BODOCO")
        {
            contracheque.Ccoluna21 = "1";
        }

        if (Vinculo.ContainsKey(colunas[16].Trim()))
        {
            contracheque.Ccoluna16 = Vinculo[colunas[16].Trim()];
        }

        switch (contracheque.Ccoluna16)
        {
            case "2":
            case "1":
                contracheque.Ccoluna18 = "390";
                break;
            default:
                contracheque.Ccoluna18 = "392";
                break;
        }


        return Task.FromResult(new List<ContrachequeModel> { contracheque });
    }
}
//======================================    FMS - CUPiRA    ============================================\\

public class FMSCupiraService
{
    private static readonly Dictionary<string, string> Vinculo = new()
    {
        { "Cargo Efetivo", "2" },
        { "Contratados", "5" },
        { "SERVIDOR EFETIVO CEDIDO DE OUTRA ENTIDADE", "33" },
        { "Cargo Comissionado", "7" },
    };

    public Task<List<ContrachequeModel>> ProcessarArquivoAsync(string[] colunas, Status status)
    {
        var contracheque = ContrachequePE.CriarContracheque(colunas, "BODOCO");

        if (contracheque.Ccoluna1 == "FUNDO MUNICIPAL DE SAUDE DE CUPIRA")
        {
            contracheque.Ccoluna21 = "3";
        }

        if (Vinculo.ContainsKey(colunas[16].Trim()))
        {
            contracheque.Ccoluna16 = Vinculo[colunas[16].Trim()];
        }

        switch (contracheque.Ccoluna16)
        {
            case "2":
                contracheque.Ccoluna18 = "928";
                break;
            default:
                contracheque.Ccoluna18 = "996";
                break;
        }


        return Task.FromResult(new List<ContrachequeModel> { contracheque });
    }
}
