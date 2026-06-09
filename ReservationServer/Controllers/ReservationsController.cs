using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReservationServer.Models;
using ReservationServer.Repositries;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ReservationServer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReservationsController : ControllerBase
{
    private readonly ILogger<ReservationsController> _logger;
    private readonly IReservationRepositry _reservation;


    public ReservationsController(IReservationRepositry reservation, ILogger<ReservationsController> logger)
    {
        _reservation = reservation;
        _logger = logger;
    }


    [HttpGet]
    public async Task<IActionResult> GetReservations([FromQuery] string date)
    {


        var reservations = await _reservation.GetShowAsync(DateTime.Now);

        return Ok(reservations);
    }


    [HttpPost]
    public async Task<IActionResult> PostReservationsAsync([FromBody]ReservationCreate input)
    {

        DateTime startTime = DateOnly.Parse(input.Date).ToDateTime(input.StartTime);
        DateTime endTime = DateOnly.Parse(input.Date).ToDateTime(input.EndTime);

        if(startTime >= endTime)
        {
            return BadRequest("終了時刻は開始時刻以降にしてください");
            // logを入れる
        }

        //var aaaa = _reservation.GetShowAsync(DateTime.Now);
        var existingReservations = await _reservation.GetShowAsync(startTime.Date);



        Reservation newInput = new()
        {
            ConferenceName = input.ConfrenceName,
            StartDate = startTime,
            EndDate = endTime,
            ReservationName = input.ReservationName,
        };

        await _reservation.PostInsert(newInput);

        return Ok();
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteReservationsAsync(int id)
    {
        await _reservation.DeleteAsync(id);

        return Ok();
    
    }
}
