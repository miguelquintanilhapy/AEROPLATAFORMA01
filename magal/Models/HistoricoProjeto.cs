using System;

namespace magal.Models
{
    public class HistoricoProjeto : BaseModel
    {
        // id_historico (PK) conforme seu DER
        public int id_historico { get; set; }

        // id_projeto (FK) conforme seu DER
        public int id_projeto { get; set; }

        public string tipo_projeto { get; set; }

        public string complexidade { get; set; }

        public decimal custo_real { get; set; }

        public decimal margem_estimada { get; set; }

        public decimal margem_real { get; set; }

        public decimal desvio_percentual { get; set; }

        public DateTime data_conclusao { get; set; }

        public string observacoes { get; set; }
    }
}