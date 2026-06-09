// 過去の予約をしないようにするため
const Today = new Date().toLocaleDateString("ja-JP", { timeZone: "Asia/Tokyo" ,
        year: "numeric",  // 年を数字で（2026）
        month: "2-digit", // 月を2桁で（06）
        day: "2-digit"    // 日を2桁で（08）
    })
    .replaceAll('/', '-');
document.getElementById('bookingDate').min = Today;