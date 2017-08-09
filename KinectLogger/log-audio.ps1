param(
[string]$LogBase,
[string]$Device
)

write-host "starting audio logger"

# how long to record in seconds
$Duration = 1200
$LogPath = "$LogBase\audio\"
$Failures = 0

while($true)
{
    $Date = get-date -format yyyy\-MM\-dd\-HH\-mm\-ss
    $File = "{0}{1}.flac" -F $LogPath, $Date
    
    # record a clip
    #fmedia.exe --record --out=$File --dev-capture=$Device --until=$Duration --channels=mono
    ffmpeg -f dshow -i audio="Microphone Array (Xbox NUI Sensor)" -t $Duration $File

    # delay on failure, exit if too many failures in a row
    if($LASTEXITCODE -ne 0)
    {
        $Failures++
        if($Failures -ge 10)
        {
            break
        }
        start-sleep -s 5
    }
    else
    {
        $Failures = 0
    }
}