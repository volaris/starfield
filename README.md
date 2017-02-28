Starfield
=========

Software (simulation and control) and electronics for the starfield display.

## Overview

The Starfield is a large grid of color controllable LEDs. They're spaced on vertical strands so that people can walk through them. People will be able to watch it from afar, walk through, or lounge in the space. There will be ambient animations designed to calm and intrigue. Calming music will play throughout the space. At certain times a DJ can connect to the Starfield or it picks up noise in the environment and displays animations that enhance and respond to that sound. IR cameras will detect where people are in the space and how active they are and modify the animations to respond to them. In the future, hand tracking stations will be used to create an individual or collaborative animation looper. Other methods of interaction such as converting EEG patterns to light are being investigated. The first version is a prototype designed to fit in a living room. A 20'x20'x20' version is being constructed for [Critical NW 2015](http://www.criticalnw.org/) and an even larger one is being planned for Burning Man 2016.

## Get Involved

Read the developer's guide and documentation at [volaris.github.io/starfield](http://volaris.github.io/starfield/)

## Technical Details

The core of the Starfield is driven by a [Fadecandy](https://github.com/scanlime/fadecandy) running on a [Raspberry Pi](https://www.raspberrypi.org/).

### Prototype

The prototype Starfield is a 7x5x4 voxel grid designed for hardware, animation, and construction technique testing.

![Prototype Diagram](https://raw.githubusercontent.com/volaris/starfield/master/documentation/images/Prototype.png)

### Critical NW

The Critical NW Starfield is a 11x11x10 voxel grid.

### Burning Man

The Burning Man Starfield is a 16x16x15 voxel grid.

## Repository Layout
```
/starfield
    /controllers - Starfield drivers and control software
        /AlgorithmDemo - Single display algorithm wrapper, displays list of all available drivers and their configuration options
        /Ambient - Randomly cycles through ambient algorithms
        /DualController - like AlgorithmDemo except it splits the display and displays two different algorithms
        /StarfieldDrivers - all of the driver classes, the controllers listed above use reflection to enumerate the drivers and their types from this library
    /docs - Starfield documentation for generating [volaris.github.io/starfield](http://volaris.github.io/starfield/)
    /documentation - Papers, datasheets, images related to algorithms 
    /electronics - Electronics designs: power distribution, voxel design, etc.
    /lib - Shared code and utility classes
        /.NET - .NET utility classes
            /StarfieldClient - base starfield classes, types, and interfaces (includes display model, driver interface, and network protocols)
            /Utils - Math, color, sound, and other algorithm utilities
        /external - 3rd party libraries
    /mechanical - Mechanical engineering designs and calculations for the structure
    /simulator - Unity simulator
```