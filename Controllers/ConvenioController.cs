using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NPOI.HSSF.UserModel;
using Servidor.Data;
using Servidor.ErrosService;
using Servidor.Models;
using Servidor.Models.Enums;

namespace Servidor.Controllers
{
    public class ConvenioController : Controller
    {
        private readonly BancoContext _context;
        private readonly MatriculaService _matriculaservice;
        private readonly SecretariaService _secretariaservice;
        private readonly ServidorService _servidorService;
        private readonly CategoriaService _categoriaService;
        private readonly CleanupService _cleanupService;
        private readonly Preenchimento _preenchimento;

        private readonly AbareService _abareservice;

        public ConvenioController(BancoContext context, MatriculaService matriculaservice,
                                    SecretariaService secretariaservice, ServidorService servidorService,
                                    CategoriaService categoriaService, CleanupService cleanupService,
                                    Preenchimento preenchimento, AbareService abareservice)
        {
            _context = context;
            _matriculaservice = matriculaservice;
            _secretariaservice = secretariaservice;
            _servidorService = servidorService;
            _categoriaService = categoriaService;
            _cleanupService = cleanupService;
            _preenchimento = preenchimento;
            _abareservice = abareservice;
        }

        public IActionResult Index()
        {
            var model = new EnumModel();
            var statusList = Enum.GetValues(typeof(Status))
                                 .Cast<Status>()
                                 .Select(s => new SelectListItem
                                 {
                                     Value = ((int)s).ToString(),
                                     Text = s.ToString()
                                 }).ToList();
            ViewBag.Statuses = statusList;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ProcessarArquivo(IFormFile arquivoTxt, IFormFile arquivoExcel, EnumModel status)
        {
            if ((arquivoTxt == null || arquivoTxt.Length == 0) && (arquivoExcel == null || arquivoExcel.Length == 0))
            {
                TempData["Mensagem"] = "Erro nos arquivos enviados.";
                return RedirectToAction("Index");
            }

            var statusSelecionado = status.StatusSelecionado;

            try
            {
                // Processar o arquivo TXT
                var registrosTxt = arquivoTxt != null ? await ProcessarArquivoTxt(arquivoTxt, statusSelecionado) : new List<ContrachequeModel>();
                if (registrosTxt.Any())
                {
                    await SalvarRegistrosTxt(registrosTxt);
                }

                // Processar o arquivo Excel
                var registrosExcel = arquivoExcel != null ? ProcessarArquivoExcel(arquivoExcel) : new List<AdministrativoModel>();
                if (registrosExcel.Any())
                {
                    await SalvarRegistrosExcel(registrosExcel);
                }

                // Executar etapas subsequentes
                await ExecutarEtapasDeProcessamento();

                TempData["Mensagem"] = "Arquivos processados com sucesso!";
            }
            catch (Exception ex)
            {
                TempData["Mensagem"] = $"Erro ao processar arquivos: {ex.Message}";
            }

            return RedirectToAction("Index");
        }

        private async Task SalvarRegistrosTxt(List<ContrachequeModel> registrosTxt)
        {
            _context.Contracheque.AddRange(registrosTxt);
            await _context.SaveChangesAsync();
            TempData["Mensagem"] = $"{registrosTxt.Count} registros do TXT salvos com sucesso!";
        }

        private async Task SalvarRegistrosExcel(List<AdministrativoModel> registrosExcel)
        {
            _context.Administrativo.AddRange(registrosExcel);
            await _context.SaveChangesAsync();
            TempData["Mensagem"] = $"{registrosExcel.Count} registros do Excel salvos com sucesso!";
        }

        private async Task ExecutarEtapasDeProcessamento()
        {
            // Etapa 1: Gerar o arquivo de servidores com discrepâncias
            await _servidorService.GerarEncontradoAsync();

            // Etapa 2: Atualizar a tabela Administrativo com as discrepâncias do arquivo SERVIDOR
            await InserirNovosServidores("SERVIDOR.txt");

            // Etapa 3: Gerar o arquivo de Matrículas com discrepâncias
            await _matriculaservice.GerarMatriculasAsync();

            // Etapa 4: Atualizar a tabela Administrativo com as discrepâncias do arquivo MATRÍCULA
            await AtualizarBancoComDiscrepancias("MATRÍCULA.txt");
        }


        private async Task<List<ContrachequeModel>> ProcessarArquivoTxt(IFormFile arquivoTxt, Status statusSelecionado)
        {
            var registros = new List<ContrachequeModel>();
            var serviceMap = new Dictionary<Status, Func<string[], Task<List<ContrachequeModel>>>>
    {
        { Status.PREF_Abare_BA, colunas => _abareservice.ProcessarArquivoAsync(colunas, Status.PREF_Abare_BA) },
    };

            if (serviceMap.TryGetValue(statusSelecionado, out var processarArquivo))
            {
                using var reader = new StreamReader(arquivoTxt.OpenReadStream(), Encoding.UTF8);
                while (!reader.EndOfStream)
                {
                    var linha = await reader.ReadLineAsync();
                    if (string.IsNullOrWhiteSpace(linha) || !linha.StartsWith("F", StringComparison.OrdinalIgnoreCase))
                        continue;

                    var colunas = linha.Split(';').Select(c => c.Trim()).ToArray();
                    var contracheques = await processarArquivo(colunas);
                    registros.AddRange(contracheques);

                    // Log para depuração
                    Console.WriteLine($"Processando linha: {linha}");
                }
            }

            // Log para saber quantos registros foram processados
            Console.WriteLine($"Total de registros processados do TXT: {registros.Count}");

            return registros;
        }

        private List<AdministrativoModel> ProcessarArquivoExcel(IFormFile arquivoExcel)
        {
            var registros = new List<AdministrativoModel>();

            using (var stream = arquivoExcel.OpenReadStream())
            {
                var workbook = new HSSFWorkbook(stream); // Para arquivos .xls
                var sheet = workbook.GetSheetAt(0); // Pega a primeira aba

                for (int rowIdx = 1; rowIdx <= sheet.LastRowNum; rowIdx++) // Começa da linha 1 para ignorar cabeçalho
                {
                    var row = sheet.GetRow(rowIdx);
                    if (row == null) continue;

                    var administrativo = new AdministrativoModel
                    {
                        Acoluna1 = row.GetCell(2)?.ToString() ?? "",
                        Acoluna2 = row.GetCell(3)?.ToString()?.PadLeft(10, '0') ?? "0000000000",
                        Acoluna3 = row.GetCell(4)?.ToString() ?? "",
                        Acoluna4 = row.GetCell(12)?.ToString() ?? "",
                        Acoluna5 = _preenchimento.MapearVinculo(row.GetCell(13)?.ToString() ?? ""),
                        Acoluna6 = row.GetCell(14)?.ToString() ?? "",
                    };

                    registros.Add(administrativo);
                }
            }

            // Log para saber quantos registros foram processados
            Console.WriteLine($"Total de registros processados do Excel: {registros.Count}");

            return registros;
        }

        private async Task ProcessarDiscrepancias(string nomeArquivo, bool atualizarExistentes)
        {
            var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var filePath = Path.Combine(desktopPath, nomeArquivo);

            if (!System.IO.File.Exists(filePath))
            {
                Console.WriteLine($"Arquivo {nomeArquivo} não encontrado. Nenhuma ação realizada.");
                return;
            }

            var linhas = await System.IO.File.ReadAllLinesAsync(filePath);
            foreach (var linha in linhas)
            {
                var colunas = linha.Split(';').Select(c => c.Trim()).ToArray();

                // Criar um novo objeto para o modelo Administrativo
                var novoAdministrativo = new AdministrativoModel
                {
                    Acoluna1 = colunas[1],
                    Acoluna2 = colunas[2],
                    Acoluna3 = colunas[3],
                    Acoluna4 = colunas[4],
                    Acoluna5 = colunas[5],
                    Acoluna6 = colunas[6],

                };
                Console.WriteLine($"Inserindo: {novoAdministrativo.Acoluna2}, {novoAdministrativo.Acoluna3},{novoAdministrativo.Acoluna4}");

                // Verificar se o registro já existe
                var registroExistente = _context.Administrativo
                         .AsNoTracking() // Evita problemas de cache
                         .FirstOrDefault(a => a.Acoluna1.Trim() == novoAdministrativo.Acoluna1.Trim() &&
                         a.Acoluna2.Trim() == novoAdministrativo.Acoluna2.Trim());


                if (registroExistente != null && atualizarExistentes)
                {
                    // Atualizar registro existente
                    registroExistente.Acoluna3 = novoAdministrativo.Acoluna3;
                    registroExistente.Acoluna4 = novoAdministrativo.Acoluna4;
                    registroExistente.Acoluna5 = novoAdministrativo.Acoluna5;
                    registroExistente.Acoluna6 = novoAdministrativo.Acoluna6;
                }
                else if (registroExistente == null)
                {
                    // Inserir novos registros
                    _context.Administrativo.Add(novoAdministrativo);
                }
            }

            // Salvar alterações no banco
            await _context.SaveChangesAsync();
            Console.WriteLine($"Processamento do arquivo {nomeArquivo} concluído com sucesso.");
        }

        private async Task InserirNovosServidores(string nomeArquivo)
        {
            await ProcessarDiscrepancias(nomeArquivo, atualizarExistentes: false);
        }

        private async Task AtualizarBancoComDiscrepancias(string nomeArquivo)
        {
            await ProcessarDiscrepancias(nomeArquivo, atualizarExistentes: true);
        }


    }
}
