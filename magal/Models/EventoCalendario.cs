using System;

namespace magal.Models
{
    public class EventoCalendario : BaseModel
    {
        private int _idEvento;
        private int _idAnoCalendario;
        private DateTime _dataOriginal;
        private DateTime _dataObservada;
        private string _descricao = string.Empty;
        private string _tipo = string.Empty;

        public int id_evento
        {
            get => _idEvento;
            set { _idEvento = value; OnPropertyChanged(); }
        }

        public int id_ano_calendario
        {
            get => _idAnoCalendario;
            set { _idAnoCalendario = value; OnPropertyChanged(); }
        }

        public DateTime data_original
        {
            get => _dataOriginal;
            set
            {
                _dataOriginal = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsSubstituido));
                OnPropertyChanged(nameof(DescricaoCompleta));
            }
        }

        public DateTime data_observada
        {
            get => _dataObservada;
            set
            {
                _dataObservada = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsSubstituido));
                OnPropertyChanged(nameof(DescricaoCompleta));
            }
        }

        public string descricao
        {
            get => _descricao;
            set
            {
                _descricao = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DescricaoCompleta));
            }
        }

        public string tipo
        {
            get => _tipo;
            set { _tipo = value; OnPropertyChanged(); }
        }

        public bool IsSubstituido => data_original.Date != data_observada.Date;

        public string DescricaoCompleta => IsSubstituido
            ? $"{descricao} ({data_original:d/M} Substituído por {data_observada:d/M})"
            : descricao;

        public System.Windows.Media.SolidColorBrush CorLegendaBrush
        {
            get
            {
                string hex = tipo switch
                {
                    "Feriado Nacional" => "#4472C4",
                    "Feriado Estadual" => "#ED7D31",
                    "Feriado Municipal" => "#ED7D31",
                    "Ponto Facultativo" => "#7030A0",
                    _ => "#00B0F0"
                };
                return new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(hex));
            }
        }
    }

}