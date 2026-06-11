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

        if (DateTime.Parse(date) < new DateTime(2026, 6, 9))
        {
            _logger.LogWarning("不正な日付が指定されました");
            return BadRequest("2026-06-09以降にしてください");
        }

        try
        {
            _logger.LogInformation("予約一覧取得開始");

            // 指定日の予約一覧を取得
            var reservations = await _reservation.GetShowAsync(date);

            _logger.LogInformation("予約一覧取得成功");
            return Ok(reservations);
        }
        catch
        {
            _logger.LogError("予約一覧取得失敗");
            return StatusCode(500 , "予約一覧を取得できませんでした");
        }
    }


    [HttpPost]
    public async Task<IActionResult> PostReservationsAsync([FromBody]ReservationCreate input)
    {
        try
        {
            // 年月日と時刻を結合してDateTimeへ
            DateOnly reservationDate = DateOnly.Parse(input.Date);

            DateTime startTime = reservationDate.ToDateTime(input.StartTime);
            DateTime endTime = reservationDate.ToDateTime(input.EndTime);
            string[] allowedRooms = { "会議室A", "会議室B", "会議室C" };

            if (!allowedRooms.Contains(input.ConferenceName))
            {
                _logger.LogWarning("存在しない会議室を指定されました");
                return BadRequest("存在する会議室を選択してください");
            }


            if (reservationDate < DateOnly.FromDateTime(DateTime.Today))
            {
                _logger.LogWarning("過去日の予約が指定されました");
                return BadRequest("予約日は今日以降を指定してください");
            }

            // 開始時刻より終了時刻が後であること
            if(startTime >= endTime)
            {
                _logger.LogWarning("不正な予約時間です");
                return BadRequest("終了時刻は開始時刻以降にしてください");
            }

            // 予約情報を取得
            var existingReservations = await _reservation.GetShowAsync(input.Date);

            // 同じ会議室で予約が重複してないか判別
            bool isOverlapping = existingReservations.Any(r =>
            input.ConferenceName == r.ConferenceName &&
            startTime < r.EndAt &&
            endTime > r.StartAt);

            if (isOverlapping)
            {
                _logger.LogWarning("予約重複");
                return BadRequest("指定された時間帯は、既に他の予約が入っています。");
            }

            // 登録用のエンティティを作成
            Reservation newInput = new()
            {
                ConferenceName = input.ConferenceName,
                StartAt = startTime,
                EndAt = endTime,
                ReservationName = input.ReservationName,
            };

            // 予約を登録
            await _reservation.PostInsert(newInput);

            _logger.LogInformation("予約登録成功");
            return Created("", null);
        }
        catch
        {
            _logger.LogError("予約登録失敗");
            return StatusCode(500, "予約登録できませんでした");
        }
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteReservationsAsync(int id)
    {
        try
        {
            // 指定IDの予約を削除
            bool deleted = await _reservation.DeleteAsync(id);
            if (!deleted)
            {
                _logger.LogWarning("指定したIDがない");
                return BadRequest("指定したIDが見つかりません");
            }

            _logger.LogInformation("削除に成功しました。");
            return Ok();
        }
        catch
        {
            _logger.LogError("削除失敗");
            return StatusCode(500, "削除失敗しました");
        }
    
    }
}
