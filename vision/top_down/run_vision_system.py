from subprocess import call
from time import sleep
from image_processor import doProcessing
from image_aggregator import aggregate
from image_blender import loadAndBlend

call(["py", "-3", "wait_for_servers.py"], shell=True)
while True:
    try:
        aggregate()
        loadAndBlend()
        doProcessing()
    except Exception:
        pass