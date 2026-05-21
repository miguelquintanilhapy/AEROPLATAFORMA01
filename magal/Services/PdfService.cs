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
    /// <summary>
    /// Serviço responsável pela geração e exportação de documentos em formato PDF no sistema.
    /// </summary>
    public class PdfService
    {
        #region Atributos e Campos Privados

        /// <summary>
        /// Cultura padrão utilizada para a formatação de valores monetários e datas em formato PT-BR.
        /// </summary>
        private static readonly CultureInfo _ptBR = new("pt-BR");

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Gera e salva o arquivo de Proposta Técnica Comercial em PDF com base nos dados do projeto e custos extras informados.
        /// </summary>
        public void GerarPropostaTecnica(Projeto projeto, List<Custo> custosExtras, string caminhoArquivo)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                    page.Header().Column(col => ConstruirCabecalho(col, projeto));
                    page.Content().PaddingTop(16).Column(col => ConstruirConteudoPrincipal(col, projeto, custosExtras));
                    page.Footer().BorderTop(1).BorderColor("#E0E0E0").PaddingTop(8).Row(ConstruirRodape);
                });
            }).GeneratePdf(caminhoArquivo);
        }

        /// <summary>
        /// Gera e salva um relatório gerencial em PDF contendo a listagem tabular dos projetos.
        /// </summary>
        public void GerarRelatorioTabelaProjetos(List<Projeto> projetos, string caminhoArquivo)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape()); // Modo Paisagem
                    page.Margin(1.5f, Unit.Centimetre);
                    page.PageColor(Colors.White);

                    // ALTERADO: Aumentada a fonte padrão do relatório para maior legibilidade
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

                    page.Header().Column(col => ConstruirCabecalhoRelatorio(col, projetos.Count));
                    page.Content().PaddingTop(16).Column(col => ConstruirTabelaRelatorioProjetos(col, projetos));
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
                    c.Item().Text("AERO CONCEPTS — ENGENHARIA AERONÁUTICA").FontSize(9).FontColor("#A8C4E0");
                });
                row.ConstantItem(100).AlignRight().AlignMiddle()
                    .Text($"#{DateTime.Now:yyyyMMdd}")
                    .FontSize(9).FontColor("#A8C4E0");
            });

            col.Item().BorderBottom(1).BorderColor("#E0E0E0").PaddingVertical(10).Row(row =>
            {
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text("CLIENTE").FontSize(7).FontColor("#999999").Bold();
                    c.Item().Text(projeto.Cliente?.nome ?? "Consumidor Final").FontSize(12).Bold().FontColor("#1E3A5F");
                });

                row.ConstantItem(20);

                row.RelativeItem().Column(c =>
                {
                    c.Item().Text("PROJETO").FontSize(7).FontColor("#999999").Bold();
                    c.Item().Text(projeto.nome).FontSize(12).Bold().FontColor("#1E3A5F");
                });

                row.ConstantItem(150).Column(c =>
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
                    cols.RelativeColumn(3); cols.RelativeColumn(2); cols.RelativeColumn(1); cols.RelativeColumn(2); cols.RelativeColumn(2);
                });

                table.Header(header =>
                {
                    header.Cell().Background("#1E3A5F").Padding(8).Text("TAREFAS/DESCRIÇÃO").FontColor(Colors.White).Bold().FontSize(9);
                    header.Cell().Background("#1E3A5F").Padding(8).Text("RESPONSÁVEL").FontColor(Colors.White).Bold().FontSize(9);
                    header.Cell().Background("#1E3A5F").Padding(8).AlignCenter().Text("HORAS").FontColor(Colors.White).Bold().FontSize(9);
                    header.Cell().Background("#1E3A5F").Padding(8).AlignRight().Text("VALOR/HORA").FontColor(Colors.White).Bold().FontSize(9);
                    header.Cell().Background("#1E3A5F").Padding(8).AlignRight().Text("TOTAL").FontColor(Colors.White).Bold().FontSize(9);
                });

                foreach (var item in projeto.Tarefas)
                {
                    table.Cell().BorderBottom(1).BorderColor("#E8EDF2").Padding(8).Text(item.descricao).FontSize(9);
                    table.Cell().BorderBottom(1).BorderColor("#E8EDF2").Padding(8).Text(item.Funcionario?.nome ?? "N/D").FontSize(9);
                    table.Cell().BorderBottom(1).BorderColor("#E8EDF2").Padding(8).AlignCenter().Text($"{item.horas_estimadas:0.#}h").FontSize(9);

                    decimal vHora = item.Funcionario?.Cargo?.custo_medio_hora ?? 0;
                    table.Cell().BorderBottom(1).BorderColor("#E8EDF2").Padding(8).AlignRight().Text(vHora.ToString("C2", _ptBR)).FontSize(9);
                    table.Cell().BorderBottom(1).BorderColor("#E8EDF2").Padding(8).AlignRight().Text(item.custo_real.ToString("C2", _ptBR)).FontSize(9).Bold();
                }
            });

            if (custosExtras != null && custosExtras.Any())
            {
                col.Item().Text("2. EQUIPAMENTOS, LICENÇAS E CUSTOS ADICIONAIS").FontSize(9).Bold().FontColor("#555555");
                col.Item().PaddingTop(6).Table(table =>
                {
                    table.ColumnsDefinition(cols =>
                    {
                        cols.RelativeColumn(4); cols.RelativeColumn(2); cols.RelativeColumn(2);
                    });

                    table.Header(header =>
                    {
                        header.Cell().Background("#4A5568").Padding(8).Text("DESCRIÇÃO DO ITEM").FontColor(Colors.White).Bold().FontSize(9);
                        header.Cell().Background("#4A5568").Padding(8).Text("CATEGORIA").FontColor(Colors.White).Bold().FontSize(9);
                        header.Cell().Background("#4A5568").Padding(8).AlignRight().Text("VALOR").FontColor(Colors.White).Bold().FontSize(9);
                    });

                    foreach (var custo in custosExtras)
                    {
                        table.Cell().BorderBottom(1).BorderColor("#E8EDF2").Padding(8).Text(custo.nome).FontSize(9);
                        table.Cell().BorderBottom(1).BorderColor("#E8EDF2").Padding(8).Text(custo.categoria).FontSize(9);
                        table.Cell().BorderBottom(1).BorderColor("#E8EDF2").Padding(8).AlignRight().Text(custo.valor.ToString("C2", _ptBR)).FontSize(9).Bold();
                    }
                });
            }

            col.Item().PaddingTop(30).Row(row =>
            {
                row.RelativeItem();
                row.ConstantItem(300).Column(resumo =>
                {
                    resumo.Item().Text("RESUMO FINANCEIRO FINAL").FontSize(9).Bold().FontColor("#555555");
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
        }

        private void ConstruirCabecalhoRelatorio(ColumnDescriptor col, int totalRegistros)
        {
            col.Item().Background("#1E3A5F").Padding(14).Row(row =>
            {
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text("RELATÓRIO GERENCIAL DE PROJETOS").FontSize(18).Bold().FontColor(Colors.White);
                    c.Item().Text("AERO CONCEPTS — SISTEMA INTERNO DE HISTÓRICO").FontSize(10).FontColor("#A8C4E0");
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
                // ALTERADO: Distribuição das colunas ajustada para acomodar letras maiores sem quebras drásticas
                table.ColumnsDefinition(cols =>
                {
                    cols.ConstantColumn(45);   // ID
                    cols.RelativeColumn(3);    // Nome do Projeto
                    cols.RelativeColumn(2.5f); // Cliente
                    cols.RelativeColumn(1.3f); // Tipo
                    cols.RelativeColumn(1.5f); // Status
                    cols.ConstantColumn(85);   // Vencimento
                    cols.RelativeColumn(2.2f); // Valor Final
                });

                // ALTERADO: Fonte do cabeçalho subiu para 9.5 e padding aumentou para 8
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

                    // ALTERADO: Fonte das células aumentou para 10 e padding aumentou para 8 (espaçamento vertical maior para idosos)
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

            // ALTERADO: Aumentado o tamanho do bloco de totais e o texto interno para 10
            col.Item().PaddingTop(12).AlignRight().Width(280).Table(t =>
            {
                t.ColumnsDefinition(c => { c.RelativeColumn(1); c.RelativeColumn(1); });

                t.Cell().Background("#EDF2F7").Padding(6).Text("Lucro Estimado:").FontSize(10).FontColor("#2D3748").Bold();
                t.Cell().Background("#EDF2F7").Padding(6).AlignRight().Text(totalLucro.ToString("C2", _ptBR)).FontSize(10).FontColor("#2D3748").Bold();

                t.Cell().Background("#1E3A5F").Padding(6).Text("Faturamento Total:").FontSize(10).FontColor(Colors.White).Bold();
                t.Cell().Background("#1E3A5F").Padding(6).AlignRight().Text(totalFaturado.ToString("C2", _ptBR)).FontSize(10).FontColor(Colors.White).Bold();
            });
        }

        private void ConstruirRodape(RowDescriptor row)
        {
            row.RelativeItem().Text("Aero Concepts — Tecnologia em Engenharia Aeronáutica").FontSize(8).FontColor("#AAAAAA");
            row.ConstantItem(80).AlignRight().Text(x =>
            {
                x.Span("Página ").FontSize(8);
                x.CurrentPageNumber().FontSize(8);
                x.Span(" de ").FontSize(8);
                x.TotalPages().FontSize(8);
            });
        }

        #endregion
    }
}