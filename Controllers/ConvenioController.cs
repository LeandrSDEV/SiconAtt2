using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NPOI.HSSF.UserModel;
using Servidor.Data;
using Servidor.ErrosService;
using Servidor.Models;

namespace Servidor.Controllers
{
    public class ConvenioController : Controller
    {
        private readonly BancoContext _context;
        private readonly ServidorService _servidorService;
        private readonly CategoriaService _categoriaService;
        private readonly MatriculaService _matriculaService;
        private readonly SecretariaService _secretariaService;
        private readonly PerfilCalculo _perfilCalculo;
        private readonly CleanupService _cleanupService;

        public ConvenioController(BancoContext context, ServidorService servidorService, CategoriaService categoriaService,
                                  MatriculaService matriculaService, SecretariaService secretariaService, PerfilCalculo perfilCalculo,
                                  CleanupService cleanupService)
        {
            _context = context;
            _servidorService = servidorService;
            _categoriaService = categoriaService;
            _matriculaService = matriculaService;
            _secretariaService = secretariaService;
            _perfilCalculo = perfilCalculo;
            _cleanupService = cleanupService;
        }

        public IActionResult Index()
        {
            var options = _context.SelectOptions
                .Select(x => new SelectOptionModel
                {
                    Id = x.Id,
                    Nome = x.Nome
                })
                .ToList();

            var statuses = options.Select(option => new SelectListItem
            {
                Value = option.Id.ToString(),
                Text = option.Nome
            }).ToList();

            ViewBag.Statuses = statuses;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ProcessarArquivo(IFormFile arquivoTxt, IFormFile arquivoExcel, int SelectOptionId)
        {
            if (arquivoTxt == null || arquivoTxt.Length == 0 || arquivoExcel == null || arquivoExcel.Length == 0)
            {
                return Json(new { success = false, message = "Erro nos arquivos enviados. Por favor, envie arquivos válidos." });
            }

            if (SelectOptionId == 0)
            {
                return Json(new { success = false, message = "Selecione um município válido." });
            }

            var selectOptionFromDb = await _context.SelectOptions
                .FirstOrDefaultAsync(x => x.Id == SelectOptionId);

            if (selectOptionFromDb == null)
            {
                return Json(new { success = false, message = "Município não encontrado no banco de dados." });
            }

            // Processar os arquivos
            var contracheque = await ProcessarArquivoTxt(arquivoTxt, selectOptionFromDb);
            var administrativo = await ProcessarArquivoExcel(arquivoExcel);

            // Salvar os dados no banco
            if (contracheque.Any()) _context.Contracheque.AddRange(contracheque);
            if (administrativo.Any()) _context.Administrativo.AddRange(administrativo);
            await _context.SaveChangesAsync();

            // Retornar sucesso para o frontend
            return Json(new { success = true });
        }

        //   ============  Coluna 1 ==================

        [HttpGet]
        public async Task<IActionResult> ObterValoresCcoluna1()
        {
            var valores = await _context.Contracheque
                .Select(x => x.Ccoluna1)
                .Distinct()
                .ToListAsync();

            return Json(valores);
        }

        [HttpGet]
        public async Task<IActionResult> ObterValoresCargo()
        {
            var valores1 = await _context.Contracheque
                .Select(x => x.Cargo)
                .Distinct()
                .ToListAsync();

            return Json(valores1);
        }

        [HttpGet]
        public async Task<IActionResult> ObterValoresPrefeituraCategoria()
        {
            var valores = await _context.Contracheque
                .Select(c => new { c.Ccoluna1, c.Ccoluna16 })
                .Distinct()
                .ToListAsync();

            return Json(valores);
        }

        // ============ Atualização Categoria/Cargo ==================

        [HttpPost]
        public async Task<IActionResult> AtualizarValoresCcoluna181([FromBody] List<AtualizacaoCategoriaDto> atualizacoes)
        {
            if (atualizacoes == null || !atualizacoes.Any())
                return BadRequest(new { success = false, message = "Nenhuma atualização recebida." });

            foreach (var item in atualizacoes)
            {
                var registros = await _context.Contracheque
                    .Where(c => c.Ccoluna1 == item.Ccoluna1 && c.Ccoluna16 == item.Ccoluna16)
                    .ToListAsync();

                foreach (var registro in registros)
                {
                    registro.Ccoluna18 = item.Ccoluna18;
                }
            }

            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }


        // ============ Atualização de Ccoluna21 ==================
        [HttpPost]
        public async Task<IActionResult> AtualizarValoresCcoluna21([FromBody] Dictionary<string, string> valoresAtualizados)
        {
            if (valoresAtualizados == null || !valoresAtualizados.Any())
            {
                return Json(new { success = false, message = "Nenhum valor foi enviado para atualização." });
            }

            var chaves = valoresAtualizados.Keys.ToList();

            var registros = await _context.Contracheque
                .Where(x => chaves.Contains(x.Ccoluna1) || chaves.Contains(x.Cargo))
                .ToListAsync();

            foreach (var registro in registros)
            {
                if (valoresAtualizados.ContainsKey(registro.Cargo))
                {
                    registro.Ccoluna21 = valoresAtualizados[registro.Cargo];
                }
                else if (valoresAtualizados.ContainsKey(registro.Ccoluna1))
                {
                    registro.Ccoluna21 = valoresAtualizados[registro.Ccoluna1];
                }
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Valores de Ccoluna21 atualizados com sucesso!" });
        }


        //   ============  Coluna 16 ==================

        [HttpGet]
        public async Task<IActionResult> ObterValoresDistintosCcoluna16()
        {
            var valoresDistintos = await _context.Contracheque
                .Select(x => x.Ccoluna16)
                .Distinct()
                .ToListAsync();

            return Json(valoresDistintos);
        }

        // ============ Atualização de Ccoluna16 ==================
        [HttpPost]
        public async Task<IActionResult> AtualizarValoresCcoluna16([FromBody] Dictionary<string, string> valoresAtualizados)
        {
            if (valoresAtualizados == null || !valoresAtualizados.Any())
            {
                return Json(new { success = false, message = "Nenhum valor foi enviado para atualização." });
            }

            var chaves = valoresAtualizados.Keys.ToList();

            var registros = await _context.Contracheque
                .Where(x => chaves.Contains(x.Ccoluna16))
                .ToListAsync();

            foreach (var registro in registros)
            {
                if (valoresAtualizados.TryGetValue(registro.Ccoluna16, out var novoValor))
                {
                    registro.Ccoluna16 = novoValor;
                }
            }
            
            await _context.SaveChangesAsync();

            

            return Json(new { success = true, message = "Valores de Ccoluna16 atualizados com sucesso!" });

        }

        [HttpPost]
        public async Task<IActionResult> AtualizarValoresCcoluna16Fluxo2([FromBody] Dictionary<string, string> valoresAtualizados1)
        {
            if (valoresAtualizados1 == null || !valoresAtualizados1.Any())
            {
                return Json(new { success = false, message = "Nenhum valor foi enviado para atualização." });
            }

            var chaves = valoresAtualizados1.Keys.ToList();

            var registros = await _context.Contracheque
                .Where(x => chaves.Contains(x.Ccoluna16))
                .ToListAsync();

            foreach (var registro in registros)
            {
                if (valoresAtualizados1.TryGetValue(registro.Ccoluna16, out var novoValor))
                {
                    registro.Ccoluna16 = novoValor;
                }
            }

            await _context.SaveChangesAsync();

            await _servidorService.GerarEncontradoAsync();
            await _matriculaService.GerarMatriculasAsync();
            await _categoriaService.GerarVinculoAsync();
            await _secretariaService.GerarSecretariasAsync();
            await _perfilCalculo.GeradorPerfilCalculo();
            await _cleanupService.LimparTabelasAsync();

            return Json(new { success = true, message = "Valores de Ccoluna16 atualizados com sucesso!" });

        }

        //   ============  Calculo ==================

        [HttpGet]
        public async Task<IActionResult> ObterValoresCalculo()
        {
            var categorias = new Dictionary<string, string>
    {
        { "1", "PENSIONISTA" }, { "2", "EFETIVO" }, { "3", "MILITAR" }, { "4", "APOSENTADO" },
        { "5", "CONTRATADO" }, { "6", "PRESTADOR DE SERVIÇO" }, { "7", "COMISSIONADO" },
        { "8", "ESTAGIARIO" }, { "9", "CELETISTA" }, { "10", "ESTATUTARIO" }, { "11", "TEMPORARIO" },
        { "12", "BENEFICÍARIO" }, { "13", "AGENTE POLITICO" }, { "14", "AGUARDANDO ESPECIFICAR" },
        { "15", "EFETIVO/COMISSÃO" }, { "16", "ESTÁVEL" }, { "17", "CONSELHEIRO TUTELAR" },
        { "18", "REGIME ADMINISTRATIVO" }, { "19", "TRABALHADOR AVULSO" }, { "20", "PENSÃO POR MORTE" },
        { "21", "INTERESSE PÚBLICO" }, { "22", "EMPREGO PÚBLICO" }, { "23", "REINTEGRAÇÃO" },
        { "24", "REGIME JURÍDICO" }, { "25", "CONTRATADO/COMISSIONADO" }, { "26", "SEM CATEGORIA" },
        { "27", "PENSÃO ALIMENTÍCIA" }, { "28", "INATIVO" }, { "29", "FUNÇÃO PÚBLICA RELEVANTE" },
        { "30", "PENSÃO ESPECIAL" }, { "31", "Efetivo/Cedido" }, { "32", "Avulsos" }, { "33", "CEDIDO" },
        { "34", "Autônomo" }, { "35", "Comissionado/Estatutário" }, { "36", "Temporário/Estatutário" },
        { "37", "Concursado" }, { "38", "Contribuinte Individual" }, { "39", "Eletivo" },
        { "41", "Estatutário/Agente Político" }, { "42", "Auxílio" }, { "48", "Bolsa Auxílio" },
        { "49", "Temporário - CLT" }, { "51", "Prefeito" }, { "52", "TUTELAR" }
    };

            var valoresCcoluna16 = await _context.Contracheque
                .Select(x => x.Ccoluna16)
                .Distinct()
                .ToListAsync();

            var valoresFormatados = valoresCcoluna16
                .Select(valor => $"{valor} - {categorias.GetValueOrDefault(valor, "DESCONHECIDO")}")
                .ToList();

            return Json(valoresFormatados);
        }


        // ============ Atualização de Ccoluna18 ==================
        [HttpPost]
        public async Task<IActionResult> AtualizarValoresCcoluna18([FromBody] Dictionary<string, object> valoresAtualizados, SelectOptionModel selectOption)
        {
            if (valoresAtualizados == null || !valoresAtualizados.Any())
                return BadRequest(new { message = "Nenhum valor foi enviado para atualização." });

            int totalAlterados = 0;

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    foreach (var item in valoresAtualizados)
                    {
                        var valorOriginal = item.Key.Trim().ToUpper();
                        var novoValor = item.Value.ToString().Trim();

                        // Extrai apenas o número antes do " - " para comparar com Ccoluna16
                        var numeroOriginal = valorOriginal.Split(" - ")[0];

                        // Atualiza diretamente no banco para melhor performance
                        int alterados = await _context.Contracheque
                            .Where(x => x.Ccoluna16 == numeroOriginal)
                            .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.Ccoluna18, novoValor));

                        totalAlterados += alterados;
                    }

                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(500, new { message = "Erro ao atualizar os registros.", error = ex.Message });
                }
            }

            if (totalAlterados == 0)
                return BadRequest(new { message = "Nenhum registro foi atualizado." });

            await _servidorService.GerarEncontradoAsync();
            await _matriculaService.GerarMatriculasAsync();
            await _categoriaService.GerarVinculoAsync();
            await _secretariaService.GerarSecretariasAsync();
            await _perfilCalculo.GeradorPerfilCalculo();
            await _cleanupService.LimparTabelasAsync();
            return Ok(new { success = true, message = $"{totalAlterados} valores atualizados com sucesso." });
        }

        [HttpGet]
        public JsonResult ObterQuantidadeDiscrepancias()
        {
            try
            {
                var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                var servidorPath = Path.Combine(desktopPath, "SERVIDOR.txt");
                var matriculaPath = Path.Combine(desktopPath, "MATRICULA.txt");
                var categoriaPath = Path.Combine(desktopPath, "CATEGORIA.txt");
                var secretariaPath = Path.Combine(desktopPath, "SECRETARIAS.txt");
                var perfilCalculoPath = Path.Combine(desktopPath, "PERFIL DE CALCULO.txt");

                int servidor = System.IO.File.Exists(servidorPath) ? System.IO.File.ReadAllLines(servidorPath).Length : 0;
                int matricula = System.IO.File.Exists(matriculaPath) ? System.IO.File.ReadAllLines(matriculaPath).Length : 0;
                int categoria = System.IO.File.Exists(categoriaPath) ? System.IO.File.ReadAllLines(categoriaPath).Length : 0;
                int secretaria = System.IO.File.Exists(secretariaPath) ? System.IO.File.ReadAllLines(secretariaPath).Length : 0;
                int perfilCalculo = System.IO.File.Exists(perfilCalculoPath) ? System.IO.File.ReadAllLines(perfilCalculoPath).Length : 0;

                return Json(new
                {
                    servidor,
                    matricula,
                    categoria,
                    secretaria,
                    perfilCalculo
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
         
        }
     
        //<<<<<<<   =================================   PRIVATES ===================================== >>>>>>>>>>>>>>>>>
        private async Task<List<ContrachequeModel>> ProcessarArquivoTxt(IFormFile arquivoTxt, SelectOptionModel selectOptionFromDb)
        {
            var registros = new List<ContrachequeModel>();

            using (var stream = arquivoTxt.OpenReadStream())
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                string linha;

                while ((linha = reader.ReadLine()) != null)
                {
                    if (linha.StartsWith("F"))
                    {
                        var colunas = linha.Split(';');
                        linha = linha.Trim();

                        if (colunas.Length >= 20)
                        {
                            var item = new ContrachequeModel
                            {
                                Ccoluna1 = colunas[7],
                                Ccoluna2 = colunas[3],
                                Ccoluna3 = colunas[4],
                                Ccoluna4 = colunas[5],
                                Ccoluna5 = "Rua A",
                                Ccoluna6 = "S/N",
                                Ccoluna7 = "CASA",
                                Ccoluna8 = "CENTRO",
                                Ccoluna9 = selectOptionFromDb.ValorColuna9,
                                Ccoluna10 = selectOptionFromDb.ValorColuna10,
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
                                Ccoluna25 = "0",
                                Cargo = colunas[8]
                            };

                            registros.Add(item);
                        }
                    }
                }
            }

            return registros;
        }

        private async Task<List<AdministrativoModel>> ProcessarArquivoExcel(IFormFile arquivoExcel)
        {
            var registros = new List<AdministrativoModel>();

            var secretariaMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
    {
        {"MUNICÍPIO DE ALCINÓPOLIS/MS", 1},
        {"FUNDO MUNICIPAL DE EDUCAÇÃO", 2},
        {"PREFEITURA", 1},
        {"EDUCAÇÃO", 2},
        {"PREFEITURA MUNICIPAL DE ARACATU", 1},
        {"SECRETARIA DE EDUCAÇÃO E JUVENTUDE", 4},
        {"PREFEITURA MUNICIPAL DE JUAZEIRO", 1},
        {"SECRETARIA DE SAÚDE", 5},
        {"AMA", 2},
        {"SECRETARIA DE ORDEM PUBLICA", 31},
        {"CSTT", 3},
        {"PREFEITURA MUNICIPAL DE ITAPETINGA", 1},
        {"NADA", 99},
    };

            var categoriaMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
    {
        {"PENSIONISTA", 1},
        {"EFETIVO", 2},
        {"MILITAR", 3},
        {"APOSENTADO", 4},
        {"CONTRATADO", 5},
        {"PRESTADOR DE SERVIÇO", 6},
        {"COMISSIONADO", 7},
        {"ESTAGIARIO", 8},
        {"CELETISTA", 9},
        {"ESTATUTARIO", 10},
        {"Estatutário", 10},
        {"TEMPORARIO", 11},
        {"BENEFICÍARIO", 12},
        {"Beneficiário", 12},
        {"AGENTE POLITICO", 13},
        {"AGUARDANDO ESPECIFICAR", 14},
        {"EFETIVO/COMISSÃO", 15},
        {"ESTÁVEL", 16},
        {"CONSELHEIRO TUTELAR", 17},
        {"REGIME ADMINISTRATIVO", 18},
        {"TRABALHADOR AVULSO", 19},
        {"PENSÃO POR MORTE", 20},
        {"INTERESSE PÚBLICO", 21},
        {"EMPREGO PÚBLICO", 22},
        {"REINTEGRAÇÃO", 23},
        {"REGIME JURÍDICO", 24},
        {"CONTRATADO/COMISSIONADO", 25},
        {"SEM CATEGORIA", 26},
        {"PENSÃO ALIMENTÍCIA", 27},
        {"INATIVO", 28},
        {"FUNÇÃO PÚBLICA RELEVANTE", 29},
        {"PENSÃO ESPECIAL", 30},
        {"Efetivo/Cedido", 31},
        {"Avulsos", 32},
        {"CEDIDO", 33},
        {"Autônomo", 34},
        {"Comissionado/Estatutário", 35},
        {"Temporário/Estatutário", 36},
        {"Concursado", 37},
        {"Contribuinte Individual", 38},
        {"Eletivo", 39},
        {"Estatutário/Agente Político", 41},
        {"Auxílio", 42},
        {"Bolsa Auxílio", 48},
        {"Temporário - CLT", 49},
        {"Prefeito", 51},
        {"TUTELAR", 52},
        {"Temporário",11},
    };

            using (var stream = arquivoExcel.OpenReadStream())
            {
                var workbook = new HSSFWorkbook(stream);
                var sheet = workbook.GetSheetAt(0);

                for (int rowIdx = 1; rowIdx <= sheet.LastRowNum; rowIdx++)
                {
                    var row = sheet.GetRow(rowIdx);
                    if (row == null) continue;

                    string valorSecretaria = row.GetCell(12)?.ToString().Trim() ?? "";
                    string valorCategoria = row.GetCell(13)?.ToString().Trim() ?? "";

                    var administrativo = new AdministrativoModel
                    {
                        Acoluna1 = row.GetCell(2)?.ToString() ?? "",
                        Acoluna2 = row.GetCell(3)?.ToString().Length >= 10
                                    ? row.GetCell(3).ToString().Substring(row.GetCell(3).ToString().Length - 10)
                                    : row.GetCell(3)?.ToString().PadLeft(10, '0') ?? "0000000000",
                        Acoluna3 = row.GetCell(4)?.ToString() ?? "",
                        Acoluna4 = secretariaMap.TryGetValue(valorSecretaria, out var idSecretaria) ? idSecretaria.ToString() : valorCategoria,
                        Acoluna5 = categoriaMap.TryGetValue(valorCategoria, out var idCategoria) ? idCategoria.ToString() : row.GetCell(13).ToString(),
                        Acoluna6 = row.GetCell(14)?.ToString() ?? "",
                    };
                    registros.Add(administrativo);
                }
            }

            return registros;
        }

        private int ContarLinhasArquivo(string caminhoArquivo)
        {
            try
            {
                if (System.IO.File.Exists(caminhoArquivo))
                {
                    return System.IO.File.ReadAllLines(caminhoArquivo).Length;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao contar linhas do arquivo: {ex.Message}");
            }

            return 0;
        }

    }

}