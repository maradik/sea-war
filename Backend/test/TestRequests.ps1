$body = "{ PlayerName: 'name1' }"
$head = @{'content-type' = 'application/json'}

Write-Host "Запрос на создание комнаты с игроком 'name1'`n" -ForegroundColor Green
$result = Invoke-WebRequest -Uri "http://5.189.18.251:8765/room/create" -Method Post -Body $body -Headers $head -UseBasicParsing
$roomGuid = $result.Content | ConvertFrom-Json | Select-Object -ExpandProperty roomId
Write-Host "Получен guid - $roomGuid`n" -ForegroundColor Green

Write-Host "GET запрос с guid" -ForegroundColor Green

$result = Invoke-WebRequest -Uri "http://5.189.18.251:8765/room/$roomGuid/getStatus" -Method get -UseBasicParsing
$result.Content | ConvertFrom-Json | fl *