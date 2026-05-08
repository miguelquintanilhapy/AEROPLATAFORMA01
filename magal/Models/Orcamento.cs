using magal.ViewModels;
using magal.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace magal.Models
{
    public class Orcamento : BaseModel
    {
        private int _id_orcamento;
        public int id_orcamento
        {
            get => _id_orcamento;
            set { _id_orcamento = value; OnPropertyChanged(); }
        }

        private int _id_projeto;
        public int id_projeto
        {
            get => _id_projeto;
            set { _id_projeto = value; OnPropertyChanged(); }
        }

        private decimal _margem_percentual;
        public decimal margem_percentual
        {
            get => _margem_percentual;
            set
            {
                if (_margem_percentual == value) return;
                _margem_percentual = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(valor_margem));
                OnPropertyChanged(nameof(valor_final));
            }
        }

        private decimal _percentual_impostos;
        public decimal percentual_impostos
        {
            get => _percentual_impostos;
            set
            {
                if (_percentual_impostos == value) return;
                _percentual_impostos = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(valor_impostos));
                OnPropertyChanged(nameof(valor_final));
            }
        }

        private decimal _custo_base;
        public decimal custo_base
        {
            get => _custo_base;
            set
            {
                if (_custo_base == value) return;
                _custo_base = value;
                OnPropertyChanged();

                OnPropertyChanged(nameof(valor_final));
                OnPropertyChanged(nameof(valor_margem));
                OnPropertyChanged(nameof(valor_impostos));
            }
        }
        public decimal valor_final
        {
            get
            {
                // 1. Calculamos a margem de lucro sobre o custo base
                decimal margemCalculada = custo_base * (margem_percentual / 100);

                // 2. Somamos para ter o valor com margem
                decimal valorComMargem = custo_base + margemCalculada;

                // 3. Calculamos o imposto sobre o valor com margem
                decimal impostoCalculado = valorComMargem * (percentual_impostos / 100);

                // 4. Retornamos a soma total
                decimal total = valorComMargem + impostoCalculado;

                // Se a conta der zero (caso campos estejam nulos), 
                // usamos o valor que veio direto da coluna 'valor_final' do banco
                return total > 0 ? total : _valor_final;
            }
            set
            {
                _valor_final = value;
                OnPropertyChanged();
            }
        }
        private decimal _valor_final;
        public DateTime data_criacao { get; set; } = DateTime.Now;

        // Propriedades calculadas que não vão para o banco, somente leitura
        public decimal valor_margem => custo_base * (margem_percentual / 100);
        public decimal valor_impostos => (custo_base + valor_margem) * (percentual_impostos / 100);

        public void CalcularTotal(List<Tarefa> tarefas, List<Custo> custosExtras)
        {
            if (tarefas == null || custosExtras == null) return;

            decimal totalMaoDeObra = tarefas.Sum(t => t.custo_real);
            decimal totalCustosExtras = custosExtras.Sum(c => c.valor);

            decimal novoCustoBase = totalMaoDeObra + totalCustosExtras;
            decimal margem = novoCustoBase * (margem_percentual / 100);
            decimal valorComMargem = novoCustoBase + margem;
            decimal vImpostos = valorComMargem * (percentual_impostos / 100);
            decimal novoValorFinal = valorComMargem + vImpostos;

            custo_base = novoCustoBase;
            valor_final = novoValorFinal;
        }
    }
}