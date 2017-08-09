param(
[string]$LogBase="D:\",
[string]$Device="1"
)

write-host "starting loggers"

$AudioLogger = "{0}\log-audio.ps1" -F $PSScriptRoot
$EventLogger = "{0}\log-events.ps1" -F $PSScriptRoot
$StreamLogger = "{0}\log-streams.ps1" -F $PSScriptRoot

$AudioParam = "-noexit", "{0} {1} {2}" -F $AudioLogger, $LogBase, $evice
$EventParam = "-noexit", "{0} {1}" -F $EventLogger, $LogBase
$StreamParam = "-noexit", "{0} {1}" -F $StreamLogger, $LogBase

#start-process powershell -ArgumentList $AudioParam
#start-process powershell -ArgumentList $EventParam
#start-process powershell -ArgumentList $StreamParam