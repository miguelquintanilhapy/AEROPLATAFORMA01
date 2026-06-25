using System;

namespace magal.Models
{
    public class AnoCalendario : BaseModel
    {
        private int _idAnoCalendario;
        private int _ano;
        private decimal _horasDia;
        private DateTime? _inicioFerias;
        private DateTime? _fimFerias;

        public int id_ano_calendario
        {
            get => _idAnoCalendario;
            set { _idAnoCalendario = value; OnPropertyChanged(); }
        }

        public int ano
        {
            get => _ano;
            set { _ano = value; OnPropertyChanged(); }
        }

        public decimal horas_dia
        {
            get => _horasDia;
            set { _horasDia = value; OnPropertyChanged(); }
        }

        public DateTime? inicio_ferias
        {
            get => _inicioFerias;
            set { _inicioFerias = value; OnPropertyChanged(); }
        }

        public DateTime? fim_ferias
        {
            get => _fimFerias;
            set { _fimFerias = value; OnPropertyChanged(); }
        }

        public override string ToString() => ano.ToString();
    }
}