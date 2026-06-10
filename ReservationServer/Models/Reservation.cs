namespace ReservationServer.Models;

public class Reservation
{
    public int Id { get; set; }


    public string ConferenceName { get; set; }


    public DateTime StartAt { get; set; }


    public DateTime EndAt { get; set; }


    public string ReservationName { get; set; }
}
