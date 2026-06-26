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
            _evento = evento;

            TxtDescricao.Text = evento.descricao;
            DpData.SelectedDate = evento.data_observada;
            DpDataOriginal.SelectedDate = evento.data_original != evento.data_observada
                                          ? evento.data_original
                                          : (DateTime?)null;

            foreach (ComboBoxItem item in ComboTipo.Items)
            {
                if (item.Content.ToString() == evento.tipo)
                {
                    ComboTipo.SelectedItem = item;
                    break;
                }
            }

            if (ComboTipo.SelectedIndex < 0)
                ComboTipo.SelectedIndex = 0;
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
                    id_evento = _evento.id_evento,
                    id_ano_calendario = _evento.id_ano_calendario,
                    descricao = TxtDescricao.Text.Trim(),
                    tipo = (ComboTipo.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Feriado Nacional",
                    data_observada = dataObservada,
                    data_original = dataOriginal
                };

                var repo = new EventoCalendarioRepository();
                await repo.Atualizar(evento);

                MessageBox.Show("Evento atualizado com sucesso!", "Aero Concepts",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao atualizar evento do calendário: " + ex.Message, "Aero Concepts",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}