using Servidor.Models.Enums;
using Servidor.Models;

namespace Servidor.ErrosService
{
    public abstract class MunicipioServiceBase
    {
        protected abstract string NomeMunicipio { get; }
        protected abstract Dictionary<string, string> VinculoMapeamento { get; }
        protected abstract string DefaultCcoluna18 { get; }
        protected abstract Dictionary<string, string> Ccoluna18Mapeamento { get; }

        // Método CriarContracheque
        protected ContrachequeModel CriarContracheque(string[] colunas, string municipio)
        {
            var contracheque = new ContrachequeModel
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
                Ccoluna12 = "0",
                Ccoluna13 = "0",
                Ccoluna14 = "99999999999",
                Ccoluna15 = colunas[9],
                Ccoluna16 = colunas[16],
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

            return contracheque;
        }

        public Task<List<ContrachequeModel>> ProcessarArquivoAsync(string[] colunas, Status status)
        {
            var contracheque = CriarContracheque(colunas, NomeMunicipio);

            // Valida o nome do município
            if (contracheque.Ccoluna1 == $"PREFEITURA MUNICIPAL DE {NomeMunicipio.ToUpper()}")
            {
                contracheque.Ccoluna21 = "1";
            }

            // Atualiza Ccoluna16 com base no mapeamento
            if (VinculoMapeamento.TryGetValue(colunas[16].Trim(), out var ccoluna16))
            {
                contracheque.Ccoluna16 = ccoluna16;
            }

            // Define Ccoluna18 com base no mapeamento
            if (Ccoluna18Mapeamento.TryGetValue(contracheque.Ccoluna16, out var ccoluna18))
            {
                contracheque.Ccoluna18 = ccoluna18;
            }
            else
            {
                contracheque.Ccoluna18 = DefaultCcoluna18;
            }

            return Task.FromResult(new List<ContrachequeModel> { contracheque });
        }

    }
}
