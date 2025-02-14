using Servidor.Models.Enums;
using Servidor.Models;
using Servidor.ErrosService;

//======================================    ABARE    ============================================\\

public class AbareService : MunicipioServiceBase
{
    protected override string NomeMunicipio => "ABARE";
    protected override Dictionary<string, string> VinculoMapeamento => new Dictionary<string, string>
    {
        { "Efetivo", "2" },
        { "Conselho Tutelar", "17" },
        { "Processo Seletivo", "5" },
        { "Trabalhador Temporário", "11" },
        { "Cargo em Comissão", "7" },
        { "Agente Político", "13" }
        // Adicione mais mapeamentos conforme necessário
    };

    protected override string DefaultCcoluna18 => "679"; // Defina um valor padrão para Ccoluna18

    protected override Dictionary<string, string> Ccoluna18Mapeamento => new Dictionary<string, string>
    {
        { "2", "678" },
        { "7", "679" }
        // Adicione mais mapeamentos conforme necessário
    };
}

//======================================    ABARE    ============================================\\

public class CupiraService : MunicipioServiceBase
{
    protected override string NomeMunicipio => "CUPIRA";
    protected override Dictionary<string, string> VinculoMapeamento => new Dictionary<string, string>
    {
        { "Efetivo", "2" },
        { "Conselho Tutelar", "17" },
        { "Processo Seletivo", "5" },
        { "Trabalhador Temporário", "11" },
        { "Cargo em Comissão", "7" },
        { "Agente Político", "13" }
        // Adicione mais mapeamentos conforme necessário
    };

    protected override string DefaultCcoluna18 => "679"; // Defina um valor padrão para Ccoluna18

    protected override Dictionary<string, string> Ccoluna18Mapeamento => new Dictionary<string, string>
    {
        { "2", "678" },
        { "7", "679" }
        // Adicione mais mapeamentos conforme necessário
    };
}
