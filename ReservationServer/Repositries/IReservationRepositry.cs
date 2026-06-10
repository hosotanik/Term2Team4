using ReservationServer.Models;

namespace ReservationServer.Repositries;

public interface IReservationRepositry
{
    Task<List<Reservation>> GetShowAsync(string startDate);

    Task PostInsert(Reservation reservation);

    Task DeleteAsync(int id);

}
