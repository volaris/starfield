import json
from subprocess import call
import os
import shutil
import urllib.request

configFile = "./config/scp.json"
imagesDir = "./img"

config = None

def loadConfig():
    global config
    with open(configFile) as data_file:
        config = json.load(data_file)
        
def getImages():
    for remote_file in config["files"]:
        urllib.request.urlretrieve("http://"+remote_file["server"]+"/"+remote_file["remote_path"], remote_file["temp_path"])
        
def renameImages():
    for remote_file in config["files"]:
            shutil.copy(remote_file["temp_path"], remote_file["local_path"])

def aggregate():
    loadConfig()
    getImages()
    renameImages()
    
aggregate()