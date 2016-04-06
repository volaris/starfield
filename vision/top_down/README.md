Starfield Vision System
=======================

Software to capture and process IR images. This version assumes that cameras are facing straight down from the center support structure.

## image_aggregator.py
Retrieves the images from the remote cameras and prepares them for image_blender.py.

## image_blender.py
Takes the 5 images and blends them into a single image for heat map processing.

## image_processor.py
Takes the output from image_blender.py and creates a json file indicating the current presence levels for each grid coordinate.

## activity_processor.py
Takes the current output from image_blender.py and combines it with historical data to get a change in presence measure. Outputs a json file.