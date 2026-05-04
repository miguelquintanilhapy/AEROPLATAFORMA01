using System;
using System.Collections.ObjectModel;

namespace magal.Models
{
    public class Projeto : BaseModel
    {
        // id_projeto (PK) conforme seu DER
        public int id_projeto { get; set; }

        // id_usuario (FK) conforme seu DER
        public int id_usuario { get; set; }

        // id_cliente (FK) conforme seu DER
        public int id_cliente { get; set; }

        public Cliente Cliente { get; set; }

        public string nome { get; set; }

        // No seu DER: "Produto/Serviço"
        public string tipo { get; set; }

        // No seu DER: "Rascunho/Orçado/Aprovado/Executando/Concluído"
        public string status { get; set; }

        public DateTime data_criacao { get; set; } = DateTime.Now;

        public DateTime? data_conclusao_prevista { get; set; }

        // Objetos de navegação e coleções
        public Orcamento Orcamento { get; set; } = new Orcamento();
        public ObservableCollection<Tarefa> Tarefas { get; set; } = new ObservableCollection<Tarefa>();
        public ObservableCollection<Custo> Custos { get; set; } = new ObservableCollection<Custo>();
    }
}