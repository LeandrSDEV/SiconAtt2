using Servidor.Models.Enums;
using Servidor.Models;

public class ContrachequeBA
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
            Ccoluna10 = "BA",
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

//======================================    ABARE    ============================================\\

public class AbareService
{
    private static readonly Dictionary<string, string> MapeamentoStatus = new()
    {
        { "Efetivo", "2" },
        { "Conselho Tutelar", "17" },
        { "Processo Seletivo", "11" },
        { "Trabalhador Temporário", "11" },
        { "Cargo em Comissão", "7" },
        { "Agente Político", "13" }
    };

    public Task<List<ContrachequeModel>> ProcessarArquivoAsync(string[] colunas, Status status)
    {
        var contracheque = ContrachequeBA.CriarContracheque(colunas, "ABARE");

        if (contracheque.Ccoluna1 == "PREFEITURA MUNICIPAL DE ABARE")
        {
            contracheque.Ccoluna21 = "1";
        }

        // Verifica e atualiza Ccoluna16 com base no mapeamento
        if (MapeamentoStatus.ContainsKey(colunas[16].Trim()))
        {
            contracheque.Ccoluna16 = MapeamentoStatus[colunas[16].Trim()];
        }

        switch (contracheque.Ccoluna16)
        {           
            case "2":
                contracheque.Ccoluna18 = "678";
                break;
            default:
                contracheque.Ccoluna18 = "679";
                break;
        }

        return Task.FromResult(new List<ContrachequeModel> { contracheque });
    }
}

//======================================    XIQUE-XIQUE    ============================================\\

public class XiqueXiqueService
{
    private static readonly Dictionary<string, string> Vinculo = new()
    {
        { "Estatutário", "10" },
        { "Agente Político", "13" },
        { "Cedidos", "33" },
        { "Cargo em Comissão", "7" },
        { "TRABALHADOR TEMPORÁRIO", "11" },
        { "Conselho Tutelar", "17" }
    };

    public Task<List<ContrachequeModel>> ProcessarArquivoAsync(string[] colunas, Status status)
    {
        var contracheque = ContrachequeBA.CriarContracheque(colunas, "XIQUEXIQUE");

        if (contracheque.Ccoluna1 == "PREFEITURA MUNICIPAL DE XIQUE XIQUE")
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
                contracheque.Ccoluna18 = "265";
                break;
        }


        return Task.FromResult(new List<ContrachequeModel> { contracheque });
    }
}

//======================================    CAFARNAUM    ============================================\\

public class CafarnaumService
{
    private static readonly Dictionary<string, string> Vinculo = new()
    {
        { "Cargo em Comiss?o", "7" },
        { "Estatut?rio", "10" },
        { "Agente Pol?tico", "13" },
        { "Pensionista", "1" },
        { "Trabalhador Tempor?rio", "11" },
        { "CONSELHO TUTELAR", "17" }
    };

    public Task<List<ContrachequeModel>> ProcessarArquivoAsync(string[] colunas, Status status)
    {
        var contracheque = ContrachequeBA.CriarContracheque(colunas, "CAFARNAUM");

        if (contracheque.Ccoluna1 == "PREFEITURA MUNICIPAL DE CAFARNAUM")
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
            case "10":
                contracheque.Ccoluna18 = "936";
                break;
            default:
                contracheque.Ccoluna18 = "937";
                break;
        }


        return Task.FromResult(new List<ContrachequeModel> { contracheque });
    }
}

//======================================    CANSANÇÃO    ============================================\\

public class CansancaoService
{
    private static readonly Dictionary<string, string> MapeamentoStatus = new()
    {
       { "Estatutário", "10" },
        { "Agente Político", "13" },
        { "Trabalhador Temporário", "11" },
        { "Cargo em Comissão", "7" },
        { "Conselho Tutelar", "17" }
    };

    public Task<List<ContrachequeModel>> ProcessarArquivoAsync(string[] colunas, Status status)
    {
        var contracheque = ContrachequeBA.CriarContracheque(colunas, "CANSANCAO");

        if (contracheque.Ccoluna1 == "PREFEITURA MUNICIPAL DE CANSANCAO")
        {
            contracheque.Ccoluna19 = "1";
        }

        // Verifica e atualiza Ccoluna16 com base no mapeamento
        if (MapeamentoStatus.ContainsKey(colunas[16].Trim()))
        {
            contracheque.Ccoluna16 = MapeamentoStatus[colunas[16].Trim()];
        }

        switch (contracheque.Ccoluna16)
        {
            case "10":
                contracheque.Ccoluna18 = "128";
                break;
            default:
                contracheque.Ccoluna18 = "833";
                break;
        }

        return Task.FromResult(new List<ContrachequeModel> { contracheque });
    }
   
}
//======================================    CATU    ============================================\\

public class CatuService
{
    private static readonly Dictionary<string, string> MapeamentoStatus = new()
    {
       { "Estatutario", "10" },
       { "Regime Administrativo", "18" },
       { "Comissionado", "7" },
       { "Estagiario", "8" },
       { "Agente Politico", "13" },
       { "Aposentado", "4" },
       { "CONSELHO TUTELAR", "17" },
       
    };

    public Task<List<ContrachequeModel>> ProcessarArquivoAsync(string[] colunas, Status status)
    {
        var contracheque = ContrachequeBA.CriarContracheque(colunas, "CATU");

        if (contracheque.Ccoluna1 == "PREFEITURA MUNICIPAL DE CATU")
        {
            contracheque.Ccoluna19 = "1";
        }

        // Verifica e atualiza Ccoluna16 com base no mapeamento
        if (MapeamentoStatus.ContainsKey(colunas[16].Trim()))
        {
            contracheque.Ccoluna16 = MapeamentoStatus[colunas[16].Trim()];
        }

        switch (contracheque.Ccoluna16)
        {
            case "10":
            case "17":
                contracheque.Ccoluna18 = "994";
                break;
            default:
                contracheque.Ccoluna18 = "995";
                break;
        }

        return Task.FromResult(new List<ContrachequeModel> { contracheque });
    }

}

//======================================    REMANSO    ============================================\\

public class RemansoService
{
    private static readonly Dictionary<string, string> MapeamentoStatus = new()
    {
       { "Estatutário - Comissão", "15" },
       { "Estatutário", "10" },
       { "Estável", "2" },
       { "Trabalhador Temporário", "11" },
       { "Agente Político", "13" },
       { "Cargo em Comissão", "7" },
       { "Processo Seletivo", "11" },
       { "Conselho Tutelar", "17" },
       
    };

    public Task<List<ContrachequeModel>> ProcessarArquivoAsync(string[] colunas, Status status)
    {
        var contracheque = ContrachequeBA.CriarContracheque(colunas, "REMANSO");

        if (contracheque.Ccoluna1 == "PREFEITURA MUNICIPAL DE REMANSO")
        {
            contracheque.Ccoluna19 = "1";
        }

        // Verifica e atualiza Ccoluna16 com base no mapeamento
        if (MapeamentoStatus.ContainsKey(colunas[16].Trim()))
        {
            contracheque.Ccoluna16 = MapeamentoStatus[colunas[16].Trim()];
        }

        switch (contracheque.Ccoluna16)
        {
            default:
                contracheque.Ccoluna18 = "353";
                break;
        }

        return Task.FromResult(new List<ContrachequeModel> { contracheque });
    }

}
//======================================    SANTA MARIA DA VITORIA    ============================================\\

public class SantaMariaVitoriaService
{
    private static readonly Dictionary<string, string> MapeamentoStatus = new()
    {
       { "Estatutário", "10" },
       { "Aposentado", "4" },
       { "Pensionista", "1" },
       { "Agente Político", "13" },
       { "Cargo em Comissão", "7" },
       { "Trabalhador Temporário", "11" },
       { "Cedidos", "33" },
       { "Conselho Tutelar", "17" },
    };

    public Task<List<ContrachequeModel>> ProcessarArquivoAsync(string[] colunas, Status status)
    {
        var contracheque = ContrachequeBA.CriarContracheque(colunas, "SANTA MARIA DA VITORIA");

        if (contracheque.Ccoluna1 == "PREFEITURA DE SANTA MARIA DA VITORIA")
        {
            contracheque.Ccoluna19 = "1";
        }

        if (MapeamentoStatus.ContainsKey(colunas[16].Trim()))
        {
            contracheque.Ccoluna16 = MapeamentoStatus[colunas[16].Trim()];
        }

        switch (contracheque.Ccoluna16)
        {
            case "1":
            case "10":
            case "4":
            case "13":
                contracheque.Ccoluna18 = "1009";
                break;
            default:
                contracheque.Ccoluna18 = "1010";
                break;
        }

        return Task.FromResult(new List<ContrachequeModel> { contracheque });
    }

}
//======================================    CABACEIRAS DO PARAGUAÇU    ============================================\\

public class CabaceiraParaguacuService
{
    private static readonly Dictionary<string, string> MapeamentoStatus = new()
    {
       { "Efetivo", "2" },
       { "Efetivo/Comissionado", "15" },
       { "Pensionista", "1" },
       { "Efetivo/ Agente Politico", "15" },
       { "Contrato", "5" },
       { "Comissionado", "7" },
       { "Conselheiro Tutelar", "17" },
       { "Eleito", "13" },
       { "Agente Político", "13" },
       { "Celetista", "9" },
    };

    public Task<List<ContrachequeModel>> ProcessarArquivoAsync(string[] colunas, Status status)
    {
        var contracheque = ContrachequeBA.CriarContracheque(colunas, "CABACEIRAS DO PARAGUAÇU");

        if (contracheque.Ccoluna1 == "PREFEITURA MUNICIPAL DE CABACEIRAS DO PARAGUACU")
        {
            contracheque.Ccoluna19 = "1";
        }

        if (MapeamentoStatus.ContainsKey(colunas[16].Trim()))
        {
            contracheque.Ccoluna16 = MapeamentoStatus[colunas[16].Trim()];
        }

        switch (contracheque.Ccoluna16)
        {
            case "1":
            case "2":
            case "15":
                contracheque.Ccoluna18 = "987";
                break;
            default:
                contracheque.Ccoluna18 = "988";
                break;
        }

        return Task.FromResult(new List<ContrachequeModel> { contracheque });
    }

}
