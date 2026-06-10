# 会議室使用予約システム


## 実装した機能 
1. **予約追加機能**  
   日時、場所、予約者を入力して追加する
2. **予約削除機能**  
   削除ボタンクリックすると予約していたのが削除される
3. **予約されている一覧の表示機能**  
   選択した日の予約一覧が表示される

## 役割分担
**設計: 細谷・渋谷**  
**テスト項目作成: 細谷・細谷**  
**クライアント側: 細谷**  
**サーバ側: 渋谷**  
**テスト実施: 細谷・渋谷**

## 処理の流れ
```mermaid
sequenceDiagram
    autonumber
    participant Client as クライアント (フロントエンド)
    participant Server as サーバー (ASP.NET Core API)
    participant DB as データベース (Dapper)

    rect rgb(240, 248, 255)
        note right of User: 【GET】予約データ一覧の取得
        User->>Client: 画面を開くまたは日付を選択
        Client->>Server: HTTP GET /api/reservation
        Server->>DB: SQL実行 (SELECT)
        DB-->>Server: 予約済みのデータリスト
        Server-->>Client: HTTP 200 OK
        Client->>User: 指定した日の予約一覧を表示
    end

    rect rgb(255, 245, 238)
        note right of User: 【POST】予約データの追加
        User->>Client: データを入力して追加
        Client->>Server: HTTP POST /api/reservation
        note over Server: バリデーション
        alt エラーあり
            Server-->>Client: HTTP 400 Bad Request
            Client->>User: エラーを画面に表示
        else 正常
            Server->>DB: SQL実行 (INSERT)
            DB-->>Server: 挿入成功
            Server-->>Client: HTTP 201 Created
            Client->>User: 追加した行を画面に即座に反映
        end
    end

    rect rgb(245, 255, 250)
        note right of User: 【DELETE】予約データの削除
        User->>Client: 削除ボタンを押す
        Client->>Server: HTTP DELETE /api/reservations/{id}
        Server->>DB: SQL実行 (DELETE)
        DB-->>Server: 削除成功
        Server-->>Client: HTTP 200 OK
        Client->>User: 削除された行を画面から消去
    end
```



