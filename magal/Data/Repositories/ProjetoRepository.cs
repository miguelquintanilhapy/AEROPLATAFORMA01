using System;
using MySql.Data.MySqlClient;
using magal.Data;
using magal.Models;
using System.Collections.Generic;

namespace magal.Data.Repositories
{
    public class ProjetoRepository
    {
        public void SalvarProjetoCompleto(Projeto projeto, List<Custo> custosExtras)
        {
            using (var conn = (MySqlConnection)DbConnectionFactory.CreateConnection())
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // 1. INSERIR O PROJETO
                        using (var cmd = new MySqlCommand(@"INSERT INTO projeto (nome, id_cliente, id_usuario, data_criacao, tipo, status) 
                                                          VALUES (@nome, @idCliente, @idUsuario, @data, @tipo, @status);", conn, transaction))
                        {
                            // Acessando as propriedades minúsculas conforme os novos Models
                            cmd.Parameters.AddWithValue("@nome", projeto.nome);
                            cmd.Parameters.AddWithValue("@idCliente", projeto.id_cliente);
                            cmd.Parameters.AddWithValue("@idUsuario", projeto.id_usuario == 0 ? 1 : projeto.id_usuario);
                            cmd.Parameters.AddWithValue("@data", projeto.data_criacao);
                            cmd.Parameters.AddWithValue("@tipo", projeto.tipo ?? "Serviço");
                            cmd.Parameters.AddWithValue("@status", projeto.status ?? "Rascunho");

                            cmd.ExecuteNonQuery();

                            // Recupera o ID gerado (id_projeto)
                            cmd.CommandText = "SELECT LAST_INSERT_ID();";
                            projeto.id_projeto = Convert.ToInt32(cmd.ExecuteScalar());
                        }

                        // 2. INSERIR O ORÇAMENTO
                        using (var cmd = new MySqlCommand(@"INSERT INTO orcamento (id_projeto, custo_base, percentual_impostos, margem_percentual, valor_final) 
                                                          VALUES (@idProj, @custo, @imp, @marg, @final);", conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@idProj", projeto.id_projeto);
                            cmd.Parameters.AddWithValue("@custo", projeto.Orcamento.custo_base);
                            cmd.Parameters.AddWithValue("@imp", projeto.Orcamento.percentual_impostos);
                            cmd.Parameters.AddWithValue("@marg", projeto.Orcamento.margem_percentual);
                            cmd.Parameters.AddWithValue("@final", projeto.Orcamento.valor_final);
                            cmd.ExecuteNonQuery();
                        }

                        // 3. INSERIR AS TAREFAS (Mão de Obra)
                        foreach (var tarefa in projeto.Tarefas)
                        {
                            using (var cmd = new MySqlCommand(@"INSERT INTO tarefa (id_projeto, descricao, id_funcionario, horas_estimadas, status) 
                                                              VALUES (@idProj, @desc, @idFunc, @horas, @status);", conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@idProj", projeto.id_projeto);
                                cmd.Parameters.AddWithValue("@desc", tarefa.descricao);
                                cmd.Parameters.AddWithValue("@idFunc", tarefa.id_funcionario);
                                cmd.Parameters.AddWithValue("@horas", tarefa.horas_estimadas);
                                cmd.Parameters.AddWithValue("@status", tarefa.status);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        // 4. INSERIR OS CUSTOS EXTRAS 
                        foreach (var custo in custosExtras)
                        {
                            using (var cmd = new MySqlCommand(@"INSERT INTO custo (id_projeto, nome, categoria, tipo, valor, unidade) 
                                                              VALUES (@idProj, @nome, @cat, @tipo, @valor, @unidade);", conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@idProj", projeto.id_projeto);
                                cmd.Parameters.AddWithValue("@nome", custo.nome);
                                cmd.Parameters.AddWithValue("@cat", custo.categoria);
                                cmd.Parameters.AddWithValue("@tipo", custo.tipo ?? "Direto");
                                cmd.Parameters.AddWithValue("@valor", custo.valor);
                                cmd.Parameters.AddWithValue("@unidade", custo.unidade ?? "Unitário");
                                cmd.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception("Erro ao salvar no MySQL: " + ex.Message);
                    }
                }
            }
        }
    }
}