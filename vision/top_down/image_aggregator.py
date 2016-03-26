import json
from subprocess import call

configFile = "./config/scp.json"
imagesDir = "./img"

config = None

def loadConfig():
    global config
    with open(configFile) as data_file:
        config = json.load(data_file)
        
def getImages():
    for remote_file in config["files"]:
        command = remote_file["user"] + "@" + remote_file["server"] + ":" + remote_file["remote_path"]
        call(["scp", command, remote_file["temp_path"]], shell=True)
        
def renameImages():
    for remote_file in config["files"]:
        call(["mv", "-f", remote_file["temp_path"], remote_file["local_path"]], shell=True)

loadConfig()
getImages()
renameImages()