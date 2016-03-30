from subprocess import call
from time import sleep

while True:
    try:
        call(["py", "-3", "image_aggregator.py"], shell=True)
        call(["py", "-3", "image_blender.py"], shell=True)
        call(["py", "-3", "image_processor.py"], shell=True)
    except Exception:
        pass
    sleep(.25)