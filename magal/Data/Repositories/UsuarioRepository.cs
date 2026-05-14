using System;
using MySql.Data.MySqlClient;
using magal.Data;
using magal.Models;
using System.Collections.Generic;

namespace magal.Data.Repositories
{
    public class UsuarioRepository
    {
        public void Salvar(Usuario usuario)
        {
            using (var conn = (MySqlConnection)DbConnectionFactory.CreateConnection())
            {
                conn.Open();
                // Como é um cadastro simples, não usei transaction, 
                // mas segui o padrão de Parameters para segurança.
                string sql = @"INSERT INTO usuario (nome, email, senha, status) 
                               VALUES (@nome, @email, @senha, @status)";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@nome", usuario.nome);
                    cmd.Parameters.AddWithValue("@email", usuario.email);
                    cmd.Parameters.AddWithValue("@senha", usuario.senha);
                    cmd.Parameters.AddWithValue("@status", usuario.status ?? "Ativo");

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<Usuario> BuscarTodos()
        {
            var lista = new List<Usuario>();
            using (var conn = (MySqlConnection)DbConnectionFactory.CreateConnection())
            {
                conn.Open();
                string sql = "SELECT * FROM usuario ORDER BY nome ASC";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Usuario
                            {
                                id_usuario = Convert.ToInt32(reader["id_usuario"]),
                                nome = reader["nome"].ToString(),
                                email = reader["email"].ToString(),
                                senha = reader["senha"].ToString(),
                                status = reader["status"].ToString()
                            });
                        }
                    }
                }
            }
            return lista;
        }

        public void ExcluirUsuario(int idUsuario)
        {
            using (var conn = (MySqlConnection)DbConnectionFactory.CreateConnection())
            {
                conn.Open();
                string sql = "DELETE FROM usuario WHERE id_usuario = @id";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", idUsuario);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}