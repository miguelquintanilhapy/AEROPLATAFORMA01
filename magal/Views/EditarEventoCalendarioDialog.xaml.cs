using System;
using System.Windows;
using System.Windows.Controls;
using magal.Models;
using magal.Data.Repositories;

namespace magal.Views
{
    public partial class EditarEventoCalendarioDialog : Window
    {
        private readonly EventoCalendario _evento;

        public EditarEventoCalendarioDialog(EventoCalendario evento)
        {
            InitializeComponent();
            _evento = evento ?? throw new ArgumentNullException(nameof(evento));
            PreencherCampos();
        }

        private void PreencherCampos()
        {
            TxtDescricao.Text = _evento.descricao;
            DpData.SelectedDate = _evento.data_observada;

            if (_evento.IsSubstituido)
                DpDataOriginal.SelectedDate = _evento.data_original;

            foreach (ComboBoxItem item in ComboTipo.Items)
            {
                if (item.Content?.ToString() == _evento.tipo)
                {
                    ComboTipo.SelectedItem = item;
                    break;
                }
            }

            if (ComboTipo.SelectedItem == null)
                ComboTipo.SelectedIndex = 0;
        }

        private async void BtnSalvar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtDescricao.Text))
            {
                MessageBox.Show(
                    "Preencha a descrição do evento.",
                    "Aero Concepts",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            if (DpData.SelectedDate == null)
            {
                MessageBox.Show(
                    "Selecione a data do evento.",
                    "Aero Concepts",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            try
            {
                var dataObservada = DpData.SelectedDate.Value;
                var dataOriginal = DpDataOriginal.SelectedDate ?? dataObservada;

                _evento.descricao = TxtDescricao.Text.Trim();
                _evento.tipo = (ComboTipo.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Feriado Nacional";
                _evento.data_observada = dataObservada;
                _evento.data_original = dataOriginal;

                var repo = new EventoCalendarioRepository();
                await repo.Atualizar(_evento);

                MessageBox.Show(
                    "Evento atualizado com sucesso!",
                    "Aero Concepts",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Erro ao salvar evento:\n\n" + ex.Message,
                    "Aero Concepts",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}