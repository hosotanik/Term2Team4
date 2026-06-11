using Dapper;
using Microsoft.Data.SqlClient;
using ReservationServer.Controllers;
using ReservationServer.Models;
using System.Diagnostics.CodeAnalysis;

namespace ReservationServer.Repositries;

public class ReservationRepositry : IReservationRepositry
{
    private readonly ILogger<ReservationsController> _logger;
    private readonly IConfiguration _config;
    private readonly string connectionString;
    public ReservationRepositry(IConfiguration config, ILogger<ReservationsController> logger)
    {
        _config = config;
        connectionString = _config.GetConnectionString("DefaultConnection");
        _logger = logger;

    }

    private List<Reservation> reservations;

    public async Task<List<Reservation>> GetShowAsync(string startDate)
    {
        using (var connection = new SqlConnection(connectionString))
        {
            try
            {
                var sql = @"SELECT id AS Id,
                            conference_name AS ConferenceName,
                            start_at AS StartAt,
                            end_at AS EndAt,
                            reservation_name AS ReservationName
                            FROM reservation
                            WHERE CAST(start_at AS DATE) = @TargetDate";

                reservations = (await connection
                    .QueryAsync<Reservation>(sql, new { TargetDate = startDate })
                    ).ToList();

                return reservations;
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex,"予約一覧取得失敗");
                throw;
            }
        }
    }

    public async Task PostInsert(Reservation reservation)
    {
        using (var connection = new SqlConnection(connectionString))
        {
            try
            {
                var sql = @"INSERT INTO reservation
                            (
                                conference_name,
                                start_at,
                                end_at,
                                reservation_name
                            )
                            VALUES
                            (
                                @ConferenceName,
                                @StartAt,
                                @EndAt,
                                @ReservationName
                            )";

                await connection.ExecuteAsync(sql, reservation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "予約登録失敗");
                throw;
            }
        }

    }

    public async Task<bool> DeleteAsync(int id)
    {
        using (var connection = new SqlConnection(connectionString))
        {
            try
            {
                int count = await connection.ExecuteAsync("DELETE FROM reservation WHERE Id = @Id", new { Id = id });

                return count > 0;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "削除失敗");
                throw;
            }
        }

    }
}
