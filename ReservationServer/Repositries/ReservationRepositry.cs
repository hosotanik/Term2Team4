using ReservationServer.Models;
using System.Diagnostics.CodeAnalysis;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ReservationServer.Repositries;

public class ReservationRepositry : IReservationRepositry
{

    private readonly IConfiguration _config;
    private readonly string connectionString;
    public ReservationRepositry(IConfiguration config)
    {
        _config = config;
        connectionString = _config.GetConnectionString("DefaultConnection");
    }

    private List<Reservation> reservations;

    public async Task<List<Reservation>> GetShowAsync(string startDate)
    {

        using (var connection = new SqlConnection(connectionString))
        {
            var sql = @"SELECT id AS Id,
                            conference_name AS ConferenceName,
                            start_at AS StartAt,
                            end_at AS EndAt,
                            reservation_name AS ReservationName
                            FROM reservation
                            WHERE CAST(start_at AS DATE) = @TargetDate";

            reservations = ( await connection
                .QueryAsync<Reservation>(sql, new { TargetDate = startDate })
                ).ToList();

            return reservations;

        }
    }

    public async Task PostInsert(Reservation reservation)
    {
        using (var connection = new SqlConnection(connectionString))
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

    }

    public async Task DeleteAsync(int id)
    {
        using (var connection = new SqlConnection(connectionString))
        {
           await connection.ExecuteAsync("DELETE FROM reservation WHERE Id = @Id", new { Id = id });
        }

    }
}
