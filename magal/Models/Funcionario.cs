using magal.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace magal.Models
{
    public class Funcionario : BaseModel
    {
        // Alterado para bater com id_funcionario (PK)
        public int id_funcionario { get; set; }

        // Alterado para bater com id_cargo (FK)
        public int id_cargo { get; set; }

        public string nome { get; set; }

        // Alterado para bater com custo_hora conforme seu DER
        public decimal custo_hora { get; set; }

        // Alterado para bater com tipo_vinculo conforme seu DER
        public string tipo_vinculo { get; set; }

        public string status { get; set; }

        public Cargo Cargo { get; set; }

        public override string ToString()
        {
            return nome;
        }

        public string Iniciais
        {
            get
            {
                if (string.IsNullOrWhiteSpace(nome)) return "??";
                var partes = nome.Trim().Split(' ');
                if (partes.Length == 1) return partes[0].Substring(0, Math.Min(2, partes[0].Length)).ToUpper();
                return (partes[0][0].ToString() + partes[partes.Length - 1][0].ToString()).ToUpper();
            }
        }
    }
}