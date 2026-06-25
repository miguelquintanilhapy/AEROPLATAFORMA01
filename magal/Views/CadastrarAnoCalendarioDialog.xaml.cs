using System;
using System.Windows;
using magal.Models;
using magal.Data.Repositories;

namespace magal.Views
{
    public partial class CadastrarAnoCalendarioDialog : Window
    {
        public CadastrarAnoCalendarioDialog()
        {
            InitializeComponent();
        }

        private async void BtnSalvar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtAno.Text) ||
                string.IsNullOrWhiteSpace(TxtHorasDia.Text))
            {
                MessageBox.Show(
                    "Preencha todos os campos obrigatórios.",
                    "Aero Concepts",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(TxtAno.Text, out int ano) || ano < 2000 || ano > 2100)
            {
                MessageBox.Show(
                    "Ano inválido.",
                    "Aero Concepts",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(TxtHorasDia.Text,
         System.Globalization.NumberStyles.Any,
         System.Globalization.CultureInfo.CurrentCulture,
         out decimal horasDia) || horasDia <= 0)
            {
                MessageBox.Show(
                    "Horas por dia inválidas.",
                    "Aero Concepts",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            try
            {
                var anoCalendario = new AnoCalendario
                {
                    ano = ano,
                    horas_dia = horasDia,
                    inicio_ferias = DpInicioFerias.SelectedDate,
                    fim_ferias = DpFimFerias.SelectedDate
                };

                var repo = new AnoCalendarioRepository();
                await repo.Inserir(anoCalendario);

                MessageBox.Show(
                    "Ano de calendário cadastrado com sucesso!",
                    "Aero Concepts",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Erro ao salvar:\n\n" + ex.Message,
                    "Aero Concepts",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}