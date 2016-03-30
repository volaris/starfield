import json
from PIL import Image

configFile = "./config/processor.json"
outDir = "./output/"

config = None
verbose = False

def loadConfig():
    global config
    with open(configFile) as data_file:
        config = json.load(data_file)
        
def loadImage():
    image = Image.open(outDir + "out.jpg").copy().convert("RGBA")
    return image
    
def processImage(image):
    activity = [[0 for x in range(11)] for x in range(11)]
    base = (config["origin"]["x"], config["origin"]["y"])
    step = config["pixel_step"]
    width = config["pixel_width"]
    for x in range(11):
        for y in range(11):
            point = (base[0] + x * step, base[1] + y * step)
            for pixX in range(int(point[0] - width), int(point[0] + width)):
                for pixY in range(int(point[1] - width), int(point[1] + width)):
                    pixel = image.getpixel((pixX, pixY))
                    val = ((pixel[0] + pixel[1] + pixel[2]) / 3) / 255.0
                    activity[x][y] = activity[x][y] + val
    return activity
    
def exportActivity(activity):
    points = []
    for x in range(11):
        string = ""
        x_list = []
        for y in range(11):
            if verbose:
                string = string + "\t" + str(int(activity[x][y]))
            x_list.append({"activity" : activity[x][y]})
        points.append(x_list)
        if verbose:
            print(string)
    with open(outDir + "activity.json", "w+") as out_file:
        json.dump(points, out_file, indent=2)
    
loadConfig()
img = loadImage()
act = processImage(img)
exportActivity(act)