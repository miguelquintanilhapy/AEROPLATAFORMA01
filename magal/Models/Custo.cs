using System;

namespace magal.Models
{
    public class Custo : BaseModel
    {
        // Alterado para bater com id_custo (PK)
        public int id_custo { get; set; }

        // Alterado para bater com id_projeto (FK)
        public int id_projeto { get; set; }

        public string nome { get; set; }

        public string categoria { get; set; }

        // No seu DER: "Direto/Indireto"
        public string tipo { get; set; }

        public decimal valor { get; set; }

        // No seu DER: "Unitário/Hora/Dia/Mês"
        public string unidade { get; set; }

        public DateTime data_cadastro { get; set; } = DateTime.Now;
    }
}