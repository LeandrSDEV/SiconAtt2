namespace Servidor.ErrosService
{
    public class Preenchimento
    {
        public string MapearVinculo(string vinculo)
        {
            var vinculos = new Dictionary<string, string>
    {
        { "Contratado", "5" },
        { "Comissionado", "7" },
        { "Agente politico", "13" },
        { "Efetivo", "2" },
        { "Inativo", "14" },
        { "Pensionista", "1" },
        { "Cedido", "33" },
        { "Eletivo", "13" },
        { "Temporário", "11"},
        { "Aguardando Especificar", "14" },
        { "Conselheiro Tutelar", "17"},
        { "Estatutário", "10"},
        { "Militar", "14"},
        { "Celetista", "9"},
        { "Efetivo/Cedido", "15"},
        { "Função Pública Relevante", "29"},
        { "Estagiario", "8"},
        { "Aposentado", "4"},
    };

            return vinculos.TryGetValue(vinculo, out var mappedValue) ? mappedValue : "0";
        }
    }
}
