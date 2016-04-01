from subprocess import call
from time import sleep

while True:
    call("~/Downloads/LeptonModule-master/software/raspberrypi_capture/raspberrypi_capture", shell=True)
    call(["mogrify", "-format", "jpg", "IMG_0000.pgm"])
    call(["mv", "IMG_0000.jpg", "latest.jpg"])
    call(["rm", "IMG_0000.pgm"])
    sleep(.25)