using ReservationServer.Models;

namespace ReservationServer.Repositries;

public interface IReservationRepositry
{
    Task<List<Reservation>> GetShowAsync(DateTime startDate);

    Task PostInsert(Reservation reservation);

    Task DeleteAsync(int id);

}
