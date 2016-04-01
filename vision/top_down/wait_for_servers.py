import json
from subprocess import call
import os
import shutil
import urllib.request

configFile = "./config/scp.json"
config = None

def loadConfig():
    global config
    with open(configFile) as data_file:
        config = json.load(data_file)
        
def getImages():
    for remote_file in config["files"]:
        while True:
            try:
                urllib.request.urlretrieve("http://"+remote_file["server"]+"/"+remote_file["remote_path"], remote_file["temp_path"])
            except Exception:
                continue
            break
        print("server: " + remote_file["server"] + " is up")

loadConfig()
getImages()