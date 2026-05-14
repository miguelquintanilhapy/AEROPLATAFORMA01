using magal.Data.Repositories;
using magal.Models;
using System.Windows;

namespace magal.Views
{
    public partial class CadastrarUsuarioDialog : Window
    {
        public CadastrarUsuarioDialog()
        {
            InitializeComponent();
        }

        private void BtnSalvar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtNome.Text) || string.IsNullOrWhiteSpace(TxtEmail.Text) || string.IsNullOrWhiteSpace(TxtSenha.Password))
            {
                MessageBox.Show("Por favor, preencha todos os campos.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var novoUsuario = new Usuario
                {
                    nome = TxtNome.Text,
                    email = TxtEmail.Text,
                    senha = TxtSenha.Password,
                    status = "Ativo"
                };

                var repo = new UsuarioRepository();
                repo.Salvar(novoUsuario);

                MessageBox.Show("Usuário cadastrado com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);

                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao salvar no banco: " + ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}