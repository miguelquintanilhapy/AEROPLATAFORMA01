using System;
using System.Windows;
using magal.Models;
using magal.Data.Repositories;

namespace magal.Views
{
    public partial class EditarAnoCalendarioDialog : Window
    {
        private readonly AnoCalendario _ano;

        public EditarAnoCalendarioDialog(AnoCalendario ano)
        {
            InitializeComponent();
            _ano = ano ?? throw new ArgumentNullException(nameof(ano));
            PreencherCampos();
        }

        private void PreencherCampos()
        {
            TxtAno.Text = _ano.ano.ToString();
            TxtHorasDia.Text = _ano.horas_dia.ToString("G");

            if (_ano.inicio_ferias.HasValue)
                DpInicioFerias.SelectedDate = _ano.inicio_ferias.Value;

            if (_ano.fim_ferias.HasValue)
                DpFimFerias.SelectedDate = _ano.fim_ferias.Value;
        }

        private async void BtnSalvar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtHorasDia.Text))
            {
                MessageBox.Show(
                    "Preencha todos os campos obrigatórios.",
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
                _ano.horas_dia = horasDia;
                _ano.inicio_ferias = DpInicioFerias.SelectedDate;
                _ano.fim_ferias = DpFimFerias.SelectedDate;

                var repo = new AnoCalendarioRepository();
                await repo.Atualizar(_ano);

                MessageBox.Show(
                    "Ano de calendário atualizado com sucesso!",
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