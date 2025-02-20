﻿using Servidor.Models.Enums;
using Servidor.Models;
using Microsoft.EntityFrameworkCore;
using Servidor.Data;

public class ContrachequeAL
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
            Ccoluna10 = "AL",
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
//======================================    ANADIA    ============================================\\

public class AnadiaService
{
    private static readonly Dictionary<string, string> Vinculo = new()
    {
        { "Contratado", "5" },
        { "Efetivo", "2" },
        { "Comissionado", "7" },
        { "Eletivo", "13" }
    };

    public Task<List<ContrachequeModel>> ProcessarArquivoAsync(string[] colunas, Status status)
    {
        var contracheque = ContrachequeAL.CriarContracheque(colunas, "ANADIA");

        if (contracheque.Ccoluna1 == "FUNDO MUNICIPAL DE SAUDE")
        {
            contracheque.Ccoluna21 = "351";
        }

        if (contracheque.Ccoluna1 == "FUNDO MUNICIPAL DE EDUCACAO" || contracheque.Ccoluna1 == "PREFEITURA MUNICIPAL DE ANADIA" || contracheque.Ccoluna1 == "FUNDO MUN DE ASSISTENCIA SOCIAL")
        {
            contracheque.Ccoluna21 = "300";
        }

        if (Vinculo.ContainsKey(colunas[16].Trim()))
        {
            contracheque.Ccoluna16 = Vinculo[colunas[16].Trim()];
        }

        switch (contracheque.Ccoluna16)
        {
            case "2":
                contracheque.Ccoluna18 = "171";
                break;
            default:
                contracheque.Ccoluna18 = "329";
                break;
        }


        return Task.FromResult(new List<ContrachequeModel> { contracheque });
    }
    
}

//======================================    GIRAU DO PONCIANO    ============================================\\

public class GiraDoPoncianoService
{
    private static readonly Dictionary<string, string> Vinculo = new()
    {
        { "Contratado", "5" },
        { "Efetivo", "2" },
        { "Comissionado", "7" },
        { "Eletivo", "13" },
        { "Pensionista", "1" },
        { "Aposentado", "4" }
    };

    public Task<List<ContrachequeModel>> ProcessarArquivoAsync(string[] colunas, Status status)
    {
        var contracheque = ContrachequeAL.CriarContracheque(colunas, "GIRAU DO PONCIANO");

        if (contracheque.Ccoluna1 == "PREFEITURA MUNICIPAL DE GIRAU DO PONCIANO-AL")
        {
            contracheque.Ccoluna21 = "1";
        }
        if (contracheque.Ccoluna1 == "INST.MUN.DE PREV.SOCIAL DE GIRAU DO PONCIANO-AL")
        {
            contracheque.Ccoluna21 = "3";
        }
        if (contracheque.Ccoluna1 == "Sec. Mun. de Educa??o de Girau do Ponciano-AL")
        {
            contracheque.Ccoluna21 = "2";
        }


        if (Vinculo.ContainsKey(colunas[16].Trim()))
        {
            contracheque.Ccoluna16 = Vinculo[colunas[16].Trim()];
        }

        switch (contracheque.Ccoluna16)
        {
            case "2":
            case "1":
            case "4":
                contracheque.Ccoluna18 = "793";
                break;
            default:
                contracheque.Ccoluna18 = "743";
                break;
        }


        return Task.FromResult(new List<ContrachequeModel> { contracheque });
    }

}


