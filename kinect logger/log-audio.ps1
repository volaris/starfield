# how long to record in seconds
$duration = 1200
$log_path = D:\audio\
$device=1

while($true)
{
    $date = get-date -format yyyy\-MM\-dd\-HH\-mm\-ss
    
    # record a clip
    fmedia.exe --record --out="$log_path$date.flac --dev-capture=$device --until=$duration
}