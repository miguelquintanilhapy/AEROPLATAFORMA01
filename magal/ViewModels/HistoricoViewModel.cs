using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.ComponentModel;
using System.Windows.Data;
using magal.Models;
using magal.Data.Repositories;

namespace magal.ViewModels
{
    public class HistoricoViewModel : BaseModel
    {
        private readonly ProjetoRepository _repository;
        private Projeto _projetoSelecionado;
        private string _filtroTexto;
        private string _totalFinanceiro;
        private int _quantidadeProjetos;

        public string TotalFinanceiro
        {
            get => _totalFinanceiro;
            set { _totalFinanceiro = value; OnPropertyChanged(); }
        }

        public int QuantidadeProjetos
        {
            get => _quantidadeProjetos;
            set { _quantidadeProjetos = value; OnPropertyChanged(); }
        }

        public string FiltroTexto
        {
            get => _filtroTexto;
            set
            {
                _filtroTexto = value;
                OnPropertyChanged();
                ProjetosView?.Refresh();
            }
        }

        private string _totalLucro;
        public string TotalLucro
        {
            get => _totalLucro;
            set { _totalLucro = value; OnPropertyChanged(); }
        }

        public Projeto ProjetoSelecionado
        {
            get => _projetoSelecionado;
            set { _projetoSelecionado = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Projeto> Projetos { get; } = new ObservableCollection<Projeto>();
        public ICollectionView ProjetosView { get; private set; }

        public RelayCommand ExcluirCommand { get; }
        public RelayCommand EditarCommand { get; }
        public RelayCommand AtualizarCommand { get; }

        public HistoricoViewModel()
        {
            _repository = new ProjetoRepository();

            ProjetosView = CollectionViewSource.GetDefaultView(Projetos);
            ProjetosView.Filter = FiltroDeProjetos;

            ExcluirCommand = new RelayCommand(p => ExecutarExclusao(p as Projeto));
            EditarCommand = new RelayCommand(p => ExecutarEdicao(p as Projeto));
            AtualizarCommand = new RelayCommand(_ => CarregarHistorico());

            CarregarHistorico();
        }

        private bool FiltroDeProjetos(object obj)
        {
            if (string.IsNullOrWhiteSpace(FiltroTexto)) return true;
            var projeto = obj as Projeto;
            if (projeto == null) return false;

            var busca = FiltroTexto.ToLower().Trim();

            // Adicionamos a verificação do 'tipo' aqui:
            return (projeto.nome?.ToLower().Contains(busca) ?? false) ||
                   (projeto.status?.ToLower().Contains(busca) ?? false) ||
                   (projeto.tipo?.ToLower().Contains(busca) ?? false);
        }

        private void CarregarHistorico()
        {
            try
            {
                // Busca os dados do banco
                var lista = _repository.BuscarTodosPorUsuario(1);

                Projetos.Clear();

                foreach (var p in lista)
                {
                    if (p.Orcamento != null)
                    {
                        // IMPORTANTE: Como você mudou o Model, aqui forçamos a interface 
                        // a ler a propriedade 'valor_final' calculada. 
                        // Se você criou o método AtualizarNotificacoes() no Model, use-o aqui:
                        // p.Orcamento.AtualizarNotificacoes();
                    }
                    Projetos.Add(p);
                }

                // Atualiza os indicadores de tela (Soma e Quantidade)
                AtualizarIndicadores();

                // Força o Refresh da lista visual
                ProjetosView?.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar histórico: {ex.Message}", "Aero Concepts", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AtualizarIndicadores()
        {
            var listaFiltrada = ProjetosView.Cast<Projeto>().ToList();
            QuantidadeProjetos = listaFiltrada.Count;

            // Soma do Valor Final (Faturamento)
            decimal somaFaturamento = listaFiltrada
                .Where(p => p.Orcamento != null)
                .Sum(p => p.Orcamento.valor_final);

            // Soma do Lucro (valor_margem que criamos na Model)
            decimal somaLucro = listaFiltrada
                .Where(p => p.Orcamento != null)
                .Sum(p => p.Orcamento.valor_margem);

            TotalFinanceiro = somaFaturamento.ToString("C2");
            TotalLucro = somaLucro.ToString("C2");
        }

        private void ExecutarExclusao(Projeto projeto)
        {
            if (projeto == null) return;

            var msg = $"Tem certeza que deseja excluir o projeto '{projeto.nome}'?";
            if (MessageBox.Show(msg, "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    _repository.ExcluirProjeto(projeto.id_projeto);
                    Projetos.Remove(projeto);
                    AtualizarIndicadores();
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            }
        }

        private void ExecutarEdicao(Projeto projeto)
        {
            if (projeto == null) return;
            var mainWindow = Application.Current.Windows.OfType<magal.MainWindow>().FirstOrDefault();
            if (mainWindow != null) mainWindow.IrParaEdicao(projeto);
        }
    }
}