using magal.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace magal.Models
{
    public class Cargo : BaseModel
    {
        // Alterado para bater com 'id_cargo' do seu DER
        public int id_cargo { get; set; }

        public string nome { get; set; }

        public string nivel { get; set; }

        // Alterado para bater com 'custo_medio_hora' do seu DER
        public decimal custo_medio_hora { get; set; }

        public string descricao { get; set; }
    }
}