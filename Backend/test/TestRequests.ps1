#Создание первого игрока
$body = "{ PlayerName: 'name1' }"
$head = @{'content-type' = 'application/json'}
Write-Host "`nЗапрос на создание комнаты с игроком 'name1'" -ForegroundColor Green
$result = Invoke-WebRequest -Uri "http://5.189.18.251:8765/room/create" -Method Post -Body $body -Headers $head -UseBasicParsing
$roomGuid = $result.Content | ConvertFrom-Json | Select-Object -ExpandProperty roomId
$player1ID = $result.Content | ConvertFrom-Json | Select-Object -ExpandProperty playerId
$result.Content
Write-Host $result.StatusCode -ForegroundColor Yellow

#Запрашиваем состояние комнаты 
Write-Host "`nЗапрашиваем состояние комнаты $roomGuid" -ForegroundColor Green
$result = Invoke-WebRequest -Uri "http://5.189.18.251:8765/room/$roomGuid/getStatus?playerId=$player1ID" -Method get -UseBasicParsing
$result.Content | ConvertFrom-Json | fl *
Write-Host $result.StatusCode -ForegroundColor Yellow

#Создание второго игрока
$body = "{ PlayerName: 'name2' }"
$head = @{'content-type' = 'application/json'}
Write-Host "`nЗапрос на создание комнаты с игроком 'name2'`n" -ForegroundColor Green
$result = Invoke-WebRequest -Uri "http://5.189.18.251:8765/room/create" -Method Post -Body $body -Headers $head -UseBasicParsing
$player2ID = $result.Content | ConvertFrom-Json | Select-Object -ExpandProperty playerId
$result.Content
$roomGuid = $result.Content | ConvertFrom-Json | Select-Object -ExpandProperty roomId
Write-Host $result.StatusCode -ForegroundColor Yellow

#Запрашиваем состояние комнаты 
Write-Host "`nGET запрос с guid" -ForegroundColor Green
$result = Invoke-WebRequest -Uri "http://5.189.18.251:8765/room/$roomGuid/getStatus?playerId=$player1ID" -Method get -UseBasicParsing
$result.Content | ConvertFrom-Json | fl *
Write-Host $result.StatusCode -ForegroundColor Yellow

#fire
Write-Host "`nСтреляем в 2:3" -ForegroundColor Green
$body = "{ x: 2, y: 3 }"
$head = @{'content-type' = 'application/json'}
$result = Invoke-WebRequest -Uri "http://5.189.18.251:8765/room/$roomGuid/game/fire?playerId=$player1ID" -Method POST -Body $body -Headers $head -UseBasicParsing
Write-Host $result.StatusCode -ForegroundColor Yellow
