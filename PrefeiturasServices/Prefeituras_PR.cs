using Servidor.Models.Enums;
using Servidor.Models;

public class ContrachequePR
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
            Ccoluna10 = "PR",
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
//======================================    CAMBIRA    ============================================\\

public class CambiraService
{
    private static readonly Dictionary<string, string> Vinculo = new()
    {
        { "Efetivo (Estatutário)", "10" },
        { "Comissionado", "7" },
        { "Contrato Prazo Determinado", "5" },
        { "Contrato Prazo Indeterminado", "9" },
        { "Aposentado", "4" },
        { "Pensionista", "1" },
        { "Prefeito e Vice", "13" },
        { "Secretário Municipal", "13" },
        { "Conselheiro Tutelar", "17" },
    };

    public Task<List<ContrachequeModel>> ProcessarArquivoAsync(string[] colunas, Status status)
    {       
        var contracheque = ContrachequeSP.CriarContracheque(colunas, "CAMBIRA");

        if (Vinculo.ContainsKey(colunas[16].Trim()))
        {
            contracheque.Ccoluna16 = Vinculo[colunas[16].Trim()];
        }
        if (contracheque.Ccoluna1 == "AUTARQUIA MUNICIPAL DE EDUCAÇÃO DE CAMBIRA") 
        {
            contracheque.Ccoluna21 = "3";
            switch (contracheque.Ccoluna16)
            {
                case "10":             
                case "7":      
                case "9": 
                case "13":
                case "1": 
                case "17":
                case "4": 
                    contracheque.Ccoluna18 = "1016";
                    break;            
            }
        }
        if (contracheque.Ccoluna1 == "AUTARQUIA MUNICIPAL DE SAUDE DE CAMBIRA")
        {
            contracheque.Ccoluna21 = "2";
            switch (contracheque.Ccoluna16)
            {
                case "10":
                case "7":
                case "9":
                case "13":
                case "1":
                case "17":
                case "4":
                    contracheque.Ccoluna18 = "1015";
                    break;
            }
        }
        if (contracheque.Ccoluna1 == "PREFEITURA MUNICIPAL DE CAMBIRA")
        {
            contracheque.Ccoluna21 = "1";
            switch (contracheque.Ccoluna16)
            {
                case "10":
                case "7":
                case "9":
                case "13":
                case "1":
                case "17":
                case "4":
                    contracheque.Ccoluna18 = "1014";
                    break;
            }
        }
        else 
        {
            contracheque.Ccoluna18 = "1017";
        }

                
        return Task.FromResult(new List<ContrachequeModel> { contracheque });
    }
}
