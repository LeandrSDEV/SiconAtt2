﻿using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        private readonly GeradorDePerfil _geradorDePerfil;
        private readonly PerfilCalculo _perfilCalculo;
        private readonly CleanupService _cleanupService;        

        private readonly AbareService _abareservice;
        private readonly CupiraService _cupiraservice;
        private readonly CansancaoService _cansancaoservice;
        private readonly XiqueXiqueService _xiquexiqueservice;
        private readonly AlcinopolisService _alcinopolisService;
        private readonly CafarnaumService _cafarnaumService;
        private readonly IndiaporaService _IndiaporaService;
        private readonly AnadiaService _anadiaService;
        private readonly GiraDoPoncianoService _giraDoPoncianoService;
        private readonly FUNBodocoService _fUNBodocoService;
        private readonly BodocoService _bodocoService;

        public ConvenioController(BancoContext context, AbareService abareservice, 
                                  CupiraService cupiraservice, CansancaoService cansancaoservice,
                                  MatriculaService matriculaservice, SecretariaService secretariaservice,
                                  ServidorService servidorService, CategoriaService categoriaService,
                                  CleanupService cleanupService, XiqueXiqueService xiqueXiqueService,
                                  AlcinopolisService alcinopolisService, CafarnaumService cafarnaumService,
                                  IndiaporaService indiaporaService, GeradorDePerfil geradorDePerfil,
                                  AnadiaService anadiaService, PerfilCalculo perfilCalculo,
                                  GiraDoPoncianoService giraDoPoncianoService, FUNBodocoService fUNBodocoService, 
                                  BodocoService bodocoService)
        {
            _context = context;
            _servidorService = servidorService;
            _matriculaservice = matriculaservice;
            _categoriaService = categoriaService;
            _secretariaservice = secretariaservice;
            _geradorDePerfil = geradorDePerfil;
            _perfilCalculo = perfilCalculo;
            _cleanupService = cleanupService;
            _xiquexiqueservice = xiqueXiqueService;
            _alcinopolisService = alcinopolisService;
            _cafarnaumService = cafarnaumService;
            _IndiaporaService = indiaporaService;
            _abareservice = abareservice;
            _cansancaoservice = cansancaoservice;
            _cupiraservice = cupiraservice;
            _anadiaService = anadiaService;
            _giraDoPoncianoService = giraDoPoncianoService;
            _fUNBodocoService = fUNBodocoService;
            _bodocoService = bodocoService;
        }

        public IActionResult Index()
        {
            // Cria uma lista de SelectListItem a partir do enum Status
            var statuses = Enum.GetValues(typeof(Status))
                .Cast<Status>()
                .Select(s => new SelectListItem
                {
                    Value = ((int)s).ToString(),  // O valor que será enviado no form
                    Text = GetMunicipioDisplayName(s)  // O nome que será exibido na lista
                })
                .ToList();

            // Passa a lista para a view
            ViewBag.Statuses = statuses;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ProcessarArquivo(IFormFile arquivoTxt, IFormFile arquivoExcel, EnumModel status)
        {
            if (arquivoTxt == null || arquivoTxt.Length == 0 && arquivoExcel == null || arquivoExcel.Length == 0)
            {
                TempData["Mensagem"] = "Erro nos arquivos enviado.";
                return RedirectToAction("Index");
            }

            var registros = new List<ContrachequeModel>();
            var registros2 = new List<AdministrativoModel>();

            try
            {
                var serviceMap = new Dictionary<Status, Func<string[], Task<List<ContrachequeModel>>>>
{
                    { Status.PREF_Abare_BA, colunas => _abareservice.ProcessarArquivoAsync(colunas, Status.PREF_Abare_BA) },
                    { Status.PREF_Cupira_PE, colunas => _cupiraservice.ProcessarArquivoAsync(colunas, Status.PREF_Cupira_PE) },
                    { Status.PREF_Cansanção_BA, colunas => _cansancaoservice.ProcessarArquivoAsync(colunas, Status.PREF_Cansanção_BA) },
                    { Status.PREF_XiqueXique_BA, colunas => _xiquexiqueservice.ProcessarArquivoAsync(colunas, Status.PREF_XiqueXique_BA) },
                    { Status.PREF_Alcinópolis_MS, colunas => _alcinopolisService.ProcessarArquivoAsync(colunas, Status.PREF_Alcinópolis_MS) },
                    { Status.PREF_Cafarnaum_BA, colunas => _cafarnaumService.ProcessarArquivoAsync(colunas, Status.PREF_Cafarnaum_BA) },
                    { Status.PREF_Indiaporã_SP, colunas => _IndiaporaService.ProcessarArquivoAsync(colunas, Status.PREF_Indiaporã_SP) },
                    { Status.PREF_Anadia_AL, colunas => _anadiaService.ProcessarArquivoAsync(colunas, Status.PREF_Anadia_AL) },
                    { Status.PREF_GirauDoPonciano, colunas => _giraDoPoncianoService.ProcessarArquivoAsync(colunas, Status.PREF_GirauDoPonciano) },
                    { Status.FUNPREBO_Bodoco_PE, colunas => _fUNBodocoService.ProcessarArquivoAsync(colunas, Status.FUNPREBO_Bodoco_PE) },
                    { Status.PREF_Bodoco_PE, colunas => _bodocoService.ProcessarArquivoAsync(colunas, Status.PREF_Bodoco_PE) },
};

                if (serviceMap.TryGetValue(status.StatusSelecionado, out var processarArquivo))
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
                    }
                }

                if (registros.Any())
                {
                    _context.Contracheque.AddRange(registros);
                    await _context.SaveChangesAsync();
                    TempData["Mensagem"] = $"{registros.Count} registros salvos com sucesso!";
                }
                else
                {
                    TempData["Mensagem"] = "Nenhum dado válido encontrado no arquivo TXT.";
                }
            }
            catch (Exception ex)
            {
                TempData["Mensagem"] = $"Erro ao processar o arquivo TXT: {ex.Message}";
            }
//=====================================================================================================\\
            try
            {
                using (var stream = arquivoExcel.OpenReadStream())
                {
                    var workbook = new HSSFWorkbook(stream); // Para arquivos .xls
                    var sheet = workbook.GetSheetAt(0); // Pega a primeira aba

                    for (int rowIdx = 1; rowIdx <= sheet.LastRowNum; rowIdx++) // Começa da linha 1 para ignorar cabeçalho
                    {
                        var row = sheet.GetRow(rowIdx);
                        if (row == null) continue; // Ignora linhas vazias

                        var administrativo = new AdministrativoModel
                        {
                            Acoluna1 = row.GetCell(2)?.ToString() ?? "",

                            // Lê apenas os 10 últimos dígitos ou preenche com zeros se for menor
                            Acoluna2 = row.GetCell(3)?.ToString().Length >= 10
                            ? row.GetCell(3).ToString().Substring(row.GetCell(3).ToString().Length - 10)
                            : row.GetCell(3)?.ToString().PadLeft(10, '0') ?? "0000000000",

                            Acoluna3 = row.GetCell(4)?.ToString() ?? "", // Coluna 5
                            Acoluna4 = row.GetCell(12)?.ToString() ?? "", // Coluna 13
                            Acoluna5 = row.GetCell(13)?.ToString() ?? "", // Coluna 14
                            Acoluna6 = row.GetCell(14)?.ToString() ?? "", // Coluna 15
                            
                        };

                         //Mapeamento de valores para Acoluna5
                        var Vinculo = new Dictionary<string, string>
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

                         //Atualiza Acoluna5 com base no mapeamento
                        if (Vinculo.ContainsKey(administrativo.Acoluna5))
                        {
                            administrativo.Acoluna5 = Vinculo[administrativo.Acoluna5];
                        }
                        registros2.Add(administrativo);
                    }
                }

                if (registros2.Any())
                {
                    _context.Administrativo.AddRange(registros2);
                    await _context.SaveChangesAsync();
                    TempData["Mensagem"] = $"{registros2.Count} registros salvos com sucesso!";
                }
                else
                {
                    TempData["Mensagem"] = "Nenhum dado válido encontrado no arquivo Excel.";
                }
            }
            catch (Exception ex)
            {
                TempData["Mensagem"] = $"Erro ao processar o arquivo Excel: {ex.Message}";
            }

            await _servidorService.GerarEncontradoAsync();

            await _matriculaservice.GerarMatriculasAsync();

            await _categoriaService.GerarVinculoAsync();

            await _secretariaservice.GerarSecretariasAsync(status.StatusSelecionado);

            await _geradorDePerfil.GerarPerfilAcessoAsync(status.StatusSelecionado);  //Perfil de Acesso (Limitado)

            await _perfilCalculo.GeradorPerfilCalculo(status.StatusSelecionado);

            //await _cleanupService.LimparTabelasAsync();

            return RedirectToAction("Index");
        }

        private string GetMunicipioDisplayName(Status status)
        {
            switch (status)
            {
                case Status.PREF_Abare_BA: return "Município de Abaré/BA";
                case Status.PREF_Cupira_PE: return "Município de Cupira/PE";
                case Status.PREF_Cansanção_BA: return "Município de Cansanção/BA";
                case Status.PREF_XiqueXique_BA: return "Município de XiqueXique/BA";
                case Status.PREF_Alcinópolis_MS: return "Município de Alcinópolis/BA";
                case Status.PREF_Cafarnaum_BA: return "Município de Cafarnaum/BA";
                case Status.PREF_Anadia_AL: return "Município de Anadia/AL";
                case Status.PREF_Indiaporã_SP: return "Município de Indiaporã/SP";
                case Status.PREF_GirauDoPonciano: return "Município de Girau do Ponciano/AL";
                case Status.FUNPREBO_Bodoco_PE: return "FUNPREBO - Bodocó/PE";
                case Status.PREF_Bodoco_PE: return "Município de Bodocó/PE";
                default: return "Selecione o Município";
            }
        }
    }

}
