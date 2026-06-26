using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using magal.Data.Repositories;
using magal.Models;
using magal.Services;
using Microsoft.Win32;

namespace magal.ViewModels
{
    // ---------------------------------------------------------------------------
    // Célula individual da grade visual
    // ---------------------------------------------------------------------------
    public class CelulaCalendario
    {
        public string Texto { get; set; } = "";
        private string _corFundo = "#FFFFFF";
        private string _corTexto = "#1E293B";

        public string CorFundo
        {
            get => _corFundo;
            set { _corFundo = value; CorFundoBrush = Brush(value); }
        }

        public string CorTexto
        {
            get => _corTexto;
            set { _corTexto = value; CorTextoBrush = Brush(value); }
        }

        public SolidColorBrush CorFundoBrush { get; private set; } = new SolidColorBrush(Colors.White);
        public SolidColorBrush CorTextoBrush { get; private set; } = new SolidColorBrush(Color.FromRgb(30, 41, 59));
        public FontWeight PesoFonte { get; set; } = FontWeights.Normal;
        public double TamanhoFonte { get; set; } = 12;
        public string? ToolTip { get; set; }
        public bool IsHeader { get; set; }

        private static SolidColorBrush Brush(string hex)
        {
            try { return new SolidColorBrush((Color)ColorConverter.ConvertFromString(hex)); }
            catch { return new SolidColorBrush(Colors.White); }
        }
    }

    // ---------------------------------------------------------------------------
    // Mês com células e notas de substituição
    // ---------------------------------------------------------------------------
    public class MesCalendario
    {
        public int Numero { get; set; }
        public string Nome { get; set; } = "";
        public int Ano { get; set; }
        public List<CelulaCalendario> Celulas { get; set; } = new();
        public string NotasSubstituicao { get; set; } = "";
        public bool TemNota => !string.IsNullOrEmpty(NotasSubstituicao);
    }

    // ---------------------------------------------------------------------------
    // Linha de resumo mensal
    // ---------------------------------------------------------------------------
    public class ResumoMes
    {
        public string NomeMes { get; set; } = "";
        public int TotalDiasUteis { get; set; }
        public int FeriadosEmDU { get; set; }
        public int Pontes { get; set; }

        public int DiasUteisSemPontes => TotalDiasUteis - FeriadosEmDU;
        public int DiasUteisComPontes => DiasUteisSemPontes - Pontes;
        public bool TemFeriado => FeriadosEmDU > 0;
    }

    // ---------------------------------------------------------------------------
    // ViewModel principal do Calendário Corporativo
    // ---------------------------------------------------------------------------
    public class CalendarioViewModel : BaseModel
    {
        #region Campos

        private readonly AnoCalendarioRepository _anoRepo;
        private readonly EventoCalendarioRepository _eventoRepo;
        private readonly PdfService _pdfService;

        private AnoCalendario? _anoSelecionado;
        private EventoCalendario? _eventoSelecionado;
        private bool _isLoading;

        private static readonly string[] NomesMeses =
        {
            "Janeiro", "Fevereiro", "Março", "Abril", "Maio", "Junho",
            "Julho", "Agosto", "Setembro", "Outubro", "Novembro", "Dezembro"
        };

        #endregion

        #region Propriedades

        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; OnPropertyChanged(); }
        }

        public AnoCalendario? AnoSelecionado
        {
            get => _anoSelecionado;
            set
            {
                _anoSelecionado = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TemAnoSelecionado));
                OnPropertyChanged(nameof(FeriasColetivasFormatado));
                if (value != null)
                    _ = CarregarDadosDoAno(value);
            }
        }

        public EventoCalendario? EventoSelecionado
        {
            get => _eventoSelecionado;
            set { _eventoSelecionado = value; OnPropertyChanged(); }
        }

        public bool TemAnoSelecionado => AnoSelecionado != null;

        public string FeriasColetivasFormatado
        {
            get
            {
                if (AnoSelecionado?.inicio_ferias == null || AnoSelecionado?.fim_ferias == null)
                    return "—";
                return $"{AnoSelecionado.inicio_ferias:dd/MM} a {AnoSelecionado.fim_ferias:dd/MM}";
            }
        }

        public string TotalHorasAnoFormatado
        {
            get
            {
                if (AnoSelecionado == null || !ResumoMensal.Any()) return "—";
                int totalDU = ResumoMensal.Sum(r => r.DiasUteisSemPontes);
                decimal total = totalDU * AnoSelecionado.horas_dia;
                return total.ToString("N1", new CultureInfo("pt-BR")) + " h";
            }
        }

        public string TotalDUSemPontesFormatado
        {
            get
            {
                if (!ResumoMensal.Any()) return "—";
                return ResumoMensal.Sum(r => r.DiasUteisSemPontes) + " dias";
            }
        }

        public string TotalDUComPontesFormatado
        {
            get
            {
                if (!ResumoMensal.Any()) return "—";
                return ResumoMensal.Sum(r => r.DiasUteisComPontes) + " dias";
            }
        }

        public string TotalPontesFormatado
        {
            get
            {
                if (!ResumoMensal.Any()) return "—";
                return ResumoMensal.Sum(r => r.Pontes).ToString();
            }
        }

        public IEnumerable<EventoCalendario> EventosLegenda =>
    EventosDoAno.Where(e => e.tipo != "Ponte").OrderBy(e => e.data_observada);

        public int TotalDiasUteis => ResumoMensal.Sum(r => r.TotalDiasUteis);
        public int TotalFeriadosEmDU => ResumoMensal.Sum(r => r.FeriadosEmDU);
        public int TotalPontes => ResumoMensal.Sum(r => r.Pontes);
        public int TotalDUSemPontes => ResumoMensal.Sum(r => r.DiasUteisSemPontes);
        public int TotalDUComPontes => ResumoMensal.Sum(r => r.DiasUteisComPontes);
        public decimal HorasDia => AnoSelecionado?.horas_dia ?? 0;
        public decimal TotalHorasAno => TotalDUSemPontes * HorasDia;

        #endregion

        #region Coleções

        public ObservableCollection<AnoCalendario> AnosDisponiveis { get; } = new();
        public ObservableCollection<MesCalendario> MesesCalendario { get; } = new();
        public ObservableCollection<ResumoMes> ResumoMensal { get; } = new();
        public ObservableCollection<EventoCalendario> EventosDoAno { get; } = new();

        #endregion

        #region Comandos

        public RelayCommand CarregarCommand { get; }
        public RelayCommand CriarAnoCommand { get; }
        public RelayCommand EditarAnoCommand { get; }
        public RelayCommand ExcluirAnoCommand { get; }
        public RelayCommand CriarEventoCommand { get; }
        public RelayCommand EditarEventoCommand { get; }
        public RelayCommand ExcluirEventoCommand { get; }
        public RelayCommand ExportarPdfCommand { get; }

        #endregion

        #region Construtor

        public CalendarioViewModel()
        {
            _anoRepo = new AnoCalendarioRepository();
            _eventoRepo = new EventoCalendarioRepository();
            _pdfService = new PdfService();

            CarregarCommand = new RelayCommand(_ => Carregar());
            CriarAnoCommand = new RelayCommand(_ => ExecutarCriarAno());
            EditarAnoCommand = new RelayCommand(_ => ExecutarEditarAno(), _ => TemAnoSelecionado);
            ExcluirAnoCommand = new RelayCommand(_ => ExecutarExcluirAno(), _ => TemAnoSelecionado);
            CriarEventoCommand = new RelayCommand(_ => ExecutarCriarEvento(), _ => TemAnoSelecionado);
            EditarEventoCommand = new RelayCommand(p => ExecutarEditarEvento(p as EventoCalendario));
            ExcluirEventoCommand = new RelayCommand(p => ExecutarExcluirEvento(p as EventoCalendario));
            ExportarPdfCommand = new RelayCommand(_ => ExecutarExportarPdf(),
                _ => TemAnoSelecionado && MesesCalendario.Count > 0);
        }

        #endregion

        #region Carregamento

        public async void Carregar()
        {
            try
            {
                IsLoading = true;
                var anos = await _anoRepo.ListarTodos();

                AnosDisponiveis.Clear();
                foreach (var a in anos)
                    AnosDisponiveis.Add(a);

                AnoSelecionado = AnosDisponiveis.Count > 0 ? AnosDisponiveis[0] : null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar calendários: {ex.Message}",
                    "Aero Concepts", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async System.Threading.Tasks.Task CarregarDadosDoAno(AnoCalendario ano)
        {
            try
            {
                IsLoading = true;
                var eventos = await _eventoRepo.ListarPorAno(ano.id_ano_calendario);

                EventosDoAno.Clear();
                foreach (var e in eventos)
                    EventosDoAno.Add(e);

                ComputarCalendario(ano, eventos);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar dados do ano: {ex.Message}",
                    "Aero Concepts", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        #endregion

        #region Cálculo da Grade e Resumo

        private void ComputarCalendario(AnoCalendario ano, List<EventoCalendario> eventos)
        {
            MesesCalendario.Clear();
            ResumoMensal.Clear();

            var eventosPorData = eventos.ToLookup(e => e.data_observada.Date);

            for (int mes = 1; mes <= 12; mes++)
            {
                MesesCalendario.Add(CriarMesCalendario(ano, mes, eventos, eventosPorData));
                ResumoMensal.Add(ComputarResumoMes(ano, mes, eventos));
            }

            ResumoMensal.Add(new ResumoMes
            {
                NomeMes = "TOTAL",
                TotalDiasUteis = ResumoMensal.Sum(r => r.TotalDiasUteis),
                FeriadosEmDU = ResumoMensal.Sum(r => r.FeriadosEmDU),
                Pontes = ResumoMensal.Sum(r => r.Pontes)
            });

            AtualizarTotais();
        }

        private void AtualizarTotais()
        {
            OnPropertyChanged(nameof(TotalHorasAnoFormatado));
            OnPropertyChanged(nameof(TotalDUSemPontesFormatado));
            OnPropertyChanged(nameof(TotalDUComPontesFormatado));
            OnPropertyChanged(nameof(TotalPontesFormatado));
            OnPropertyChanged(nameof(EventosLegenda));
            OnPropertyChanged(nameof(TotalDiasUteis));
            OnPropertyChanged(nameof(TotalFeriadosEmDU));
            OnPropertyChanged(nameof(TotalPontes));
            OnPropertyChanged(nameof(TotalDUSemPontes));
            OnPropertyChanged(nameof(TotalDUComPontes));
            OnPropertyChanged(nameof(HorasDia));
            OnPropertyChanged(nameof(TotalHorasAno));
        }

        private MesCalendario CriarMesCalendario(AnoCalendario ano, int mes,
            List<EventoCalendario> eventos,
            ILookup<DateTime, EventoCalendario> eventosPorData)
        {
            var m = new MesCalendario { Numero = mes, Nome = NomesMeses[mes - 1], Ano = ano.ano };

            // Cabeçalho: SMA + dias da semana
            m.Celulas.Add(Cell("SMA", "#1F3864", "#FFFFFF", FontWeights.Bold, isHeader: true));
            foreach (var label in new[] { "Seg", "Ter", "Qua", "Qui", "Sex", "Sáb", "Dom" })
                m.Celulas.Add(Cell(label, "#1F3864", "#FFFFFF", FontWeights.SemiBold, isHeader: true));

            DateTime primeiroDia = new DateTime(ano.ano, mes, 1);
            DateTime ultimoDia = new DateTime(ano.ano, mes, DateTime.DaysInMonth(ano.ano, mes));

            int dow = (int)primeiroDia.DayOfWeek;
            int offset = dow == 0 ? 6 : dow - 1;
            DateTime inicioSemana = primeiroDia.AddDays(-offset);

            var notas = new List<string>();

            while (inicioSemana <= ultimoDia)
            {
                int semana = ISOWeek.GetWeekOfYear(inicioSemana);
                m.Celulas.Add(Cell(semana.ToString("D2"), "#E2E8F0", "#64748B", FontWeights.SemiBold));

                for (int d = 0; d < 7; d++)
                {
                    DateTime dia = inicioSemana.AddDays(d);
                    if (dia.Month == mes)
                    {
                        var (fundo, texto, tip) = ObterCorDia(dia, ano, eventosPorData);
                        m.Celulas.Add(Cell(dia.Day.ToString(), fundo, texto, FontWeights.Bold, tip, tamanhoFonte: 14));
                        var evSub = eventos.FirstOrDefault(e =>
                            e.data_observada.Date == dia.Date && e.IsSubstituido);
                        if (evSub != null)
                            notas.Add($"* {evSub.data_original:d/M} Substituído por {evSub.data_observada:d/M}");
                    }
                    else
                    {
                        m.Celulas.Add(Cell(dia.Day.ToString(), "#F1F5F9", "#CBD5E1", FontWeights.Normal));
                    }
                }

                inicioSemana = inicioSemana.AddDays(7);
            }

            m.NotasSubstituicao = string.Join("   ", notas.Distinct());
            return m;
        }

        private ResumoMes ComputarResumoMes(AnoCalendario ano, int mes, List<EventoCalendario> eventos)
        {
            int totalDU = 0, feriadosEmDU = 0, pontes = 0;
            int diasNoMes = DateTime.DaysInMonth(ano.ano, mes);

            for (int d = 1; d <= diasNoMes; d++)
            {
                DateTime dia = new DateTime(ano.ano, mes, d);
                if (dia.DayOfWeek == DayOfWeek.Saturday || dia.DayOfWeek == DayOfWeek.Sunday)
                    continue;

                totalDU++;

                if (ano.inicio_ferias.HasValue && ano.fim_ferias.HasValue &&
                    dia.Date >= ano.inicio_ferias.Value.Date &&
                    dia.Date <= ano.fim_ferias.Value.Date)
                    continue;

                var evento = eventos.FirstOrDefault(e => e.data_observada.Date == dia.Date);
                if (evento == null) continue;

                switch (evento.tipo)
                {
                    case "Feriado Nacional":
                    case "Feriado Estadual":
                    case "Feriado Municipal":
                    case "Ponto Facultativo":
                        feriadosEmDU++;
                        break;
                    case "Ponte":
                        pontes++;
                        break;
                }
            }

            return new ResumoMes
            {
                NomeMes = NomesMeses[mes - 1],
                TotalDiasUteis = totalDU,
                FeriadosEmDU = feriadosEmDU,
                Pontes = pontes
            };
        }

        private (string fundo, string texto, string? tip) ObterCorDia(
            DateTime dia, AnoCalendario ano,
            ILookup<DateTime, EventoCalendario> eventosPorData)
        {
            bool isSabado = dia.DayOfWeek == DayOfWeek.Saturday;
            bool isDomingo = dia.DayOfWeek == DayOfWeek.Sunday;

            if (ano.inicio_ferias.HasValue && ano.fim_ferias.HasValue &&
                dia.Date >= ano.inicio_ferias.Value.Date &&
                dia.Date <= ano.fim_ferias.Value.Date)
                return ("#00B0F0", "#FFFFFF", "Férias Coletivas");

            var ev = eventosPorData[dia.Date].FirstOrDefault();
            if (ev != null)
            {
                var (f, t) = CorTipo(ev.tipo);
                return (f, t, ev.DescricaoCompleta);
            }

            if (isDomingo) return ("#2D9B6C", "#000000", null);
            if (isSabado) return ("#00FF00", "#000000", null);
            return ("#FFFFFF", "#1E293B", null);
        }

        private static (string fundo, string texto) CorTipo(string tipo) => tipo switch
        {
            "Feriado Nacional" => ("#4472C4", "#FFFFFF"),
            "Feriado Estadual" => ("#ED7D31", "#FFFFFF"),
            "Feriado Municipal" => ("#ED7D31", "#FFFFFF"),
            "Ponto Facultativo" => ("#7030A0", "#FFFFFF"),
            "Ponte" => ("#BDD7EE", "#1E293B"),
            "Férias Coletivas" => ("#00B0F0", "#FFFFFF"),
            _ => ("#FFFFFF", "#1E293B")
        };

        private static CelulaCalendario Cell(string texto, string fundo, string texto2,
    FontWeight peso, string? tip = null, bool isHeader = false, double tamanhoFonte = 12)
        {
            return new CelulaCalendario
            {
                Texto = texto,
                CorFundo = fundo,
                CorTexto = texto2,
                PesoFonte = peso,
                ToolTip = tip,
                IsHeader = isHeader,
                TamanhoFonte = tamanhoFonte
            };
        }

        #endregion

        #region CRUD Ano

        private void ExecutarCriarAno()
        {
            var dialog = new magal.Views.CadastrarAnoCalendarioDialog();
            dialog.Owner = Application.Current.Windows.OfType<magal.MainWindow>().FirstOrDefault();
            if (dialog.ShowDialog() == true) Carregar();
        }

        private void ExecutarEditarAno()
        {
            if (AnoSelecionado == null) return;
            var dialog = new magal.Views.EditarAnoCalendarioDialog(AnoSelecionado);
            dialog.Owner = Application.Current.Windows.OfType<magal.MainWindow>().FirstOrDefault();
            if (dialog.ShowDialog() == true) Carregar();
        }

        private async void ExecutarExcluirAno()
        {
            if (AnoSelecionado == null) return;
            var msg = $"Excluir o calendário de {AnoSelecionado.ano}?\n" +
                      "Todos os eventos vinculados serão removidos permanentemente.";
            if (MessageBox.Show(msg, "Confirmar Exclusão",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                return;
            try { await _anoRepo.Excluir(AnoSelecionado.id_ano_calendario); Carregar(); }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao excluir: {ex.Message}", "Aero Concepts",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region CRUD Evento

        private void ExecutarCriarEvento()
        {
            if (AnoSelecionado == null) return;
            var dialog = new magal.Views.CadastrarEventoCalendarioDialog(AnoSelecionado.id_ano_calendario);
            dialog.Owner = Application.Current.Windows.OfType<magal.MainWindow>().FirstOrDefault();
            if (dialog.ShowDialog() == true && _anoSelecionado != null)
                _ = CarregarDadosDoAno(_anoSelecionado);
        }

        private void ExecutarEditarEvento(EventoCalendario? ev)
        {
            if (ev == null) return;
            var dialog = new magal.Views.EditarEventoCalendarioDialog(ev);
            dialog.Owner = Application.Current.Windows.OfType<magal.MainWindow>().FirstOrDefault();
            if (dialog.ShowDialog() == true && _anoSelecionado != null)
                _ = CarregarDadosDoAno(_anoSelecionado);
        }

        private async void ExecutarExcluirEvento(EventoCalendario? ev)
        {
            if (ev == null) return;
            var msg = $"Excluir o evento '{ev.descricao}' ({ev.data_observada:dd/MM/yyyy})?";
            if (MessageBox.Show(msg, "Confirmar Exclusão",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                return;
            try
            {
                await _eventoRepo.Excluir(ev.id_evento);
                if (_anoSelecionado != null)
                    await CarregarDadosDoAno(_anoSelecionado);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao excluir evento: {ex.Message}", "Aero Concepts",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region PDF

        private void ExecutarExportarPdf()
        {
            if (AnoSelecionado == null) return;
            string pasta = System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");

            var dlg = new SaveFileDialog
            {
                Filter = "Arquivos PDF (*.pdf)|*.pdf",
                FileName = $"Calendario_{AnoSelecionado.ano}.pdf",
                InitialDirectory = System.IO.Directory.Exists(pasta) ? pasta : string.Empty,
                Title = "Exportar Calendário"
            };

            if (dlg.ShowDialog() != true) return;

            try
            {
                _pdfService.GerarCalendarioCorporativo(
                AnoSelecionado, EventosDoAno.ToList(), dlg.FileName);
                MessageBox.Show("Calendário exportado com sucesso!", "Aero Concepts",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
    MessageBox.Show($"Erro ao exportar PDF: {ex.Message}", "Aero Concepts",
        MessageBoxButton.OK, MessageBoxImage.Error);
}
        }

        #endregion
    }
}