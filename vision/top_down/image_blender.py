from PIL import Image, ImageChops
import json
from os import listdir
from os.path import isfile, join

configFile = "./config/blender.json"
imagesDir = "./img"
outDir = "./output/"

config = None

def loadConfig():
    global config
    with open(configFile) as data_file:
        config = json.load(data_file)
        
def loadImages():
    files = [join(imagesDir, f) for f in listdir(imagesDir) if isfile(join(imagesDir, f))]
    files.sort()
    
    images = []
    
    for file in files:
        image = Image.open(file).copy()
        images.append(image)
    return images
    
def blendImages(images):
    output_size = config["output_size"]
    output = Image.open("in.jpg").copy()
    blended = output.copy()
    i = 0
    for image in images:
        if i > 4:
            break
        
        camera_param = config["camera_params"][i]
        rotated = image
        
        if camera_param["orientation"] != 0:
            rotated = image.rotate(camera_param["orientation"],expand=1)
            
        drawn = output.copy()
        box = (camera_param["x"], camera_param["y"], camera_param["x"] + camera_param["width"], camera_param["y"] + camera_param["height"])
        
        drawn.paste(rotated, box)
        
        blended = ImageChops.lighter(blended, drawn)
        
        i = i + 1
    return blended
    
def loadAndBlend():
    loadConfig()
    imgs = loadImages()
    output = blendImages(imgs)
    output.save(outDir + "out.jpg", "JPEG")

loadAndBlend()