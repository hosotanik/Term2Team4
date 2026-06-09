const API_URL = 'https://localhost:7209/api/reservation';
let allReservations = [];
const today = new Date().toLocaleDateString("ja-JP", { timeZone: "Asia/Tokyo" ,
        year: "numeric",  // 年を数字で（2026）
        month: "2-digit", // 月を2桁で（06）
        day: "2-digit"    // 日を2桁で（08）
    })
    .replaceAll('/', '-');
document.getElementById('bookingDate').min = today;
function clearMessage() {
    document.getElementById("message-area").innerHTML = "";
}
function showError(message) {
    document.getElementById("message-area").innerHTML = `
        <div class="alert alert-danger" role="alert">
            ${message}
        </div>
    `;
}
function initApp() { 
    const today = new Date().toISOString().split('T')[0]; 
    document.getElementById('display-date').value = today; 
    document.getElementById('bookingDate').value = today; 
    loadReservations(); 
} 

initApp();