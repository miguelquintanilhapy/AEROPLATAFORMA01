using magal.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace magal.Services
{
    public class PdfService
    {
        #region Atributos e Campos Privados

        private static readonly CultureInfo _ptBR = new("pt-BR");

        #endregion

        #region Construtor Estático

        static PdfService()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }

        #endregion

        #region Métodos Públicos

        public void GerarPropostaTecnica(Projeto projeto, List<Custo> custosExtras, string caminhoArquivo)
        {
            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(0.6f, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                    page.Header().Column(col => ConstruirCabecalho(col, projeto));
                    page.Content().PaddingTop(16).Column(col => ConstruirConteudoPrincipal(col, projeto, custosExtras));
                    page.Footer().BorderTop(1).BorderColor("#E0E0E0").PaddingTop(8).Row(ConstruirRodape);
                });
            }).GeneratePdf(caminhoArquivo);
        }

        public void GerarRelatorioTabelaProjetos(List<Projeto> projetos, string caminhoArquivo)
        {
            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(0.6f, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

                    page.Header().Column(col => ConstruirCabecalhoRelatorio(col, "PROJETOS", projetos.Count));
                    page.Content().PaddingTop(16).Column(col => ConstruirTabelaRelatorioProjetos(col, projetos));
                    page.Footer().BorderTop(1).BorderColor("#E0E0E0").PaddingTop(8).Row(ConstruirRodape);
                });
            }).GeneratePdf(caminhoArquivo);
        }

        public void GerarRelatorioTabelaFuncionarios(List<Funcionario> funcionarios, string caminhoArquivo)
        {
            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(0.6f, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

                    page.Header().Column(col => ConstruirCabecalhoRelatorio(col, "FUNCIONÁRIOS", funcionarios.Count));
                    page.Content().PaddingTop(16).Column(col => ConstruirTabelaRelatorioFuncionarios(col, funcionarios));
                    page.Footer().BorderTop(1).BorderColor("#E0E0E0").PaddingTop(8).Row(ConstruirRodape);
                });
            }).GeneratePdf(caminhoArquivo);
        }

        public void GerarRelatorioTabelaCargos(List<Cargo> cargos, string caminhoArquivo)
        {
            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(0.6f, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

                    page.Header().Column(col => ConstruirCabecalhoRelatorio(col, "CARGOS", cargos.Count));
                    page.Content().PaddingTop(16).Column(col => ConstruirTabelaRelatorioCargos(col, cargos));
                    page.Footer().BorderTop(1).BorderColor("#E0E0E0").PaddingTop(8).Row(ConstruirRodape);
                });
            }).GeneratePdf(caminhoArquivo);
        }

        public void GerarRelatorioTabelaClientes(List<Cliente> clientes, string caminhoArquivo)
        {
            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(0.6f, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

                    page.Header().Column(col => ConstruirCabecalhoRelatorio(col, "CLIENTES", clientes.Count));
                    page.Content().PaddingTop(16).Column(col => ConstruirTabelaRelatorioClientes(col, clientes));
                    page.Footer().BorderTop(1).BorderColor("#E0E0E0").PaddingTop(8).Row(ConstruirRodape);
                });
            }).GeneratePdf(caminhoArquivo);
        }

        public void GerarRelatorioTabelaCustos(List<CatalogoCusto> custos, string caminhoArquivo)
        {
            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(0.6f, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

                    page.Header().Column(col => ConstruirCabecalhoRelatorio(col, "CATÁLOGO DE CUSTOS", custos.Count));
                    page.Content().PaddingTop(16).Column(col => ConstruirTabelaRelatorioCustos(col, custos));
                    page.Footer().BorderTop(1).BorderColor("#E0E0E0").PaddingTop(8).Row(ConstruirRodape);
                });
            }).GeneratePdf(caminhoArquivo);
        }

        public void GerarRelatorioTabelaUsuarios(List<Usuario> usuarios, string caminhoArquivo)
        {
            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(0.6f, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

                    page.Header().Column(col => ConstruirCabecalhoRelatorio(col, "USUÁRIOS", usuarios.Count));
                    page.Content().PaddingTop(16).Column(col => ConstruirTabelaRelatorioUsuarios(col, usuarios));
                    page.Footer().BorderTop(1).BorderColor("#E0E0E0").PaddingTop(8).Row(ConstruirRodape);
                });
            }).GeneratePdf(caminhoArquivo);
        }

        #endregion

        #region Métodos Auxiliares / Privados

        private void ConstruirCabecalho(ColumnDescriptor col, Projeto projeto)
        {
            col.Item().Background("#1E3A5F").Padding(16).Row(row =>
            {
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text("PROPOSTA TÉCNICA COMERCIAL").FontSize(20).Bold().FontColor(Colors.White);
                    c.Item().Text(" AERO CONCEPTS - AEROESPACIAL, INDUSTRIAL E DEFESA LTDA").FontSize(9).Bold().FontColor("#A8C4E0");

                    c.Item().PaddingTop(6);
                    c.Item().Text("CNPJ: 23.995.416/0002-73  |  Insc. Estadual: 125.380.094.115").FontSize(7).FontColor("#A8C4E0");
                    c.Item().Text("Filial SJC: São José dos Campos - SP  |  CEP: 12247-016").FontSize(7).FontColor("#A8C4E0");
                    c.Item().Text("Contato: contato@aeroconcepts.com.br  | +55 12 3905-4003").FontSize(7).FontColor("#A8C4E0");
                });

                row.ConstantItem(100).AlignRight().AlignMiddle()
                    .Text($"#{DateTime.Now:yyyyMMdd}")
                    .FontSize(9).FontColor("#A8C4E0");
            });

            col.Item().BorderBottom(1).BorderColor("#E0E0E0").PaddingVertical(10).Row(row =>
            {
                row.RelativeItem(1.5f).Column(c =>
                {
                    c.Item().Text("CLIENTE").FontSize(7).FontColor("#999999").Bold();
                    c.Item().Text(projeto.Cliente?.nome ?? "Consumidor Final").FontSize(12).Bold().FontColor("#1E3A5F");

                    c.Item().PaddingTop(2);

                    string docCliente = projeto.Cliente?.cpf_cnpj ?? "Não Informado";
                    c.Item().Text($"CNPJ/CPF: {docCliente}").FontSize(8).FontColor("#555555");

                    string contatoCliente = projeto.Cliente?.contato ?? "Não Informado";
                    c.Item().Text($"Contato: {contatoCliente}").FontSize(8).FontColor("#555555");

                    if (projeto.Cliente != null && !string.IsNullOrEmpty(projeto.Cliente.cidade))
                    {
                        c.Item().Text($"Localidade: {projeto.Cliente.cidade}/{projeto.Cliente.estado}").FontSize(8).FontColor("#555555");
                    }
                });

                row.ConstantItem(15);
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text("PROJETO").FontSize(7).FontColor("#999999").Bold();
                    c.Item().Text(projeto.nome).FontSize(12).Bold().FontColor("#1E3A5F");

                    c.Item().PaddingTop(2);
                    c.Item().Text($"Cód. Projeto: PRJ-{projeto.id_projeto}").FontSize(8).FontColor("#555555");
                });

                row.ConstantItem(120).AlignRight().Column(c =>
                {
                    c.Item().Text("DATA DE EMISSÃO").FontSize(7).FontColor("#999999").Bold();
                    c.Item().Text(projeto.Orcamento?.data_criacao.ToString("dd/MM/yyyy") ?? DateTime.Now.ToString("dd/MM/yyyy")).FontSize(11).Bold().FontColor("#1E3A5F");

                    c.Item().PaddingTop(4);

                    c.Item().Text("VÁLIDO ATÉ").FontSize(7).FontColor("#999999").Bold();
                    c.Item().Text(projeto.DataExpiracao.ToString("dd/MM/yyyy")).FontSize(11).Bold().FontColor("#EF4444");
                });
            });
        }

        private void ConstruirConteudoPrincipal(ColumnDescriptor col, Projeto projeto, List<Custo> custosExtras)
        {
            col.Item().Text("1. COMPOSIÇÃO DE MÃO DE OBRA E TAREFAS").FontSize(9).Bold().FontColor("#555555");
            col.Item().PaddingTop(6).PaddingBottom(15).Table(table =>
            {
                table.ColumnsDefinition(cols =>
                {
                    cols.RelativeColumn(4.5f);
                    cols.RelativeColumn(2.5f);
                    cols.RelativeColumn(1);
                    cols.RelativeColumn(2);
                });

                table.Header(header =>
                {
                    header.Cell().Background("#1E3A5F").Padding(8).Text("TAREFAS/DESCRIÇÃO").FontColor(Colors.White).Bold().FontSize(9);
                    header.Cell().Background("#1E3A5F").Padding(8).Text("RESPONSÁVEL").FontColor(Colors.White).Bold().FontSize(9);
                    header.Cell().Background("#1E3A5F").Padding(8).AlignCenter().Text("HORAS").FontColor(Colors.White).Bold().FontSize(9);
                    header.Cell().Background("#1E3A5F").Padding(8).AlignRight().Text("TOTAL").FontColor(Colors.White).Bold().FontSize(9);
                });

                foreach (var item in projeto.Tarefas)
                {
                    table.Cell().BorderBottom(1).BorderColor("#E8EDF2").Padding(8).Text(item.descricao).FontSize(9);
                    table.Cell().BorderBottom(1).BorderColor("#E8EDF2").Padding(8).Text(item.Funcionario?.nome ?? "N/D").FontSize(9);

                    string sufixoHoras = item.horas_estimadas == 1 ? " hora" : " horas";
                    table.Cell().BorderBottom(1).BorderColor("#E8EDF2").Padding(8).AlignCenter().Text($"{item.horas_estimadas:0.#}{sufixoHoras}").FontSize(9);

                    table.Cell().BorderBottom(1).BorderColor("#E8EDF2").Padding(8).AlignRight().Text(item.custo_real.ToString("C2", _ptBR)).FontSize(9).Bold();
                }
            });

            if (custosExtras != null && custosExtras.Any())
            {
                col.Item().Text("2. EQUIPAMENTOS, LICENÇAS E CUSTOS ADICIONAIS").FontSize(9).Bold().FontColor("#555555");
                col.Item().PaddingTop(6).PaddingBottom(15).Table(table =>
                {
                    table.ColumnsDefinition(cols =>
                    {
                        cols.RelativeColumn(5.5f); cols.RelativeColumn(1.5f); cols.RelativeColumn(2);
                    });

                    table.Header(header =>
                    {
                        header.Cell().Background("#1E3A5F").Padding(8).Text("DESCRIÇÃO DO ITEM").FontColor(Colors.White).Bold().FontSize(9);
                        header.Cell().Background("#1E3A5F").Padding(8).Text("CATEGORIA").FontColor(Colors.White).Bold().FontSize(9);
                        header.Cell().Background("#1E3A5F").Padding(8).AlignRight().Text("VALOR").FontColor(Colors.White).Bold().FontSize(9);
                    });

                    foreach (var custo in custosExtras)
                    {
                        table.Cell().BorderBottom(1).BorderColor("#E8EDF2").Padding(8).Text(custo.nome).FontSize(9);
                        table.Cell().BorderBottom(1).BorderColor("#E8EDF2").Padding(8).Text(custo.categoria).FontSize(9);
                        table.Cell().BorderBottom(1).BorderColor("#E8EDF2").Padding(8).AlignRight().Text(custo.valor.ToString("C2", _ptBR)).FontSize(9).Bold();
                    }
                });
            }

            col.Item().Text("3. CONDIÇÕES COMERCIAIS").FontSize(9).Bold().FontColor("#555555");
            col.Item().PaddingTop(6).PaddingBottom(15).Table(tCondicoes =>
            {
                tCondicoes.ColumnsDefinition(c =>
                {
                    c.RelativeColumn();
                    c.RelativeColumn();
                });

                tCondicoes.Header(header =>
                {
                    header.Cell().Background("#1E3A5F").Padding(8).Text("PRAZO TOTAL ESTIMADO").FontColor(Colors.White).Bold().FontSize(9);
                    header.Cell().Background("#1E3A5F").Padding(8).Text("FORMA DE PAGAMENTO").FontColor(Colors.White).Bold().FontSize(9);
                });

                string prazoExibicao = projeto.Orcamento?.prazo_entrega?.ToString("dd/MM/yyyy") ?? "A combinar";
                tCondicoes.Cell().BorderBottom(1).BorderColor("#E8EDF2").Padding(8)
                    .Text(prazoExibicao).FontSize(9);

                tCondicoes.Cell().BorderBottom(1).BorderColor("#E8EDF2").Padding(8)
                    .Text(projeto.Orcamento?.forma_pagamento ?? "A combinar").FontSize(9);
            });

            if (!string.IsNullOrWhiteSpace(projeto.Orcamento?.observacoes))
            {
                col.Item().Column(obsCol =>
                {
                    obsCol.Item().Text("4. OBSERVAÇÕES DA PROPOSTA").FontSize(9).Bold().FontColor("#555555");
                    obsCol.Item().PaddingTop(6).PaddingBottom(15).Table(tObs =>
                    {
                        tObs.ColumnsDefinition(c => c.RelativeColumn());
                        tObs.Header(header =>
                        {
                            header.Cell().Background("#1E3A5F").Padding(8)
                                .Text("NOTAS E OBSERVAÇÕES COMPLEMENTARES").FontColor(Colors.White).Bold().FontSize(9);
                        });

                        tObs.Cell().BorderBottom(1).BorderColor("#E8EDF2").Padding(8).Text(txt =>
                        {
                            var linhas = projeto.Orcamento.observacoes.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

                            foreach (var linha in linhas)
                            {
                                if (!string.IsNullOrWhiteSpace(linha))
                                    txt.Line(linha).FontSize(9);
                                else
                                    txt.Line("");
                            }
                        });
                    });
                });
            }

            col.Item().PaddingTop(15).Row(row =>
            {
                row.RelativeItem();

                row.ConstantItem(300).Column(resumo =>
                {
                    resumo.Item().Text("RESUMO FINANCEIRO").FontSize(9).Bold().FontColor("#555555");
                    resumo.Item().PaddingTop(6).Table(t =>
                    {
                        t.ColumnsDefinition(c => { c.RelativeColumn(3); c.RelativeColumn(1); c.RelativeColumn(2); });

                        void Linha(string label, string pct, string valor, bool destaque = false)
                        {
                            var bg = destaque ? "#1E3A5F" : "#FFFFFF";
                            var fg = destaque ? "#FFFFFF" : "#333333";

                            var cLabel = t.Cell().Background(bg).Padding(6);
                            if (!destaque) cLabel = cLabel.BorderBottom(1).BorderColor("#F1F5F9");
                            var tLabel = cLabel.Text(label).FontSize(9).FontColor(fg);
                            if (destaque) tLabel.Bold();

                            var cPct = t.Cell().Background(bg).Padding(6).AlignCenter();
                            if (!destaque) cPct = cPct.BorderBottom(1).BorderColor("#F1F5F9");
                            var tPct = cPct.Text(pct).FontSize(9).FontColor(fg);
                            if (destaque) tPct.Bold();

                            var cValor = t.Cell().Background(bg).Padding(6).AlignRight();
                            if (!destaque) cValor = cValor.BorderBottom(1).BorderColor("#F1F5F9");
                            var tValor = cValor.Text(valor).FontSize(9).FontColor(fg);
                            if (destaque) tValor.Bold();
                        }

                        decimal custoBase = projeto.Orcamento?.custo_base ?? 0;
                        decimal pctImpostos = projeto.Orcamento?.percentual_impostos ?? 0;
                        decimal valImpostos = projeto.Orcamento?.valor_impostos ?? 0;
                        decimal pctMargem = projeto.Orcamento?.margem_percentual ?? 0;
                        decimal valMargem = projeto.Orcamento?.valor_margem ?? 0;
                        decimal valFinal = projeto.Orcamento?.valor_final ?? 0;

                        Linha("Custo Total Base", "", custoBase.ToString("C2", _ptBR));
                        Linha("Impostos", $"{pctImpostos:0.#}%", valImpostos.ToString("C2", _ptBR));
                        Linha("Margem de Lucro", $"{pctMargem:0.#}%", valMargem.ToString("C2", _ptBR));
                        Linha("VALOR TOTAL DA PROPOSTA", "", valFinal.ToString("C2", _ptBR), destaque: true);
                    });
                });
            });

            col.Item().PaddingTop(80).Row(row =>
            {
                row.RelativeItem().Column(assinaturaEmpresa =>
                {
                    assinaturaEmpresa.Item().BorderBottom(1).BorderColor("#A0AEC0").PaddingBottom(2);
                    assinaturaEmpresa.Item().PaddingTop(4).Text("Aero Concepts — Engenharia Aeronáutica").FontSize(9).Bold().FontColor("#2D3748");
                    assinaturaEmpresa.Item().Text("Responsável Técnico / Comercial").FontSize(8).FontColor("#718096");
                });

                row.ConstantItem(40);

                row.RelativeItem().Column(assinaturaCliente =>
                {
                    assinaturaCliente.Item().BorderBottom(1).BorderColor("#A0AEC0").PaddingBottom(2);
                    assinaturaCliente.Item().PaddingTop(4).Text($"De acordo: {projeto.Cliente?.nome ?? "Funcate"}").FontSize(9).Bold().FontColor("#2D3748");
                    assinaturaCliente.Item().Text("Assinatura do Cliente / Data").FontSize(8).FontColor("#718096");
                });
            });
        }

        private void ConstruirCabecalhoRelatorio(ColumnDescriptor col, string tipoRelatorio, int totalRegistros)
        {
            col.Item().Background("#1E3A5F").Padding(14).Row(row =>
            {
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text($"RELATÓRIO GERENCIAL DE {tipoRelatorio.ToUpper()}").FontSize(18).Bold().FontColor(Colors.White);
                    c.Item().Text("AERO CONCEPTS — AEROESPACIAL, INDUSTRIAL E DEFESA LTDA").FontSize(10).FontColor("#A8C4E0");
                });

                row.ConstantItem(220).AlignRight().AlignMiddle().Column(c =>
                {
                    c.Item().Text($"Emitido em: {DateTime.Now:dd/MM/yyyy HH:mm}").FontSize(10).FontColor(Colors.White);
                    c.Item().Text($"Total de registros exibidos: {totalRegistros}").FontSize(10).FontColor("#A8C4E0").Bold();
                });
            });
        }

        private void ConstruirTabelaRelatorioProjetos(ColumnDescriptor col, List<Projeto> projetos)
        {
            col.Item().Table(table =>
            {
                table.ColumnsDefinition(cols =>
                {
                    cols.ConstantColumn(45);
                    cols.RelativeColumn(3);
                    cols.RelativeColumn(2.5f);
                    cols.RelativeColumn(1.3f);
                    cols.RelativeColumn(1.5f);
                    cols.ConstantColumn(85);
                    cols.RelativeColumn(2.2f);
                });

                table.Header(header =>
                {
                    header.Cell().Background("#2D3748").Padding(8).Text("ID").FontColor(Colors.White).Bold().FontSize(9.5f);
                    header.Cell().Background("#2D3748").Padding(8).Text("NOME DO PROJETO").FontColor(Colors.White).Bold().FontSize(9.5f);
                    header.Cell().Background("#2D3748").Padding(8).Text("CLIENTE").FontColor(Colors.White).Bold().FontSize(9.5f);
                    header.Cell().Background("#2D3748").Padding(8).Text("TIPO").FontColor(Colors.White).Bold().FontSize(9.5f);
                    header.Cell().Background("#2D3748").Padding(8).Text("STATUS").FontColor(Colors.White).Bold().FontSize(9.5f);
                    header.Cell().Background("#2D3748").Padding(8).Text("VENCIMENTO").FontColor(Colors.White).Bold().FontSize(9.5f);
                    header.Cell().Background("#2D3748").Padding(8).AlignRight().Text("VALOR FINAL").FontColor(Colors.White).Bold().FontSize(9.5f);
                });

                bool listraAlternada = false;
                foreach (var p in projetos)
                {
                    string corFundo = listraAlternada ? "#F8FAFC" : "#FFFFFF";

                    table.Cell().Background(corFundo).BorderBottom(1).BorderColor("#E2E8F0").Padding(8).AlignCenter().Text(p.id_projeto.ToString()).FontSize(10);
                    table.Cell().Background(corFundo).BorderBottom(1).BorderColor("#E2E8F0").Padding(8).Text(p.nome ?? "-").FontSize(10).Bold();
                    table.Cell().Background(corFundo).BorderBottom(1).BorderColor("#E2E8F0").Padding(8).Text(p.Cliente?.nome ?? "Consumidor Final").FontSize(10);
                    table.Cell().Background(corFundo).BorderBottom(1).BorderColor("#E2E8F0").Padding(8).Text(p.tipo ?? "-").FontSize(10);
                    table.Cell().Background(corFundo).BorderBottom(1).BorderColor("#E2E8F0").Padding(8).Text(p.status?.ToUpper() ?? "N/D").FontSize(10).Bold();

                    string corData = p.EstaVencido ? "#EF4444" : "#333333";
                    table.Cell().Background(corFundo).BorderBottom(1).BorderColor("#E2E8F0").Padding(8).AlignCenter().Text(p.DataExpiracao.ToString("dd/MM/yyyy")).FontSize(10).FontColor(corData);

                    decimal valorFinal = p.Orcamento?.valor_final ?? 0;
                    table.Cell().Background(corFundo).BorderBottom(1).BorderColor("#E2E8F0").Padding(8).AlignRight().Text(valorFinal.ToString("C2", _ptBR)).FontSize(10).Bold();

                    listraAlternada = !listraAlternada;
                }
            });

            decimal totalFaturado = projetos.Where(p => p.Orcamento != null).Sum(p => p.Orcamento.valor_final);
            decimal totalLucro = projetos.Where(p => p.Orcamento != null).Sum(p => p.Orcamento.valor_margem);

            col.Item().PaddingTop(12).AlignRight().Width(280).Table(t =>
            {
                t.ColumnsDefinition(c => { c.RelativeColumn(1); c.RelativeColumn(1); });

                t.Cell().Background("#EDF2F7").Padding(6).Text("Lucro Estimado:").FontSize(10).FontColor("#2D3748").Bold();
                t.Cell().Background("#EDF2F7").Padding(6).AlignRight().Text(totalLucro.ToString("C2", _ptBR)).FontSize(10).FontColor("#2D3748").Bold();

                t.Cell().Background("#1E3A5F").Padding(6).Text("Faturamento Total:").FontSize(10).FontColor(Colors.White).Bold();
                t.Cell().Background("#1E3A5F").Padding(6).AlignRight().Text(totalFaturado.ToString("C2", _ptBR)).FontSize(10).FontColor(Colors.White).Bold();
            });
        }

        private void ConstruirTabelaRelatorioCargos(ColumnDescriptor col, List<Cargo> cargos)
        {
            col.Item().Table(table =>
            {
                table.ColumnsDefinition(cols =>
                {
                    cols.ConstantColumn(60);
                    cols.RelativeColumn(6);
                    cols.RelativeColumn(3);
                });

                table.Header(header =>
                {
                    header.Cell().Background("#2D3748").Padding(8).Text("ID").FontColor(Colors.White).Bold().FontSize(9.5f);
                    header.Cell().Background("#2D3748").Padding(8).Text("NOME DO CARGO").FontColor(Colors.White).Bold().FontSize(9.5f);
                    header.Cell().Background("#2D3748").Padding(8).AlignCenter().Text("CUSTO MÉDIO / HORA").FontColor(Colors.White).Bold().FontSize(9.5f);
                });

                bool listraAlternada = false;
                foreach (var c in cargos)
                {
                    string corFundo = listraAlternada ? "#F8FAFC" : "#FFFFFF";

                    table.Cell().Background(corFundo).BorderBottom(1).BorderColor("#E2E8F0").Padding(8).AlignCenter().Text(c.id_cargo.ToString()).FontSize(10);
                    table.Cell().Background(corFundo).BorderBottom(1).BorderColor("#E2E8F0").Padding(8).Text(c.nome ?? "-").FontSize(10).Bold();

                    decimal custoHora = c.custo_medio_hora;
                    table.Cell().Background(corFundo).BorderBottom(1).BorderColor("#E2E8F0").Padding(8).AlignCenter()
                        .Text(custoHora.ToString("C2", _ptBR)).FontSize(10).Bold().FontColor("#009140");

                    listraAlternada = !listraAlternada;
                }
            });
        }

        private void ConstruirTabelaRelatorioFuncionarios(ColumnDescriptor col, List<Funcionario> funcionarios)
        {
            col.Item().Table(table =>
            {
                table.ColumnsDefinition(cols =>
                {
                    cols.ConstantColumn(50);
                    cols.RelativeColumn(4);
                    cols.RelativeColumn(2.5f);
                    cols.RelativeColumn(2.2f);
                    cols.RelativeColumn(1.5f);
                });

                table.Header(header =>
                {
                    header.Cell().Background("#2D3748").Padding(8).Text("ID").FontColor(Colors.White).Bold().FontSize(9.5f);
                    header.Cell().Background("#2D3748").Padding(8).Text("NOME COMPLETO").FontColor(Colors.White).Bold().FontSize(9.5f);
                    header.Cell().Background("#2D3748").Padding(8).Text("TIPO DE VÍNCULO").FontColor(Colors.White).Bold().FontSize(9.5f);
                    header.Cell().Background("#2D3748").Padding(8).AlignLeft().Text("CUSTO/HORA").FontColor(Colors.White).Bold().FontSize(9.5f);
                    header.Cell().Background("#2D3748").Padding(8).Text("STATUS").FontColor(Colors.White).Bold().FontSize(9.5f);
                });

                bool listraAlternada = false;
                foreach (var f in funcionarios)
                {
                    string corFundo = listraAlternada ? "#F8FAFC" : "#FFFFFF";

                    table.Cell().Background(corFundo).BorderBottom(1).BorderColor("#E2E8F0").Padding(8).AlignCenter().Text(f.id_funcionario.ToString()).FontSize(10);
                    table.Cell().Background(corFundo).BorderBottom(1).BorderColor("#E2E8F0").Padding(8).Text(f.nome ?? "-").FontSize(10).Bold();
                    table.Cell().Background(corFundo).BorderBottom(1).BorderColor("#E2E8F0").Padding(8).Text(f.tipo_vinculo ?? "-").FontSize(10);

                    decimal custoHora = f.custo_hora;
                    table.Cell().Background(corFundo).BorderBottom(1).BorderColor("#E2E8F0").Padding(8).AlignLeft().Text(custoHora.ToString("C2", _ptBR)).FontSize(10);

                    string corStatus = (f.status?.ToLower() == "ativo") ? "#009140" : "#EF4444";
                    table.Cell().Background(corFundo).BorderBottom(1).BorderColor("#E2E8F0").Padding(8).Text(f.status?.ToUpper() ?? "N/D").FontSize(10).Bold().FontColor(corStatus);

                    listraAlternada = !listraAlternada;
                }
            });
        }

        private void ConstruirTabelaRelatorioClientes(ColumnDescriptor col, List<Cliente> clientes)
        {
            col.Item().Table(table =>
            {
                table.ColumnsDefinition(cols =>
                {
                    cols.ConstantColumn(45);
                    cols.RelativeColumn(3.0f);
                    cols.RelativeColumn(1.0f);
                    cols.RelativeColumn(1.8f);
                    cols.RelativeColumn(2.2f);
                    cols.RelativeColumn(1.8f);
                });

                table.Header(header =>
                {
                    header.Cell().Background("#2D3748").Padding(8).Text("ID").FontColor(Colors.White).Bold().FontSize(9.5f);
                    header.Cell().Background("#2D3748").Padding(8).Text("NOME / RAZÃO SOCIAL").FontColor(Colors.White).Bold().FontSize(9.5f);
                    header.Cell().Background("#2D3748").Padding(8).Text("TIPO").FontColor(Colors.White).Bold().FontSize(9.5f);
                    header.Cell().Background("#2D3748").Padding(8).Text("CPF / CNPJ").FontColor(Colors.White).Bold().FontSize(9.5f);
                    header.Cell().Background("#2D3748").Padding(8).Text("LOCALIZAÇÃO").FontColor(Colors.White).Bold().FontSize(9.5f);
                    header.Cell().Background("#2D3748").Padding(8).Text("CONTATO").FontColor(Colors.White).Bold().FontSize(9.5f);
                });

                bool listraAlternada = false;
                foreach (var c in clientes)
                {
                    string corFundo = listraAlternada ? "#F8FAFC" : "#FFFFFF";

                    table.Cell().Background(corFundo).BorderBottom(1).BorderColor("#E2E8F0").Padding(8).AlignCenter().Text(c.id_cliente.ToString()).FontSize(10);
                    table.Cell().Background(corFundo).BorderBottom(1).BorderColor("#E2E8F0").Padding(8).Text(c.nome ?? "-").FontSize(10).Bold();
                    table.Cell().Background(corFundo).BorderBottom(1).BorderColor("#E2E8F0").Padding(8).Text(c.tipo ?? "-").FontSize(10);
                    table.Cell().Background(corFundo).BorderBottom(1).BorderColor("#E2E8F0").Padding(8).Text(c.cpf_cnpj ?? "-").FontSize(10);
                    table.Cell().Background(corFundo).BorderBottom(1).BorderColor("#E2E8F0").Padding(8).Text($"{c.cidade ?? "-"} - {c.estado ?? "-"}").FontSize(10);
                    table.Cell().Background(corFundo).BorderBottom(1).BorderColor("#E2E8F0").Padding(8).Text(c.contato ?? "-").FontSize(10);

                    listraAlternada = !listraAlternada;
                }
            });
        }

        private void ConstruirTabelaRelatorioCustos(ColumnDescriptor col, List<CatalogoCusto> custos)
        {
            col.Item().Table(table =>
            {
                table.ColumnsDefinition(cols =>
                {
                    cols.ConstantColumn(60);
                    cols.RelativeColumn(5.5f);
                    cols.RelativeColumn(3.5f);
                    cols.RelativeColumn(2.5f);
                });

                table.Header(header =>
                {
                    header.Cell().Background("#2D3748").Padding(8).Text("ID").FontColor(Colors.White).Bold().FontSize(9.5f);
                    header.Cell().Background("#2D3748").Padding(8).Text("ITEM / DESCRIÇÃO").FontColor(Colors.White).Bold().FontSize(9.5f);
                    header.Cell().Background("#2D3748").Padding(8).Text("CATEGORIA").FontColor(Colors.White).Bold().FontSize(9.5f);
                    header.Cell().Background("#2D3748").Padding(8).AlignRight().Text("VALOR").FontColor(Colors.White).Bold().FontSize(9.5f);
                });

                bool listraAlternada = false;
                foreach (var c in custos)
                {
                    string corFundo = listraAlternada ? "#F8FAFC" : "#FFFFFF";

                    table.Cell().Background(corFundo).BorderBottom(1).BorderColor("#E2E8F0").Padding(8).AlignCenter().Text(c.id_catalogo_custo.ToString()).FontSize(10);
                    table.Cell().Background(corFundo).BorderBottom(1).BorderColor("#E2E8F0").Padding(8).Text(c.nome ?? "-").FontSize(10).Bold();
                    table.Cell().Background(corFundo).BorderBottom(1).BorderColor("#E2E8F0").Padding(8).Text(c.categoria ?? "-").FontSize(10);
                    table.Cell().Background(corFundo).BorderBottom(1).BorderColor("#E2E8F0").Padding(8).AlignRight().Text(c.valor.ToString("C2", _ptBR)).FontSize(10).Bold().FontColor("#009140");

                    listraAlternada = !listraAlternada;
                }
            });

            decimal somaCustos = custos.Sum(c => c.valor);

            col.Item().PaddingTop(12).AlignRight().Width(250).Table(t =>
            {
                t.ColumnsDefinition(c => { c.RelativeColumn(1); c.RelativeColumn(1.2f); });

                t.Cell().Background("#1E3A5F").Padding(6).Text("Valor Acumulado:").FontSize(10).FontColor(Colors.White).Bold();
                t.Cell().Background("#1E3A5F").Padding(6).AlignRight().Text(somaCustos.ToString("C2", _ptBR)).FontSize(10).FontColor(Colors.White).Bold();
            });
        }

        private void ConstruirTabelaRelatorioUsuarios(ColumnDescriptor col, List<Usuario> usuarios)
        {
            col.Item().Table(table =>
            {
                table.ColumnsDefinition(cols =>
                {
                    cols.ConstantColumn(50);
                    cols.RelativeColumn(3.5f);
                    cols.RelativeColumn(3.5f);
                    cols.RelativeColumn(2.0f);
                });

                table.Header(header =>
                {
                    header.Cell().Background("#2D3748").Padding(8).Text("ID").FontColor(Colors.White).Bold().FontSize(9.5f);
                    header.Cell().Background("#2D3748").Padding(8).Text("NOME COMPLETO").FontColor(Colors.White).Bold().FontSize(9.5f);
                    header.Cell().Background("#2D3748").Padding(8).Text("E-MAIL DE ACESSO").FontColor(Colors.White).Bold().FontSize(9.5f);
                    header.Cell().Background("#2D3748").Padding(8).Text("PERFIL DE SISTEMA").FontColor(Colors.White).Bold().FontSize(9.5f);
                });

                bool listraAlternada = false;
                foreach (var u in usuarios)
                {
                    string corFundo = listraAlternada ? "#F8FAFC" : "#FFFFFF";

                    table.Cell().Background(corFundo).BorderBottom(1).BorderColor("#E2E8F0").Padding(8).AlignCenter().Text(u.id_usuario.ToString()).FontSize(10);
                    table.Cell().Background(corFundo).BorderBottom(1).BorderColor("#E2E8F0").Padding(8).Text(u.nome ?? "-").FontSize(10).Bold();
                    table.Cell().Background(corFundo).BorderBottom(1).BorderColor("#E2E8F0").Padding(8).Text(u.email ?? "-").FontSize(10);
                    table.Cell().Background(corFundo).BorderBottom(1).BorderColor("#E2E8F0").Padding(8).Text(u.status?.ToUpper() ?? "-").FontSize(10).Bold().FontColor("#009140");

                    listraAlternada = !listraAlternada;
                }
            });
        }

        private void ConstruirRodape(RowDescriptor row)
        {
            row.RelativeItem().Text(" AERO CONCEPTS - AEROESPACIAL, INDUSTRIAL E DEFESA LTDA").FontSize(8).FontColor("#AAAAAA");
            row.ConstantItem(80).AlignRight().Text(x =>
            {
                x.Span("Página ").FontSize(8);
                x.CurrentPageNumber().FontSize(8);
                x.Span(" de ").FontSize(8);
                x.TotalPages().FontSize(8);
            });
        }

        #endregion

        #region Calendário Corporativo

        public void GerarCalendarioCorporativo(
            AnoCalendario ano,
            List<EventoCalendario> eventos,
            List<magal.ViewModels.ResumoMes> resumo,
            string caminhoArquivo)
        {
            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(8, Unit.Millimetre);
                    page.DefaultTextStyle(x => x.FontSize(8).FontFamily("Arial"));

                    page.Header().Element(ConstruirCabecalhoCalendario(ano));
                    page.Content().Element(ConstruirCorpoCalendario(ano, eventos, resumo));
                    page.Footer().Row(row => ConstruirRodape(row));
                });

                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(8, Unit.Millimetre);
                    page.DefaultTextStyle(x => x.FontSize(8).FontFamily("Arial"));

                    page.Header().Element(ConstruirCabecalhoCalendario(ano));
                    page.Content().Element(ConstruirResumoCalendarioPdf(resumo, ano));
                    page.Footer().Row(row => ConstruirRodape(row));
                });
            }).GeneratePdf(caminhoArquivo);
        }

        private Action<IContainer> ConstruirCabecalhoCalendario(AnoCalendario ano)
        {
            return container => container
                .BorderBottom(1).BorderColor("#1E293B")
                .Padding(4)
                .Row(row =>
                {
                    row.RelativeItem().Text($"AERO CONCEPTS — Calendário Corporativo {ano.ano}")
                        .FontSize(12).Bold().FontColor("#1E293B");
                    row.ConstantItem(200).AlignRight()
                        .Text($"Horas/dia: {ano.horas_dia:F1}   Gerado em: {DateTime.Now:dd/MM/yyyy}")
                        .FontSize(8).FontColor("#64748B");
                });
        }

        private Action<IContainer> ConstruirCorpoCalendario(
    AnoCalendario ano,
    List<EventoCalendario> eventos,
    List<magal.ViewModels.ResumoMes> resumo)
        {
            return container =>
            {
                container.Column(col =>
                {
                    col.Spacing(6);

                    // Lista de eventos acima da grade
                    var eventosLegenda = eventos.OrderBy(e => e.data_observada).ToList();
                    if (eventosLegenda.Any())
                    {
                        col.Item().Border(1).BorderColor("#CBD5E1").Column(legCol =>
                        {
                            legCol.Item().Background("#F8FAFC").Padding(4)
                                .Text("LEGENDA").FontSize(8).Bold().AlignCenter().FontColor("#1E293B");

                            legCol.Item().Padding(6).Row(row =>
                            {
                                int porColuna = Math.Min(4, (int)Math.Ceiling(eventosLegenda.Count / 4.0));
                                int numColunas = (int)Math.Ceiling(eventosLegenda.Count / (double)porColuna);

                                for (int i = 0; i < numColunas; i++)
                                {
                                    var grupo = eventosLegenda.Skip(i * porColuna).Take(porColuna).ToList();
                                    row.RelativeItem().Column(c =>
                                    {
                                        foreach (var ev in grupo)
                                            c.Item().Text($"{ev.data_observada:dd.MM.}  {ev.descricao}")
                                                .FontSize(7).FontColor("#374151");
                                    });
                                    if (i < numColunas - 1) row.ConstantItem(8);
                                }
                            });
                        });
                    }
                    // Grid 4×3 de meses
                    for (int row = 0; row < 4; row++)
                    {
                        col.Item().Row(rowDesc =>
                        {
                            for (int c = 0; c < 3; c++)
                            {
                                int mes = row * 3 + c + 1;
                                rowDesc.RelativeItem().Padding(3)
                                    .Element(ConstruirMesPdf(mes, ano, eventos));
                                if (c < 2) rowDesc.ConstantItem(4);
                            }
                        });
                    }

                    // Legenda de cores abaixo da grade
                    col.Item().PaddingTop(4).Row(legendRow =>
                    {
                        LegendItem(legendRow, "#4472C4", "Feriado Nacional");
                        LegendItem(legendRow, "#ED7D31", "Feriado Est./Mun.");
                        LegendItem(legendRow, "#BDD7EE", "Ponte");
                        LegendItem(legendRow, "#00B0F0", "Férias Coletivas");
                    });
                });
            };
        }

        private Action<IContainer> ConstruirMesPdf(int mes, AnoCalendario ano, List<EventoCalendario> eventos)
        {
            var nomesMes = new[] { "", "Janeiro", "Fevereiro", "Março", "Abril", "Maio", "Junho",
                               "Julho", "Agosto", "Setembro", "Outubro", "Novembro", "Dezembro" };

            return container =>
            {
                container.Border(1).BorderColor("#CBD5E1").Column(col =>
                {
                    col.Item().Background("#1F3864").Padding(3)
                        .Text(nomesMes[mes]).FontSize(8).Bold().FontColor("#FFFFFF").AlignCenter();

                    col.Item().Row(hr =>
                    {
                        DayHeader(hr, "SMA");
                        DayHeader(hr, "Seg");
                        DayHeader(hr, "Ter");
                        DayHeader(hr, "Qua");
                        DayHeader(hr, "Qui");
                        DayHeader(hr, "Sex");
                        DayHeader(hr, "Sáb");
                        DayHeader(hr, "Dom");
                    });

                    var primeiroDia = new DateTime(ano.ano, mes, 1);
                    int dow = (int)primeiroDia.DayOfWeek;
                    int offset = dow == 0 ? 6 : dow - 1;
                    var inicioSemana = primeiroDia.AddDays(-offset);

                    while (inicioSemana <= new DateTime(ano.ano, mes, DateTime.DaysInMonth(ano.ano, mes)))
                    {
                        var semanaDias = Enumerable.Range(0, 7).Select(d => inicioSemana.AddDays(d)).ToList();
                        int semNum = System.Globalization.ISOWeek.GetWeekOfYear(inicioSemana);

                        col.Item().Row(wr =>
                        {
                            wr.ConstantItem(22).Background("#E2E8F0").BorderBottom(1).BorderColor("#CBD5E1")
                                .Padding(2).AlignCenter().Text(semNum.ToString()).FontSize(7).FontColor("#64748B");

                            foreach (var dia in semanaDias)
                            {
                                bool deMes = dia.Month == mes;
                                bool isSabado = dia.DayOfWeek == DayOfWeek.Saturday;
                                bool isDomingo = dia.DayOfWeek == DayOfWeek.Sunday;

                                string bg = "#FFFFFF";
                                string fg = "#1E293B";

                                if (deMes)
                                {
                                    if (isSabado)
                                    {
                                        bg = "#00FF00"; fg = "#000000";
                                    }
                                    else if (isDomingo)
                                    {
                                        bg = "#2D9B6C"; fg = "#000000";
                                    }
                                    else if (ano.inicio_ferias.HasValue && ano.fim_ferias.HasValue
                                        && dia.Date >= ano.inicio_ferias.Value.Date
                                        && dia.Date <= ano.fim_ferias.Value.Date)
                                    {
                                        bg = "#00B0F0"; fg = "#1E293B";
                                    }
                                    else
                                    {
                                        var ev = eventos.FirstOrDefault(e => e.data_observada.Date == dia.Date);
                                        if (ev != null)
                                        {
                                            (bg, fg) = ev.tipo switch
                                            {
                                                "Feriado Nacional" => ("#4472C4", "#FFFFFF"),
                                                "Feriado Estadual/Municipal" => ("#ED7D31", "#1E293B"),
                                                "Facultativo" => ("#7030A0", "#FFFFFF"),
                                                "Ponte" => ("#BDD7EE", "#1E293B"),
                                                "Férias Coletivas" => ("#00B0F0", "#1E293B"),
                                                _ => ("#FFFFFF", "#1E293B")
                                            };
                                        }
                                    }
                                }
                                else
                                {
                                    bg = "#F8FAFC"; fg = "#CBD5E1";
                                }

                                string txt = deMes ? dia.Day.ToString() : "";
                                wr.RelativeItem().Background(bg).BorderBottom(1).BorderColor("#E2E8F0")
                                    .Padding(2).AlignCenter().Text(txt).FontSize(7).FontColor(fg).SemiBold();
                            }
                        });

                        inicioSemana = inicioSemana.AddDays(7);
                    }
                });
            };
        }

        private static void DayHeader(RowDescriptor row, string text)
        {
            if (text == "SMA")
                row.ConstantItem(22).Background("#1F3864").Padding(2).AlignCenter()
                    .Text(text).FontSize(6).Bold().FontColor("#FFFFFF");
            else
                row.RelativeItem().Background("#1F3864").Padding(2).AlignCenter()
                    .Text(text).FontSize(6).Bold().FontColor("#FFFFFF");
        }

        private Action<IContainer> ConstruirResumoCalendarioPdf(
    List<magal.ViewModels.ResumoMes> resumo,
    AnoCalendario ano)
        {
            return container =>
            {
                // Exclui a linha "TOTAL" que o ViewModel já adiciona
                var resumoMeses = resumo.Where(r => r.NomeMes != "TOTAL").ToList();

                container.Column(col =>
                {
                    col.Spacing(8);
                    col.Item().Text("Resumo Anual).FontSize(12).Bold().FontColor("#1E293B");

                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(cols =>
                        {
                            cols.RelativeColumn(2);
                            cols.RelativeColumn();
                            cols.RelativeColumn();
                            cols.RelativeColumn();
                            cols.RelativeColumn();
                            cols.RelativeColumn();
                        });

                        string[] headers = { "Mês", "DU s/Pontes", "Feriados DU", "Pontes", "Total", "DU c/Pontes" };
                        table.Header(header =>
                        {
                            foreach (var h in headers)
                                header.Cell().Background("#1E293B").Padding(5)
                                    .Text(h).FontSize(8).Bold().FontColor("#FFFFFF").AlignCenter();
                        });

                        bool alt = false;
                        foreach (var r in resumoMeses)
                        {
                            string bg = alt ? "#F8FAFC" : "#FFFFFF";
                            table.Cell().Background(bg).BorderBottom(1).BorderColor("#E2E8F0").Padding(5)
                                .Text(r.NomeMes).FontSize(9).Bold();
                            table.Cell().Background("#FFF9C4").BorderBottom(1).BorderColor("#E2E8F0").Padding(5)
                                .AlignCenter().Text(r.DiasUteisSemPontes.ToString()).FontSize(9);
                            table.Cell().Background(bg).BorderBottom(1).BorderColor("#E2E8F0").Padding(5)
                                .AlignCenter().Text(r.FeriadosEmDU.ToString()).FontSize(9);
                            table.Cell().Background(bg).BorderBottom(1).BorderColor("#E2E8F0").Padding(5)
                                .AlignCenter().Text(r.Pontes.ToString()).FontSize(9);
                            table.Cell().Background(bg).BorderBottom(1).BorderColor("#E2E8F0").Padding(5)
                                .AlignCenter().Text(r.TotalDiasUteis.ToString()).FontSize(9);
                            table.Cell().Background("#DCFCE7").BorderBottom(1).BorderColor("#E2E8F0").Padding(5)
                                .AlignCenter().Text(r.DiasUteisComPontes.ToString()).FontSize(9);
                            alt = !alt;
                        }

                        int totDUSP = resumoMeses.Sum(r => r.DiasUteisSemPontes);
                        int totFer = resumoMeses.Sum(r => r.FeriadosEmDU);
                        int totPontes = resumoMeses.Sum(r => r.Pontes);
                        int totTotal = resumoMeses.Sum(r => r.TotalDiasUteis);
                        int totDUCP = resumoMeses.Sum(r => r.DiasUteisComPontes);

                        table.Cell().Background("#1E293B").Padding(5).Text("TOTAL").FontSize(9).Bold().FontColor("#FFFFFF");
                        table.Cell().Background("#1E293B").Padding(5).AlignCenter().Text(totDUSP.ToString()).FontSize(9).Bold().FontColor("#FFFFFF");
                        table.Cell().Background("#1E293B").Padding(5).AlignCenter().Text(totFer.ToString()).FontSize(9).Bold().FontColor("#FFFFFF");
                        table.Cell().Background("#1E293B").Padding(5).AlignCenter().Text(totPontes.ToString()).FontSize(9).Bold().FontColor("#FFFFFF");
                        table.Cell().Background("#1E293B").Padding(5).AlignCenter().Text(totTotal.ToString()).FontSize(9).Bold().FontColor("#FFFFFF");
                        table.Cell().Background("#1E293B").Padding(5).AlignCenter().Text(totDUCP.ToString()).FontSize(9).Bold().FontColor("#FFFFFF");
                    });

                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Background("#0F172A").Padding(10).Text(
                            $"Total Horas/Ano: {(resumoMeses.Sum(r => r.DiasUteisSemPontes) * ano.horas_dia):F1} h   " +
                            $"({ano.horas_dia:F1} h/dia × {resumoMeses.Sum(r => r.DiasUteisSemPontes)} DU s/Pontes)")
                            .FontSize(10).Bold().FontColor("#FFFFFF");
                    });
                });
            };
        }

        private static void LegendItem(RowDescriptor row, string cor, string label)
        {
            row.ConstantItem(12).Height(12).Background(cor);
            row.ConstantItem(4);
            row.AutoItem().PaddingRight(10).Text(label).FontSize(7).FontColor("#374151");
        }

       
        #endregion
    }
}