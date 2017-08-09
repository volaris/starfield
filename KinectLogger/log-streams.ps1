param(
[string]$LogBase
)

write-host "starting stream logger"

# how long to record in seconds
$duration = 1200
$log_path = "$LogBase\streams\"
$Failures = 0

while($true)
{
    $date = get-date -format yyyy\-MM\-dd\-HH\-mm\-ss
    
    # record a clip
    ksutil.exe -record "$log_path$date.xef" $duration –stream rawir color

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