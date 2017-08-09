param(
[string]$LogBase
)

write-host "starting event logger"

# how long to record in seconds
$Duration = 1200
$LogPath = "$LogBase\events\"
$Failures = 0

#while
if($true)
{
    $Date = get-date -format yyyy\-MM\-dd\-HH\-mm\-ss
    $File = "{0}{1}.xef" -F $LogPath, $Date
    
    # record a clip
    ksutil.exe -record $File $Duration –stream depth ir body

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