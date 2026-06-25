using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using magal.Models;
using magal.Data;

namespace magal.Data.Repositories
{
    public class AnoCalendarioRepository
    {
        public async Task<List<AnoCalendario>> ListarTodos()
        {
            var lista = new List<AnoCalendario>();

            try
            {
                using var conn = (MySqlConnection)DbConnectionFactory.CreateConnection();
                await conn.OpenAsync();

                string sql = @"
                    SELECT id_ano_calendario, ano, horas_dia, inicio_ferias, fim_ferias
                    FROM ano_calendario
                    ORDER BY ano DESC";

                using var cmd = new MySqlCommand(sql, conn);
                using var reader = (MySqlDataReader)await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                    lista.Add(MapearAno(reader));
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao listar anos do calendário: " + ex.Message);
            }

            return lista;
        }

        public async Task<AnoCalendario?> ObterPorId(int id)
        {
            try
            {
                using var conn = (MySqlConnection)DbConnectionFactory.CreateConnection();
                await conn.OpenAsync();

                string sql = @"
                    SELECT id_ano_calendario, ano, horas_dia, inicio_ferias, fim_ferias
                    FROM ano_calendario
                    WHERE id_ano_calendario = @id";

                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", id);

                using var reader = (MySqlDataReader)await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                    return MapearAno(reader);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao obter ano do calendário: " + ex.Message);
            }

            return null;
        }

        public async Task<int> Inserir(AnoCalendario ano)
        {
            try
            {
                using var conn = (MySqlConnection)DbConnectionFactory.CreateConnection();
                await conn.OpenAsync();

                string sql = @"
                    INSERT INTO ano_calendario (ano, horas_dia, inicio_ferias, fim_ferias)
                    VALUES (@ano, @horasDia, @inicioFerias, @fimFerias);
                    SELECT LAST_INSERT_ID();";

                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ano", ano.ano);
                cmd.Parameters.AddWithValue("@horasDia", ano.horas_dia);
                cmd.Parameters.AddWithValue("@inicioFerias", (object?)ano.inicio_ferias ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@fimFerias", (object?)ano.fim_ferias ?? DBNull.Value);

                return Convert.ToInt32(await cmd.ExecuteScalarAsync());
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao inserir ano do calendário: " + ex.Message);
            }
        }

        public async Task Atualizar(AnoCalendario ano)
        {
            try
            {
                using var conn = (MySqlConnection)DbConnectionFactory.CreateConnection();
                await conn.OpenAsync();

                string sql = @"
                    UPDATE ano_calendario
                    SET ano = @ano,
                        horas_dia = @horasDia,
                        inicio_ferias = @inicioFerias,
                        fim_ferias = @fimFerias
                    WHERE id_ano_calendario = @id";

                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", ano.id_ano_calendario);
                cmd.Parameters.AddWithValue("@ano", ano.ano);
                cmd.Parameters.AddWithValue("@horasDia", ano.horas_dia);
                cmd.Parameters.AddWithValue("@inicioFerias", (object?)ano.inicio_ferias ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@fimFerias", (object?)ano.fim_ferias ?? DBNull.Value);

                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao atualizar ano do calendário: " + ex.Message);
            }
        }

        public async Task Excluir(int id)
        {
            try
            {
                using var conn = (MySqlConnection)DbConnectionFactory.CreateConnection();
                await conn.OpenAsync();

                using var cmd = new MySqlCommand(
                    "DELETE FROM ano_calendario WHERE id_ano_calendario = @id", conn);
                cmd.Parameters.AddWithValue("@id", id);

                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao excluir ano do calendário: " + ex.Message);
            }
        }

        private static AnoCalendario MapearAno(MySqlDataReader reader)
        {
            int ordInicioFerias = reader.GetOrdinal("inicio_ferias");
            int ordFimFerias = reader.GetOrdinal("fim_ferias");

            return new AnoCalendario
            {
                id_ano_calendario = reader.GetInt32(reader.GetOrdinal("id_ano_calendario")),
                ano = reader.GetInt32(reader.GetOrdinal("ano")),
                horas_dia = reader.GetDecimal(reader.GetOrdinal("horas_dia")),
                inicio_ferias = reader.IsDBNull(ordInicioFerias) ? null : reader.GetDateTime(ordInicioFerias),
                fim_ferias = reader.IsDBNull(ordFimFerias) ? null : reader.GetDateTime(ordFimFerias)
            };
        }
    }
}