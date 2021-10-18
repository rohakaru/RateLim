## Rate Limiter
- 一個 Middleware，讓伺服器可以限制每個 IP 在給定時間內對 API 的請求數量。
- 可以在任何 API 的 Response header 看到以下資訊：
    - X-RateLimit-Remaining: 剩餘請求數。
    - X-RateLimit-Reset: 歸零時間(以GMT時區顯示)。
- 若 IP 已超過請求次數限制，Middleware 將回傳 Status code 429，且程式不會進入 Routing。

## 服務架構
- .NET 5.0 (ASP.NET Core WebAPI)
- Redis 6.2.6
- Docker

## 啟動服務
- 在本機執行 Clone Repository。
- 安裝 [Docker Desktop](https://www.docker.com/products/docker-desktop) (Windows/MacOS) 或 Docker Engine (Linux)
    - Redis 需要在 Linux 環境下執行，安裝 Docker 時需要勾選 Linux containers
- 在 Repository 根目錄執行以下指令，將會自動建置程式及映像檔:
```
docker-compose up --build
```
- 對預設 API `localhost:5000/v1/Run/` 提出 `GET` 請求。
- 按 `Ctrl + C` 結束服務，並執行以下指令清除 Docker containers。
```
docker-compose down
```

## 參數設定
- 連接埠
    - .NET 服務預設使用連接埠為 5000、5001。
      若需調整可以在 `docker-compose.yml` 更改 `service: web: ports` 。
    - Redis 服務預設使用連接埠為 6379。
      若需調整可以在 `docker-compose.yml` 更改 `service: redis_image: ports` ，
      並且需到 `./RateLim/RateLim/appsettings.json` 更改 `Redis: ConnectionString` 應用程式連接參數。
- 請求次數上限及歸零時間
    - 請求次數上限預設 1000 次，可以在 `./RateLim/RateLim/appsettings.json` 更改 `Redis: Limit` 參數。
    - 歸零時間預設為 1 小時，可以在 `./RateLim/RateLim/appsettings.json` 更改 `Redis: Expiry` 參數。
