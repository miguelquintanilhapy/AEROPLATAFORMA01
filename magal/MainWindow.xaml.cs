using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using magal.Data.Repositories;
using magal.Models;
using magal.ViewModels;
using magal.Views;

namespace magal
{
    public partial class MainWindow : Window
    {
        private HomeView _homeView;
        private HistoricoView _historicoView;
        private OrcamentoView _orcamentoView;

        public MainWindow()
        {
            InitializeComponent();
            this.WindowState = WindowState.Maximized;

            // 1. Carrega os dados do usuário assim que a janela é instanciada
            CarregarDadosUsuario();

            // 2. Inicia na Home
            AbrirHome();
        }

        private void CarregarDadosUsuario()
        {
            // Verificamos se a sessão global existe
            if (Sessao.UsuarioLogado != null)
            {
                // ATENÇÃO: Verifique se as propriedades no seu Model são 'nome' ou 'Nome' (C# costuma usar Maiúsculo)
                string nome = Sessao.UsuarioLogado.nome;
                string email = Sessao.UsuarioLogado.email;

                TxtNomeUsuarioSidebar.Text = !string.IsNullOrWhiteSpace(nome) ? nome : "Usuário sem Nome";
                TxtEmailUsuarioSidebar.Text = !string.IsNullOrWhiteSpace(email) ? email : "Sem e-mail cadastrado";

                // Atualiza a inicial do Avatar
                if (!string.IsNullOrWhiteSpace(nome))
                {
                    TxtIniciaisAvatar.Text = nome.Trim().Substring(0, 1).ToUpper();
                }
                else
                {
                    TxtIniciaisAvatar.Text = "U";
                }
            }
            else
            {
                // Fallback caso a sessão falhe por algum motivo
                TxtNomeUsuarioSidebar.Text = "Acesso Convidado";
                TxtEmailUsuarioSidebar.Text = "Sessão não identificada";
                TxtIniciaisAvatar.Text = "?";
            }
        }

        // --- NAVEGAÇÃO ---
        private void BtnHome_Click(object sender, RoutedEventArgs e) => AbrirHome();
        private void BtnOrcamentos_Click(object sender, RoutedEventArgs e) => AbrirOrcamento();
        private void BtnHistorico_Click(object sender, RoutedEventArgs e) => AbrirHistorico();
        private void BtnGerenciamento_Click(object sender, RoutedEventArgs e) => AbrirGerenciamento();

        public void AbrirHome()
        {
            if (_homeView == null) _homeView = new HomeView();
            MainContent.Content = _homeView;
        }

        public void AbrirOrcamento()
        {
            _orcamentoView = new OrcamentoView();
            MainContent.Content = _orcamentoView;
        }

        public void AbrirHistorico()
        {
            if (_historicoView == null) _historicoView = new HistoricoView();
            MainContent.Content = _historicoView;
        }

        public void AbrirGerenciamento()
        {
            MainContent.Content = new GerenciamentoView();
        }

        public void IrParaEdicao(Projeto projetoSimplificado)
        {
            var repo = new ProjetoRepository();
            Projeto projetoCompleto = repo.CarregarProjetoCompleto(projetoSimplificado.id_projeto);

            var viewModel = new OrcamentoViewModel();
            viewModel.CarregarProjetoParaEdicao(projetoCompleto);

            var view = new OrcamentoView();
            view.DataContext = viewModel;
            MainContent.Content = view;
        }

        // --- PERFIL E USUÁRIO ---
        private void BtnPerfil_Click(object sender, RoutedEventArgs e)
        {
            // Abre o ContextMenu (Adicionar Usuário) ao clicar no perfil
            if (sender is Button btn && btn.ContextMenu != null)
            {
                btn.ContextMenu.PlacementTarget = btn;
                btn.ContextMenu.IsOpen = true;
            }
        }

        private void MenuCadastrarUsuario_Click(object sender, RoutedEventArgs e)
        {
            CadastrarUsuarioDialog dialog = new CadastrarUsuarioDialog();
            dialog.Owner = this;
            dialog.ShowDialog();
        }

        private void MenuLogoff_Click(object sender, RoutedEventArgs e)
        {
            var resultado = MessageBox.Show("Deseja realmente sair e voltar para o login?", "Logoff",
                                            MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (resultado == MessageBoxResult.Yes)
            {
                Sessao.UsuarioLogado = null; // Limpa a sessão
                LoginView login = new LoginView();
                login.Show();
                this.Close(); // Fecha a MainWindow
            }
        }

        public ContentControl MainContentControl => MainContent;
    }
}