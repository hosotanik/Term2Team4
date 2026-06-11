const API_URL = 'https://localhost:7119/api/reservations';
let allReservations = [];
let pollingTimer = null; 
const tbody = document.getElementById('reservationList'); 
const today = new Date().toLocaleDateString("ja-JP", { timeZone: "Asia/Tokyo" ,
        year: "numeric",  // 年を数字で（2026）
        month: "2-digit", // 月を2桁で（06）
        day: "2-digit"    // 日を2桁で（08）
    })
    .replaceAll('/', '-');
document.getElementById('reservationDate').min = today;

function clearMessage() {
    document.getElementById("messageArea").innerHTML = "";
}
function showError(message) {
    document.getElementById("messageArea").innerHTML = `
        <div class="alert alert-danger" role="alert">${message}</div>
    `;
}

function initApp() { 
    document.getElementById('displayDate').value = today; 
    document.getElementById('reservationDate').value = today; 
    loadReservations(); 
    
    // ポーリングの開始（例: 5000ミリ秒 = 5秒ごと）
    startPolling(5000);
} 

// ポーリングを開始する関数
function startPolling(intervalMs) {
    // 既存のタイマーがあれば二重起動を防ぐためにクリア
    if (pollingTimer) clearInterval(pollingTimer);
    
    pollingTimer = setInterval(() => {
        loadReservations();
    }, intervalMs);
}

// 画面が閉じられたり遷移したりした際にタイマーを解放（メモリリーク防止）
window.addEventListener('beforeunload', () => {
    if (pollingTimer) clearInterval(pollingTimer);
});

function filterDisplay(targetDate) { 
    const selectedData = allReservations 
    if (selectedData.length === 0) { 
        tbody.innerHTML = `<tr><td colspan="4">選択された日付（${targetDate}）の予約はありません。</td></tr>`;       
        return; 
    } 
    tbody.innerHTML = selectedData.map(item => { 
    return `
    <tr>
    <td>${item.conferenceName}</strong></td> 
    <td>${item.startAt.split('T')[1].slice(0, 5)} - ${item.endAt.split('T')[1].slice(0, 5)}</td> 
    <td>${item.reservationName}</td> 
    <td><button class="btn btn-danger" onclick="deleteReservation(${item.id})">削除</button></td> </tr> `;
    }).join(''); 
} 

async function loadReservations() {
    const dateInput = document.getElementById('displayDate');
    if (!dateInput.checkValidity()){
        tbody.innerHTML = `<tr><td colspan="4">2026/06/09以降を指定してください。</td></tr>`;     
        return; 
    }
    const displayDate = dateInput.value;
    //clearMessage();
    try {
        // コントローラーの [FromQuery] DateOnly date に合わせてパラメータを付与
        const response = await fetch(`${API_URL}?date=${displayDate}`, {
            method: 'GET',
            headers: { 'Accept': 'application/json' }
        });

        if (response.ok) { 
            allReservations = await response.json(); 
            filterDisplay(displayDate);
        } 

    } catch (error) {
        console.error("GETエラー:", error);
    }
}

async function createReservation(event) {
    // フォームのデフォルトの画面リロードを防止
    event.preventDefault(); 
    clearMessage();

    const conferenceName = document.getElementById('conferenceName').value;
    const reservationDate = document.getElementById('reservationDate').value;
    const startTime = document.getElementById('startTime').value; 
    const endTime = document.getElementById('endTime').value;     
    const reservationName = document.getElementById('reservationName').value;

    // サーバーの TimeOnly 変換失敗を防ぐため、秒数（:00）を強制付与する
    const formatTimeOnly = (timeStr) => {
        if (!timeStr) return "00:00:00";
        return timeStr.split(':').length === 2 ? `${timeStr}:00` : timeStr;
    };

    // // C#の「ReservationCreate」に対応
    const bodyData = {
        conferenceName: conferenceName, 
        date: reservationDate,
        startTime: formatTimeOnly(startTime),
        endTime: formatTimeOnly(endTime),
        reservationName: reservationName
    };

    try {
        const response = await fetch(API_URL, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Accept': 'application/json'
            },
            body: JSON.stringify(bodyData)
        });

        //バリデーションに引っかかった場合
        if (response.status === 400) {
            const errorText = await response.text();
            showError(errorText); 
            return;
        }
        else if (response.status === 201) {  
            clearMessage();
            document.getElementById('displayDate').value = bodyData.date;  
            document.getElementById('reservationDate').value = today; 
            document.getElementById('reservationName').value = '';
            document.getElementById('startTime').value = '';
            document.getElementById('endTime').value = '';
            loadReservations(); 
        }
    } catch (error) {
        console.error("POST追加エラー:", error);
    }
}
async function deleteReservation(id) {
    clearMessage();

    try {
        // コントローラーの [HttpDelete("{id}")] に合わせてパスにIDを結合
        const response = await fetch(`${API_URL}/${id}`, {
            method: 'DELETE',
            headers: {
                'Accept': 'application/json'
            },
        });
        if (response.ok) await loadReservations();
    } catch (error) {
        console.error("DELETE削除エラー:", error);
    }
}

// 表示日付（display-date）が変更されたら、自動でその日の予約一覧をリロードする
document.getElementById('displayDate').addEventListener('blur', loadReservations);

// クリックしたらcreateReservationを読み込む
const Form = document.getElementById('reservationForm');
if (Form){
    Form.addEventListener('submit', createReservation);
    Form.addEventListener('input', clearMessage); 
}

initApp();