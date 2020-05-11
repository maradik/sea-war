#Создание первого игрока
$body = "{ PlayerName: 'name1' }"
$head = @{'content-type' = 'application/json'}
Write-Host "`nЗапрос на создание комнаты с игроком 'name1'" -ForegroundColor Green
$result = Invoke-WebRequest -Uri "http://5.189.18.251:8765/room/create" -Method Post -Body $body -Headers $head -UseBasicParsing
$roomGuid = $result.Content | ConvertFrom-Json | Select-Object -ExpandProperty roomId
$player1ID = $result.Content | ConvertFrom-Json | Select-Object -ExpandProperty playerId
$result.Content

#Запрашиваем состояние комнаты 
Write-Host "`nЗапрашиваем состояние комнаты $roomGuid" -ForegroundColor Green
$result = Invoke-WebRequest -Uri "http://5.189.18.251:8765/room/$roomGuid/getStatus?playerId=$player1ID" -Method get -UseBasicParsing
$result.Content | ConvertFrom-Json | fl *

#Создание второго игрока
$body = "{ PlayerName: 'name2' }"
$head = @{'content-type' = 'application/json'}
Write-Host "`nЗапрос на создание комнаты с игроком 'name2'`n" -ForegroundColor Green
$result = Invoke-WebRequest -Uri "http://5.189.18.251:8765/room/create" -Method Post -Body $body -Headers $head -UseBasicParsing
$player2ID = $result.Content | ConvertFrom-Json | Select-Object -ExpandProperty playerId
$result.Content
$roomGuid = $result.Content | ConvertFrom-Json | Select-Object -ExpandProperty roomId

#Запрашиваем состояние комнаты 
Write-Host "`nGET запрос с guid" -ForegroundColor Green
$result = Invoke-WebRequest -Uri "http://5.189.18.251:8765/room/$roomGuid/getStatus?playerId=$player1ID" -Method get -UseBasicParsing
$result.Content | ConvertFrom-Json | fl *


function Fire($x, $y){
    Write-Host "`nСтреляем в $x`:$y" -ForegroundColor Green
    $body = "{ x: $x, y: $y }"
    $head = @{'content-type' = 'application/json'}
    $result = Invoke-WebRequest -Uri "http://5.189.18.251:8765/room/$roomGuid/game/fire?playerId=$player1ID" -Method POST -Body $body -Headers $head -UseBasicParsing
    return ($result.Content | ConvertFrom-Json | select -ExpandProperty enemymap).cells
}

function WriteMap($cells){
    $out = ''
    foreach($cell in $cells){
        switch ($cell.Status){
            Empty              { $out += '* ' }
            ShipNeighbour      { $out += '- ' }
            EngagedByShip      { $out += '+ ' }
            EmptyFired         { $out += 'X ' }
            EngagedByShipFired { $out += 'D ' }
            default            { $out += 'E ' }
        }
    }
    for ($i=0; $i -le 180; $i=$i+20 ) {
        $out.Substring($i,20)
    }
}

#fire
WriteMap (Fire 0 0)
WriteMap (Fire 1 1)
WriteMap (Fire 9 0)