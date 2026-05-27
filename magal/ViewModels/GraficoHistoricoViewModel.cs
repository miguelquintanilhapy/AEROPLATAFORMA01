using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Collections.Generic;
using magal.Models;
using magal.Data.Repositories;
using LiveCharts;
using LiveCharts.Wpf;

namespace magal.ViewModels
{
    /// <summary>
    /// ViewModel responsável por gerenciar a exibição de gráficos analíticos do histórico de projetos.
    /// </summary>
    public class GraficoHistoricoViewModel : BaseModel
    {
        #region Campos Privados

        private readonly ProjetoRepository _repository;

        private string _totalFinanceiro;
        private string _totalLucro;
        private int _quantidadeProjetos;

        #endregion

        #region Cultura

        private static readonly CultureInfo _ptBR = new("pt-BR");

        #endregion

        #region Indicadores

        public string TotalFinanceiro
        {
            get => _totalFinanceiro;
            set
            {
                _totalFinanceiro = value;
                OnPropertyChanged();
            }
        }

        public string TotalLucro
        {
            get => _totalLucro;
            set
            {
                _totalLucro = value;
                OnPropertyChanged();
            }
        }

        public int QuantidadeProjetos
        {
            get => _quantidadeProjetos;
            set
            {
                _quantidadeProjetos = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Gráficos

        public SeriesCollection SeriesStatus { get; set; }

        public SeriesCollection SeriesTipo { get; set; }

        #endregion

        #region Comandos

        public RelayCommand AtualizarCommand { get; }

        #endregion

        #region Construtor

        public GraficoHistoricoViewModel()
        {
            _repository = new ProjetoRepository();

            AtualizarCommand = new RelayCommand(_ => CarregarDados());

            CarregarDados();
        }

        #endregion

        #region Métodos

        public void CarregarDados()
        {
            try
            {
                var projetos = _repository.BuscarTodosPorUsuario(1);

                foreach (var projeto in projetos)
                {
                    if (projeto.Orcamento == null)
                    {
                        projeto.Orcamento = new Orcamento();
                    }
                }

                AtualizarIndicadores(projetos);
                AtualizarGraficoStatus(projetos);
                AtualizarGraficoTipo(projetos);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erro ao carregar gráficos: {ex.Message}",
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        private void AtualizarIndicadores(List<Projeto> projetos)
        {
            decimal totalFaturamento = projetos
                .Where(p => p.Orcamento != null)
                .Sum(p => p.Orcamento.valor_final);

            decimal totalLucro = projetos
                .Where(p => p.Orcamento != null)
                .Sum(p => p.Orcamento.valor_margem);

            QuantidadeProjetos = projetos.Count;

            TotalFinanceiro = totalFaturamento.ToString("C2", _ptBR);

            TotalLucro = totalLucro.ToString("C2", _ptBR);
        }

        private void AtualizarGraficoStatus(List<Projeto> projetos)
        {
            var agrupado = projetos
                .GroupBy(p => p.status ?? "Sem Status")
                .Select(g => new
                {
                    Nome = g.Key,
                    Quantidade = g.Count()
                });

            SeriesStatus = new SeriesCollection();

            foreach (var item in agrupado)
            {
                SeriesStatus.Add(new PieSeries
                {
                    Title = item.Nome,
                    Values = new ChartValues<int> { item.Quantidade },
                    DataLabels = true
                });
            }

            OnPropertyChanged(nameof(SeriesStatus));
        }

        private void AtualizarGraficoTipo(List<Projeto> projetos)
        {
            var agrupado = projetos
                .GroupBy(p => p.tipo ?? "Sem Tipo")
                .Select(g => new
                {
                    Nome = g.Key,
                    Quantidade = g.Count()
                });

            SeriesTipo = new SeriesCollection();

            foreach (var item in agrupado)
            {
                SeriesTipo.Add(new PieSeries
                {
                    Title = item.Nome,
                    Values = new ChartValues<int> { item.Quantidade },
                    DataLabels = true
                });
            }

            OnPropertyChanged(nameof(SeriesTipo));
        }

        #endregion
    }
}