using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Microsoft.Win32;
using magal.Models;
using magal.Data.Repositories;
using magal.Services;
using System.Windows.Input;

namespace magal.ViewModels
{
    public class OrcamentoViewModel : BaseModel
    {
        private Projeto _projetoAtual;
        private bool _processando = false;
        private bool _isUpdating = false; // TRAVA CRÍTICA: Impede o StackOverflow (loop infinito)

        public Projeto ProjetoAtual
        {
            get => _projetoAtual;
            set { _projetoAtual = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Cliente> Clientes { get; } = new ObservableCollection<Cliente>();
        public ObservableCollection<Funcionario> Funcionarios { get; } = new ObservableCollection<Funcionario>();
        public ObservableCollection<Custo> CustosExtras { get; } = new ObservableCollection<Custo>();

        public List<string> CategoriasCustos { get; } = new List<string>
        {
            "Equipamentos", "Licenças de Software", "Energia Elétrica",
            "Transporte/Deslocamento", "Manutenção", "Aluguel/Estrutura", "EPIs/Ferramentas"
        };

        public bool BotaoAtivo => !_processando;
        public RelayCommand AdicionarTarefaCommand { get; }
        public RelayCommand DeletarTarefaCommand { get; }
        public RelayCommand AdicionarCustoCommand { get; }
        public RelayCommand DeletarCustoCommand { get; }
        public RelayCommand GerarPdfCommand { get; }

        public OrcamentoViewModel()
        {
            AdicionarTarefaCommand = new RelayCommand(_ => AdicionarTarefa());
            DeletarTarefaCommand = new RelayCommand(param => DeletarTarefa(param as Tarefa));
            AdicionarCustoCommand = new RelayCommand(_ => AdicionarCustoExtra());
            DeletarCustoCommand = new RelayCommand(param => DeletarCustoExtra(param as Custo));
            GerarPdfCommand = new RelayCommand(_ => ExecutarFluxoFinal());

            NovoProjeto();
            CarregarDadosIniciais();
        }

        private void NovoProjeto()
        {
            ProjetoAtual = new Projeto
            {
                Orcamento = new Orcamento { margem_percentual = 20, percentual_impostos = 15 },
                Tarefas = new ObservableCollection<Tarefa>(),
                id_usuario = 1,
                nome = "Novo Orçamento Aero"
            };

            CustosExtras.Clear();

            // CORREÇÃO: Removido o PropertyChanged do Orcamento aqui, 
            // pois ele gerava loop infinito ao chamar o CalcularTotal.

            OnPropertyChanged(nameof(ProjetoAtual));
        }

        private void CarregarDadosIniciais()
        {
            try
            {
                var listaClientes = new ClienteRepository().ListarTodos();
                var listaFuncionarios = new FuncionarioRepository().ListarTodos();

                Clientes.Clear();

                foreach (var c in listaClientes) Clientes.Add(c);

                Funcionarios.Clear();
                foreach (var f in listaFuncionarios) Funcionarios.Add(f);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar dados: " + ex.Message);
            }
        }

        private void AdicionarTarefa()
        {
            var novaTarefa = new Tarefa { descricao = "Nova Atividade Técnica", horas_estimadas = 0 };

            novaTarefa.PropertyChanged += (s, e) => {
                // Escutamos apenas as entradas manuais. 
                // O custo_real é uma consequência, não precisa ser escutado aqui.
                if (e.PropertyName == nameof(Tarefa.Funcionario) ||
                    e.PropertyName == nameof(Tarefa.horas_estimadas))
                {
                    AtualizarFinanceiro();
                }
            };

            ProjetoAtual.Tarefas.Add(novaTarefa);
            AtualizarFinanceiro();
        }

        private void DeletarTarefa(Tarefa tarefa)
        {
            if (tarefa != null && ProjetoAtual.Tarefas.Contains(tarefa))
            {
                ProjetoAtual.Tarefas.Remove(tarefa);
                AtualizarFinanceiro();
            }
        }

        private void AdicionarCustoExtra()
        {
            var novoCusto = new Custo { nome = "Novo Item", valor = 0, categoria = "Equipamentos", tipo = "Direto" };

            novoCusto.PropertyChanged += (s, e) => {
                if (e.PropertyName == nameof(Custo.valor)) AtualizarFinanceiro();
            };

            CustosExtras.Add(novoCusto);
            AtualizarFinanceiro();
        }

        private void DeletarCustoExtra(Custo custo)
        {
            if (custo != null && CustosExtras.Contains(custo))
            {
                CustosExtras.Remove(custo);
                AtualizarFinanceiro();
            }
        }

        private void AtualizarFinanceiro()
        {
            // PROTEÇÃO: Se já estiver calculando ou se o projeto sumiu, aborta.
            if (_isUpdating || ProjetoAtual?.Orcamento == null) return;

            try
            {
                _isUpdating = true;

                // 1. Executa a lógica matemática no Model
                ProjetoAtual.Orcamento.CalcularTotal(
                    ProjetoAtual.Tarefas.ToList(),
                    CustosExtras.ToList()
                );

                // 2. Notifica a UI especificamente sobre o que mudou no objeto Orçamento.
                // Isso evita o OnPropertyChanged(nameof(ProjetoAtual)) que travava a tela.
                ProjetoAtual.Orcamento.OnPropertyChanged("valor_total");
                ProjetoAtual.Orcamento.OnPropertyChanged("valor_impostos");
                ProjetoAtual.Orcamento.OnPropertyChanged("lucro_estimado");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Erro no cálculo: " + ex.Message);
            }
            finally
            {
                _isUpdating = false; // Libera para a próxima atualização
            }
        }

        private void ExecutarFluxoFinal()
        {
            if (_processando) return;

            if (string.IsNullOrWhiteSpace(ProjetoAtual.nome) || (ProjetoAtual.id_cliente == 0 && ProjetoAtual.Cliente == null))
            {
                MessageBox.Show("Preencha o Nome do Projeto e selecione um Cliente.", "Aero Concepts");
                return;
            }

            try
            {
                _processando = true;
                OnPropertyChanged(nameof(BotaoAtivo));

                if (SalvarNoBancoSilencioso())
                {
                    if (GerarRelatorioPdf())
                    {
                        MessageBox.Show("Proposta finalizada e salva com sucesso!", "Sucesso");
                        NovoProjeto();
                    }
                }
            }
            finally
            {
                _processando = false;
                OnPropertyChanged(nameof(BotaoAtivo));
            }
        }

        private bool SalvarNoBancoSilencioso()
        {
            try
            {
                if (ProjetoAtual.Cliente != null)
                {
                    ProjetoAtual.id_cliente = ProjetoAtual.Cliente.id_cliente;
                }

                var repo = new ProjetoRepository();
                repo.SalvarProjetoCompleto(ProjetoAtual, CustosExtras.ToList());
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao salvar no banco: " + ex.Message, "Erro");
                return false;
            }
        }

        private bool GerarRelatorioPdf()
        {
            var sfd = new SaveFileDialog
            {
                Filter = "PDF|*.pdf",
                FileName = $"Proposta_{ProjetoAtual.nome}"
            };

            if (sfd.ShowDialog() == true)
            {
                new PdfService().GerarPropostaTecnica(ProjetoAtual, CustosExtras.ToList(), sfd.FileName);
                return true;
            }
            return false;
        }
    }
}