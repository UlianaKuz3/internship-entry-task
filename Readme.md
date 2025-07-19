# NxN Tic-Tac-Toe Game API

REST API для онлайн-игры в крестики-нолики произвольного размера (NxN) с вероятностью случайной подмены символа игрока, 
поддержкой отказоустойчивости и idempotent-запросами.

# Технологии:
.NET 9

ASP.NET Core Web API

EF Core + SQLite

Docker

xUnit (интеграционные и модульные тесты)

# Архитектурные решения:
Чистая архитектура (разделение логики, инфраструктуры и API).

Idempotent-запросы.

Fail-safe обработка (защита от гонок с помощью транзакций и AsTracking).

# API

**Создание новой игры.**

POST /game/create

Request:

{

  "boardSize": 3,

  "winLength": 3

}

Response:

{

  "id": "GUID",

  "status": "InProgress",

  "board": [["", "", ""], ["", "", ""], ["", "", ""]]

}

**Получить текущее состояние игры.**

GET /game/{id}

Response:

{

  "id": "GUID",

  "status": "InProgress",

  "currentPlayer": "O",

  "board": [["X", "", ""], ["", "O", ""], ["", "", ""]]

}

**Сделать ход.**

POST /game/{id}/moves

Request:

{

  "row": 1,

  "column": 2,

  "symbole": "X"

}

Response:

{

  "success": true,

  "message": "Move accepted",

  "status": "InProgress"

}
