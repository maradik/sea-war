$ErrorActionPreference = 'Stop'

clear

#$server = '5.189.18.251:8765'
$server = 'locaLhost:8765'

function addPlayer($playername){
    $server = $Global:server
    $body = "{ PlayerName: '$playername' }"
    $head = @{'content-type' = 'application/json'}
    Write-Host "`nЗапрос на создание комнаты с игроком '$playername'" -ForegroundColor Green
    $result = Invoke-WebRequest -Uri "http://$server/room/create" -Method Post -Body $body -Headers $head -UseBasicParsing
    $roomGuid = $result.Content | ConvertFrom-Json | Select-Object -ExpandProperty roomId
    $player1ID = $result.Content | ConvertFrom-Json | Select-Object -ExpandProperty playerId
    write-host $result.Content -ForegroundColor Cyan
    return $roomGuid, $player1ID    
}

function Fire($x, $y, $roomGuid, $player1ID){
    $server = $Global:server
    Write-Host "`nСтреляем в $x`:$y" -ForegroundColor Green
    $body = "{ x: $x, y: $y }"
    $head = @{'content-type' = 'application/json'}
    $result = Invoke-WebRequest -Uri "http://$server/room/$roomGuid/game/fire?playerId=$player1ID" -Method POST -Body $body -Headers $head -UseBasicParsing
    return ($result.Content | ConvertFrom-Json | select -ExpandProperty enemymap).cells
}

function RoomStatus($roomGuid, $player1ID){
    $server = $Global:server
    Write-Host "`nЗапрашиваем состояние комнаты $roomGuid" -ForegroundColor Green
    $result = Invoke-WebRequest -Uri "http://$server/room/$roomGuid/getStatus?playerId=$player1ID" -Method get -UseBasicParsing
    write-host $result.Content -ForegroundColor Cyan
}

function WriteMap($cells){
    $out = ''
    foreach($cell in $cells){
        switch ($cell.Status){
            Unknown { $out += '* ' }
            Missed  { $out += '- ' }
            Damaged { $out += '+ ' }
            default { $out += 'E ' }
        }
    }
    for ($i=0; $i -le 180; $i=$i+20 ) {
        $out.Substring($i,20)
    }
}

#Создание первого игрока
$roomGuid, $player1ID = addPlayer 'name1'

#Запрашиваем состояние комнаты
RoomStatus $roomGuid $player1ID

#Создание второго игрока
$roomGuid, $player2ID = addPlayer 'name2'

#Запрашиваем состояние комнаты
RoomStatus $roomGuid $player1ID

#fire
#Fire 0 0 $roomGuid $player1ID | Out-Null

WriteMap (Fire 0 0 $roomGuid $player1ID)
WriteMap (Fire 1 1 $roomGuid $player1ID)
WriteMap (Fire 9 0 $roomGuid $player1ID)
WriteMap (Fire 5 0 $roomGuid $player1ID)