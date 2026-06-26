using System;
using System.Windows;
using System.Windows.Controls;
using magal.Models;
using magal.Data.Repositories;

namespace magal.Views
{
    public partial class CadastrarEventoCalendarioDialog : Window
    {
        private readonly int _idAnoCalendario;

        public CadastrarEventoCalendarioDialog(int idAnoCalendario)
        {
            InitializeComponent();
            _idAnoCalendario = idAnoCalendario;
        }

        private async void BtnSalvar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtDescricao.Text))
            {
                MessageBox.Show("Preencha a descrição do evento.", "Aero Concepts",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (DpData.SelectedDate == null)
            {
                MessageBox.Show("Selecione a data do evento.", "Aero Concepts",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var dataObservada = DpData.SelectedDate.Value;
                var dataOriginal = DpDataOriginal.SelectedDate ?? dataObservada;

                var evento = new EventoCalendario
                {
                    id_ano_calendario = _idAnoCalendario,
                    descricao = TxtDescricao.Text.Trim(),
                    tipo = (ComboTipo.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Feriado Nacional",
                    data_observada = dataObservada,
                    data_original = dataOriginal
                };

                var repo = new EventoCalendarioRepository();
                await repo.Inserir(evento);

                MessageBox.Show("Evento cadastrado com sucesso!", "Aero Concepts",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao salvar evento:\n\n" + ex.Message, "Aero Concepts",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}