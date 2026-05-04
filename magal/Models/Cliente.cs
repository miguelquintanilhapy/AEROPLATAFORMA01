using magal.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace magal.Models
{
    public class Cliente : BaseModel
    {
        // Alterado para id_cliente conforme seu DER
        public int id_cliente { get; set; }

        public string nome { get; set; }

        // Mantido tipo (conforme seu DER: PF/PJ)
        public string tipo { get; set; }

        // Alterado para cpf_cnpj conforme seu DER
        public string cpf_cnpj { get; set; }

        public string cidade { get; set; }

        public string estado { get; set; }

        public string contato { get; set; }
    }
}