# how long to record in seconds
$duration = 1200
$log_path = D:\raw\

while($true)
{
    $date = get-date -format yyyy\-MM\-dd\-HH\-mm\-ss
    
    # record a clip
    ksutil.exe -record "$log_path$date.xef" $duration –stream rawir color
}