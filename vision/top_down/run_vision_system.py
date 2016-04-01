from subprocess import call
from time import sleep

call(["py", "-3", "wait_for_servers.py"], shell=True)
while True:
    try:
        call(["py", "-3", "image_aggregator.py"], shell=True)
        call(["py", "-3", "image_blender.py"], shell=True)
        call(["py", "-3", "image_processor.py"], shell=True)
    except Exception:
        pass