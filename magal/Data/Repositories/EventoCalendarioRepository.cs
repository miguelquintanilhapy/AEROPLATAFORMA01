using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using magal.Models;
using magal.Data;

namespace magal.Data.Repositories
{
    public class EventoCalendarioRepository
    {
        public async Task<List<EventoCalendario>> ListarPorAno(int idAnoCalendario)
        {
            var lista = new List<EventoCalendario>();

            try
            {
                using var conn = (MySqlConnection)DbConnectionFactory.CreateConnection();
                await conn.OpenAsync();

                string sql = @"
                    SELECT id_evento, id_ano_calendario, data_original, data_observada,
                           descricao, tipo
                    FROM evento_calendario
                    WHERE id_ano_calendario = @idAno
                    ORDER BY data_observada";

                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@idAno", idAnoCalendario);

                using var reader = (MySqlDataReader)await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                    lista.Add(MapearEvento(reader));
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao listar eventos do calendário: " + ex.Message);
            }

            return lista;
        }

        public async Task<int> Inserir(EventoCalendario evento)
        {
            try
            {
                using var conn = (MySqlConnection)DbConnectionFactory.CreateConnection();
                await conn.OpenAsync();

                string sql = @"
                    INSERT INTO evento_calendario
                        (id_ano_calendario, data_original, data_observada, descricao, tipo)
                    VALUES
                        (@idAno, @dataOriginal, @dataObservada, @descricao, @tipo);
                    SELECT LAST_INSERT_ID();";

                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@idAno", evento.id_ano_calendario);
                cmd.Parameters.AddWithValue("@dataOriginal", evento.data_original.Date);
                cmd.Parameters.AddWithValue("@dataObservada", evento.data_observada.Date);
                cmd.Parameters.AddWithValue("@descricao", evento.descricao);
                cmd.Parameters.AddWithValue("@tipo", evento.tipo);

                return Convert.ToInt32(await cmd.ExecuteScalarAsync());
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao inserir evento do calendário: " + ex.Message);
            }
        }

        public async Task Atualizar(EventoCalendario evento)
        {
            try
            {
                using var conn = (MySqlConnection)DbConnectionFactory.CreateConnection();
                await conn.OpenAsync();

                string sql = @"
                    UPDATE evento_calendario
                    SET data_original  = @dataOriginal,
                        data_observada = @dataObservada,
                        descricao      = @descricao,
                        tipo           = @tipo
                    WHERE id_evento = @id";

                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", evento.id_evento);
                cmd.Parameters.AddWithValue("@dataOriginal", evento.data_original.Date);
                cmd.Parameters.AddWithValue("@dataObservada", evento.data_observada.Date);
                cmd.Parameters.AddWithValue("@descricao", evento.descricao);
                cmd.Parameters.AddWithValue("@tipo", evento.tipo);

                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao atualizar evento do calendário: " + ex.Message);
            }
        }

        public async Task Excluir(int id)
        {
            try
            {
                using var conn = (MySqlConnection)DbConnectionFactory.CreateConnection();
                await conn.OpenAsync();

                using var cmd = new MySqlCommand(
                    "DELETE FROM evento_calendario WHERE id_evento = @id", conn);
                cmd.Parameters.AddWithValue("@id", id);

                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao excluir evento do calendário: " + ex.Message);
            }
        }

        private static EventoCalendario MapearEvento(MySqlDataReader reader)
        {
            return new EventoCalendario
            {
                id_evento = reader.GetInt32(reader.GetOrdinal("id_evento")),
                id_ano_calendario = reader.GetInt32(reader.GetOrdinal("id_ano_calendario")),
                data_original = reader.GetDateTime(reader.GetOrdinal("data_original")),
                data_observada = reader.GetDateTime(reader.GetOrdinal("data_observada")),
                descricao = reader.GetString(reader.GetOrdinal("descricao")),
                tipo = reader.GetString(reader.GetOrdinal("tipo"))
            };
        }
    }
}